// Arquivo gerado por scripts/importar_megas.py (reexecutar o script recria este arquivo).
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data.Persistence.Initialization;

/// <summary>
/// Registra na tabela MegaEvolutions a relação entre a forma base de um Pokémon e a sua forma
/// Mega. As formas Mega em si são criadas por GenerationVFormsInitializer a partir de
/// generation-vi-megas-seed.json; este inicializador só liga as duas, conforme
/// pokemon-mega-evolutions-seed.json (Mega Pedra ou golpe conhecido).
/// A importação é incremental: uma forma Mega que já tem relação cadastrada não é alterada
/// nem duplicada.
/// </summary>
public static class MegaEvolutionsInitializer
{
    private const string SeedFileName = "pokemon-mega-evolutions-seed.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static void Inicializar(AppDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        string seedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Persistence",
            "Seed",
            SeedFileName);

        if (!File.Exists(seedPath))
            return;

        List<MegaEvolutionSeed> seeds;
        using (FileStream stream = File.OpenRead(seedPath))
        {
            seeds = JsonSerializer.Deserialize<List<MegaEvolutionSeed>>(stream, JsonOptions)
                ?? throw new InvalidDataException("O seed de Mega Evoluções está vazio ou inválido.");
        }

        if (seeds.Count == 0)
            throw new InvalidDataException("O seed de Mega Evoluções não contém registros.");

        var movesById = db.Moves.ToDictionary(move => move.Id);
        var seenMegas = new HashSet<(int Dex, string Name)>();
        foreach (MegaEvolutionSeed seed in seeds)
        {
            ValidateSeed(seed, movesById);

            if (!seenMegas.Add((seed.NationalDexNumber, NormalizeName(seed.MegaFormName))))
                throw new InvalidDataException(
                    $"O seed repete a forma Mega {seed.MegaFormName} de #{seed.NationalDexNumber:000}.");
        }

        var pokemonsByDex = db.Pokemons.ToDictionary(pokemon => pokemon.NationalDexNumber);
        int[] missingDex = seeds
            .Select(seed => seed.NationalDexNumber)
            .Distinct()
            .Where(dex => !pokemonsByDex.ContainsKey(dex))
            .ToArray();
        if (missingDex.Length > 0)
            throw new InvalidDataException(
                "As espécies a seguir não existem na Pokédex atual: " +
                string.Join(", ", missingDex.Select(dex => $"#{dex:000}")));

        int[] pokemonIds = seeds
            .Select(seed => pokemonsByDex[seed.NationalDexNumber].Id)
            .Distinct()
            .ToArray();
        var formsByKey = db.PokemonForms
            .Where(form => pokemonIds.Contains(form.PokemonId))
            .ToList()
            .ToDictionary(form => (form.PokemonId, NormalizeName(form.FormName)));

        // Cada forma Mega só pode ter uma relação (índice único em MegaFormId).
        var linkedMegaIds = db.MegaEvolutions
            .Select(link => link.MegaFormId)
            .ToHashSet();

        foreach (MegaEvolutionSeed seed in seeds)
        {
            Pokemon pokemon = pokemonsByDex[seed.NationalDexNumber];

            if (!formsByKey.TryGetValue((pokemon.Id, NormalizeName(seed.BaseFormName)), out PokemonForm? baseForm))
                throw new InvalidDataException(
                    $"A forma base {seed.BaseFormName} de #{seed.NationalDexNumber:000} não existe no banco.");

            // Uma forma Mega cadastrada com outro nome não é reconhecida: a relação é ignorada
            // (e avisada) em vez de impedir a abertura do aplicativo.
            if (!formsByKey.TryGetValue((pokemon.Id, NormalizeName(seed.MegaFormName)), out PokemonForm? megaForm))
            {
                Console.WriteLine(
                    $"Mega Evolução ignorada: a forma {seed.MegaFormName} de #{seed.NationalDexNumber:000} não existe no banco.");
                continue;
            }

            if (!linkedMegaIds.Add(megaForm.Id))
                continue;

            db.MegaEvolutions.Add(new PokemonMegaEvolution
            {
                BaseFormId = baseForm.Id,
                BaseForm = baseForm,
                MegaFormId = megaForm.Id,
                MegaForm = megaForm,
                Method = seed.Method,
                RequiredItem = seed.Method == MegaEvolutionMethod.MegaStone ? seed.RequiredItem : null,
                RequiredMoveId = seed.Method == MegaEvolutionMethod.KnownMove ? seed.RequiredMoveId : null
            });
        }

        db.SaveChanges();
    }

    private static void ValidateSeed(MegaEvolutionSeed seed, IReadOnlyDictionary<int, PokemonMove> movesById)
    {
        if (seed.NationalDexNumber <= 0)
            throw new InvalidDataException($"Número de Pokédex inválido: {seed.NationalDexNumber}.");

        if (string.IsNullOrWhiteSpace(seed.BaseFormName) || string.IsNullOrWhiteSpace(seed.MegaFormName))
            throw new InvalidDataException(
                $"Nome de forma ausente na Mega Evolução de #{seed.NationalDexNumber:000}.");

        if (NormalizeName(seed.BaseFormName) == NormalizeName(seed.MegaFormName))
            throw new InvalidDataException(
                $"A forma base e a forma Mega de #{seed.NationalDexNumber:000} têm o mesmo nome.");

        if (!Enum.IsDefined(seed.Method))
            throw new InvalidDataException($"Método de Mega Evolução inválido em {seed.MegaFormName}.");

        if (seed.Method == MegaEvolutionMethod.MegaStone)
        {
            if (string.IsNullOrWhiteSpace(seed.RequiredItem) || seed.RequiredMoveId is not null)
                throw new InvalidDataException(
                    $"{seed.MegaFormName} deve informar apenas a Mega Pedra necessária.");
        }
        else
        {
            if (seed.RequiredMoveId is null || !string.IsNullOrWhiteSpace(seed.RequiredItem))
                throw new InvalidDataException(
                    $"{seed.MegaFormName} deve informar apenas o movimento necessário.");

            if (!movesById.ContainsKey(seed.RequiredMoveId.Value))
                throw new InvalidDataException(
                    $"{seed.MegaFormName} exige o movimento #{seed.RequiredMoveId}, que não existe em Moves. " +
                    "Gere novamente o catálogo antes de iniciar o aplicativo.");
        }
    }

    private static string NormalizeName(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();

    public sealed class MegaEvolutionSeed
    {
        public int NationalDexNumber { get; set; }
        public string BaseFormName { get; set; } = "Normal";
        public string MegaFormName { get; set; } = string.Empty;
        public MegaEvolutionMethod Method { get; set; }
        public string? RequiredItem { get; set; }
        public int? RequiredMoveId { get; set; }
    }
}
