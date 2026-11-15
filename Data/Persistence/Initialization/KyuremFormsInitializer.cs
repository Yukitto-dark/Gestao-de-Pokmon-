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
/// Importa as formas de fusão de Kyurem como formas da espécie #646.
/// Não cria Pokémon adicionais nem relações de evolução.
/// </summary>
public static class KyuremFormsInitializer
{
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
            "kyurem-forms-seed.json");

        if (!File.Exists(seedPath))
            return;

        List<KyuremFormSeed> seeds;
        using (FileStream stream = File.OpenRead(seedPath))
        {
            seeds = JsonSerializer.Deserialize<List<KyuremFormSeed>>(stream, JsonOptions)
                ?? throw new InvalidDataException("O seed das formas de Kyurem está vazio ou inválido.");
        }

        if (seeds.Count != 2)
            throw new InvalidDataException(
                $"Eram esperadas exatamente duas formas de Kyurem; foram encontradas {seeds.Count}.");

        Pokemon kyurem = db.Pokemons.SingleOrDefault(pokemon => pokemon.NationalDexNumber == 646)
            ?? throw new InvalidDataException("O Pokémon #646 (Kyurem) não foi encontrado no banco.");

        var movesById = db.Moves.ToDictionary(move => move.Id);
        var abilities = db.Abilities.ToList();
        var abilitiesByKey = abilities
            .GroupBy(ability => NormalizeName(ability.Name))
            .ToDictionary(group => group.Key, group => group.First());

        foreach (KyuremFormSeed seed in seeds)
        {
            ValidateSeed(seed, movesById);

            PokemonForm? form = db.PokemonForms.SingleOrDefault(existing =>
                existing.PokemonId == kyurem.Id && existing.FormName == seed.FormName);

            if (form is null)
            {
                form = new PokemonForm
                {
                    PokemonId = kyurem.Id,
                    Pokemon = kyurem,
                    FormName = seed.FormName,
                    Stats = seed.Stats
                };

                foreach (PokemonType type in seed.Types.Distinct())
                    form.AddType(type);

                db.PokemonForms.Add(form);
                db.SaveChanges();
            }
            else
            {
                form.Stats = seed.Stats;
                db.Entry(form).Property<List<PokemonType>>("_types").CurrentValue =
                    seed.Types.Distinct().ToList();
            }

            var abilityLinks = new List<PokemonFormAbility>();
            foreach (KyuremAbilitySeed abilitySeed in seed.Abilities)
            {
                string key = NormalizeName(abilitySeed.Name);
                if (!abilitiesByKey.TryGetValue(key, out PokemonAbility? ability))
                {
                    ability = new PokemonAbility
                    {
                        Name = abilitySeed.Name,
                        Description = abilitySeed.Description
                    };
                    db.Abilities.Add(ability);
                    db.SaveChanges();
                    abilitiesByKey.Add(key, ability);
                }
                else if (string.IsNullOrWhiteSpace(ability.Description) &&
                         !string.IsNullOrWhiteSpace(abilitySeed.Description))
                {
                    ability.Description = abilitySeed.Description;
                }

                abilityLinks.Add(new PokemonFormAbility
                {
                    PokemonFormId = form.Id,
                    PokemonForm = form,
                    AbilityId = ability.Id,
                    Ability = ability,
                    Slot = abilitySeed.Slot
                });
            }

            var oldAbilityLinks = db.FormAbilities
                .Where(link => link.PokemonFormId == form.Id)
                .ToList();
            db.FormAbilities.RemoveRange(oldAbilityLinks);
            db.SaveChanges();
            db.FormAbilities.AddRange(abilityLinks);

            string[] spritePaths =
            {
                seed.Sprites.ExhibitionDefault,
                seed.Sprites.BoxDefault,
                seed.Sprites.ExhibitionShiny
            };
            foreach (string spritePath in spritePaths)
            {
                if (!AssetExists(spritePath))
                    throw new FileNotFoundException(
                        $"Sprite de {seed.FormName} não encontrado no diretório de saída: {spritePath}");
            }

            var oldSprites = db.Sprites
                .Where(sprite => sprite.PokemonFormId == form.Id)
                .ToList();
            db.Sprites.RemoveRange(oldSprites);
            db.SaveChanges();
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

            var oldLearnset = db.LearnableMoves
                .Where(learnable => learnable.PokemonFormId == form.Id)
                .ToList();
            db.LearnableMoves.RemoveRange(oldLearnset);
            db.SaveChanges();

            foreach (KyuremLearnsetSeed learnsetSeed in seed.Learnset)
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

            db.SaveChanges();
        }
    }

    private static void ValidateSeed(KyuremFormSeed seed, IReadOnlyDictionary<int, PokemonMove> movesById)
    {
        if (string.IsNullOrWhiteSpace(seed.FormName) || seed.FormName == "Normal")
            throw new InvalidDataException("Nome da forma de Kyurem inválido.");

        if (seed.Types.Count is < 1 or > 2 || seed.Types.Any(type => !Enum.IsDefined(type)))
            throw new InvalidDataException($"Tipos inválidos na forma {seed.FormName}.");

        if (seed.Stats.HP <= 0 || seed.Stats.Atk <= 0 || seed.Stats.Def <= 0 ||
            seed.Stats.SpAtk <= 0 || seed.Stats.SpDef <= 0 || seed.Stats.Speed <= 0)
            throw new InvalidDataException($"Atributos-base inválidos na forma {seed.FormName}.");

        if (seed.Abilities.Count == 0)
            throw new InvalidDataException($"Nenhuma habilidade cadastrada para {seed.FormName}.");

        if (seed.Learnset.Count == 0)
            throw new InvalidDataException($"Nenhum movimento cadastrado para {seed.FormName}.");

        foreach (KyuremAbilitySeed ability in seed.Abilities)
        {
            if (string.IsNullOrWhiteSpace(ability.Name) || !Enum.IsDefined(ability.Slot))
                throw new InvalidDataException($"Habilidade inválida na forma {seed.FormName}.");
        }

        foreach (KyuremLearnsetSeed learnset in seed.Learnset)
        {
            if (!Enum.IsDefined(learnset.Method))
                throw new InvalidDataException($"Método de aprendizagem inválido em {seed.FormName}.");

            if (!movesById.ContainsKey(learnset.MoveId))
                throw new InvalidDataException(
                    $"O learnset de {seed.FormName} referencia o movimento #{learnset.MoveId}, " +
                    "que não existe no catálogo Moves. Gere o seed de movimentos novamente.");
        }
    }

    private static bool AssetExists(string relativePath)
    {
        string normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
        return File.Exists(Path.Combine(AppContext.BaseDirectory, normalized));
    }

    private static string NormalizeName(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();

    public sealed class KyuremFormSeed
    {
        public string FormName { get; set; } = string.Empty;
        public List<PokemonType> Types { get; set; } = new();
        public BaseStats Stats { get; set; }
        public List<KyuremAbilitySeed> Abilities { get; set; } = new();
        public KyuremSpriteSeed Sprites { get; set; } = new();
        public List<KyuremLearnsetSeed> Learnset { get; set; } = new();
    }

    public sealed class KyuremAbilitySeed
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AbilitySlot Slot { get; set; }
        public int ApiId { get; set; }
    }

    public sealed class KyuremSpriteSeed
    {
        public string ExhibitionDefault { get; set; } = string.Empty;
        public string BoxDefault { get; set; } = string.Empty;
        public string ExhibitionShiny { get; set; } = string.Empty;
        public string? BoxShiny { get; set; }
    }

    public sealed class KyuremLearnsetSeed
    {
        public int MoveId { get; set; }
        public MoveLearnMethod Method { get; set; }
        public int? Level { get; set; }
        public string GameVersion { get; set; } = string.Empty;
    }
}
