// Arquivo gerado por scripts/importar_regioes.py (reexecutar o script recria este arquivo).
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
/// Golpes dos jogos da Geração VII em diante (Sun/Moon ... Legends: Z-A, Champions), em formato
/// compacto: uma entrada por Pokémon (ou forma) e jogo, lida de generation-vii-ix-learnsets-seed.json.
/// A importação é incremental: linhas que já existem (mesma forma, golpe, método, nível e jogo)
/// não são duplicadas, e o seed antigo de learnsets não é alterado.
/// </summary>
public static class GameLearnsetsInitializer
{
    private const string SeedFileName = "generation-vii-ix-learnsets-seed.json";
    private const int BatchSize = 5000;
    private const int MaxGameVersionLength = 80;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly record struct LearnsetKey(
        int PokemonFormId,
        int MoveId,
        MoveLearnMethod Method,
        int? Level,
        string GameVersion);

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

        List<GameLearnsetSeed> seeds;
        using (FileStream stream = File.OpenRead(seedPath))
        {
            seeds = JsonSerializer.Deserialize<List<GameLearnsetSeed>>(stream, JsonOptions)
                ?? throw new InvalidDataException("O seed de golpes por jogo está vazio ou inválido.");
        }

        if (seeds.Count == 0)
            throw new InvalidDataException("O seed de golpes por jogo não contém registros.");

        string[] games = seeds.Select(seed => seed.GameVersion).Distinct().ToArray();
        foreach (string game in games)
        {
            if (string.IsNullOrWhiteSpace(game) || game.Length > MaxGameVersionLength)
                throw new InvalidDataException($"Jogo inválido no seed de golpes: '{game}'.");
        }

        var movesById = db.Moves.ToDictionary(move => move.Id);
        var pokemonsByDex = db.Pokemons.ToDictionary(pokemon => pokemon.NationalDexNumber);
        var formsByKey = db.PokemonForms
            .ToList()
            .ToDictionary(form => (form.PokemonId, NormalizeName(form.FormName)));

        // Só as linhas dos jogos deste seed entram na comparação.
        var existing = db.LearnableMoves
            .AsNoTracking()
            .Where(learnable => games.Contains(learnable.GameVersion!))
            .Select(learnable => new
            {
                learnable.PokemonFormId,
                learnable.MoveId,
                learnable.Method,
                learnable.Level,
                learnable.GameVersion
            })
            .AsEnumerable()
            .Select(learnable => new LearnsetKey(
                learnable.PokemonFormId,
                learnable.MoveId,
                learnable.Method,
                learnable.Level,
                learnable.GameVersion ?? string.Empty))
            .ToHashSet();

        int pending = 0;
        int ignoredEntries = 0;
        bool autoDetectChanges = db.ChangeTracker.AutoDetectChangesEnabled;
        db.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            foreach (GameLearnsetSeed seed in seeds)
            {
                if (!pokemonsByDex.TryGetValue(seed.NationalDexNumber, out Pokemon? pokemon))
                    throw new InvalidDataException(
                        $"O seed de golpes cita a espécie inexistente #{seed.NationalDexNumber:000}.");

                string formName = string.IsNullOrWhiteSpace(seed.FormName) ? "Normal" : seed.FormName;
                if (!formsByKey.TryGetValue((pokemon.Id, NormalizeName(formName)), out PokemonForm? form))
                {
                    if (NormalizeName(formName) == NormalizeName("Normal"))
                        throw new InvalidDataException(
                            $"Não existe forma Normal para o Pokémon #{seed.NationalDexNumber:000}.");

                    // Forma cadastrada com outro nome no banco: ignora (e avisa) em vez de impedir a abertura.
                    ignoredEntries++;
                    continue;
                }

                foreach (int[] pair in seed.LevelUp)
                {
                    if (pair.Length != 2 || pair[0] < 1 || pair[0] > 100)
                        throw new InvalidDataException(
                            $"Golpe por nível inválido em #{seed.NationalDexNumber:000} ({seed.GameVersion}).");

                    AddMove(form.Id, pair[1], MoveLearnMethod.LevelUp, pair[0], seed.GameVersion);
                }

                foreach (int moveId in seed.Evolution)
                    AddMove(form.Id, moveId, MoveLearnMethod.Evolution, null, seed.GameVersion);

                foreach (int moveId in seed.TM)
                    AddMove(form.Id, moveId, MoveLearnMethod.TM, null, seed.GameVersion);

                foreach (int moveId in seed.Tutor)
                    AddMove(form.Id, moveId, MoveLearnMethod.Tutor, null, seed.GameVersion);

                foreach (int moveId in seed.Egg)
                    AddMove(form.Id, moveId, MoveLearnMethod.Egg, null, seed.GameVersion);
            }

            FlushBatch();
        }
        finally
        {
            db.ChangeTracker.AutoDetectChangesEnabled = autoDetectChanges;
        }

        if (ignoredEntries > 0)
            Console.WriteLine(
                $"Golpes por jogo: {ignoredEntries} entrada(s) ignorada(s) porque a forma não existe no banco.");

        void AddMove(int formId, int moveId, MoveLearnMethod method, int? level, string gameVersion)
        {
            if (!existing.Add(new LearnsetKey(formId, moveId, method, level, gameVersion)))
                return;

            if (!movesById.TryGetValue(moveId, out PokemonMove? move))
                throw new InvalidDataException(
                    $"O seed de golpes referencia o movimento inexistente #{moveId}. " +
                    "Gere novamente o catálogo antes de iniciar o aplicativo.");

            db.LearnableMoves.Add(new LearnableMove
            {
                PokemonFormId = formId,
                MoveId = move.Id,
                Move = move,
                Method = method,
                Level = level,
                GameVersion = gameVersion
            });

            pending++;
            if (pending >= BatchSize)
                FlushBatch();
        }

        void FlushBatch()
        {
            if (pending == 0)
                return;

            db.SaveChanges();

            // Solta só os golpes já gravados; formas e golpes do catálogo continuam rastreados.
            foreach (var entry in db.ChangeTracker.Entries<LearnableMove>().ToList())
                entry.State = EntityState.Detached;

            pending = 0;
        }
    }

    private static string NormalizeName(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();

    public sealed class GameLearnsetSeed
    {
        public int NationalDexNumber { get; set; }
        public string FormName { get; set; } = "Normal";
        public string GameVersion { get; set; } = string.Empty;

        /// <summary>Pares [nível, id do golpe].</summary>
        public List<int[]> LevelUp { get; set; } = new();

        public List<int> Evolution { get; set; } = new();
        public List<int> TM { get; set; } = new();
        public List<int> Tutor { get; set; } = new();
        public List<int> Egg { get; set; } = new();
    }
}
