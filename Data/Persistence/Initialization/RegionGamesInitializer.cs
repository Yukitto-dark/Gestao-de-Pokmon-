using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.model;

namespace GestaoDePokemon.Data.Persistence.Initialization;

/// <summary>
/// Completa Region.MainGames (lista JSON dos jogos principais de cada região, pelo nome do jogo:
/// "Ruby", "Sapphire", "Omega Ruby"...). Os jogos que já estão na lista são mantidos como estão;
/// só entram os que faltam (por exemplo Omega Ruby e Alpha Sapphire em Hoenn). Uma lista que não
/// é um JSON válido de textos não é alterada.
/// </summary>
public static class RegionGamesInitializer
{
    private static readonly Dictionary<int, string[]> GamesByRegion = new()
    {
        [1] = new[] { "Red", "Blue", "Yellow" }, // Kanto
        [2] = new[] { "Gold", "Silver", "Crystal" }, // Johto
        [3] = new[] { "Ruby", "Sapphire", "Emerald" }, // Hoenn
    };

    // Nomes de version group da PokéAPI que uma versão anterior deste script chegou a gravar em
    // MainGames; são trocados pelos nomes dos jogos. A comparação diferencia maiúsculas, para
    // não confundir "emerald" com o "Emerald" que já era do projeto.
    private static readonly HashSet<string> LegacyVersionGroups = new(StringComparer.Ordinal)
    {
        "red-blue",
        "yellow",
        "firered-leafgreen",
        "lets-go-pikachu-lets-go-eevee",
        "gold-silver",
        "crystal",
        "heartgold-soulsilver",
        "ruby-sapphire",
        "emerald",
        "omega-ruby-alpha-sapphire",
        "diamond-pearl",
        "platinum",
        "brilliant-diamond-shining-pearl",
        "black-white",
        "black-2-white-2",
        "x-y",
        "legends-za",
        "mega-dimension",
        "sun-moon",
        "ultra-sun-ultra-moon",
        "sword-shield",
        "the-isle-of-armor",
        "the-crown-tundra",
        "scarlet-violet",
        "the-teal-mask",
        "the-indigo-disk",
        "legends-arceus",
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        // Não escapa o apóstrofo de "Let's Go" (o padrão gravaria o código Unicode no lugar).
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static void Inicializar(AppDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        foreach (Region region in db.Regions.ToList())
        {
            if (!GamesByRegion.TryGetValue(region.Id, out string[]? games))
                continue;

            List<string>? current = ReadGames(region.MainGames);
            if (current is null)
                continue;

            int replaced = current.RemoveAll(LegacyVersionGroups.Contains);
            HashSet<string> known = current.Select(NormalizeName).ToHashSet();
            bool added = false;

            foreach (string game in games)
            {
                if (!known.Add(NormalizeName(game)))
                    continue;

                current.Add(game);
                added = true;
            }

            if (added || replaced > 0)
                region.MainGames = JsonSerializer.Serialize(current, JsonOptions);
        }
    }

    private static List<string>? ReadGames(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string NormalizeName(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();
}
