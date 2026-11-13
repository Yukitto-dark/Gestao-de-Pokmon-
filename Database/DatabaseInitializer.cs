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

        List<PokemonSeed> pokemonSeeds = LoadPokemonSeeds();

        if (!db.Pokemons.Any())
        {
            SeedPokemons(db, pokemonSeeds);
            db.SaveChanges();
        }

        // BaseStats já faz parte de PokemonForm. Esta rotina apenas repara
        // formas existentes que estejam sem valores válidos, sem sobrescrever
        // atributos que já foram cadastrados corretamente.
        EnsureBaseStats(db, pokemonSeeds);
        db.SaveChanges();

        // Corrige dados herdados da versão anterior e remove a região
        // Sinnoh, que não pertence ao recorte atual da Pokédex (1 a 386).
        NormalizeLegacyPokedexData(db);

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

    private static List<PokemonSeed> LoadPokemonSeeds()
    {
        using Stream stream = typeof(DatabaseInitializer).Assembly.GetManifestResourceStream(
            "GestaoDePokemon.Database.Seed.pokemon-seed.json")
            ?? throw new InvalidOperationException(
                "O arquivo incorporado Database/Seed/pokemon-seed.json não foi encontrado.");

        return JsonSerializer.Deserialize<List<PokemonSeed>>(stream, SeedJsonOptions)
            ?? throw new InvalidOperationException("O arquivo de dados iniciais está vazio ou inválido.");
    }

    private static void SeedPokemons(AppDbContext db, IReadOnlyList<PokemonSeed> seeds)
    {
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
        string seedDirectory = Path.Combine(AppContext.BaseDirectory, "Database", "Seed");
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
    private static void SeedEvolutions(AppDbContext db)
    {
        string seedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Database",
            "Seed",
            "pokemon-evolutions-seed.json");

        if (!File.Exists(seedPath))
            return;

        List<PokemonEvolutionSeed> seeds;
        using (FileStream stream = File.OpenRead(seedPath))
        {
            seeds = JsonSerializer.Deserialize<List<PokemonEvolutionSeed>>(stream, SeedJsonOptions)
                ?? throw new InvalidDataException("O seed de evoluções está vazio ou inválido.");
        }

        if (seeds.Count == 0)
            throw new InvalidDataException("O seed de evoluções não contém relacionamentos.");

        var pokemonsByDex = db.Pokemons.ToDictionary(pokemon => pokemon.NationalDexNumber);
        var moveIds = db.Moves.Select(move => move.Id).ToHashSet();

        var existing = db.Evolutions
            .AsNoTracking()
            .Select(evolution => new
            {
                evolution.FromPokemonId,
                evolution.ToPokemonId,
                evolution.Method,
                evolution.RequiredLevel,
                evolution.RequiredItem,
                evolution.RequiredTime,
                evolution.MinimumFriendship,
                evolution.MinimumBeauty,
                evolution.RequiredGender,
                evolution.RequiredLocation,
                evolution.RequiredMoveId,
                evolution.GameVersion
            })
            .AsEnumerable()
            .Select(evolution => new EvolutionKey(
                evolution.FromPokemonId,
                evolution.ToPokemonId,
                evolution.Method,
                evolution.RequiredLevel,
                evolution.RequiredItem ?? string.Empty,
                evolution.RequiredTime,
                evolution.MinimumFriendship,
                evolution.MinimumBeauty,
                evolution.RequiredGender,
                evolution.RequiredLocation ?? string.Empty,
                evolution.RequiredMoveId,
                evolution.GameVersion ?? string.Empty))
            .ToHashSet();

        foreach (PokemonEvolutionSeed seed in seeds)
        {
            if (!Enum.IsDefined(seed.Method))
                throw new InvalidDataException(
                    $"Método de evolução inválido entre #{seed.FromNationalDexNumber:000} " +
                    $"e #{seed.ToNationalDexNumber:000}.");

            if (!pokemonsByDex.TryGetValue(seed.FromNationalDexNumber, out Pokemon? fromPokemon))
                throw new InvalidDataException(
                    $"Pokémon de origem #{seed.FromNationalDexNumber:000} não encontrado.");

            if (!pokemonsByDex.TryGetValue(seed.ToNationalDexNumber, out Pokemon? toPokemon))
                throw new InvalidDataException(
                    $"Pokémon de destino #{seed.ToNationalDexNumber:000} não encontrado.");

            if (seed.FromNationalDexNumber == seed.ToNationalDexNumber)
                throw new InvalidDataException("Uma espécie não pode evoluir para si mesma.");

            if (seed.RequiredLevel is < 1 or > 100)
                throw new InvalidDataException("Nível de evolução deve estar entre 1 e 100.");

            if (seed.MinimumFriendship is < 0 or > 255 || seed.MinimumBeauty is < 0 or > 255)
                throw new InvalidDataException("Amizade/beleza de evolução deve estar entre 0 e 255.");

            if (seed.RequiredTime.HasValue && !Enum.IsDefined(seed.RequiredTime.Value))
                throw new InvalidDataException("Horário de evolução inválido.");

            if (seed.RequiredGender.HasValue && !Enum.IsDefined(seed.RequiredGender.Value))
                throw new InvalidDataException("Gênero de evolução inválido.");

            if (seed.RequiredMoveId.HasValue && !moveIds.Contains(seed.RequiredMoveId.Value))
                throw new InvalidDataException(
                    $"A evolução de #{seed.FromNationalDexNumber:000} para " +
                    $"#{seed.ToNationalDexNumber:000} exige o movimento #{seed.RequiredMoveId}, " +
                    "mas ele não existe na tabela Moves. Gere/import os movimentos antes das evoluções.");

            var key = new EvolutionKey(
                fromPokemon.Id,
                toPokemon.Id,
                seed.Method,
                seed.RequiredLevel,
                seed.RequiredItem ?? string.Empty,
                seed.RequiredTime,
                seed.MinimumFriendship,
                seed.MinimumBeauty,
                seed.RequiredGender,
                seed.RequiredLocation ?? string.Empty,
                seed.RequiredMoveId,
                seed.GameVersion ?? string.Empty);

            if (!existing.Add(key))
                continue;

            db.Evolutions.Add(new PokemonEvolution
            {
                FromPokemonId = fromPokemon.Id,
                ToPokemonId = toPokemon.Id,
                Method = seed.Method,
                RequiredLevel = seed.RequiredLevel,
                RequiredItem = seed.RequiredItem,
                RequiredTime = seed.RequiredTime,
                MinimumFriendship = seed.MinimumFriendship,
                MinimumBeauty = seed.MinimumBeauty,
                RequiredGender = seed.RequiredGender,
                RequiredLocation = seed.RequiredLocation,
                RequiredMoveId = seed.RequiredMoveId,
                GameVersion = seed.GameVersion
            });
        }
    }

    private readonly record struct EvolutionKey(
        int FromPokemonId,
        int ToPokemonId,
        EvolutionMethod Method,
        int? RequiredLevel,
        string RequiredItem,
        EvolutionTime? RequiredTime,
        int? MinimumFriendship,
        int? MinimumBeauty,
        PokemonGender? RequiredGender,
        string RequiredLocation,
        int? RequiredMoveId,
        string GameVersion);

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
