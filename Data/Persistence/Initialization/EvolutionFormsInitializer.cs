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
/// Evoluções que só valem para uma FORMA, lidas de pokemon-evolution-forms-seed.json: Meowth de Galar -> Perrserker,
/// Rattata de Alola -> Raticate de Alola, Quilava -> Typhlosion de Hisui etc. Cada linha tem as mesmas
/// condições de uma linha da tabela Evolutions, mais FromFormName e/ou ToFormName (nomes das formas em
/// PokemonForms.FormName), que viram FromFormId e/ou ToFormId.
///
/// Este inicializador precisa rodar DEPOIS das formas de Alola, Galar, Hisui e Paldea, que são criadas mais
/// adiante que SeedEvolutions. A importação é incremental e não apaga nada:
///   * linha que já existe com as mesmas formas: ignorada;
///   * linha por espécie já gravada (FromFormId e ToFormId nulos) com as mesmas condições, como o Perrserker de
///     bancos criados por uma versão anterior do script: ela ganha as formas (só estas duas colunas mudam);
///   * as demais são inseridas.
/// </summary>
public static class EvolutionFormsInitializer
{
    private const string SeedFileName = "pokemon-evolution-forms-seed.json";
    private const int MaxGameVersionLength = 80;

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

        List<DatabaseInitializer.PokemonEvolutionSeed> seeds;
        using (FileStream stream = File.OpenRead(seedPath))
        {
            seeds = JsonSerializer.Deserialize<List<DatabaseInitializer.PokemonEvolutionSeed>>(stream, JsonOptions)
                ?? throw new InvalidDataException("O seed de evoluções por forma está vazio ou inválido.");
        }

        if (seeds.Count == 0)
            throw new InvalidDataException("O seed de evoluções por forma não contém relações.");

        var pokemonsByDex = db.Pokemons.ToDictionary(pokemon => pokemon.NationalDexNumber);
        var moveIds = db.Moves.Select(move => move.Id).ToHashSet();
        var formsByKey = new Dictionary<(int, string), PokemonForm>();
        foreach (PokemonForm form in db.PokemonForms.AsNoTracking().ToList())
            formsByKey[(form.PokemonId, NormalizeName(form.FormName))] = form;

        // Linhas já gravadas, rastreadas pelo EF para poder marcar a forma nas que ainda não têm.
        List<PokemonEvolution> existing = db.Evolutions.ToList();
        var knownKeys = new HashSet<string>(existing.Select(evolution => FullKey(evolution)));
        var adoptable = new Dictionary<string, Queue<PokemonEvolution>>();
        foreach (PokemonEvolution evolution in existing)
        {
            if (evolution.FromFormId != null || evolution.ToFormId != null)
                continue;

            string key = BaseKey(evolution);
            if (!adoptable.TryGetValue(key, out Queue<PokemonEvolution>? queue))
            {
                queue = new Queue<PokemonEvolution>();
                adoptable[key] = queue;
            }
            queue.Enqueue(evolution);
        }

