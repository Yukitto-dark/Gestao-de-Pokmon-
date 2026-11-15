using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

using GestaoDePokemon.Data.Persistence;
namespace GestaoDePokemon.Data.Persistence.Initialization;

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
        EnsureEvolutionSchema(db);

        // O seed é incremental: não recria nem apaga Pokémon existentes.
        // Isso permite executar a atualização dos sprites em bancos já criados.
        using var transaction = db.Database.BeginTransaction();

        SeedRegions(db);
        db.SaveChanges();

        List<PokemonSeed> pokemonSeeds = LoadPokemonSeeds();

        // Inserção incremental: inclui apenas espécies ausentes, mesmo em bancos já preenchidos.
        SeedPokemons(db, pokemonSeeds);
        db.SaveChanges();

        // BaseStats já faz parte de PokemonForm. Esta rotina apenas repara
        // formas existentes que estejam sem valores válidos, sem sobrescrever
        // atributos que já foram cadastrados corretamente.
        EnsureBaseStats(db, pokemonSeeds);
        db.SaveChanges();

        // Corrige dados legados sem remover Sinnoh, necessária à Geração IV.
        NormalizeLegacyPokedexData(db);

        // Importa o catálogo completo de habilidades e seus slots (1, 2 e oculto).
        SeedAbilities(db);
        db.SaveChanges();

        SeedSprites(db);
        db.SaveChanges();

        // Os arquivos são gerados uma única vez por scripts/gerar_seed_moves.py.
        // A importação é incremental e não duplica movimentos nem learnsets.
        SeedMoveCatalogAndLearnsets(db);
        db.SaveChanges();

        // Relacionamentos pai/filho: importa as evoluções sem duplicar registros
        // e sem apagar relações que já existam no banco.
        SeedEvolutions(db);
        db.SaveChanges();

        // Formas alternativas de Kyurem: não criam espécies nem evoluções.
        KyuremFormsInitializer.Inicializar(db);
        db.SaveChanges();

        // Formas alternativas das gerações I-V: sincroniza PokemonForms, habilidades, sprites e learnsets, sem modificar espécies ou evoluções.
        GenerationVFormsInitializer.Inicializar(db);
        db.SaveChanges();

        // Atualiza altura e peso somente depois de todas as formas já existirem.
        PokemonMeasurementsInitializer.Inicializar(db);

        transaction.Commit();
    }

    /// <summary>
    /// Acrescenta colunas de condições evolutivas a bancos SQLite já existentes.
    /// EnsureCreated não altera tabelas preexistentes, por isso esta atualização
    /// incremental preserva os registros da Pokédex e das equipes.
    /// </summary>
    private static void EnsureEvolutionSchema(AppDbContext db)
    {
        var connection = db.Database.GetDbConnection();
        bool openedHere = connection.State != ConnectionState.Open;
        if (openedHere)
            connection.Open();

        try
        {
            var existingColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var inspect = connection.CreateCommand())
            {
                inspect.CommandText = "PRAGMA table_info('Evolutions');";
                using var reader = inspect.ExecuteReader();
                while (reader.Read())
                    existingColumns.Add(reader.GetString(1));
            }

            (string Name, string SqlType)[] columns =
            {
                ("RequiredHeldItem", "TEXT NULL"),
                ("RequiredPartySpeciesId", "INTEGER NULL"),
                ("RequiredTradeSpeciesId", "INTEGER NULL"),
                ("RequiredPartyType", "TEXT NULL"),
                ("RequiredMoveType", "TEXT NULL"),
                ("RelativePhysicalStats", "TEXT NULL"),
                ("MinimumAffection", "INTEGER NULL"),
                ("MinimumSteps", "INTEGER NULL"),
                ("MinimumMoveCount", "INTEGER NULL"),
                ("NeedsOverworldRain", "INTEGER NOT NULL DEFAULT 0"),
                ("TurnUpsideDown", "INTEGER NOT NULL DEFAULT 0"),
                ("RequiresEmptyPartySlot", "INTEGER NOT NULL DEFAULT 0"),
                ("RequiresPokeBallInBag", "INTEGER NOT NULL DEFAULT 0")
            };

            foreach (var column in columns)
            {
                if (existingColumns.Contains(column.Name))
                    continue;

                using var alter = connection.CreateCommand();
                alter.CommandText = $"ALTER TABLE \"Evolutions\" ADD COLUMN \"{column.Name}\" {column.SqlType};";
                alter.ExecuteNonQuery();
            }

            using (var partyIndex = connection.CreateCommand())
            {
                partyIndex.CommandText = "CREATE INDEX IF NOT EXISTS IX_Evolutions_RequiredPartySpeciesId ON Evolutions (RequiredPartySpeciesId);";
                partyIndex.ExecuteNonQuery();
            }
            using (var tradeIndex = connection.CreateCommand())
            {
                tradeIndex.CommandText = "CREATE INDEX IF NOT EXISTS IX_Evolutions_RequiredTradeSpeciesId ON Evolutions (RequiredTradeSpeciesId);";
                tradeIndex.ExecuteNonQuery();
            }

            // IDs de espécies são validados pelo banco sem reconstruir a tabela antiga.
            using (var insertTrigger = connection.CreateCommand())
            {
                insertTrigger.CommandText =
                    "CREATE TRIGGER IF NOT EXISTS TR_Evolutions_ValidateSpecies_Insert " +
                    "BEFORE INSERT ON Evolutions " +
                    "WHEN (NEW.RequiredPartySpeciesId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Pokemons WHERE Id = NEW.RequiredPartySpeciesId)) " +
                    "OR (NEW.RequiredTradeSpeciesId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Pokemons WHERE Id = NEW.RequiredTradeSpeciesId)) " +
                    "BEGIN SELECT RAISE(ABORT, 'Evolution references an unknown Pokemon'); END;";
                insertTrigger.ExecuteNonQuery();
            }
            using (var updateTrigger = connection.CreateCommand())
            {
                updateTrigger.CommandText =
                    "CREATE TRIGGER IF NOT EXISTS TR_Evolutions_ValidateSpecies_Update " +
                    "BEFORE UPDATE OF RequiredPartySpeciesId, RequiredTradeSpeciesId ON Evolutions " +
                    "WHEN (NEW.RequiredPartySpeciesId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Pokemons WHERE Id = NEW.RequiredPartySpeciesId)) " +
                    "OR (NEW.RequiredTradeSpeciesId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Pokemons WHERE Id = NEW.RequiredTradeSpeciesId)) " +
                    "BEGIN SELECT RAISE(ABORT, 'Evolution references an unknown Pokemon'); END;";
                updateTrigger.ExecuteNonQuery();
            }
        }
        finally
        {
            if (openedHere)
                connection.Close();
        }
    }

    private static void SeedRegions(AppDbContext db)
    {
        Region[] regions =
        {
            new() { Id = 1, Name = "Kanto", Generation = 1 },
            new() { Id = 2, Name = "Johto", Generation = 2 },
            new() { Id = 3, Name = "Hoenn", Generation = 3 },
            new() { Id = 4, Name = "Sinnoh", Generation = 4 },
            new() { Id = 5, Name = "Unova", Generation = 5 }
        };

        foreach (Region region in regions)
        {
            Region? existing = db.Regions.SingleOrDefault(item => item.Id == region.Id);
            if (existing is null)
            {
                db.Regions.Add(region);
                continue;
            }

            existing.Name = region.Name;
            existing.Generation = region.Generation;
        }
    }

    /// <summary>
    /// Corrige a entrada #358, que em uma versão anterior estava cadastrada
    /// incorretamente como Chingling. #358 é Chimecho (Hoenn); Chingling é #433 (Sinnoh).
    /// Não remove Sinnoh, pois a região faz parte da Pokédex ampliada até Unova.
    /// </summary>
    private static void NormalizeLegacyPokedexData(AppDbContext db)
    {
        Region hoenn = db.Regions.Single(region => region.Id == 3);

        Pokemon? chimecho = db.Pokemons
            .Include(pokemon => pokemon.Forms)
            .SingleOrDefault(pokemon => pokemon.NationalDexNumber == 358);

        if (chimecho is null)
            return;

        chimecho.Name = "Chimecho";
        chimecho.Generation = 3;
        chimecho.OriginRegionId = hoenn.Id;
        chimecho.Category = "Wind Chime";

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

    private static List<PokemonSeed> LoadPokemonSeeds()
    {
        using Stream stream = typeof(DatabaseInitializer).Assembly.GetManifestResourceStream(
            "GestaoDePokemon.Data.Persistence.Seed.pokemon-seed.json")
            ?? throw new InvalidOperationException(
                "O arquivo incorporado Database/Seed/pokemon-seed.json não foi encontrado.");

        return JsonSerializer.Deserialize<List<PokemonSeed>>(stream, SeedJsonOptions)
            ?? throw new InvalidOperationException("O arquivo de dados iniciais está vazio ou inválido.");
    }

    private static void SeedPokemons(AppDbContext db, IReadOnlyList<PokemonSeed> seeds)
    {
        var existingDexNumbers = db.Pokemons
            .Select(pokemon => pokemon.NationalDexNumber)
            .ToHashSet();

        var newPokemons = new List<Pokemon>();

        foreach (PokemonSeed seed in seeds)
        {
            if (!existingDexNumbers.Add(seed.NationalDexNumber))
                continue;

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
            newPokemons.Add(pokemon);
        }

        if (newPokemons.Count > 0)
            db.Pokemons.AddRange(newPokemons);
    }

    private static void EnsureBaseStats(AppDbContext db, IReadOnlyList<PokemonSeed> seeds)
    {
        var species = db.Pokemons
            .Include(pokemon => pokemon.Forms)
            .ToDictionary(pokemon => pokemon.NationalDexNumber);

        foreach (PokemonSeed seed in seeds)
        {
            if (!species.TryGetValue(seed.NationalDexNumber, out Pokemon? pokemon))
                continue;

            PokemonForm? form = pokemon.Forms.FirstOrDefault(existing =>
                existing.FormName == seed.Form.FormName);

            if (form is null)
            {
                form = new PokemonForm
                {
                    Pokemon = pokemon,
                    PokemonId = pokemon.Id,
                    FormName = seed.Form.FormName,
                    Stats = seed.Form.Stats
                };

                foreach (PokemonType type in seed.Form.Types.Distinct())
                    form.AddType(type);

                pokemon.Forms.Add(form);
                continue;
            }

            if (!HasValidBaseStats(form.Stats))
                form.Stats = seed.Form.Stats;

            if (form.Types.Count == 0)
            {
                foreach (PokemonType type in seed.Form.Types.Distinct())
                    form.AddType(type);
            }
        }
    }

    private static bool HasValidBaseStats(BaseStats stats) =>
        stats.HP > 0 && stats.Atk > 0 && stats.Def > 0 &&
        stats.SpAtk > 0 && stats.SpDef > 0 && stats.Speed > 0;

    /// <summary>
    /// Importa o catálogo de movimentos e os learnsets gerados a partir dos CSVs
    /// da PokeAPI. Os arquivos são opcionais até o usuário executar o gerador;
    /// se apenas um deles existir, a inicialização falha com mensagem explícita.
    /// </summary>
    private static void SeedMoveCatalogAndLearnsets(AppDbContext db)
    {
        string seedDirectory = Path.Combine(AppContext.BaseDirectory, "Data", "Persistence", "Seed");
        string moveCatalogPath = Path.Combine(seedDirectory, "move-catalog-seed.json");
        string learnsetsPath = Path.Combine(seedDirectory, "pokemon-learnsets-seed.json");

        bool hasMoveCatalog = File.Exists(moveCatalogPath);
        bool hasLearnsets = File.Exists(learnsetsPath);

        if (!hasMoveCatalog && !hasLearnsets)
            return;

        if (!hasMoveCatalog || !hasLearnsets)
        {
            throw new InvalidOperationException(
                "Os seeds de movimentos estão incompletos. Execute " +
                "python3 scripts/gerar_seed_moves.py para gerar os dois arquivos JSON.");
        }

        List<PokemonMoveSeed> moveSeeds;
        using (FileStream stream = File.OpenRead(moveCatalogPath))
        {
            moveSeeds = JsonSerializer.Deserialize<List<PokemonMoveSeed>>(stream, SeedJsonOptions)
                ?? throw new InvalidDataException("O catálogo de movimentos está vazio ou inválido.");
        }

        List<PokemonLearnsetSeed> learnsetSeeds;
        using (FileStream stream = File.OpenRead(learnsetsPath))
        {
            learnsetSeeds = JsonSerializer.Deserialize<List<PokemonLearnsetSeed>>(stream, SeedJsonOptions)
                ?? throw new InvalidDataException("O seed de learnsets está vazio ou inválido.");
        }

        if (moveSeeds.Count == 0 || learnsetSeeds.Count == 0)
            throw new InvalidDataException("Os arquivos de movimentos não contêm registros.");

        // Upsert do catálogo pelo ID original da PokeAPI.
        var movesById = db.Moves.ToDictionary(move => move.Id);
        foreach (PokemonMoveSeed seed in moveSeeds)
        {
            if (!Enum.IsDefined(seed.Type) || !Enum.IsDefined(seed.Category))
                throw new InvalidDataException($"Tipo/categoria inválidos no movimento #{seed.Id}.");

            if (movesById.TryGetValue(seed.Id, out PokemonMove? existing))
            {
                existing.Name = seed.Name;
                existing.Type = seed.Type;
                existing.Category = seed.Category;
                existing.Power = seed.Power;
                existing.Accuracy = seed.Accuracy;
                existing.PP = seed.PP;
            }
            else
            {
                var move = new PokemonMove
                {
                    Id = seed.Id,
                    Name = seed.Name,
                    Type = seed.Type,
                    Category = seed.Category,
                    Power = seed.Power,
                    Accuracy = seed.Accuracy,
                    PP = seed.PP
                };
                db.Moves.Add(move);
                movesById.Add(move.Id, move);
            }
        }

        db.SaveChanges();

        var formsByDex = db.PokemonForms
            .Include(form => form.Pokemon)
            .Where(form => form.FormName == "Normal")
            .ToDictionary(form => form.Pokemon.NationalDexNumber);

        var existingLearnsets = db.LearnableMoves
            .AsNoTracking()
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

        foreach (PokemonLearnsetSeed seed in learnsetSeeds)
        {
            if (!formsByDex.TryGetValue(seed.NationalDexNumber, out PokemonForm? form))
                throw new InvalidDataException(
                    $"Não existe forma Normal para o Pokémon #{seed.NationalDexNumber:000}.");

            if (!movesById.TryGetValue(seed.MoveId, out PokemonMove? move))
                throw new InvalidDataException(
                    $"O learnset referencia o movimento inexistente #{seed.MoveId}.");

            if (!Enum.IsDefined(seed.Method))
                throw new InvalidDataException(
                    $"Método de aprendizagem inválido para o movimento #{seed.MoveId}.");

            var key = new LearnsetKey(
                form.Id,
                seed.MoveId,
                seed.Method,
                seed.Level,
                seed.GameVersion);

            if (!existingLearnsets.Add(key))
                continue;

            db.LearnableMoves.Add(new LearnableMove
            {
                PokemonFormId = form.Id,
                PokemonForm = form,
                MoveId = move.Id,
                Move = move,
                Method = seed.Method,
                Level = seed.Level,
                GameVersion = seed.GameVersion
            });
        }
    }

    /// <summary>
    /// Importa as relações diretas de evolução a partir do JSON gerado por
    /// scripts/gerar_seed_evolucoes.py. O seed é opcional até ser gerado.
    /// </summary>
    /// <summary>
    /// Reconstrói o catálogo informativo de evoluções a partir do seed completo.
    /// Não executa evoluções nem valida builds; apenas armazena suas condições.
    /// </summary>
    private static void SeedEvolutions(AppDbContext db)
    {
        string seedPath = Path.Combine(
            AppContext.BaseDirectory, "Data", "Persistence", "Seed", "pokemon-evolutions-seed.json");
        if (!File.Exists(seedPath))
            return;

        List<PokemonEvolutionSeed> seeds;
        using (FileStream stream = File.OpenRead(seedPath))
        {
            seeds = JsonSerializer.Deserialize<List<PokemonEvolutionSeed>>(stream, SeedJsonOptions)
                ?? throw new InvalidDataException("O seed de evoluções está vazio ou inválido.");
        }
        if (seeds.Count == 0)
            throw new InvalidDataException("O seed de evoluções não contém relações.");

        var pokemonsByDex = db.Pokemons.ToDictionary(pokemon => pokemon.NationalDexNumber);
        var moveIds = db.Moves.Select(move => move.Id).ToHashSet();

        // Evoluções são dados de referência importados do catálogo externo.
        // Substituir o conjunto evita manter linhas antigas sem as novas condições.
        db.Evolutions.RemoveRange(db.Evolutions);
        db.SaveChanges();

        var importedKeys = new HashSet<EvolutionKey>();
        foreach (PokemonEvolutionSeed seed in seeds)
        {
            if (!Enum.IsDefined(seed.Method))
                throw new InvalidDataException($"Método de evolução inválido entre #{seed.FromNationalDexNumber:000} e #{seed.ToNationalDexNumber:000}.");
            if (!pokemonsByDex.TryGetValue(seed.FromNationalDexNumber, out Pokemon? fromPokemon))
                throw new InvalidDataException($"Pokémon de origem #{seed.FromNationalDexNumber:000} não encontrado.");
            if (!pokemonsByDex.TryGetValue(seed.ToNationalDexNumber, out Pokemon? toPokemon))
                throw new InvalidDataException($"Pokémon de destino #{seed.ToNationalDexNumber:000} não encontrado.");
            if (seed.FromNationalDexNumber == seed.ToNationalDexNumber)
                throw new InvalidDataException("Uma espécie não pode evoluir para si mesma.");
            if (seed.RequiredLevel is < 1 or > 100)
                throw new InvalidDataException("Nível de evolução deve estar entre 1 e 100.");
            if (seed.MinimumFriendship is < 0 or > 255 || seed.MinimumBeauty is < 0 or > 255 || seed.MinimumAffection is < 0)
                throw new InvalidDataException("Amizade, beleza ou afeição mínima inválida.");
            if (seed.MinimumSteps is < 0 || seed.MinimumMoveCount is < 0)
                throw new InvalidDataException("Quantidade mínima de passos ou movimentos inválida.");
            if (seed.RequiredTime.HasValue && !Enum.IsDefined(seed.RequiredTime.Value))
                throw new InvalidDataException("Horário de evolução inválido.");
            if (seed.RequiredGender.HasValue && !Enum.IsDefined(seed.RequiredGender.Value))
                throw new InvalidDataException("Gênero de evolução inválido.");
            if (seed.RelativePhysicalStats.HasValue && !Enum.IsDefined(seed.RelativePhysicalStats.Value))
                throw new InvalidDataException("Comparação entre Attack e Defense inválida.");
            if (seed.RequiredPartyType.HasValue && !Enum.IsDefined(seed.RequiredPartyType.Value))
                throw new InvalidDataException("Tipo exigido na equipe inválido.");
            if (seed.RequiredMoveType.HasValue && !Enum.IsDefined(seed.RequiredMoveType.Value))
                throw new InvalidDataException("Tipo de movimento exigido inválido.");
            if (seed.RequiredMoveId.HasValue && !moveIds.Contains(seed.RequiredMoveId.Value))
                throw new InvalidDataException($"Movimento #{seed.RequiredMoveId.Value} exigido pela evolução não existe na tabela Moves.");

            int? partySpeciesId = null;
            if (seed.RequiredPartySpeciesNationalDexNumber.HasValue)
            {
                if (!pokemonsByDex.TryGetValue(seed.RequiredPartySpeciesNationalDexNumber.Value, out Pokemon? partySpecies))
                    throw new InvalidDataException($"Espécie exigida na equipe #{seed.RequiredPartySpeciesNationalDexNumber.Value:000} não encontrada.");
                partySpeciesId = partySpecies.Id;
            }

            int? tradeSpeciesId = null;
            if (seed.RequiredTradeSpeciesNationalDexNumber.HasValue)
            {
                if (!pokemonsByDex.TryGetValue(seed.RequiredTradeSpeciesNationalDexNumber.Value, out Pokemon? tradeSpecies))
                    throw new InvalidDataException($"Espécie exigida na troca #{seed.RequiredTradeSpeciesNationalDexNumber.Value:000} não encontrada.");
                tradeSpeciesId = tradeSpecies.Id;
            }

            var key = new EvolutionKey(
                fromPokemon.Id, toPokemon.Id, seed.Method, seed.RequiredLevel,
                seed.RequiredItem ?? string.Empty, seed.RequiredTime,
                seed.MinimumFriendship, seed.MinimumBeauty, seed.RequiredGender,
                seed.RequiredLocation ?? string.Empty, seed.RequiredMoveId,
                seed.RequiredHeldItem ?? string.Empty, partySpeciesId, tradeSpeciesId,
                seed.RequiredPartyType, seed.RequiredMoveType, seed.RelativePhysicalStats,
                seed.MinimumAffection, seed.MinimumSteps, seed.MinimumMoveCount,
                seed.NeedsOverworldRain, seed.TurnUpsideDown,
                seed.RequiresEmptyPartySlot, seed.RequiresPokeBallInBag,
                seed.GameVersion ?? string.Empty);

            if (!importedKeys.Add(key))
                continue;

            db.Evolutions.Add(new PokemonEvolution
            {
                FromPokemonId = fromPokemon.Id,
                ToPokemonId = toPokemon.Id,
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
            });
        }
    }

    private readonly record struct EvolutionKey(
        int FromPokemonId, int ToPokemonId, EvolutionMethod Method,
        int? RequiredLevel, string RequiredItem, EvolutionTime? RequiredTime,
        int? MinimumFriendship, int? MinimumBeauty, PokemonGender? RequiredGender,
        string RequiredLocation, int? RequiredMoveId, string RequiredHeldItem,
        int? RequiredPartySpeciesId, int? RequiredTradeSpeciesId,
        PokemonType? RequiredPartyType, PokemonType? RequiredMoveType,
        RelativePhysicalStats? RelativePhysicalStats, int? MinimumAffection,
        int? MinimumSteps, int? MinimumMoveCount, bool NeedsOverworldRain,
        bool TurnUpsideDown, bool RequiresEmptyPartySlot,
        bool RequiresPokeBallInBag, string GameVersion);

    public sealed class PokemonEvolutionSeed
    {
        public int FromNationalDexNumber { get; set; }
        public int ToNationalDexNumber { get; set; }
        public EvolutionMethod Method { get; set; }
        public int? RequiredLevel { get; set; }
        public string? RequiredItem { get; set; }
        public EvolutionTime? RequiredTime { get; set; }
        public int? MinimumFriendship { get; set; }
        public int? MinimumBeauty { get; set; }
        public PokemonGender? RequiredGender { get; set; }
        public string? RequiredLocation { get; set; }
        public int? RequiredMoveId { get; set; }
        public string? GameVersion { get; set; }
        public string? RequiredHeldItem { get; set; }
        public int? RequiredPartySpeciesNationalDexNumber { get; set; }
        public int? RequiredTradeSpeciesNationalDexNumber { get; set; }
        public PokemonType? RequiredPartyType { get; set; }
        public PokemonType? RequiredMoveType { get; set; }
        public RelativePhysicalStats? RelativePhysicalStats { get; set; }
        public int? MinimumAffection { get; set; }
        public int? MinimumSteps { get; set; }
        public int? MinimumMoveCount { get; set; }
        public bool NeedsOverworldRain { get; set; }
        public bool TurnUpsideDown { get; set; }
        public bool RequiresEmptyPartySlot { get; set; }
        public bool RequiresPokeBallInBag { get; set; }
    }

    private readonly record struct LearnsetKey(
        int PokemonFormId,
        int MoveId,
        MoveLearnMethod Method,
        int? Level,
        string GameVersion);

    public sealed class PokemonMoveSeed
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PokemonType Type { get; set; }
        public MoveCategory Category { get; set; }
        public int? Power { get; set; }
        public int? Accuracy { get; set; }
        public int PP { get; set; }
    }

    public sealed class PokemonLearnsetSeed
    {
        public int NationalDexNumber { get; set; }
        public int MoveId { get; set; }
        public MoveLearnMethod Method { get; set; }
        public int? Level { get; set; }
        public string GameVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Importa habilidades e associações de todas as espécies presentes no seed.
    /// Atualiza relações por forma sem remover habilidades que não estejam no seed
    /// de outras formas. A operação ocorre dentro da transação de Inicializar().
    /// </summary>
    private static void SeedAbilities(AppDbContext db)
    {
        string seedPath = Path.Combine(AppContext.BaseDirectory, "Data", "Persistence", "Seed", "abilities-seed.json");
        if (!File.Exists(seedPath))
            throw new FileNotFoundException("Seed de habilidades ausente. Execute scripts/ampliar_pokedex_unova.py.", seedPath);

        AbilityCatalogSeed seed;
        using (FileStream stream = File.OpenRead(seedPath))
        {
            seed = JsonSerializer.Deserialize<AbilityCatalogSeed>(stream, SeedJsonOptions)
                ?? throw new InvalidDataException("Seed de habilidades vazio ou inválido.");
        }
        if (seed.Abilities.Count == 0 || seed.Pokemon.Count == 0)
            throw new InvalidDataException("O seed de habilidades não contém catálogo ou associações.");

        var abilitiesByName = db.Abilities
            .ToList()
            .GroupBy(item => item.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var abilitiesBySlug = new Dictionary<string, PokemonAbility>(StringComparer.OrdinalIgnoreCase);

        foreach ((string slug, AbilitySeedEntry entry) in seed.Abilities)
        {
            if (entry.Id <= 0 || string.IsNullOrWhiteSpace(entry.Name))
                throw new InvalidDataException($"Habilidade inválida no seed: {slug}.");

            string name = entry.Name.Trim();
            if (abilitiesByName.TryGetValue(name, out PokemonAbility? existing))
            {
                existing.Description = entry.Description ?? string.Empty;
                abilitiesBySlug.Add(slug, existing);
                continue;
            }

            bool idInUse = db.Abilities.Local.Any(item => item.Id == entry.Id) ||
                           db.Abilities.Any(item => item.Id == entry.Id);
            var ability = new PokemonAbility
            {
                Id = idInUse ? 0 : entry.Id,
                Name = name,
                Description = entry.Description ?? string.Empty
            };
            db.Abilities.Add(ability);
            abilitiesByName.Add(name, ability);
            abilitiesBySlug.Add(slug, ability);
        }
        db.SaveChanges();

        int[] dexNumbers = seed.Pokemon.Select(item => item.NationalDexNumber).Distinct().ToArray();
        var formsByDex = db.PokemonForms
            .Include(form => form.Pokemon)
            .Where(form => form.FormName == "Normal" && dexNumbers.Contains(form.Pokemon.NationalDexNumber))
            .ToDictionary(form => form.Pokemon.NationalDexNumber);
        int[] formIds = formsByDex.Values.Select(form => form.Id).ToArray();
        var oldAssociations = db.FormAbilities.Where(item => formIds.Contains(item.PokemonFormId)).ToList();
        db.FormAbilities.RemoveRange(oldAssociations);
        db.SaveChanges(); // libera o índice único (PokemonFormId, Slot) antes de reinserir

        foreach (AbilityPokemonSeed pokemonSeed in seed.Pokemon)
        {
            if (!formsByDex.TryGetValue(pokemonSeed.NationalDexNumber, out PokemonForm? form))
                throw new InvalidDataException($"Forma Normal ausente para #{pokemonSeed.NationalDexNumber:000}.");

            var usedSlots = new HashSet<AbilitySlot>();
            foreach (AbilitySlotSeed association in pokemonSeed.Abilities)
            {
                if (!abilitiesBySlug.TryGetValue(association.Ability, out PokemonAbility? ability))
                    throw new InvalidDataException($"Habilidade '{association.Ability}' não encontrada no catálogo.");

                AbilitySlot slot = association.SlotNumber switch
                {
                    1 => AbilitySlot.First,
                    2 => AbilitySlot.Second,
                    3 => AbilitySlot.Hidden,
                    _ => throw new InvalidDataException($"Slot inválido no Pokémon #{pokemonSeed.NationalDexNumber:000}.")
                };
                if (!usedSlots.Add(slot))
                    throw new InvalidDataException($"Slot de habilidade duplicado em #{pokemonSeed.NationalDexNumber:000}.");

                db.FormAbilities.Add(new PokemonFormAbility
                {
                    PokemonFormId = form.Id,
                    PokemonForm = form,
                    AbilityId = ability.Id,
                    Ability = ability,
                    Slot = slot
                });
            }
        }
        db.SaveChanges();
    }

    public sealed class AbilityCatalogSeed
    {
        public Dictionary<string, AbilitySeedEntry> Abilities { get; set; } = new();
        public List<AbilityPokemonSeed> Pokemon { get; set; } = new();
    }

    public sealed class AbilitySeedEntry
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Generation { get; set; }
    }

    public sealed class AbilityPokemonSeed
    {
        [JsonPropertyName("national_dex_number")]
        public int NationalDexNumber { get; set; }

        public List<AbilitySlotSeed> Abilities { get; set; } = new();
    }

    public sealed class AbilitySlotSeed
    {
        public string Ability { get; set; } = string.Empty;
        public int Slot { get; set; }

        [JsonPropertyName("slot_number")]
        public int SlotNumber { get; set; }
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
        4 => 4, // Sinnoh
        5 => 5, // Unova
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
