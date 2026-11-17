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
/// Importa formas alternativas das gerações I-V como PokemonForm da espécie
/// já existente. Não cria Pokémon adicionais nem altera relações de evolução.
/// </summary>
public static class GenerationVFormsInitializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static void Inicializar(AppDbContext db) =>
        Inicializar(db, "generation-i-v-forms-seed.json", 1, 649);

    /// <summary>
    /// Importa as formas alternativas descritas em <paramref name="seedFileName"/>,
    /// restritas às espécies de <paramref name="minDex"/> a <paramref name="maxDex"/>.
    /// Com <paramref name="onlyInsertMissing"/>, formas que já existem no banco
    /// (mesma espécie e mesmo nome normalizado) não são alteradas nem recriadas.
    /// </summary>
    public static void Inicializar(
        AppDbContext db, string seedFileName, int minDex, int maxDex, bool onlyInsertMissing = false)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentException.ThrowIfNullOrWhiteSpace(seedFileName);

        string seedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Persistence",
            "Seed",
            seedFileName);

        if (!File.Exists(seedPath))
            return;

        List<AlternativeFormSeed> seeds;
        using (FileStream stream = File.OpenRead(seedPath))
        {
            seeds = JsonSerializer.Deserialize<List<AlternativeFormSeed>>(stream, JsonOptions)
                ?? throw new InvalidDataException("O seed de formas alternativas está vazio ou inválido.");
        }

        if (seeds.Count == 0)
            throw new InvalidDataException("O seed não contém formas alternativas.");

        var movesById = db.Moves.ToDictionary(move => move.Id);
        foreach (AlternativeFormSeed seed in seeds)
            ValidateSeed(seed, movesById, minDex, maxDex);

        var seenForms = new HashSet<(int Dex, string Name)>();
        foreach (AlternativeFormSeed seed in seeds)
        {
            if (!seenForms.Add((seed.NationalDexNumber, NormalizeName(seed.FormName))))
                throw new InvalidDataException(
                    $"O seed contém a forma duplicada {seed.FormName} para #{seed.NationalDexNumber:000}.");

            ValidateSprite(seed.Sprites.ExhibitionDefault, seed.FormName, "frontal");
            ValidateSprite(seed.Sprites.BoxDefault, seed.FormName, "box");
            if (!string.IsNullOrWhiteSpace(seed.Sprites.ExhibitionShiny))
                ValidateSprite(seed.Sprites.ExhibitionShiny, seed.FormName, "shiny");
        }

        var pokemonsByDex = db.Pokemons.ToDictionary(pokemon => pokemon.NationalDexNumber);
        var seedDexNumbers = seeds.Select(seed => seed.NationalDexNumber).Distinct().ToArray();
        var missingDex = seedDexNumbers.Where(dex => !pokemonsByDex.ContainsKey(dex)).ToArray();
        if (missingDex.Length > 0)
            throw new InvalidDataException(
                "As espécies a seguir não existem na Pokédex atual: " +
                string.Join(", ", missingDex.Select(dex => $"#{dex:000}")));

        var pokemonIds = seedDexNumbers.Select(dex => pokemonsByDex[dex].Id).ToArray();
        var formsByKey = db.PokemonForms
            .Where(form => pokemonIds.Contains(form.PokemonId))
            .ToList()
            .ToDictionary(
                form => (form.PokemonId, NormalizeName(form.FormName)),
                form => form);

        // Modo "somente inserir": formas que já existem no banco (mesma espécie e
        // mesmo nome normalizado) não são alteradas nem recriadas.
        if (onlyInsertMissing)
        {
            seeds = seeds
                .Where(seed => !formsByKey.ContainsKey(
                    (pokemonsByDex[seed.NationalDexNumber].Id, NormalizeName(seed.FormName))))
                .ToList();

            if (seeds.Count == 0)
                return;
        }

        // Primeiro cria/atualiza todas as formas. Um único SaveChanges gera os IDs
        // necessários para as tabelas de habilidades, sprites e movimentos.
        foreach (AlternativeFormSeed seed in seeds)
        {
            Pokemon pokemon = pokemonsByDex[seed.NationalDexNumber];
            var key = (pokemon.Id, NormalizeName(seed.FormName));

            if (!formsByKey.TryGetValue(key, out PokemonForm? form))
            {
                form = new PokemonForm
                {
                    PokemonId = pokemon.Id,
                    Pokemon = pokemon,
                    FormName = seed.FormName,
                    Stats = seed.Stats
                };

                foreach (PokemonType type in seed.Types.Distinct())
                    form.AddType(type);

                db.PokemonForms.Add(form);
                formsByKey.Add(key, form);
            }
            else
            {
                form.FormName = seed.FormName;
                form.Stats = seed.Stats;
                db.Entry(form).Property<List<PokemonType>>("_types").CurrentValue =
                    seed.Types.Distinct().ToList();
            }
        }
        db.SaveChanges();

        var targetFormIds = seeds
            .Select(seed => formsByKey[(pokemonsByDex[seed.NationalDexNumber].Id,
                                        NormalizeName(seed.FormName))].Id)
            .ToArray();

        // Reaproveita as habilidades já cadastradas e acrescenta somente as que faltam.
        var abilitiesByKey = db.Abilities
            .AsEnumerable()
            .GroupBy(ability => NormalizeName(ability.Name))
            .ToDictionary(group => group.Key, group => group.First());

        foreach (AlternativeAbilitySeed abilitySeed in seeds.SelectMany(seed => seed.Abilities))
        {
            string key = NormalizeName(abilitySeed.Name);
            if (abilitiesByKey.ContainsKey(key))
                continue;

            var ability = new PokemonAbility
            {
                Name = abilitySeed.Name,
                Description = abilitySeed.Description
            };
            db.Abilities.Add(ability);
            abilitiesByKey.Add(key, ability);
        }
        db.SaveChanges();

        // As associações abaixo são dados de referência da forma. São sincronizadas
        // em lote, sem apagar dados de usuário nem relações de evolução.
        db.FormAbilities.RemoveRange(db.FormAbilities
            .Where(link => targetFormIds.Contains(link.PokemonFormId)).ToList());
        db.Sprites.RemoveRange(db.Sprites
            .Where(sprite => targetFormIds.Contains(sprite.PokemonFormId)).ToList());
        db.LearnableMoves.RemoveRange(db.LearnableMoves
            .Where(learnable => targetFormIds.Contains(learnable.PokemonFormId)).ToList());
        db.SaveChanges();

        foreach (AlternativeFormSeed seed in seeds)
        {
            Pokemon pokemon = pokemonsByDex[seed.NationalDexNumber];
            PokemonForm form = formsByKey[(pokemon.Id, NormalizeName(seed.FormName))];

            foreach (AlternativeAbilitySeed abilitySeed in seed.Abilities)
            {
                PokemonAbility ability = abilitiesByKey[NormalizeName(abilitySeed.Name)];
                db.FormAbilities.Add(new PokemonFormAbility
                {
                    PokemonFormId = form.Id,
                    PokemonForm = form,
                    AbilityId = ability.Id,
                    Ability = ability,
                    Slot = abilitySeed.Slot
                });
            }

            db.Sprites.Add(new PokemonSprite
            {
                PokemonFormId = form.Id,
                PokemonForm = form,
                Gender = null,
                ExhibitionDefault = seed.Sprites.ExhibitionDefault,
                BoxDefault = seed.Sprites.BoxDefault,
                ExhibitionShiny = seed.Sprites.ExhibitionShiny,
                BoxShiny = seed.Sprites.BoxShiny
            });

            foreach (AlternativeLearnsetSeed learnsetSeed in seed.Learnset)
            {
                PokemonMove move = movesById[learnsetSeed.MoveId];
                db.LearnableMoves.Add(new LearnableMove
                {
                    PokemonFormId = form.Id,
                    PokemonForm = form,
                    MoveId = move.Id,
                    Move = move,
                    Method = learnsetSeed.Method,
                    Level = learnsetSeed.Level,
                    GameVersion = learnsetSeed.GameVersion
                });
            }
        }

        db.SaveChanges();
    }

    private static void ValidateSeed(
        AlternativeFormSeed seed,
        IReadOnlyDictionary<int, PokemonMove> movesById,
        int minDex,
        int maxDex)
    {
        if (seed.NationalDexNumber < minDex || seed.NationalDexNumber > maxDex)
            throw new InvalidDataException($"Número de Pokédex inválido: {seed.NationalDexNumber}.");

        if (string.IsNullOrWhiteSpace(seed.FormName) ||
            string.Equals(seed.FormName, "Normal", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException(
                $"Nome de forma inválido para #{seed.NationalDexNumber:000}.");

        if (seed.Types.Count is < 1 or > 2 || seed.Types.Any(type => !Enum.IsDefined(type)))
            throw new InvalidDataException($"Tipos inválidos em {seed.FormName}.");

        if (seed.Stats.HP <= 0 || seed.Stats.Atk <= 0 || seed.Stats.Def <= 0 ||
            seed.Stats.SpAtk <= 0 || seed.Stats.SpDef <= 0 || seed.Stats.Speed <= 0)
            throw new InvalidDataException($"Atributos-base inválidos em {seed.FormName}.");

        if (seed.Abilities.Count == 0)
            throw new InvalidDataException($"Nenhuma habilidade cadastrada em {seed.FormName}.");

        if (seed.Learnset.Count == 0)
            throw new InvalidDataException($"Nenhum movimento cadastrado em {seed.FormName}.");

        foreach (AlternativeAbilitySeed ability in seed.Abilities)
        {
            if (string.IsNullOrWhiteSpace(ability.Name) || !Enum.IsDefined(ability.Slot))
                throw new InvalidDataException($"Habilidade inválida em {seed.FormName}.");
        }

        foreach (AlternativeLearnsetSeed learnset in seed.Learnset)
        {
            if (!Enum.IsDefined(learnset.Method))
                throw new InvalidDataException($"Método de aprendizagem inválido em {seed.FormName}.");

            if (!movesById.ContainsKey(learnset.MoveId))
                throw new InvalidDataException(
                    $"O learnset de {seed.FormName} referencia o movimento #{learnset.MoveId}, " +
                    "que não existe em Moves. Gere novamente o catálogo antes de iniciar o aplicativo.");
        }
    }

    private static void ValidateSprite(string? relativePath, string formName, string label)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            throw new InvalidDataException($"Caminho do sprite {label} ausente em {formName}.");

        string normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
        string path = Path.Combine(AppContext.BaseDirectory, normalized);
        if (!File.Exists(path))
            throw new FileNotFoundException(
                $"Sprite {label} de {formName} não encontrado no diretório de saída: {relativePath}");
    }

    private static string NormalizeName(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();

    public sealed class AlternativeFormSeed
    {
        public int NationalDexNumber { get; set; }
        public string FormName { get; set; } = string.Empty;
        public List<PokemonType> Types { get; set; } = new();
        public BaseStats Stats { get; set; }
        public List<AlternativeAbilitySeed> Abilities { get; set; } = new();
        public AlternativeSpriteSeed Sprites { get; set; } = new();
        public List<AlternativeLearnsetSeed> Learnset { get; set; } = new();
    }

    public sealed class AlternativeAbilitySeed
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AbilitySlot Slot { get; set; }
        public int ApiId { get; set; }
    }

    public sealed class AlternativeSpriteSeed
    {
        public string ExhibitionDefault { get; set; } = string.Empty;
        public string BoxDefault { get; set; } = string.Empty;
        public string? ExhibitionShiny { get; set; }
        public string? BoxShiny { get; set; }
    }

    public sealed class AlternativeLearnsetSeed
    {
        public int MoveId { get; set; }
        public MoveLearnMethod Method { get; set; }
        public int? Level { get; set; }
        public string GameVersion { get; set; } = string.Empty;
    }
}