        foreach (DatabaseInitializer.PokemonEvolutionSeed seed in seeds)
        {
            PokemonEvolution candidate = CreateEvolution(seed, pokemonsByDex, formsByKey, moveIds);

            if (!knownKeys.Add(FullKey(candidate)))
                continue;

            if (adoptable.TryGetValue(BaseKey(candidate), out Queue<PokemonEvolution>? sameConditions)
                && sameConditions.Count > 0)
            {
                PokemonEvolution current = sameConditions.Dequeue();
                current.FromFormId = candidate.FromFormId;
                current.ToFormId = candidate.ToFormId;
                continue;
            }

            db.Evolutions.Add(candidate);
        }
    }

    private static PokemonEvolution CreateEvolution(
        DatabaseInitializer.PokemonEvolutionSeed seed,
        IReadOnlyDictionary<int, Pokemon> pokemonsByDex,
        IReadOnlyDictionary<(int, string), PokemonForm> formsByKey,
        IReadOnlySet<int> moveIds)
    {
        string label = $"#{seed.FromNationalDexNumber:000} -> #{seed.ToNationalDexNumber:000}";

        if (!Enum.IsDefined(seed.Method))
            throw new InvalidDataException($"Método de evolução inválido ({label}).");
        if (!pokemonsByDex.TryGetValue(seed.FromNationalDexNumber, out Pokemon? fromPokemon))
            throw new InvalidDataException($"Pokémon de origem #{seed.FromNationalDexNumber:000} não encontrado.");
        if (!pokemonsByDex.TryGetValue(seed.ToNationalDexNumber, out Pokemon? toPokemon))
            throw new InvalidDataException($"Pokémon de destino #{seed.ToNationalDexNumber:000} não encontrado.");
        if (seed.FromNationalDexNumber == seed.ToNationalDexNumber)
            throw new InvalidDataException($"Uma espécie não pode evoluir para si mesma ({label}).");
        if (seed.RequiredLevel is < 1 or > 100)
            throw new InvalidDataException($"Nível de evolução deve estar entre 1 e 100 ({label}).");
        if (seed.MinimumFriendship is < 0 or > 255 || seed.MinimumBeauty is < 0 or > 255 || seed.MinimumAffection is < 0)
            throw new InvalidDataException($"Amizade, beleza ou afeição mínima inválida ({label}).");
        if (seed.MinimumSteps is < 0 || seed.MinimumMoveCount is < 0)
            throw new InvalidDataException($"Quantidade mínima de passos ou movimentos inválida ({label}).");
        if (seed.RequiredTime.HasValue && !Enum.IsDefined(seed.RequiredTime.Value))
            throw new InvalidDataException($"Horário de evolução inválido ({label}).");
        if (seed.RequiredGender.HasValue && !Enum.IsDefined(seed.RequiredGender.Value))
            throw new InvalidDataException($"Gênero de evolução inválido ({label}).");
        if (seed.RelativePhysicalStats.HasValue && !Enum.IsDefined(seed.RelativePhysicalStats.Value))
            throw new InvalidDataException($"Comparação entre Attack e Defense inválida ({label}).");
        if (seed.RequiredPartyType.HasValue && !Enum.IsDefined(seed.RequiredPartyType.Value))
            throw new InvalidDataException($"Tipo exigido na equipe inválido ({label}).");
        if (seed.RequiredMoveType.HasValue && !Enum.IsDefined(seed.RequiredMoveType.Value))
            throw new InvalidDataException($"Tipo de movimento exigido inválido ({label}).");
        if (seed.RequiredMoveId.HasValue && !moveIds.Contains(seed.RequiredMoveId.Value))
            throw new InvalidDataException($"Movimento #{seed.RequiredMoveId.Value} exigido pela evolução {label} não existe na tabela Moves.");
        if (seed.GameVersion != null && seed.GameVersion.Length > MaxGameVersionLength)
            throw new InvalidDataException($"Jogo inválido na evolução {label}: '{seed.GameVersion}'.");

        int? partySpeciesId = null;
        if (seed.RequiredPartySpeciesNationalDexNumber.HasValue)
        {
            if (!pokemonsByDex.TryGetValue(seed.RequiredPartySpeciesNationalDexNumber.Value, out Pokemon? partySpecies))
                throw new InvalidDataException($"Espécie exigida na equipe #{seed.RequiredPartySpeciesNationalDexNumber.Value:000} não encontrada ({label}).");
            partySpeciesId = partySpecies.Id;
        }

        int? tradeSpeciesId = null;
        if (seed.RequiredTradeSpeciesNationalDexNumber.HasValue)
        {
            if (!pokemonsByDex.TryGetValue(seed.RequiredTradeSpeciesNationalDexNumber.Value, out Pokemon? tradeSpecies))
                throw new InvalidDataException($"Espécie exigida na troca #{seed.RequiredTradeSpeciesNationalDexNumber.Value:000} não encontrada ({label}).");
            tradeSpeciesId = tradeSpecies.Id;
        }

        int? fromFormId = ResolveForm(seed.FromFormName, fromPokemon, formsByKey, label);
        int? toFormId = ResolveForm(seed.ToFormName, toPokemon, formsByKey, label);
        if (fromFormId == null && toFormId == null)
            throw new InvalidDataException($"A evolução {label} do seed por forma não informa nenhuma forma.");

        return new PokemonEvolution
        {
            FromPokemonId = fromPokemon.Id,
            ToPokemonId = toPokemon.Id,
            FromFormId = fromFormId,
            ToFormId = toFormId,
            Method = seed.Method,
            RequiredLevel = seed.RequiredLevel,
            RequiredItem = seed.RequiredItem,
            RequiredHeldItem = seed.RequiredHeldItem,
            RequiredTime = seed.RequiredTime,
            MinimumFriendship = seed.MinimumFriendship,
            MinimumBeauty = seed.MinimumBeauty,
            RequiredGender = seed.RequiredGender,
            RequiredLocation = seed.RequiredLocation,
            RequiredMoveId = seed.RequiredMoveId,
            RequiredPartySpeciesId = partySpeciesId,
            RequiredTradeSpeciesId = tradeSpeciesId,
            RequiredPartyType = seed.RequiredPartyType,
            RequiredMoveType = seed.RequiredMoveType,
            RelativePhysicalStats = seed.RelativePhysicalStats,
            MinimumAffection = seed.MinimumAffection,
            MinimumSteps = seed.MinimumSteps,
            MinimumMoveCount = seed.MinimumMoveCount,
            NeedsOverworldRain = seed.NeedsOverworldRain,
            TurnUpsideDown = seed.TurnUpsideDown,
            RequiresEmptyPartySlot = seed.RequiresEmptyPartySlot,
            RequiresPokeBallInBag = seed.RequiresPokeBallInBag,
            GameVersion = seed.GameVersion
        };
    }

    private static int? ResolveForm(
        string? formName,
        Pokemon pokemon,
        IReadOnlyDictionary<(int, string), PokemonForm> formsByKey,
        string label)
    {
        if (string.IsNullOrWhiteSpace(formName))
            return null;

        if (!formsByKey.TryGetValue((pokemon.Id, NormalizeName(formName)), out PokemonForm? form))
            throw new InvalidDataException(
                $"A forma '{formName}' de {pokemon.Name} (evolução {label}) não existe no banco. " +
                "Gere os seeds de formas antes de iniciar o aplicativo.");

        return form.Id;
    }

    // Todas as condições da evolução (sem as formas): é o que identifica uma relação por espécie.
    private static string BaseKey(PokemonEvolution evolution) =>
        string.Join('|', new object?[]
        {
            evolution.FromPokemonId, evolution.ToPokemonId, evolution.Method,
            evolution.RequiredLevel, evolution.RequiredItem ?? string.Empty, evolution.RequiredTime,
            evolution.MinimumFriendship, evolution.MinimumBeauty, evolution.RequiredGender,
            evolution.RequiredLocation ?? string.Empty, evolution.RequiredMoveId,
            evolution.RequiredHeldItem ?? string.Empty, evolution.RequiredPartySpeciesId,
            evolution.RequiredTradeSpeciesId, evolution.RequiredPartyType, evolution.RequiredMoveType,
            evolution.RelativePhysicalStats, evolution.MinimumAffection, evolution.MinimumSteps,
            evolution.MinimumMoveCount, evolution.NeedsOverworldRain, evolution.TurnUpsideDown,
            evolution.RequiresEmptyPartySlot, evolution.RequiresPokeBallInBag,
            evolution.GameVersion ?? string.Empty
        });

    private static string FullKey(PokemonEvolution evolution) =>
        BaseKey(evolution) + "|" + evolution.FromFormId + "|" + evolution.ToFormId;

    private static string NormalizeName(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();
}
