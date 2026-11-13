using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GestaoDePokemon.Database;

/// <summary>
/// Contexto único de persistência. Todas as entidades utilizadas aqui vêm de
/// GestaoDePokemon.model; não existem cópias das entidades dentro de Database.
/// </summary>
public sealed class AppDbContext : DbContext
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public DbSet<Pokemon> Pokemons => Set<Pokemon>();
    public DbSet<PokemonForm> PokemonForms => Set<PokemonForm>();
    public DbSet<PokemonMove> Moves => Set<PokemonMove>();
    public DbSet<LearnableMove> LearnableMoves => Set<LearnableMove>();
    public DbSet<PokemonAbility> Abilities => Set<PokemonAbility>();
    public DbSet<PokemonFormAbility> FormAbilities => Set<PokemonFormAbility>();
    public DbSet<PokemonSprite> Sprites => Set<PokemonSprite>();
    public DbSet<PokemonEvolution> Evolutions => Set<PokemonEvolution>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<PokemonTeam> Teams => Set<PokemonTeam>();
    public DbSet<PokemonBuild> Builds => Set<PokemonBuild>();
    public DbSet<PokemonFavorite> Favorites => Set<PokemonFavorite>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string databaseDirectory = Path.Combine(appData, "GestaoDePokemon");
        Directory.CreateDirectory(databaseDirectory);

        string databasePath = Path.Combine(databaseDirectory, "gestaopokemon.db");
        optionsBuilder.UseSqlite($"Data Source={databasePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurePokemon(modelBuilder);
        ConfigureForms(modelBuilder);
        ConfigureMoves(modelBuilder);
        ConfigureAbilities(modelBuilder);
        ConfigureEvolutions(modelBuilder);
        ConfigureRegions(modelBuilder);
        ConfigureTeams(modelBuilder);
        ConfigureFavorites(modelBuilder);
    }

    private static void ConfigurePokemon(ModelBuilder modelBuilder)
    {
        var pokemon = modelBuilder.Entity<Pokemon>();
        pokemon.HasKey(p => p.Id);
        pokemon.HasIndex(p => p.NationalDexNumber).IsUnique();
        pokemon.Property(p => p.Name).HasMaxLength(100).IsRequired();
        pokemon.Property(p => p.Category).HasMaxLength(100);
        pokemon.Ignore(p => p.EggGroups);

        pokemon.HasOne(p => p.OriginRegion)
            .WithMany()
            .HasForeignKey(p => p.OriginRegionId)
            .OnDelete(DeleteBehavior.Restrict);

        pokemon.HasMany(p => p.Forms)
            .WithOne(form => form.Pokemon)
            .HasForeignKey(form => form.PokemonId)
            .OnDelete(DeleteBehavior.Cascade);

        pokemon.HasMany(p => p.Evolutions)
            .WithOne(evolution => evolution.FromPokemon)
            .HasForeignKey(evolution => evolution.FromPokemonId)
            .OnDelete(DeleteBehavior.Restrict);

        pokemon.HasMany(p => p.PreEvolutions)
            .WithOne(evolution => evolution.ToPokemon)
            .HasForeignKey(evolution => evolution.ToPokemonId)
            .OnDelete(DeleteBehavior.Restrict);

        pokemon.HasMany(p => p.FoundInRegions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "PokemonFoundInRegion",
                right => right.HasOne<Region>()
                    .WithMany()
                    .HasForeignKey("RegionId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Pokemon>()
                    .WithMany()
                    .HasForeignKey("PokemonId")
                    .OnDelete(DeleteBehavior.Cascade),
                join => join.HasKey("PokemonId", "RegionId"));

        pokemon.Navigation(p => p.Evolutions)
            .HasField("_evolutions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        pokemon.Navigation(p => p.PreEvolutions)
            .HasField("_preEvolutions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        pokemon.Navigation(p => p.FoundInRegions)
            .HasField("_foundInRegions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        var eggGroups = pokemon.Property<List<EggGroup>>("_eggGroups")
            .HasColumnName("EggGroups");
        eggGroups.HasConversion(CreateJsonListConverter<EggGroup>());
        eggGroups.Metadata.SetValueComparer(CreateListComparer<EggGroup>());
    }

    private static void ConfigureForms(ModelBuilder modelBuilder)
    {
        var form = modelBuilder.Entity<PokemonForm>();
        form.HasKey(p => p.Id);
        form.Ignore(p => p.Types);
        form.Property(p => p.FormName).HasMaxLength(100).IsRequired();

        var types = form.Property<List<PokemonType>>("_types")
            .HasColumnName("Types");
        types.HasConversion(CreateJsonListConverter<PokemonType>());
        types.Metadata.SetValueComparer(CreateListComparer<PokemonType>());

        form.Property(p => p.Stats)
            .HasConversion(CreateJsonConverter<BaseStats>());

        modelBuilder.Entity<PokemonSprite>()
            .Property(sprite => sprite.Gender)
            .HasConversion<string>();

        form.HasMany(p => p.Sprites)
            .WithOne(sprite => sprite.PokemonForm)
            .HasForeignKey(sprite => sprite.PokemonFormId)
            .OnDelete(DeleteBehavior.Cascade);

        form.HasMany(p => p.Abilities)
            .WithOne(ability => ability.PokemonForm)
            .HasForeignKey(ability => ability.PokemonFormId)
            .OnDelete(DeleteBehavior.Cascade);

        form.HasMany(p => p.Learnset)
            .WithOne(learnable => learnable.PokemonForm)
            .HasForeignKey(learnable => learnable.PokemonFormId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureMoves(ModelBuilder modelBuilder)
    {
        var move = modelBuilder.Entity<PokemonMove>();
        move.HasKey(p => p.Id);
        move.Property(p => p.Name).HasMaxLength(100).IsRequired();
        move.Property(p => p.Type).HasConversion<string>();
        move.Property(p => p.Category).HasConversion<string>();

        var learnable = modelBuilder.Entity<LearnableMove>();
        learnable.HasKey(p => p.Id);
        learnable.Property(p => p.Method).HasConversion<string>();
        learnable.Property(p => p.GameVersion).HasMaxLength(80);
        learnable.HasOne(p => p.Move)
            .WithMany()
            .HasForeignKey(p => p.MoveId)
            .OnDelete(DeleteBehavior.Restrict);
        learnable.HasIndex(p => new { p.PokemonFormId, p.MoveId, p.Method, p.GameVersion });
    }

    private static void ConfigureAbilities(ModelBuilder modelBuilder)
    {
        var ability = modelBuilder.Entity<PokemonAbility>();
        ability.HasKey(p => p.Id);
        ability.Property(p => p.Name).HasMaxLength(100).IsRequired();
        ability.Property(p => p.Description).HasMaxLength(1000);

        var formAbility = modelBuilder.Entity<PokemonFormAbility>();
        formAbility.HasKey(p => p.Id);
        formAbility.Property(p => p.Slot).HasConversion<string>();
        formAbility.HasOne(p => p.Ability)
            .WithMany()
            .HasForeignKey(p => p.AbilityId)
            .OnDelete(DeleteBehavior.Restrict);
        formAbility.HasIndex(p => new { p.PokemonFormId, p.Slot }).IsUnique();
    }

    private static void ConfigureEvolutions(ModelBuilder modelBuilder)
    {
        var evolution = modelBuilder.Entity<PokemonEvolution>();
        evolution.HasKey(p => p.Id);
        evolution.Property(p => p.Method).HasConversion<string>();
        evolution.Property(p => p.RequiredTime).HasConversion<string>();
        evolution.Property(p => p.RequiredGender).HasConversion<string>();
        evolution.Property(p => p.GameVersion).HasMaxLength(80);

        evolution.HasOne(p => p.RequiredMove)
            .WithMany()
            .HasForeignKey(p => p.RequiredMoveId)
            .OnDelete(DeleteBehavior.Restrict);

        evolution.HasIndex(p => new { p.FromPokemonId, p.ToPokemonId, p.Method });
    }

    private static void ConfigureRegions(ModelBuilder modelBuilder)
    {
        var region = modelBuilder.Entity<Region>();
        region.HasKey(p => p.Id);
        region.Property(p => p.Name).HasMaxLength(80).IsRequired();
        region.HasIndex(p => p.Name).IsUnique();
    }

    private static void ConfigureTeams(ModelBuilder modelBuilder)
    {
        var team = modelBuilder.Entity<PokemonTeam>();
        team.HasKey(p => p.Id);
        team.Property(p => p.Name).HasMaxLength(80).IsRequired();
        team.HasMany(p => p.Members)
            .WithOne(build => build.Team)
            .HasForeignKey(build => build.PokemonTeamId)
            .OnDelete(DeleteBehavior.Cascade);
        team.Navigation(p => p.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        var build = modelBuilder.Entity<PokemonBuild>();
        build.HasKey(p => p.Id);
        build.Property(p => p.Gender).HasConversion<string>();
        build.Property(p => p.Nature).HasConversion<string>();
        build.Property(p => p.AbilitySlot).HasConversion<string>();
        build.Property(p => p.GameVersion).HasMaxLength(80);
        build.HasOne(p => p.Species)
            .WithMany()
            .HasForeignKey(p => p.SpeciesId)
            .OnDelete(DeleteBehavior.Restrict);
        build.HasOne(p => p.Form)
            .WithMany()
            .HasForeignKey(p => p.PokemonFormId)
            .OnDelete(DeleteBehavior.Restrict);
        build.HasOne(p => p.Ability)
            .WithMany()
            .HasForeignKey(p => p.AbilityId)
            .OnDelete(DeleteBehavior.Restrict);

        build.Property(p => p.IVs)
            .HasConversion(CreateJsonConverter<IndividualValues>());

        build.OwnsOne(p => p.EVs, owned =>
        {
            owned.Property(p => p.HP).HasColumnName("EV_HP");
            owned.Property(p => p.Attack).HasColumnName("EV_Attack");
            owned.Property(p => p.Defense).HasColumnName("EV_Defense");
            owned.Property(p => p.SpecialAttack).HasColumnName("EV_SpecialAttack");
            owned.Property(p => p.SpecialDefense).HasColumnName("EV_SpecialDefense");
            owned.Property(p => p.Speed).HasColumnName("EV_Speed");
        });

        build.HasMany(p => p.Moves)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "PokemonBuildMove",
                right => right.HasOne<PokemonMove>()
                    .WithMany()
                    .HasForeignKey("MoveId")
                    .OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<PokemonBuild>()
                    .WithMany()
                    .HasForeignKey("BuildId")
                    .OnDelete(DeleteBehavior.Cascade),
                join => join.HasKey("BuildId", "MoveId"));

        build.Navigation(p => p.Moves)
            .HasField("_moves")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureFavorites(ModelBuilder modelBuilder)
    {
        var favorite = modelBuilder.Entity<PokemonFavorite>();
        favorite.HasKey(p => p.Id);
        favorite.Property(p => p.AddedAt).IsRequired();
        favorite.HasOne(p => p.Pokemon)
            .WithMany()
            .HasForeignKey(p => p.PokemonId)
            .OnDelete(DeleteBehavior.Cascade);
        favorite.HasIndex(p => p.PokemonId).IsUnique();
    }

    private static ValueConverter<List<T>, string> CreateJsonListConverter<T>() =>
        new(value => SerializeJson(value), value => DeserializeList<T>(value));

    private static ValueComparer<List<T>> CreateListComparer<T>() =>
        new(
            (left, right) => left!.SequenceEqual(right!),
            value => value!.Aggregate(0, (hash, item) => HashCode.Combine(hash, item)),
            value => value!.ToList());

    private static ValueConverter<T, string> CreateJsonConverter<T>() =>
        new(value => SerializeJson(value), value => DeserializeJson<T>(value));

    private static string SerializeJson<T>(T value) =>
        JsonSerializer.Serialize(value, JsonOptions);

    private static T DeserializeJson<T>(string value) =>
        JsonSerializer.Deserialize<T>(value, JsonOptions)!;

    private static List<T> DeserializeList<T>(string value) =>
        JsonSerializer.Deserialize<List<T>>(value, JsonOptions) ?? new List<T>();
}
