using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data.Persistence.Initialization;

/// <summary>
/// Sincroniza altura e peso de todas as formas Pokémon com o seed gerado
/// por scripts/gerar_seed_medidas_pokemon.py.
/// O seed mantém as unidades originais da PokéAPI (decímetros e hectogramas),
/// mas o banco armazena Height em metros e Weight em quilogramas.
/// </summary>
public static class PokemonMeasurementsInitializer
{
    private const int ExpectedSpeciesCount = 1025;
    private const int ExpectedAlternativeFormsCount = 500; // 500 formas alternativas: 191 de Gen I-VI (inclui 48 Mega Evoluções) + 261 das regiões Alola a Paldea + 48 Mega Evoluções de Legends: Z-A
    private const int ExpectedTotalForms = ExpectedSpeciesCount + ExpectedAlternativeFormsCount;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static void Inicializar(AppDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        string seedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Persistence",
            "Seed",
            "pokemon-measurements-seed.json");

        if (!File.Exists(seedPath))
        {
            throw new FileNotFoundException(
                "Seed de altura e peso ausente. Execute " +
                "python3 scripts/gerar_seed_medidas_pokemon.py para gerá-lo.",
                seedPath);
        }

        List<PokemonMeasurementSeed> seeds;
        using (FileStream stream = File.OpenRead(seedPath))
        {
            seeds = JsonSerializer.Deserialize<List<PokemonMeasurementSeed>>(stream, JsonOptions)
                ?? throw new InvalidDataException("O seed de medidas está vazio ou inválido.");
        }

        ValidateSeed(seeds);

        List<PokemonForm> forms = db.PokemonForms
            .Include(form => form.Pokemon)
            .ToList();

        var formsByKey = new Dictionary<FormKey, PokemonForm>();
        foreach (PokemonForm form in forms)
        {
            if (form.Pokemon is null)
                throw new InvalidDataException(
                    $"A forma '{form.FormName}' (ID {form.Id}) não possui espécie associada.");

            FormKey key = CreateKey(form.Pokemon.NationalDexNumber, form.FormName);
            if (!formsByKey.TryAdd(key, form))
            {
                throw new InvalidDataException(
                    $"Existe mais de uma forma cadastrada para #{key.NationalDexNumber:000} / {form.FormName}.");
            }
        }

        var seedKeys = seeds
            .Select(seed => CreateKey(seed.NationalDexNumber, seed.FormName))
            .ToHashSet();

        List<PokemonMeasurementSeed> missingForms = seeds
            .Where(seed => !formsByKey.ContainsKey(CreateKey(seed.NationalDexNumber, seed.FormName)))
            .ToList();

        if (missingForms.Count > 0)
        {
            string details = string.Join(
                ", ",
                missingForms.Take(15).Select(seed =>
                    $"#{seed.NationalDexNumber:000} {seed.FormName}"));
            string suffix = missingForms.Count > 15 ? $" e mais {missingForms.Count - 15}" : string.Empty;

            throw new InvalidDataException(
                $"O seed contém {missingForms.Count} formas que ainda não existem no banco: " +
                details + suffix + ". Execute os inicializadores de formas antes do inicializador de medidas.");
        }

        List<PokemonForm> formsWithoutSeed = forms
            .Where(form => !seedKeys.Contains(CreateKey(
                form.Pokemon.NationalDexNumber,
                form.FormName)))
            .ToList();

        if (formsWithoutSeed.Count > 0)
        {
            string details = string.Join(
                ", ",
                formsWithoutSeed.Take(15).Select(form =>
                    $"#{form.Pokemon.NationalDexNumber:000} {form.FormName}"));
            string suffix = formsWithoutSeed.Count > 15 ? $" e mais {formsWithoutSeed.Count - 15}" : string.Empty;

            throw new InvalidDataException(
                $"Existem {formsWithoutSeed.Count} formas no banco sem medidas correspondentes no seed: " +
                details + suffix + ". Revise o gerador e os seeds de formas alternativas.");
        }

        int updated = 0;
        foreach (PokemonMeasurementSeed seed in seeds)
        {
            FormKey key = CreateKey(seed.NationalDexNumber, seed.FormName);
            PokemonForm form = formsByKey[key];

            // A API e o JSON de seed usam dm e hg. Convertemos na importação
            // para que o SQLite e o restante do sistema trabalhem em m e kg.
            double heightMeters = seed.Height / 10.0;
            double weightKilograms = seed.Weight / 10.0;

            // O seed é a fonte de referência; executar novamente não divide
            // os valores já convertidos, pois sempre recalcula a partir do seed.
            if (form.Height != heightMeters || form.Weight != weightKilograms)
            {
                form.Height = heightMeters;
                form.Weight = weightKilograms;
                updated++;
            }
        }

        db.SaveChanges();

        Console.WriteLine(
            $"Medidas sincronizadas: {seeds.Count} formas verificadas; " +
            $"{updated} formas atualizadas.");
    }

    private static void ValidateSeed(IReadOnlyCollection<PokemonMeasurementSeed> seeds)
    {
        if (seeds.Count != ExpectedTotalForms)
        {
            throw new InvalidDataException(
                $"O seed de medidas deveria conter {ExpectedTotalForms} registros " +
                $"({ExpectedSpeciesCount} formas Normais + {ExpectedAlternativeFormsCount} alternativas), " +
                $"mas contém {seeds.Count}.");
        }

        var seen = new HashSet<FormKey>();
        foreach (PokemonMeasurementSeed seed in seeds)
        {
            if (seed.NationalDexNumber < 1 || seed.NationalDexNumber > ExpectedSpeciesCount)
                throw new InvalidDataException(
                    $"NationalDexNumber inválido no seed de medidas: {seed.NationalDexNumber}.");

            if (string.IsNullOrWhiteSpace(seed.FormName))
                throw new InvalidDataException(
                    $"Nome de forma vazio para o Pokémon #{seed.NationalDexNumber:000}.");

            if (seed.Height <= 0 || seed.Weight <= 0)
                throw new InvalidDataException(
                    $"Altura ou peso inválido para #{seed.NationalDexNumber:000} / {seed.FormName}: " +
                    $"Height={seed.Height}, Weight={seed.Weight}.");

            FormKey key = CreateKey(seed.NationalDexNumber, seed.FormName);
            if (!seen.Add(key))
                throw new InvalidDataException(
                    $"Registro duplicado no seed de medidas: #{seed.NationalDexNumber:000} / {seed.FormName}.");
        }

        int normalForms = seeds.Count(seed =>
            string.Equals(seed.FormName.Trim(), "Normal", StringComparison.OrdinalIgnoreCase));
        if (normalForms != ExpectedSpeciesCount)
        {
            throw new InvalidDataException(
                $"O seed deveria conter {ExpectedSpeciesCount} formas Normais, mas contém {normalForms}.");
        }
    }

    private static FormKey CreateKey(int nationalDexNumber, string formName) =>
        new(nationalDexNumber, NormalizeFormName(formName));

    private static string NormalizeFormName(string formName) =>
        string.Join(' ', formName.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();

    private readonly record struct FormKey(int NationalDexNumber, string FormName);

    private sealed class PokemonMeasurementSeed
    {
        public int NationalDexNumber { get; set; }
        public string FormName { get; set; } = string.Empty;
        public int Height { get; set; }
        public int Weight { get; set; }
    }
}
