using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Database;

/// <summary>
/// Cria a estrutura inicial do SQLite e carrega o conjunto inicial de espécies.
/// Os registros são convertidos diretamente para as entidades de model/Entities.
/// </summary>
public static class DatabaseInitializer
{
    private static readonly JsonSerializerOptions SeedJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static void Inicializar()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        // O seed é incremental: não recria nem apaga Pokémon existentes.
        // Isso permite executar a atualização dos sprites em bancos já criados.
        using var transaction = db.Database.BeginTransaction();

        SeedRegions(db);
        db.SaveChanges();

        if (!db.Pokemons.Any())
        {
            SeedPokemons(db);
            db.SaveChanges();
        }

        // Corrige dados herdados da versão anterior e remove a região
        // Sinnoh, que não pertence ao recorte atual da Pokédex (1 a 386).
        NormalizeLegacyPokedexData(db);

        SeedSprites(db);
        db.SaveChanges();

        transaction.Commit();
    }

    private static void SeedRegions(AppDbContext db)
    {
        Region[] regions =
        {
            new() { Id = 1, Name = "Kanto", Generation = 1 },
            new() { Id = 2, Name = "Johto", Generation = 2 },
            new() { Id = 3, Name = "Hoenn", Generation = 3 }
        };

        foreach (Region region in regions)
        {
            if (!db.Regions.Any(existing => existing.Id == region.Id))
                db.Regions.Add(region);
        }
    }

    /// <summary>
    /// Corrige a entrada #358, que em uma versão anterior estava cadastrada
    /// incorretamente como Chingling. A espécie correta do #358 é Chimecho,
    /// introduzida na Geração III. Chingling é #433 e pertence à Geração IV,
    /// portanto não integra o conjunto atual de #001 a #386.
    /// Também remove qualquer associação remanescente com Sinnoh.
    /// </summary>
    private static void NormalizeLegacyPokedexData(AppDbContext db)
    {
        Region hoenn = db.Regions.Single(region => region.Id == 3);

        Pokemon? chimecho = db.Pokemons
            .Include(pokemon => pokemon.Forms)
            .SingleOrDefault(pokemon => pokemon.NationalDexNumber == 358);

        if (chimecho is not null)
        {
            chimecho.Name = "Chimecho";
            chimecho.Generation = 3;
            chimecho.OriginRegionId = hoenn.Id;
            chimecho.Category = "Wind Chime";

            // Corrige os dados que vieram da entrada equivocada de Chingling.
            db.Entry(chimecho)
                .Property<List<EggGroup>>("_eggGroups")
                .CurrentValue = new List<EggGroup> { EggGroup.Amorphous };

            foreach (PokemonForm form in chimecho.Forms)
            {
                form.Stats = new BaseStats(75, 50, 80, 95, 90, 65);
                db.Entry(form)
                    .Property<List<PokemonType>>("_types")
                    .CurrentValue = new List<PokemonType> { PokemonType.Psychic };
            }
        }

        // Persiste primeiro a troca de região de #358 para que a consulta
        // abaixo não veja o antigo OriginRegionId no SQLite.
        db.SaveChanges();

        Region? sinnoh = db.Regions.FirstOrDefault(region =>
            region.Id == 4 || region.Name == "Sinnoh");

        if (sinnoh is null)
        {
            db.SaveChanges();
            return;
        }

        // Nunca apaga uma região que ainda seja a origem de outras espécies.
        // Nesse conjunto, somente o registro #358 (corrigido acima) podia
        // apontar para Sinnoh.
        int[] remainingOriginIds = db.Pokemons
            .Where(pokemon => pokemon.OriginRegionId == sinnoh.Id)
            .Select(pokemon => pokemon.NationalDexNumber)
            .ToArray();

        if (remainingOriginIds.Length > 0)
        {
            throw new InvalidOperationException(
                "Não foi possível remover Sinnoh: ainda existem Pokémon com essa região de origem: " +
                string.Join(", ", remainingOriginIds));
        }

        // Remove Sinnoh também das regiões onde os Pokémon podem ser encontrados.
        List<Pokemon> linkedPokemons = db.Pokemons
            .Include(pokemon => pokemon.FoundInRegions)
            .Where(pokemon => pokemon.FoundInRegions.Any(region => region.Id == sinnoh.Id))
            .ToList();

        foreach (Pokemon pokemon in linkedPokemons)
            pokemon.RemoveFoundRegion(sinnoh);

        // Salva a correção e as associações antes de excluir a região,
        // respeitando as chaves estrangeiras do SQLite.
        db.SaveChanges();
        db.Regions.Remove(sinnoh);
        db.SaveChanges();
    }

    private static void SeedPokemons(AppDbContext db)
    {
        using Stream stream = typeof(DatabaseInitializer).Assembly.GetManifestResourceStream(
            "GestaoDePokemon.Database.Seed.pokemon-seed.json")
            ?? throw new InvalidOperationException(
                "O arquivo incorporado Database/Seed/pokemon-seed.json não foi encontrado.");

        List<PokemonSeed> seeds = JsonSerializer.Deserialize<List<PokemonSeed>>(
            stream, SeedJsonOptions)
            ?? throw new InvalidOperationException("O arquivo de dados iniciais está vazio ou inválido.");

        var pokemons = new List<Pokemon>(seeds.Count);

        foreach (PokemonSeed seed in seeds)
        {
            var pokemon = new Pokemon
            {
                Id = seed.NationalDexNumber,
                NationalDexNumber = seed.NationalDexNumber,
                Name = seed.Name,
                Generation = seed.Generation,
                OriginRegionId = GetOriginRegionId(seed.Generation),
                Category = seed.Category
            };

            foreach (EggGroup eggGroup in seed.EggGroups.Distinct())
                pokemon.AddEggGroup(eggGroup);

            var form = new PokemonForm
            {
                FormName = seed.Form.FormName,
                Stats = seed.Form.Stats
            };

            foreach (PokemonType type in seed.Form.Types.Distinct())
                form.AddType(type);

            pokemon.Forms.Add(form);
            pokemons.Add(pokemon);
        }

        db.Pokemons.AddRange(pokemons);
    }

    /// <summary>
    /// Registra os sprites locais já existentes no projeto.
    /// A estrutura esperada é View/images/pokemon/{frontNormal,frontShiny,mini}/{numero}.png.
    /// Os caminhos relativos são armazenados no SQLite para funcionar tanto no build
    /// quanto no publish, independentemente do diretório de trabalho do processo.
    /// </summary>
    private static void SeedSprites(AppDbContext db)
    {
        var forms = db.PokemonForms
            .Include(form => form.Pokemon)
            .Include(form => form.Sprites)
            .Where(form => form.FormName == "Normal")
            .ToList();

        foreach (PokemonForm form in forms)
        {
            string dexNumber = form.Pokemon.NationalDexNumber.ToString();

            string frontNormal = $"View/images/pokemon/frontNormal/{dexNumber}.png";
            string frontShiny = $"View/images/pokemon/frontShiny/{dexNumber}.png";
            string mini = $"View/images/pokemon/mini/{dexNumber}.png";

            bool hasFrontNormal = AssetExists(frontNormal);
            bool hasFrontShiny = AssetExists(frontShiny);
            bool hasMini = AssetExists(mini);

            // Não cria registros vazios quando nenhum dos arquivos do Pokémon existe.
            if (!hasFrontNormal && !hasFrontShiny && !hasMini)
                continue;

            PokemonSprite? sprite = form.Sprites
                .FirstOrDefault(existing => existing.Gender is null);

            if (sprite is null)
            {
                sprite = new PokemonSprite
                {
                    PokemonFormId = form.Id,
                    Gender = null
                };
                db.Sprites.Add(sprite);
            }

            // Arte frontal grande usada na visualização/detalhes.
            if (hasFrontNormal)
                sprite.ExhibitionDefault = frontNormal;

            // Ícone menor usado nas células da Box. Se estiver ausente, o
            // repositório pode recorrer a ExhibitionDefault como alternativa.
            if (hasMini)
                sprite.BoxDefault = mini;

            // A pasta frontShiny contém a arte frontal shiny, não um ícone mini.
            if (hasFrontShiny)
                sprite.ExhibitionShiny = frontShiny;
        }
    }

    private static bool AssetExists(string relativePath)
    {
        string diskPath = Path.Combine(
            AppContext.BaseDirectory,
            relativePath.Replace('/', Path.DirectorySeparatorChar));

        return File.Exists(diskPath);
    }

    private static int GetOriginRegionId(int generation) => generation switch
    {
        1 => 1, // Kanto
        2 => 2, // Johto
        3 => 3, // Hoenn
        _ => throw new InvalidDataException(
            $"A geração {generation} não possui uma região cadastrada neste conjunto da Pokédex.")
    };

    public sealed class PokemonSeed
    {
        public PokemonSeed() { }
        public int NationalDexNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Generation { get; set; }
        public string Category { get; set; } = string.Empty;
        public List<EggGroup> EggGroups { get; set; } = new();
        public PokemonFormSeed Form { get; set; } = new();
    }

    public sealed class PokemonFormSeed
    {
        public PokemonFormSeed() { }
        public string FormName { get; set; } = "Normal";
        public List<PokemonType> Types { get; set; } = new();
        public BaseStats Stats { get; set; }
    }
}
