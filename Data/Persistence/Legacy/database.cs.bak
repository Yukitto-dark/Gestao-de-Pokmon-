using GestaoDePokemon.model;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Diagnostics;
using System.Windows;

namespace GestaoDePokemon.Database
{

    public enum PokemonType { Normal, Fire, Water, Electric, Grass, Ice, Fighting, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy }
    public enum MoveCategory { Physical, Special, Status }
    public enum PokemonGender { Male, Female, Genderless }
    public enum AbilitySlot { Slot1, Slot2, Hidden }
    public enum MoveLearnMethod { LevelUp, TM, Egg, Tutor }
    public enum PokemonStat { HP, Attack, Defense, SpecialAttack, SpecialDefense, Speed }
    public enum PokemonNature { Hardy, Lonely, Brave, Adamant, Naughty, Bold, Docile, Relaxed, Impish, Lax, Timid, Hasty, Serious, Jolly, Naive, Modest, Mild, Quiet, Bashful, Rash, Calm, Gentle, Sassy, Careful, Quirky }
    public enum EggGroup { Monster, Water1, Bug, Flying, Field, Fairy, Grass, HumanLike, Water3, Mineral, Amorphous, Water2, Ditto, Dragon, Undiscovered }


    public class BaseStats
    {
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
        public int Total => HP + Attack + Defense + SpecialAttack + SpecialDefense + Speed;
    }

    public class IndividualValues
    {
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
    }

    public class EffortValues
    {
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
        public int Total => HP + Attack + Defense + SpecialAttack + SpecialDefense + Speed;
    }

    public class NatureModifiers
    {
        public PokemonStat IncreasedStat { get; set; }
        public PokemonStat DecreasedStat { get; set; }
    }


    public class Pokemon
    {
        public int Id { get; set; }
        public int NationalDexNumber { get; set; }
        public int EvolutionLineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Generation { get; set; }
        public string Category { get; set; } = string.Empty;


        public List<EggGroup> EggGroups { get; set; } = new();
        public List<PokemonForm> Forms { get; set; } = new();
        public List<LearnableMove> Learnset { get; set; } = new();
    }

    public class PokemonForm
    {
        public int Id { get; set; }
        public int PokemonId { get; set; }
        public string FormName { get; set; } = string.Empty;

        public BaseStats Stats { get; set; } = new();
        public List<PokemonType> Types { get; set; } = new();

        public Pokemon Pokemon { get; set; } = null!;
        public List<PokemonSprite> Sprites { get; set; } = new();
        public List<PokemonFormAbility> Abilities { get; set; } = new();
    }

    public class PokemonSprite
    {
        public int Id { get; set; }
        public int PokemonFormId { get; set; }
        public PokemonGender? Gender { get; set; }
        public string FrontDefault { get; set; } = string.Empty;
        public string MiniDefault { get; set; } = string.Empty;
        public string FrontShiny { get; set; } = string.Empty;
    }

    public class PokemonMove
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PokemonType Type { get; set; }
        public MoveCategory Category { get; set; }
        public int? Power { get; set; }
        public int? Accuracy { get; set; }
        public int? PP { get; set; }
    }

    public class LearnableMove
    {
        public int Id { get; set; }
        public int PokemonId { get; set; }
        public int MoveId { get; set; }
        public MoveLearnMethod Method { get; set; }
        public int? Level { get; set; }
        public string GameVersion { get; set; } = string.Empty;

        public Pokemon Pokemon { get; set; } = null!;
        public PokemonMove Move { get; set; } = null!;
    }

    public class PokemonAbility
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PokemonFormAbility
    {
        public int Id { get; set; }
        public int PokemonFormId { get; set; }
        public int AbilityId { get; set; }
        public AbilitySlot Slot { get; set; }

        public PokemonAbility Ability { get; set; } = null!;
    }


    public class PokemonTeam
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PokemonInstance> Members { get; set; } = new();
    }

    public class PokemonInstance
    {
        public int Id { get; set; }
        public int PokemonTeamId { get; set; }
        public int PokemonFormId { get; set; }

        public PokemonGender Gender { get; set; }
        public int Level { get; set; }
        public PokemonNature Nature { get; set; }
        public int AbilityId { get; set; }
        public AbilitySlot AbilitySlot { get; set; }


        public IndividualValues IVs { get; set; } = new();
        public EffortValues EVs { get; set; } = new();


        public PokemonForm PokemonForm { get; set; } = null!;
        public List<PokemonMove> Moves { get; set; } = new();
    }

    public class PokemonFavorite
    {
        public int Id { get; set; }
        public int PokemonId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
    public class AppDbContext : DbContext
    {

        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<PokemonForm> PokemonForms { get; set; }
        public DbSet<PokemonMove> Moves { get; set; }
        public DbSet<LearnableMove> LearnableMoves { get; set; }
        public DbSet<PokemonAbility> Abilities { get; set; }
        public DbSet<PokemonFormAbility> FormAbilities { get; set; }
        public DbSet<PokemonSprite> Sprites { get; set; }

        public DbSet<PokemonTeam> Teams { get; set; }
        public DbSet<PokemonInstance> Instances { get; set; }
        public DbSet<PokemonFavorite> Favorites { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite("Data Source = Gestaopokemon.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<PokemonForm>().OwnsOne(p => p.Stats);
            modelBuilder.Entity<PokemonInstance>().OwnsOne(p => p.IVs);
            modelBuilder.Entity<PokemonInstance>().OwnsOne(p => p.EVs);


            modelBuilder.Entity<PokemonForm>()
                .HasOne(p => p.Pokemon)
                .WithMany(p => p.Forms)
                .HasForeignKey(p => p.PokemonId);

            modelBuilder.Entity<LearnableMove>()
                .HasOne(l => l.Pokemon)
                .WithMany(p => p.Learnset)
                .HasForeignKey(l => l.PokemonId);

            modelBuilder.Entity<LearnableMove>()
                .HasOne(l => l.Move)
                .WithMany()
                .HasForeignKey(l => l.MoveId);

            modelBuilder.Entity<PokemonFormAbility>()
                .HasOne(a => a.Ability)
                .WithMany()
                .HasForeignKey(a => a.AbilityId);
        }
    }

    public class DatabaseInitializer
    {
        public static void LoadPokemonSprites(AppDbContext db)
        {

            foreach (Pokemon pokemon in db.Pokemons)
            {
                var normalForm = pokemon.Forms
                    .FirstOrDefault(f => f.FormName == "Normal");

                if (normalForm == null)
                    continue;

                string dexNumber = pokemon.NationalDexNumber.ToString();

                PokemonSprite sprite = new PokemonSprite
                {
                    PokemonFormId = normalForm.Id,

                    FrontDefault = $"/view/images/pokemon/frontNormal/{dexNumber}.png",

                    FrontShiny = $"/view/images/pokemon/frontShiny/{dexNumber}.png",

                    MiniDefault = $"/view/images/pokemon/mini/{dexNumber}.png"

                };
                db.Sprites.Add(sprite);
            }
            db.SaveChanges();
        }
        public static void Inicializar()
        {
            try
            {
                using var db = new AppDbContext();
                db.Database.EnsureCreated();
                Debug.WriteLine("Banco criado com sucesso!");
                if (!db.Pokemons.Any())
                {
                    var bulbasaur = new Pokemon
                    {
                        NationalDexNumber = 1,
                        Name = "Bulbasaur",
                        Generation = 1,
                        Category = "Seed",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Grass }
                    };

                    var form = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats { HP = 45, Attack = 49, Defense = 49, SpecialAttack = 65, SpecialDefense = 65, Speed = 45 }
                    };

                    var ivysaur = new Pokemon
                    {
                        NationalDexNumber = 2,
                        Name = "Ivysaur",
                        Generation = 1,
                        Category = "Seed",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Grass }
                    };

                    var formivy = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats { HP = 60, Attack = 62, Defense = 63, SpecialAttack = 80, SpecialDefense = 80, Speed = 60 }
                    };

                    var venusaur = new Pokemon
                    {
                        NationalDexNumber = 3,
                        Name = "Venusaur",
                        Generation = 1,
                        Category = "Seed",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Grass }
                    };

                    var formvenu = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats { HP = 80, Attack = 82, Defense = 83, SpecialAttack = 100, SpecialDefense = 100, Speed = 80 }
                    };

                    var charmander = new Pokemon
                    {
                        NationalDexNumber = 4,
                        Name = "Charmander",
                        Generation = 1,
                        Category = "Lizard",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Dragon }
                    };

                    var formchar = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats { HP = 39, Attack = 52, Defense = 43, SpecialAttack = 60, SpecialDefense = 50, Speed = 65 }
                    };

                    var charmeleon = new Pokemon
                    {
                        NationalDexNumber = 5,
                        Name = "Charmeleon",
                        Generation = 1,
                        Category = "Lizard",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Dragon }
                    };

                    var formcharleon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats { HP = 58, Attack = 64, Defense = 58, SpecialAttack = 85, SpecialDefense = 85, Speed = 65 }
                    };

                    var charizard = new Pokemon
                    {
                        NationalDexNumber = 6,
                        Name = "Charizard",
                        Generation = 1,
                        Category = "Dragon",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Dragon }
                    };

                    var formcharizard = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire, PokemonType.Flying },
                        Stats = new BaseStats { HP = 78, Attack = 84, Defense = 78, SpecialAttack = 109, SpecialDefense = 115, Speed = 100 }
                    };

                    var squirtle = new Pokemon
                    {
                        NationalDexNumber = 7,
                        Name = "Squirtle",
                        Generation = 1,
                        Category = "Tiny turtle",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formsquirtle = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 44, Attack = 48, Defense = 65, SpecialAttack = 50, SpecialDefense = 64, Speed = 43 }
                    };

                    var wartortle = new Pokemon
                    {
                        NationalDexNumber = 8,
                        Name = "Wartortle",
                        Generation = 1,
                        Category = "Turtle",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formwartortle = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 59, Attack = 63, Defense = 80, SpecialAttack = 65, SpecialDefense = 80, Speed = 58 }
                    };

                    var blastoise = new Pokemon
                    {
                        NationalDexNumber = 9,
                        Name = "Blastoise",
                        Generation = 1,
                        Category = "Shellfish",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formblastoise = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 79, Attack = 83, Defense = 100, SpecialAttack = 85, SpecialDefense = 100, Speed = 78 }
                    };

                    var caterpie = new Pokemon
                    {
                        NationalDexNumber = 10,
                        Name = "Caterpie",
                        Generation = 1,
                        Category = "Worm",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formcaterpie = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug },
                        Stats = new BaseStats { HP = 45, Attack = 30, Defense = 35, SpecialAttack = 20, SpecialDefense = 20, Speed = 45 }
                    };

                    var metapod = new Pokemon
                    {
                        NationalDexNumber = 11,
                        Name = "Metapod",
                        Generation = 1,
                        Category = "Cocoon",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formmetapod = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug },
                        Stats = new BaseStats { HP = 50, Attack = 35, Defense = 55, SpecialAttack = 20, SpecialDefense = 20, Speed = 30 }
                    };

                    var butterfree = new Pokemon
                    {
                        NationalDexNumber = 12,
                        Name = "Butterfree",
                        Generation = 1,
                        Category = "Butterfly",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formbutterfree = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Flying },
                        Stats = new BaseStats { HP = 60, Attack = 45, Defense = 50, SpecialAttack = 90, SpecialDefense = 80, Speed = 70 }
                    };

                    var weedle = new Pokemon
                    {
                        NationalDexNumber = 13,
                        Name = "Weedle",
                        Generation = 1,
                        Category = "Hairy Bug",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formweedle = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Poison },
                        Stats = new BaseStats { HP = 40, Attack = 35, Defense = 30, SpecialAttack = 20, SpecialDefense = 20, Speed = 50 }
                    };

                    var kakuna = new Pokemon
                    {
                        NationalDexNumber = 14,
                        Name = "Kakuna",
                        Generation = 1,
                        Category = "Cocoon",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formkakuna = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Poison },
                        Stats = new BaseStats { HP = 45, Attack = 25, Defense = 50, SpecialAttack = 25, SpecialDefense = 25, Speed = 35 }
                    };

                    var beedrill = new Pokemon
                    {
                        NationalDexNumber = 15,
                        Name = "Beedrill",
                        Generation = 1,
                        Category = "Poison Bee",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formbeedrill = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Poison },
                        Stats = new BaseStats { HP = 65, Attack = 90, Defense = 40, SpecialAttack = 45, SpecialDefense = 80, Speed = 70 }
                    };

                    var pidgey = new Pokemon
                    {
                        NationalDexNumber = 16,
                        Name = "Pidgey",
                        Generation = 1,
                        Category = "Tiny Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formpidgey = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats { HP = 40, Attack = 45, Defense = 40, SpecialAttack = 35, SpecialDefense = 35, Speed = 56 }
                    };

                    var pidgeotto = new Pokemon
                    {
                        NationalDexNumber = 17,
                        Name = "Pidgeotto",
                        Generation = 1,
                        Category = "Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formpidgeotto = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats { HP = 63, Attack = 60, Defense = 55, SpecialAttack = 50, SpecialDefense = 50, Speed = 71 }
                    };

                    var pidgeot = new Pokemon
                    {
                        NationalDexNumber = 18,
                        Name = "Pidgeot",
                        Generation = 1,
                        Category = "Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formpidgeot = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats { HP = 83, Attack = 80, Defense = 75, SpecialAttack = 70, SpecialDefense = 70, Speed = 101 }
                    };

                    var rattata = new Pokemon
                    {
                        NationalDexNumber = 19,
                        Name = "Rattata",
                        Generation = 1,
                        Category = "Mouse",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formrattata = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 30, Attack = 56, Defense = 35, SpecialAttack = 25, SpecialDefense = 35, Speed = 72 }
                    };

                    var raticate = new Pokemon
                    {
                        NationalDexNumber = 20,
                        Name = "Raticate",
                        Generation = 1,
                        Category = "Mouse",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formraticate = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 55, Attack = 81, Defense = 60, SpecialAttack = 50, SpecialDefense = 70, Speed = 97 }
                    };

                    var spearow = new Pokemon
                    {
                        NationalDexNumber = 21,
                        Name = "Spearow",
                        Generation = 1,
                        Category = "Tiny Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formspearow = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats { HP = 40, Attack = 60, Defense = 30, SpecialAttack = 31, SpecialDefense = 31, Speed = 70 }
                    };

                    var fearow = new Pokemon
                    {
                        NationalDexNumber = 22,
                        Name = "Fearow",
                        Generation = 1,
                        Category = "Beak",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formfearow = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats { HP = 65, Attack = 90, Defense = 65, SpecialAttack = 61, SpecialDefense = 61, Speed = 100 }
                    };

                    var ekans = new Pokemon
                    {
                        NationalDexNumber = 23,
                        Name = "Ekans",
                        Generation = 1,
                        Category = "Snake",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Dragon }
                    };

                    var formekans = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 35, Attack = 60, Defense = 44, SpecialAttack = 40, SpecialDefense = 54, Speed = 55 }
                    };

                    var arbok = new Pokemon
                    {
                        NationalDexNumber = 24,
                        Name = "Arbok",
                        Generation = 1,
                        Category = "Cobra",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Dragon }
                    };

                    var formarbok = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 60, Attack = 95, Defense = 69, SpecialAttack = 65, SpecialDefense = 79, Speed = 80 }
                    };

                    var pikachu = new Pokemon
                    {
                        NationalDexNumber = 25,
                        Name = "Pikachu",
                        Generation = 1,
                        Category = "Mouse",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
                    };

                    var formpikachu = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats { HP = 35, Attack = 55, Defense = 40, SpecialAttack = 50, SpecialDefense = 50, Speed = 90 }
                    };

                    var raichu = new Pokemon
                    {
                        NationalDexNumber = 26,
                        Name = "Raichu",
                        Generation = 1,
                        Category = "Mouse",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
                    };

                    var formraichu = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats { HP = 60, Attack = 90, Defense = 55, SpecialAttack = 90, SpecialDefense = 80, Speed = 110 }
                    };

                    var sandshrew = new Pokemon
                    {
                        NationalDexNumber = 27,
                        Name = "Sandshrew",
                        Generation = 1,
                        Category = "Mouse",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formsandshrew = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats { HP = 50, Attack = 75, Defense = 85, SpecialAttack = 20, SpecialDefense = 30, Speed = 40 }
                    };

                    var sandslash = new Pokemon
                    {
                        NationalDexNumber = 28,
                        Name = "Sandslash",
                        Generation = 1,
                        Category = "Mouse",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formsandslash = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats { HP = 75, Attack = 100, Defense = 110, SpecialAttack = 45, SpecialDefense = 55, Speed = 65 }
                    };

                    var nidoranf = new Pokemon
                    {
                        NationalDexNumber = 29,
                        Name = "Nidoran♀",
                        Generation = 1,
                        Category = "Poison Pin",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Field }
                    };

                    var formnidoranf = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 55, Attack = 47, Defense = 52, SpecialAttack = 40, SpecialDefense = 40, Speed = 41 }
                    };

                    var nidorina = new Pokemon
                    {
                        NationalDexNumber = 30,
                        Name = "Nidorina",
                        Generation = 1,
                        Category = "Poison Pin",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formnidorina = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 70, Attack = 62, Defense = 67, SpecialAttack = 55, SpecialDefense = 55, Speed = 56 }
                    };

                    var nidoqueen = new Pokemon
                    {
                        NationalDexNumber = 31,
                        Name = "Nidoqueen",
                        Generation = 1,
                        Category = "Drill",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formnidoqueen = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison, PokemonType.Ground },
                        Stats = new BaseStats { HP = 90, Attack = 92, Defense = 87, SpecialAttack = 75, SpecialDefense = 85, Speed = 76 }
                    };

                    var nidoranm = new Pokemon
                    {
                        NationalDexNumber = 32,
                        Name = "Nidoran♂",
                        Generation = 1,
                        Category = "Poison Pin",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Field }
                    };

                    var formnidoranm = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 46, Attack = 57, Defense = 40, SpecialAttack = 40, SpecialDefense = 40, Speed = 50 }
                    };

                    var nidorino = new Pokemon
                    {
                        NationalDexNumber = 33,
                        Name = "Nidorino",
                        Generation = 1,
                        Category = "Poison Pin",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Field }
                    };

                    var formnidorino = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 61, Attack = 72, Defense = 57, SpecialAttack = 55, SpecialDefense = 55, Speed = 65 }
                    };

                    var nidoking = new Pokemon
                    {
                        NationalDexNumber = 34,
                        Name = "Nidoking",
                        Generation = 1,
                        Category = "Drill",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Field }
                    };

                    var formnidoking = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison, PokemonType.Ground },
                        Stats = new BaseStats { HP = 81, Attack = 102, Defense = 77, SpecialAttack = 85, SpecialDefense = 75, Speed = 85 }
                    };

                    var clefairy = new Pokemon
                    {
                        NationalDexNumber = 35,
                        Name = "Clefairy",
                        Generation = 1,
                        Category = "Fairy",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy }
                    };

                    var formclefairy = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fairy },
                        Stats = new BaseStats { HP = 70, Attack = 45, Defense = 48, SpecialAttack = 60, SpecialDefense = 65, Speed = 35 }
                    };

                    var clefable = new Pokemon
                    {
                        NationalDexNumber = 36,
                        Name = "Clefable",
                        Generation = 1,
                        Category = "Fairy",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy }
                    };

                    var formclefable = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fairy },
                        Stats = new BaseStats { HP = 95, Attack = 70, Defense = 73, SpecialAttack = 95, SpecialDefense = 90, Speed = 60 }
                    };

                    var vulpix = new Pokemon
                    {
                        NationalDexNumber = 37,
                        Name = "Vulpix",
                        Generation = 1,
                        Category = "Fox",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formvulpix = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats { HP = 38, Attack = 41, Defense = 40, SpecialAttack = 50, SpecialDefense = 65, Speed = 65 }
                    };

                    var ninetales = new Pokemon
                    {
                        NationalDexNumber = 38,
                        Name = "Ninetales",
                        Generation = 1,
                        Category = "Fox",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formninetales = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats { HP = 73, Attack = 76, Defense = 75, SpecialAttack = 81, SpecialDefense = 100, Speed = 100 }
                    };

                    var jigglypuff = new Pokemon
                    {
                        NationalDexNumber = 39,
                        Name = "Jigglypuff",
                        Generation = 1,
                        Category = "Balloon",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy }
                    };

                    var formjigglypuff = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Fairy },
                        Stats = new BaseStats { HP = 115, Attack = 45, Defense = 20, SpecialAttack = 45, SpecialDefense = 25, Speed = 20 }
                    };

                    var wigglytuff = new Pokemon
                    {
                        NationalDexNumber = 40,
                        Name = "Wigglytuff",
                        Generation = 1,
                        Category = "Balloon",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy }
                    };

                    var formwigglytuff = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Fairy },
                        Stats = new BaseStats { HP = 140, Attack = 70, Defense = 45, SpecialAttack = 85, SpecialDefense = 50, Speed = 45 }
                    };

                    var zubat = new Pokemon
                    {
                        NationalDexNumber = 41,
                        Name = "Zubat",
                        Generation = 1,
                        Category = "Bat",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formzubat = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison, PokemonType.Flying },
                        Stats = new BaseStats { HP = 40, Attack = 45, Defense = 35, SpecialAttack = 30, SpecialDefense = 40, Speed = 55 }
                    };

                    var golbat = new Pokemon
                    {
                        NationalDexNumber = 42,
                        Name = "Golbat",
                        Generation = 1,
                        Category = "Bat",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formgolbat = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison, PokemonType.Flying },
                        Stats = new BaseStats { HP = 75, Attack = 80, Defense = 70, SpecialAttack = 65, SpecialDefense = 75, Speed = 90 }
                    };

                    var oddish = new Pokemon
                    {
                        NationalDexNumber = 43,
                        Name = "Oddish",
                        Generation = 1,
                        Category = "Weed",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formoddish = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats { HP = 45, Attack = 50, Defense = 55, SpecialAttack = 75, SpecialDefense = 65, Speed = 30 }
                    };

                    var gloom = new Pokemon
                    {
                        NationalDexNumber = 44,
                        Name = "Gloom",
                        Generation = 1,
                        Category = "Weed",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formgloom = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats { HP = 60, Attack = 65, Defense = 70, SpecialAttack = 85, SpecialDefense = 75, Speed = 40 }
                    };

                    var vileplume = new Pokemon
                    {
                        NationalDexNumber = 45,
                        Name = "Vileplume",
                        Generation = 1,
                        Category = "Flower",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formvileplume = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats { HP = 75, Attack = 80, Defense = 85, SpecialAttack = 110, SpecialDefense = 90, Speed = 50 }
                    };

                    var paras = new Pokemon
                    {
                        NationalDexNumber = 46,
                        Name = "Paras",
                        Generation = 1,
                        Category = "Mushroom",
                        EggGroups = new List<EggGroup> { EggGroup.Bug, EggGroup.Grass }
                    };

                    var formparas = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Grass },
                        Stats = new BaseStats { HP = 35, Attack = 70, Defense = 55, SpecialAttack = 45, SpecialDefense = 55, Speed = 25 }
                    };

                    var parasect = new Pokemon
                    {
                        NationalDexNumber = 47,
                        Name = "Parasect",
                        Generation = 1,
                        Category = "Mushroom",
                        EggGroups = new List<EggGroup> { EggGroup.Bug, EggGroup.Grass }
                    };

                    var formparasect = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Grass },
                        Stats = new BaseStats { HP = 60, Attack = 95, Defense = 80, SpecialAttack = 60, SpecialDefense = 80, Speed = 30 }
                    };

                    var venonat = new Pokemon
                    {
                        NationalDexNumber = 48,
                        Name = "Venonat",
                        Generation = 1,
                        Category = "Insect",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formvenonat = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Poison },
                        Stats = new BaseStats { HP = 60, Attack = 55, Defense = 50, SpecialAttack = 40, SpecialDefense = 55, Speed = 45 }
                    };

                    var venomoth = new Pokemon
                    {
                        NationalDexNumber = 49,
                        Name = "Venomoth",
                        Generation = 1,
                        Category = "Poison Moth",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formvenomoth = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Poison },
                        Stats = new BaseStats { HP = 70, Attack = 65, Defense = 60, SpecialAttack = 90, SpecialDefense = 75, Speed = 90 }
                    };

                    var diglett = new Pokemon
                    {
                        NationalDexNumber = 50,
                        Name = "Diglett",
                        Generation = 1,
                        Category = "Mole",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formdiglett = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats { HP = 10, Attack = 55, Defense = 25, SpecialAttack = 35, SpecialDefense = 45, Speed = 95 }
                    };

                    var dugtrio = new Pokemon
                    {
                        NationalDexNumber = 51,
                        Name = "Dugtrio",
                        Generation = 1,
                        Category = "Mole",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formdugtrio = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats { HP = 35, Attack = 100, Defense = 50, SpecialAttack = 50, SpecialDefense = 70, Speed = 120 }
                    };

                    var meowth = new Pokemon
                    {
                        NationalDexNumber = 52,
                        Name = "Meowth",
                        Generation = 1,
                        Category = "Scratch Cat",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formmeowth = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 40, Attack = 45, Defense = 35, SpecialAttack = 40, SpecialDefense = 40, Speed = 90 }
                    };

                    var persian = new Pokemon
                    {
                        NationalDexNumber = 53,
                        Name = "Persian",
                        Generation = 1,
                        Category = "Classy Cat",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formpersian = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 65, Attack = 70, Defense = 60, SpecialAttack = 65, SpecialDefense = 65, Speed = 115 }
                    };

                    var psyduck = new Pokemon
                    {
                        NationalDexNumber = 54,
                        Name = "Psyduck",
                        Generation = 1,
                        Category = "Duck",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Water1 }
                    };

                    var formpsyduck = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 50, Attack = 52, Defense = 48, SpecialAttack = 65, SpecialDefense = 50, Speed = 55 }
                    };

                    var golduck = new Pokemon
                    {
                        NationalDexNumber = 55,
                        Name = "Golduck",
                        Generation = 1,
                        Category = "Duck",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Water1 }
                    };

                    var formgolduck = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 80, Attack = 82, Defense = 78, SpecialAttack = 95, SpecialDefense = 80, Speed = 85 }
                    };

                    var mankey = new Pokemon
                    {
                        NationalDexNumber = 56,
                        Name = "Mankey",
                        Generation = 1,
                        Category = "Pig Monkey",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formmankey = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats { HP = 40, Attack = 80, Defense = 35, SpecialAttack = 35, SpecialDefense = 45, Speed = 70 }
                    };

                    var primeape = new Pokemon
                    {
                        NationalDexNumber = 57,
                        Name = "Primeape",
                        Generation = 1,
                        Category = "Pig Monkey",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formprimeape = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats { HP = 65, Attack = 105, Defense = 60, SpecialAttack = 60, SpecialDefense = 70, Speed = 95 }
                    };

                    var growlithe = new Pokemon
                    {
                        NationalDexNumber = 58,
                        Name = "Growlithe",
                        Generation = 1,
                        Category = "Puppy",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formgrowlithe = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats { HP = 55, Attack = 70, Defense = 45, SpecialAttack = 70, SpecialDefense = 50, Speed = 60 }
                    };

                    var arcanine = new Pokemon
                    {
                        NationalDexNumber = 59,
                        Name = "Arcanine",
                        Generation = 1,
                        Category = "Legendary",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formarcanine = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats { HP = 90, Attack = 110, Defense = 80, SpecialAttack = 100, SpecialDefense = 80, Speed = 95 }
                    };

                    var poliwag = new Pokemon
                    {
                        NationalDexNumber = 60,
                        Name = "Poliwag",
                        Generation = 1,
                        Category = "Tadpole",
                        EggGroups = new List<EggGroup> { EggGroup.Water1 }
                    };

                    var formpoliwag = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 40, Attack = 50, Defense = 40, SpecialAttack = 40, SpecialDefense = 40, Speed = 90 }
                    };

                    var poliwhirl = new Pokemon
                    {
                        NationalDexNumber = 61,
                        Name = "Poliwhirl",
                        Generation = 1,
                        Category = "Tadpole",
                        EggGroups = new List<EggGroup> { EggGroup.Water1 }
                    };

                    var formpoliwhirl = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 65, Attack = 65, Defense = 65, SpecialAttack = 50, SpecialDefense = 50, Speed = 90 }
                    };

                    var poliwrath = new Pokemon
                    {
                        NationalDexNumber = 62,
                        Name = "Poliwrath",
                        Generation = 1,
                        Category = "Tadpole",
                        EggGroups = new List<EggGroup> { EggGroup.Water1 }
                    };

                    var formpoliwrath = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Fighting },
                        Stats = new BaseStats { HP = 90, Attack = 95, Defense = 95, SpecialAttack = 70, SpecialDefense = 90, Speed = 70 }
                    };

                    var abra = new Pokemon
                    {
                        NationalDexNumber = 63,
                        Name = "Abra",
                        Generation = 1,
                        Category = "Psi",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formabra = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats { HP = 25, Attack = 20, Defense = 15, SpecialAttack = 105, SpecialDefense = 55, Speed = 90 }
                    };

                    var kadabra = new Pokemon
                    {
                        NationalDexNumber = 64,
                        Name = "Kadabra",
                        Generation = 1,
                        Category = "Psi",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formkadabra = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats { HP = 40, Attack = 35, Defense = 30, SpecialAttack = 120, SpecialDefense = 70, Speed = 105 }
                    };

                    var alakazam = new Pokemon
                    {
                        NationalDexNumber = 65,
                        Name = "Alakazam",
                        Generation = 1,
                        Category = "Psi",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formalakazam = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats { HP = 55, Attack = 50, Defense = 45, SpecialAttack = 135, SpecialDefense = 95, Speed = 120 }
                    };

                    var machop = new Pokemon
                    {
                        NationalDexNumber = 66,
                        Name = "Machop",
                        Generation = 1,
                        Category = "Superpower",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formmachop = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats { HP = 70, Attack = 80, Defense = 50, SpecialAttack = 35, SpecialDefense = 35, Speed = 35 }
                    };

                    var machoke = new Pokemon
                    {
                        NationalDexNumber = 67,
                        Name = "Machoke",
                        Generation = 1,
                        Category = "Superpower",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formmachoke = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats { HP = 80, Attack = 100, Defense = 70, SpecialAttack = 50, SpecialDefense = 60, Speed = 45 }
                    };

                    var machamp = new Pokemon
                    {
                        NationalDexNumber = 68,
                        Name = "Machamp",
                        Generation = 1,
                        Category = "Superpower",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formmachamp = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats { HP = 90, Attack = 130, Defense = 80, SpecialAttack = 65, SpecialDefense = 85, Speed = 55 }
                    };

                    var bellsprout = new Pokemon
                    {
                        NationalDexNumber = 69,
                        Name = "Bellsprout",
                        Generation = 1,
                        Category = "Flower",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formbellsprout = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats { HP = 50, Attack = 75, Defense = 35, SpecialAttack = 70, SpecialDefense = 30, Speed = 40 }
                    };

                    var weepinbell = new Pokemon
                    {
                        NationalDexNumber = 70,
                        Name = "Weepinbell",
                        Generation = 1,
                        Category = "Flycatcher",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formweepinbell = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats { HP = 65, Attack = 90, Defense = 50, SpecialAttack = 85, SpecialDefense = 45, Speed = 55 }
                    };

                    var victreebel = new Pokemon
                    {
                        NationalDexNumber = 71,
                        Name = "Victreebel",
                        Generation = 1,
                        Category = "Flycatcher",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formvictreebel = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats { HP = 80, Attack = 105, Defense = 65, SpecialAttack = 100, SpecialDefense = 70, Speed = 70 }
                    };

                    var tentacool = new Pokemon
                    {
                        NationalDexNumber = 72,
                        Name = "Tentacool",
                        Generation = 1,
                        Category = "Jellyfish",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };

                    var formtentacool = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Poison },
                        Stats = new BaseStats { HP = 40, Attack = 40, Defense = 35, SpecialAttack = 50, SpecialDefense = 100, Speed = 70 }
                    };

                    var tentacruel = new Pokemon
                    {
                        NationalDexNumber = 73,
                        Name = "Tentacruel",
                        Generation = 1,
                        Category = "Jellyfish",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };

                    var formtentacruel = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Poison },
                        Stats = new BaseStats { HP = 80, Attack = 70, Defense = 65, SpecialAttack = 80, SpecialDefense = 120, Speed = 100 }
                    };

                    var geodude = new Pokemon
                    {
                        NationalDexNumber = 74,
                        Name = "Geodude",
                        Generation = 1,
                        Category = "Rock",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formgeodude = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Ground },
                        Stats = new BaseStats { HP = 40, Attack = 80, Defense = 100, SpecialAttack = 30, SpecialDefense = 30, Speed = 20 }
                    };

                    var graveler = new Pokemon
                    {
                        NationalDexNumber = 75,
                        Name = "Graveler",
                        Generation = 1,
                        Category = "Rock",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formgraveler = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Ground },
                        Stats = new BaseStats { HP = 55, Attack = 95, Defense = 115, SpecialAttack = 45, SpecialDefense = 45, Speed = 35 }
                    };

                    var golem = new Pokemon
                    {
                        NationalDexNumber = 76,
                        Name = "Golem",
                        Generation = 1,
                        Category = "Megaton",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formgolem = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Ground },
                        Stats = new BaseStats { HP = 80, Attack = 110, Defense = 130, SpecialAttack = 55, SpecialDefense = 65, Speed = 45 }
                    };

                    var ponyta = new Pokemon
                    {
                        NationalDexNumber = 77,
                        Name = "Ponyta",
                        Generation = 1,
                        Category = "Fire Horse",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formponyta = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats { HP = 50, Attack = 85, Defense = 55, SpecialAttack = 65, SpecialDefense = 65, Speed = 90 }
                    };

                    var rapidash = new Pokemon
                    {
                        NationalDexNumber = 78,
                        Name = "Rapidash",
                        Generation = 1,
                        Category = "Fire Horse",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formrapidash = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats { HP = 65, Attack = 100, Defense = 70, SpecialAttack = 80, SpecialDefense = 80, Speed = 105 }
                    };

                    var slowpoke = new Pokemon
                    {
                        NationalDexNumber = 79,
                        Name = "Slowpoke",
                        Generation = 1,
                        Category = "Dopey",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formslowpoke = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 90, Attack = 65, Defense = 65, SpecialAttack = 40, SpecialDefense = 40, Speed = 15 }
                    };

                    var slowbro = new Pokemon
                    {
                        NationalDexNumber = 80,
                        Name = "Slowbro",
                        Generation = 1,
                        Category = "Hermit Crab",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formslowbro = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 95, Attack = 75, Defense = 110, SpecialAttack = 100, SpecialDefense = 80, Speed = 30 }
                    };

                    var magnemite = new Pokemon
                    {
                        NationalDexNumber = 81,
                        Name = "Magnemite",
                        Generation = 1,
                        Category = "Magnet",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formmagnemite = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric, PokemonType.Steel },
                        Stats = new BaseStats { HP = 25, Attack = 35, Defense = 70, SpecialAttack = 95, SpecialDefense = 55, Speed = 45 }
                    };

                    var magneton = new Pokemon
                    {
                        NationalDexNumber = 82,
                        Name = "Magneton",
                        Generation = 1,
                        Category = "Magnet",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formmagneton = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric, PokemonType.Steel },
                        Stats = new BaseStats { HP = 50, Attack = 60, Defense = 95, SpecialAttack = 120, SpecialDefense = 70, Speed = 70 }
                    };

                    var farfetchd = new Pokemon
                    {
                        NationalDexNumber = 83,
                        Name = "Farfetch'd",
                        Generation = 1,
                        Category = "Wild Duck",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Flying }
                    };

                    var formfarfetchd = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats { HP = 52, Attack = 90, Defense = 55, SpecialAttack = 58, SpecialDefense = 62, Speed = 60 }
                    };

                    var doduo = new Pokemon
                    {
                        NationalDexNumber = 84,
                        Name = "Doduo",
                        Generation = 1,
                        Category = "Twin Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formdoduo = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats { HP = 35, Attack = 85, Defense = 45, SpecialAttack = 35, SpecialDefense = 35, Speed = 75 }
                    };

                    var dodrio = new Pokemon
                    {
                        NationalDexNumber = 85,
                        Name = "Dodrio",
                        Generation = 1,
                        Category = "Triple Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formdodrio = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats { HP = 60, Attack = 110, Defense = 70, SpecialAttack = 60, SpecialDefense = 60, Speed = 110 }
                    };

                    var seel = new Pokemon
                    {
                        NationalDexNumber = 86,
                        Name = "Seel",
                        Generation = 1,
                        Category = "Sea Lion",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Water1 }
                    };

                    var formseel = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 65, Attack = 45, Defense = 55, SpecialAttack = 45, SpecialDefense = 70, Speed = 45 }
                    };

                    var dewgong = new Pokemon
                    {
                        NationalDexNumber = 87,
                        Name = "Dewgong",
                        Generation = 1,
                        Category = "Sea Lion",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Water1 }
                    };

                    var formdewgong = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Ice },
                        Stats = new BaseStats { HP = 90, Attack = 70, Defense = 80, SpecialAttack = 70, SpecialDefense = 95, Speed = 70 }
                    };

                    var grimer = new Pokemon
                    {
                        NationalDexNumber = 88,
                        Name = "Grimer",
                        Generation = 1,
                        Category = "Sludge",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formgrimer = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 80, Attack = 80, Defense = 50, SpecialAttack = 40, SpecialDefense = 50, Speed = 25 }
                    };

                    var muk = new Pokemon
                    {
                        NationalDexNumber = 89,
                        Name = "Muk",
                        Generation = 1,
                        Category = "Sludge",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formmuk = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 105, Attack = 105, Defense = 75, SpecialAttack = 65, SpecialDefense = 100, Speed = 50 }
                    };

                    var shellder = new Pokemon
                    {
                        NationalDexNumber = 90,
                        Name = "Shellder",
                        Generation = 1,
                        Category = "Bivalve",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };

                    var formshellder = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 30, Attack = 65, Defense = 100, SpecialAttack = 45, SpecialDefense = 25, Speed = 40 }
                    };

                    var cloyster = new Pokemon
                    {
                        NationalDexNumber = 91,
                        Name = "Cloyster",
                        Generation = 1,
                        Category = "Bivalve",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };

                    var formcloyster = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Ice },
                        Stats = new BaseStats { HP = 50, Attack = 95, Defense = 180, SpecialAttack = 85, SpecialDefense = 45, Speed = 70 }
                    };

                    var gastly = new Pokemon
                    {
                        NationalDexNumber = 92,
                        Name = "Gastly",
                        Generation = 1,
                        Category = "Gas",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formgastly = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ghost, PokemonType.Poison },
                        Stats = new BaseStats { HP = 30, Attack = 35, Defense = 30, SpecialAttack = 100, SpecialDefense = 35, Speed = 80 }
                    };

                    var haunter = new Pokemon
                    {
                        NationalDexNumber = 93,
                        Name = "Haunter",
                        Generation = 1,
                        Category = "Gas",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formhaunter = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ghost, PokemonType.Poison },
                        Stats = new BaseStats { HP = 45, Attack = 50, Defense = 45, SpecialAttack = 115, SpecialDefense = 55, Speed = 95 }
                    };

                    var gengar = new Pokemon
                    {
                        NationalDexNumber = 94,
                        Name = "Gengar",
                        Generation = 1,
                        Category = "Shadow",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formgengar = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ghost, PokemonType.Poison },
                        Stats = new BaseStats { HP = 60, Attack = 65, Defense = 60, SpecialAttack = 130, SpecialDefense = 75, Speed = 110 }
                    };

                    var onix = new Pokemon
                    {
                        NationalDexNumber = 95,
                        Name = "Onix",
                        Generation = 1,
                        Category = "Rock Snake",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formonix = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Ground },
                        Stats = new BaseStats { HP = 35, Attack = 45, Defense = 160, SpecialAttack = 30, SpecialDefense = 45, Speed = 70 }
                    };

                    var drowzee = new Pokemon
                    {
                        NationalDexNumber = 96,
                        Name = "Drowzee",
                        Generation = 1,
                        Category = "Hypnosis",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.HumanLike }
                    };

                    var formdrowzee = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats { HP = 60, Attack = 48, Defense = 45, SpecialAttack = 43, SpecialDefense = 90, Speed = 42 }
                    };

                    var hypno = new Pokemon
                    {
                        NationalDexNumber = 97,
                        Name = "Hypno",
                        Generation = 1,
                        Category = "Hypnosis",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.HumanLike }
                    };

                    var formhypno = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats { HP = 85, Attack = 73, Defense = 70, SpecialAttack = 73, SpecialDefense = 115, Speed = 67 }
                    };

                    var krabby = new Pokemon
                    {
                        NationalDexNumber = 98,
                        Name = "Krabby",
                        Generation = 1,
                        Category = "River Crab",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };

                    var formkrabby = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 30, Attack = 105, Defense = 90, SpecialAttack = 25, SpecialDefense = 25, Speed = 50 }
                    };

                    var kingler = new Pokemon
                    {
                        NationalDexNumber = 99,
                        Name = "Kingler",
                        Generation = 1,
                        Category = "Pincer",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };

                    var formkingler = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 55, Attack = 130, Defense = 115, SpecialAttack = 50, SpecialDefense = 50, Speed = 75 }
                    };

                    var voltorb = new Pokemon
                    {
                        NationalDexNumber = 100,
                        Name = "Voltorb",
                        Generation = 1,
                        Category = "Ball",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formvoltorb = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats { HP = 40, Attack = 30, Defense = 50, SpecialAttack = 55, SpecialDefense = 55, Speed = 100 }
                    };

                    var electrode = new Pokemon
                    {
                        NationalDexNumber = 101,
                        Name = "Electrode",
                        Generation = 1,
                        Category = "Ball",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formelectrode = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats { HP = 60, Attack = 50, Defense = 70, SpecialAttack = 80, SpecialDefense = 80, Speed = 140 }
                    };

                    var exeggcute = new Pokemon
                    {
                        NationalDexNumber = 102,
                        Name = "Exeggcute",
                        Generation = 1,
                        Category = "Egg",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formexeggcute = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 60, Attack = 40, Defense = 80, SpecialAttack = 60, SpecialDefense = 45, Speed = 40 }
                    };

                    var exeggutor = new Pokemon
                    {
                        NationalDexNumber = 103,
                        Name = "Exeggutor",
                        Generation = 1,
                        Category = "Coconut",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formexeggutor = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 95, Attack = 95, Defense = 85, SpecialAttack = 125, SpecialDefense = 75, Speed = 55 }
                    };

                    var cubone = new Pokemon
                    {
                        NationalDexNumber = 104,
                        Name = "Cubone",
                        Generation = 1,
                        Category = "Lonely",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formcubone = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats { HP = 50, Attack = 50, Defense = 95, SpecialAttack = 40, SpecialDefense = 50, Speed = 35 }
                    };

                    var marowak = new Pokemon
                    {
                        NationalDexNumber = 105,
                        Name = "Marowak",
                        Generation = 1,
                        Category = "Bone Keeper",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formmarowak = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats { HP = 60, Attack = 80, Defense = 110, SpecialAttack = 50, SpecialDefense = 80, Speed = 45 }
                    };

                    var hitmonlee = new Pokemon
                    {
                        NationalDexNumber = 106,
                        Name = "Hitmonlee",
                        Generation = 1,
                        Category = "Kicking",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formhitmonlee = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats { HP = 50, Attack = 120, Defense = 53, SpecialAttack = 35, SpecialDefense = 110, Speed = 87 }
                    };

                    var hitmonchan = new Pokemon
                    {
                        NationalDexNumber = 107,
                        Name = "Hitmonchan",
                        Generation = 1,
                        Category = "Punching",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formhitmonchan = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats { HP = 50, Attack = 105, Defense = 79, SpecialAttack = 35, SpecialDefense = 110, Speed = 76 }
                    };

                    var lickitung = new Pokemon
                    {
                        NationalDexNumber = 108,
                        Name = "Lickitung",
                        Generation = 1,
                        Category = "Licking",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formlickitung = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 90, Attack = 55, Defense = 75, SpecialAttack = 60, SpecialDefense = 75, Speed = 30 }
                    };

                    var koffing = new Pokemon
                    {
                        NationalDexNumber = 109,
                        Name = "Koffing",
                        Generation = 1,
                        Category = "Poison Gas",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formkoffing = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 40, Attack = 65, Defense = 95, SpecialAttack = 60, SpecialDefense = 45, Speed = 35 }
                    };

                    var weezing = new Pokemon
                    {
                        NationalDexNumber = 110,
                        Name = "Weezing",
                        Generation = 1,
                        Category = "Poison Gas",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formweezing = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 65, Attack = 90, Defense = 120, SpecialAttack = 85, SpecialDefense = 70, Speed = 60 }
                    };

                    var rhyhorn = new Pokemon
                    {
                        NationalDexNumber = 111,
                        Name = "Rhyhorn",
                        Generation = 1,
                        Category = "Spikes",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Field }
                    };

                    var formrhyhorn = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground, PokemonType.Rock },
                        Stats = new BaseStats { HP = 80, Attack = 85, Defense = 95, SpecialAttack = 30, SpecialDefense = 30, Speed = 25 }
                    };

                    var rhydon = new Pokemon
                    {
                        NationalDexNumber = 112,
                        Name = "Rhydon",
                        Generation = 1,
                        Category = "Drill",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Field }
                    };

                    var formrhydon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground, PokemonType.Rock },
                        Stats = new BaseStats { HP = 105, Attack = 130, Defense = 120, SpecialAttack = 45, SpecialDefense = 45, Speed = 40 }
                    };

                    var chansey = new Pokemon
                    {
                        NationalDexNumber = 113,
                        Name = "Chansey",
                        Generation = 1,
                        Category = "Egg",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy }
                    };

                    var formchansey = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 250, Attack = 5, Defense = 5, SpecialAttack = 35, SpecialDefense = 105, Speed = 50 }
                    };

                    var tangela = new Pokemon
                    {
                        NationalDexNumber = 114,
                        Name = "Tangela",
                        Generation = 1,
                        Category = "Vine",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formtangela = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats { HP = 65, Attack = 55, Defense = 115, SpecialAttack = 100, SpecialDefense = 40, Speed = 60 }
                    };

                    var kangaskhan = new Pokemon
                    {
                        NationalDexNumber = 115,
                        Name = "Kangaskhan",
                        Generation = 1,
                        Category = "Parent",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formkangaskhan = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 105, Attack = 95, Defense = 80, SpecialAttack = 40, SpecialDefense = 80, Speed = 90 }
                    };

                    var horsea = new Pokemon
                    {
                        NationalDexNumber = 116,
                        Name = "Horsea",
                        Generation = 1,
                        Category = "Dragon",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Dragon }
                    };

                    var formhorsea = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 30, Attack = 40, Defense = 70, SpecialAttack = 70, SpecialDefense = 25, Speed = 60 }
                    };

                    var seadra = new Pokemon
                    {
                        NationalDexNumber = 117,
                        Name = "Seadra",
                        Generation = 1,
                        Category = "Dragon",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Dragon }
                    };

                    var formseadra = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 55, Attack = 65, Defense = 95, SpecialAttack = 95, SpecialDefense = 45, Speed = 85 }
                    };

                    var goldeen = new Pokemon
                    {
                        NationalDexNumber = 118,
                        Name = "Goldeen",
                        Generation = 1,
                        Category = "Goldfish",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };

                    var formgoldeen = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 45, Attack = 67, Defense = 60, SpecialAttack = 35, SpecialDefense = 50, Speed = 63 }
                    };

                    var seaking = new Pokemon
                    {
                        NationalDexNumber = 119,
                        Name = "Seaking",
                        Generation = 1,
                        Category = "Goldfish",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };

                    var formseaking = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 80, Attack = 92, Defense = 65, SpecialAttack = 65, SpecialDefense = 80, Speed = 68 }
                    };

                    var staryu = new Pokemon
                    {
                        NationalDexNumber = 120,
                        Name = "Staryu",
                        Generation = 1,
                        Category = "Star Shape",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };

                    var formstaryu = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 30, Attack = 45, Defense = 55, SpecialAttack = 70, SpecialDefense = 55, Speed = 85 }
                    };

                    var starmie = new Pokemon
                    {
                        NationalDexNumber = 121,
                        Name = "Starmie",
                        Generation = 1,
                        Category = "Mysterious",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };

                    var formstarmie = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 60, Attack = 75, Defense = 85, SpecialAttack = 100, SpecialDefense = 85, Speed = 115 }
                    };

                    var mrMime = new Pokemon
                    {
                        NationalDexNumber = 122,
                        Name = "Mr. Mime",
                        Generation = 1,
                        Category = "Barrier",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formmrMime = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic, PokemonType.Fairy },
                        Stats = new BaseStats { HP = 40, Attack = 45, Defense = 65, SpecialAttack = 100, SpecialDefense = 120, Speed = 90 }
                    };

                    var scyther = new Pokemon
                    {
                        NationalDexNumber = 123,
                        Name = "Scyther",
                        Generation = 1,
                        Category = "Mantis",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formscyther = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Flying },
                        Stats = new BaseStats { HP = 70, Attack = 110, Defense = 80, SpecialAttack = 55, SpecialDefense = 80, Speed = 105 }
                    };

                    var jynx = new Pokemon
                    {
                        NationalDexNumber = 124,
                        Name = "Jynx",
                        Generation = 1,
                        Category = "Human Shape",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formjynx = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 65, Attack = 50, Defense = 35, SpecialAttack = 115, SpecialDefense = 95, Speed = 95 }
                    };

                    var electabuzz = new Pokemon
                    {
                        NationalDexNumber = 125,
                        Name = "Electabuzz",
                        Generation = 1,
                        Category = "Electric",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formelectabuzz = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats { HP = 65, Attack = 83, Defense = 57, SpecialAttack = 95, SpecialDefense = 85, Speed = 105 }
                    };

                    // #126 - Magmar
                    var magmar = new Pokemon
                    {
                        NationalDexNumber = 126,
                        Name = "Magmar",
                        Generation = 1,
                        Category = "Spitfire",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formmagmar = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 95,
                            Defense = 57,
                            SpecialAttack = 100,
                            SpecialDefense = 85,
                            Speed = 93
                        }
                    };

                    magmar.Forms.Add(formmagmar);
                    db.Pokemons.Add(magmar);


                    // #127 - Pinsir
                    var pinsir = new Pokemon
                    {
                        NationalDexNumber = 127,
                        Name = "Pinsir",
                        Generation = 1,
                        Category = "Stag Beetle",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formpinsir = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 125,
                            Defense = 100,
                            SpecialAttack = 55,
                            SpecialDefense = 70,
                            Speed = 85
                        }
                    };

                    pinsir.Forms.Add(formpinsir);
                    db.Pokemons.Add(pinsir);


                    // #128 - Tauros
                    var tauros = new Pokemon
                    {
                        NationalDexNumber = 128,
                        Name = "Tauros",
                        Generation = 1,
                        Category = "Wild Bull",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formtauros = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 100,
                            Defense = 95,
                            SpecialAttack = 40,
                            SpecialDefense = 70,
                            Speed = 110
                        }
                    };

                    tauros.Forms.Add(formtauros);
                    db.Pokemons.Add(tauros);


                    // #129 - Magikarp
                    var magikarp = new Pokemon
                    {
                        NationalDexNumber = 129,
                        Name = "Magikarp",
                        Generation = 1,
                        Category = "Fish",
                        EggGroups = new List<EggGroup> { EggGroup.Water2, EggGroup.Dragon }
                    };

                    var formmagikarp = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 20,
                            Attack = 10,
                            Defense = 55,
                            SpecialAttack = 15,
                            SpecialDefense = 20,
                            Speed = 80
                        }
                    };

                    magikarp.Forms.Add(formmagikarp);
                    db.Pokemons.Add(magikarp);


                    // #130 - Gyarados
                    var gyarados = new Pokemon
                    {
                        NationalDexNumber = 130,
                        Name = "Gyarados",
                        Generation = 1,
                        Category = "Atrocious",
                        EggGroups = new List<EggGroup> { EggGroup.Water2, EggGroup.Dragon }
                    };

                    var formgyarados = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 95,
                            Attack = 125,
                            Defense = 79,
                            SpecialAttack = 60,
                            SpecialDefense = 100,
                            Speed = 81
                        }
                    };

                    gyarados.Forms.Add(formgyarados);
                    db.Pokemons.Add(gyarados);


                    // #131 - Lapras
                    var lapras = new Pokemon
                    {
                        NationalDexNumber = 131,
                        Name = "Lapras",
                        Generation = 1,
                        Category = "Transport",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formlapras = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Ice },
                        Stats = new BaseStats
                        {
                            HP = 130,
                            Attack = 85,
                            Defense = 80,
                            SpecialAttack = 85,
                            SpecialDefense = 95,
                            Speed = 60
                        }
                    };

                    lapras.Forms.Add(formlapras);
                    db.Pokemons.Add(lapras);


                    // #132 - Ditto
                    var ditto = new Pokemon
                    {
                        NationalDexNumber = 132,
                        Name = "Ditto",
                        Generation = 1,
                        Category = "Transform",
                        EggGroups = new List<EggGroup> { EggGroup.Ditto }
                    };

                    var formditto = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 48,
                            Attack = 48,
                            Defense = 48,
                            SpecialAttack = 48,
                            SpecialDefense = 48,
                            Speed = 48
                        }
                    };

                    ditto.Forms.Add(formditto);
                    db.Pokemons.Add(ditto);


                    // #133 - Eevee
                    var eevee = new Pokemon
                    {
                        NationalDexNumber = 133,
                        Name = "Eevee",
                        Generation = 1,
                        Category = "Evolution",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formEevee = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 55,
                            Attack = 55,
                            Defense = 50,
                            SpecialAttack = 45,
                            SpecialDefense = 65,
                            Speed = 55
                        }
                    };

                    eevee.Forms.Add(formEevee);
                    db.Pokemons.Add(eevee);


                    // #134 - Vaporeon
                    var vaporeon = new Pokemon
                    {
                        NationalDexNumber = 134,
                        Name = "Vaporeon",
                        Generation = 1,
                        Category = "Bubble Jet",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formvaporeon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 130,
                            Attack = 65,
                            Defense = 60,
                            SpecialAttack = 110,
                            SpecialDefense = 95,
                            Speed = 65
                        }
                    };

                    vaporeon.Forms.Add(formvaporeon);
                    db.Pokemons.Add(vaporeon);


                    // #135 - Jolteon
                    var jolteon = new Pokemon
                    {
                        NationalDexNumber = 135,
                        Name = "Jolteon",
                        Generation = 1,
                        Category = "Lightning",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formjolteon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 65,
                            Defense = 60,
                            SpecialAttack = 110,
                            SpecialDefense = 95,
                            Speed = 130
                        }
                    };

                    jolteon.Forms.Add(formjolteon);
                    db.Pokemons.Add(jolteon);


                    // #136 - Flareon
                    var flareon = new Pokemon
                    {
                        NationalDexNumber = 136,
                        Name = "Flareon",
                        Generation = 1,
                        Category = "Flame",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formflareon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 130,
                            Defense = 60,
                            SpecialAttack = 95,
                            SpecialDefense = 110,
                            Speed = 65
                        }
                    };

                    flareon.Forms.Add(formflareon);
                    db.Pokemons.Add(flareon);


                    // #137 - Porygon
                    var porygon = new Pokemon
                    {
                        NationalDexNumber = 137,
                        Name = "Porygon",
                        Generation = 1,
                        Category = "Virtual",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formporygon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 60,
                            Defense = 70,
                            SpecialAttack = 85,
                            SpecialDefense = 75,
                            Speed = 40
                        }
                    };

                    porygon.Forms.Add(formporygon);
                    db.Pokemons.Add(porygon);


                    // #138 - Omanyte
                    var omanyte = new Pokemon
                    {
                        NationalDexNumber = 138,
                        Name = "Omanyte",
                        Generation = 1,
                        Category = "Spiral",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water3 }
                    };

                    var formomanyte = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 35,
                            Attack = 40,
                            Defense = 100,
                            SpecialAttack = 90,
                            SpecialDefense = 55,
                            Speed = 35
                        }
                    };

                    omanyte.Forms.Add(formomanyte);
                    db.Pokemons.Add(omanyte);


                    // #139 - Omastar
                    var omastar = new Pokemon
                    {
                        NationalDexNumber = 139,
                        Name = "Omastar",
                        Generation = 1,
                        Category = "Spiral",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water3 }
                    };

                    var formomastar = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 60,
                            Defense = 125,
                            SpecialAttack = 115,
                            SpecialDefense = 70,
                            Speed = 55
                        }
                    };

                    omastar.Forms.Add(formomastar);
                    db.Pokemons.Add(omastar);


                    // #140 - Kabuto
                    var kabuto = new Pokemon
                    {
                        NationalDexNumber = 140,
                        Name = "Kabuto",
                        Generation = 1,
                        Category = "Shellfish",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water3 }
                    };

                    var formkabuto = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 30,
                            Attack = 80,
                            Defense = 90,
                            SpecialAttack = 55,
                            SpecialDefense = 45,
                            Speed = 55
                        }
                    };

                    kabuto.Forms.Add(formkabuto);
                    db.Pokemons.Add(kabuto);


                    // #141 - Kabutops
                    var kabutops = new Pokemon
                    {
                        NationalDexNumber = 141,
                        Name = "Kabutops",
                        Generation = 1,
                        Category = "Shellfish",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water3 }
                    };

                    var formkabutops = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 115,
                            Defense = 105,
                            SpecialAttack = 65,
                            SpecialDefense = 70,
                            Speed = 80
                        }
                    };

                    kabutops.Forms.Add(formkabutops);
                    db.Pokemons.Add(kabutops);


                    // #142 - Aerodactyl
                    var aerodactyl = new Pokemon
                    {
                        NationalDexNumber = 142,
                        Name = "Aerodactyl",
                        Generation = 1,
                        Category = "Fossil",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formaerodactyl = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 80,
                            Attack = 105,
                            Defense = 65,
                            SpecialAttack = 60,
                            SpecialDefense = 75,
                            Speed = 130
                        }
                    };

                    aerodactyl.Forms.Add(formaerodactyl);
                    db.Pokemons.Add(aerodactyl);


                    // #143 - Snorlax
                    var snorlax = new Pokemon
                    {
                        NationalDexNumber = 143,
                        Name = "Snorlax",
                        Generation = 1,
                        Category = "Sleeping",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formsnorlax = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 160,
                            Attack = 110,
                            Defense = 65,
                            SpecialAttack = 65,
                            SpecialDefense = 110,
                            Speed = 30
                        }
                    };

                    snorlax.Forms.Add(formsnorlax);
                    db.Pokemons.Add(snorlax);


                    // #144 - Articuno
                    var articuno = new Pokemon
                    {
                        NationalDexNumber = 144,
                        Name = "Articuno",
                        Generation = 1,
                        Category = "Freeze",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formarticuno = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 85,
                            Defense = 100,
                            SpecialAttack = 95,
                            SpecialDefense = 125,
                            Speed = 85
                        }
                    };

                    articuno.Forms.Add(formarticuno);
                    db.Pokemons.Add(articuno);


                    // #145 - Zapdos
                    var zapdos = new Pokemon
                    {
                        NationalDexNumber = 145,
                        Name = "Zapdos",
                        Generation = 1,
                        Category = "Electric",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formzapdos = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 90,
                            Defense = 85,
                            SpecialAttack = 125,
                            SpecialDefense = 90,
                            Speed = 100
                        }
                    };

                    zapdos.Forms.Add(formzapdos);
                    db.Pokemons.Add(zapdos);


                    // #146 - Moltres
                    var moltres = new Pokemon
                    {
                        NationalDexNumber = 146,
                        Name = "Moltres",
                        Generation = 1,
                        Category = "Flame",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formmoltres = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 100,
                            Defense = 90,
                            SpecialAttack = 125,
                            SpecialDefense = 85,
                            Speed = 90
                        }
                    };

                    moltres.Forms.Add(formmoltres);
                    db.Pokemons.Add(moltres);


                    // #147 - Dratini
                    var dratini = new Pokemon
                    {
                        NationalDexNumber = 147,
                        Name = "Dratini",
                        Generation = 1,
                        Category = "Dragon",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Dragon }
                    };

                    var formdratini = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon },
                        Stats = new BaseStats
                        {
                            HP = 41,
                            Attack = 64,
                            Defense = 45,
                            SpecialAttack = 50,
                            SpecialDefense = 50,
                            Speed = 50
                        }
                    };

                    dratini.Forms.Add(formdratini);
                    db.Pokemons.Add(dratini);


                    // #148 - Dragonair
                    var dragonair = new Pokemon
                    {
                        NationalDexNumber = 148,
                        Name = "Dragonair",
                        Generation = 1,
                        Category = "Dragon",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Dragon }
                    };

                    var formdragonair = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon },
                        Stats = new BaseStats
                        {
                            HP = 61,
                            Attack = 84,
                            Defense = 65,
                            SpecialAttack = 70,
                            SpecialDefense = 70,
                            Speed = 70
                        }
                    };

                    dragonair.Forms.Add(formdragonair);
                    db.Pokemons.Add(dragonair);


                    // #149 - Dragonite
                    var dragonite = new Pokemon
                    {
                        NationalDexNumber = 149,
                        Name = "Dragonite",
                        Generation = 1,
                        Category = "Dragon",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Dragon }
                    };

                    var formdragonite = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 91,
                            Attack = 134,
                            Defense = 95,
                            SpecialAttack = 100,
                            SpecialDefense = 100,
                            Speed = 80
                        }
                    };

                    dragonite.Forms.Add(formdragonite);
                    db.Pokemons.Add(dragonite);


                    // #150 - Mewtwo
                    var mewtwo = new Pokemon
                    {
                        NationalDexNumber = 150,
                        Name = "Mewtwo",
                        Generation = 1,
                        Category = "Genetic",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formmewtwo = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 106,
                            Attack = 110,
                            Defense = 90,
                            SpecialAttack = 154,
                            SpecialDefense = 90,
                            Speed = 130
                        }
                    };

                    mewtwo.Forms.Add(formmewtwo);
                    db.Pokemons.Add(mewtwo);

                    // #151 - Mew
                    var mew = new Pokemon
                    {
                        NationalDexNumber = 151,
                        Name = "Mew",
                        Generation = 1,
                        Category = "New Species",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formmew = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 100,
                            Defense = 100,
                            SpecialAttack = 100,
                            SpecialDefense = 100,
                            Speed = 100
                        }
                    };

                    mew.Forms.Add(formmew);
                    db.Pokemons.Add(mew);


                    // #152 - Chikorita
                    var chikorita = new Pokemon
                    {
                        NationalDexNumber = 152,
                        Name = "Chikorita",
                        Generation = 2,
                        Category = "Leaf",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Grass }
                    };

                    var formchikorita = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 45,
                            Attack = 49,
                            Defense = 65,
                            SpecialAttack = 49,
                            SpecialDefense = 65,
                            Speed = 45
                        }
                    };

                    chikorita.Forms.Add(formchikorita);
                    db.Pokemons.Add(chikorita);


                    // #153 - Bayleef
                    var bayleef = new Pokemon
                    {
                        NationalDexNumber = 153,
                        Name = "Bayleef",
                        Generation = 2,
                        Category = "Leaf",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Grass }
                    };

                    var formbayleef = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 62,
                            Defense = 80,
                            SpecialAttack = 63,
                            SpecialDefense = 80,
                            Speed = 60
                        }
                    };

                    bayleef.Forms.Add(formbayleef);
                    db.Pokemons.Add(bayleef);


                    // #154 - Meganium
                    var meganium = new Pokemon
                    {
                        NationalDexNumber = 154,
                        Name = "Meganium",
                        Generation = 2,
                        Category = "Herb",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Grass }
                    };

                    var formmeganium = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 80,
                            Attack = 82,
                            Defense = 100,
                            SpecialAttack = 83,
                            SpecialDefense = 100,
                            Speed = 80
                        }
                    };

                    meganium.Forms.Add(formmeganium);
                    db.Pokemons.Add(meganium);


                    // #155 - Cyndaquil
                    var cyndaquil = new Pokemon
                    {
                        NationalDexNumber = 155,
                        Name = "Cyndaquil",
                        Generation = 2,
                        Category = "Fire Mouse",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formcyndaquil = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 39,
                            Attack = 52,
                            Defense = 43,
                            SpecialAttack = 60,
                            SpecialDefense = 50,
                            Speed = 65
                        }
                    };

                    cyndaquil.Forms.Add(formcyndaquil);
                    db.Pokemons.Add(cyndaquil);


                    // #156 - Quilava
                    var quilava = new Pokemon
                    {
                        NationalDexNumber = 156,
                        Name = "Quilava",
                        Generation = 2,
                        Category = "Volcano",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formquilava = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 58,
                            Attack = 64,
                            Defense = 58,
                            SpecialAttack = 80,
                            SpecialDefense = 65,
                            Speed = 80
                        }
                    };

                    quilava.Forms.Add(formquilava);
                    db.Pokemons.Add(quilava);


                    // #157 - Typhlosion
                    var typhlosion = new Pokemon
                    {
                        NationalDexNumber = 157,
                        Name = "Typhlosion",
                        Generation = 2,
                        Category = "Volcano",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formtyphlosion = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 78,
                            Attack = 84,
                            Defense = 78,
                            SpecialAttack = 109,
                            SpecialDefense = 85,
                            Speed = 100
                        }
                    };

                    typhlosion.Forms.Add(formtyphlosion);
                    db.Pokemons.Add(typhlosion);


                    // #158 - Totodile
                    var totodile = new Pokemon
                    {
                        NationalDexNumber = 158,
                        Name = "Totodile",
                        Generation = 2,
                        Category = "Big Jaw",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formtotodile = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 65,
                            Defense = 64,
                            SpecialAttack = 44,
                            SpecialDefense = 48,
                            Speed = 43
                        }
                    };

                    totodile.Forms.Add(formtotodile);
                    db.Pokemons.Add(totodile);


                    // #159 - Croconaw
                    var croconaw = new Pokemon
                    {
                        NationalDexNumber = 159,
                        Name = "Croconaw",
                        Generation = 2,
                        Category = "Big Jaw",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formcroconaw = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 80,
                            Defense = 80,
                            SpecialAttack = 59,
                            SpecialDefense = 63,
                            Speed = 58
                        }
                    };

                    croconaw.Forms.Add(formcroconaw);
                    db.Pokemons.Add(croconaw);


                    // #160 - Feraligatr
                    var feraligatr = new Pokemon
                    {
                        NationalDexNumber = 160,
                        Name = "Feraligatr",
                        Generation = 2,
                        Category = "Big Jaw",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formferaligatr = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 85,
                            Attack = 105,
                            Defense = 100,
                            SpecialAttack = 79,
                            SpecialDefense = 83,
                            Speed = 78
                        }
                    };

                    feraligatr.Forms.Add(formferaligatr);
                    db.Pokemons.Add(feraligatr);


                    // #161 - Sentret
                    var sentret = new Pokemon
                    {
                        NationalDexNumber = 161,
                        Name = "Sentret",
                        Generation = 2,
                        Category = "Scout",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formsentret = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 35,
                            Attack = 46,
                            Defense = 34,
                            SpecialAttack = 35,
                            SpecialDefense = 45,
                            Speed = 20
                        }
                    };

                    sentret.Forms.Add(formsentret);
                    db.Pokemons.Add(sentret);


                    // #162 - Furret
                    var furret = new Pokemon
                    {
                        NationalDexNumber = 162,
                        Name = "Furret",
                        Generation = 2,
                        Category = "Long Body",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formfurret = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 85,
                            Attack = 76,
                            Defense = 64,
                            SpecialAttack = 45,
                            SpecialDefense = 55,
                            Speed = 90
                        }
                    };

                    furret.Forms.Add(formfurret);
                    db.Pokemons.Add(furret);


                    // #163 - Hoothoot
                    var hoothoot = new Pokemon
                    {
                        NationalDexNumber = 163,
                        Name = "Hoothoot",
                        Generation = 2,
                        Category = "Owl",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formhoothoot = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 30,
                            Defense = 30,
                            SpecialAttack = 36,
                            SpecialDefense = 56,
                            Speed = 50
                        }
                    };

                    hoothoot.Forms.Add(formhoothoot);
                    db.Pokemons.Add(hoothoot);


                    // #164 - Noctowl
                    var noctowl = new Pokemon
                    {
                        NationalDexNumber = 164,
                        Name = "Noctowl",
                        Generation = 2,
                        Category = "Owl",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formnoctowl = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 50,
                            Defense = 50,
                            SpecialAttack = 86,
                            SpecialDefense = 96,
                            Speed = 70
                        }
                    };

                    noctowl.Forms.Add(formnoctowl);
                    db.Pokemons.Add(noctowl);


                    // #165 - Ledyba
                    var ledyba = new Pokemon
                    {
                        NationalDexNumber = 165,
                        Name = "Ledyba",
                        Generation = 2,
                        Category = "Five Star",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formledyba = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 20,
                            Defense = 30,
                            SpecialAttack = 40,
                            SpecialDefense = 80,
                            Speed = 55
                        }
                    };

                    ledyba.Forms.Add(formledyba);
                    db.Pokemons.Add(ledyba);


                    // #166 - Ledian
                    var ledian = new Pokemon
                    {
                        NationalDexNumber = 166,
                        Name = "Ledian",
                        Generation = 2,
                        Category = "Five Star",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formledian = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 55,
                            Attack = 35,
                            Defense = 50,
                            SpecialAttack = 55,
                            SpecialDefense = 110,
                            Speed = 85
                        }
                    };

                    ledian.Forms.Add(formledian);
                    db.Pokemons.Add(ledian);


                    // #167 - Spinarak
                    var spinarak = new Pokemon
                    {
                        NationalDexNumber = 167,
                        Name = "Spinarak",
                        Generation = 2,
                        Category = "String Spit",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formspinarak = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Poison },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 60,
                            Defense = 40,
                            SpecialAttack = 40,
                            SpecialDefense = 40,
                            Speed = 30
                        }
                    };

                    spinarak.Forms.Add(formspinarak);
                    db.Pokemons.Add(spinarak);


                    // #168 - Ariados
                    var ariados = new Pokemon
                    {
                        NationalDexNumber = 168,
                        Name = "Ariados",
                        Generation = 2,
                        Category = "Long Leg",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formariados = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Poison },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 90,
                            Defense = 70,
                            SpecialAttack = 60,
                            SpecialDefense = 70,
                            Speed = 40
                        }
                    };

                    ariados.Forms.Add(formariados);
                    db.Pokemons.Add(ariados);


                    // #169 - Crobat
                    var crobat = new Pokemon
                    {
                        NationalDexNumber = 169,
                        Name = "Crobat",
                        Generation = 2,
                        Category = "Bat",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formcrobat = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 85,
                            Attack = 90,
                            Defense = 80,
                            SpecialAttack = 70,
                            SpecialDefense = 80,
                            Speed = 130
                        }
                    };

                    crobat.Forms.Add(formcrobat);
                    db.Pokemons.Add(crobat);


                    // #170 - Chinchou
                    var chinchou = new Pokemon
                    {
                        NationalDexNumber = 170,
                        Name = "Chinchou",
                        Generation = 2,
                        Category = "Angler",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };

                    var formchinchou = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 38,
                            Defense = 38,
                            SpecialAttack = 56,
                            SpecialDefense = 56,
                            Speed = 67
                        }
                    };

                    chinchou.Forms.Add(formchinchou);
                    db.Pokemons.Add(chinchou);


                    // #171 - Lanturn
                    var lanturn = new Pokemon
                    {
                        NationalDexNumber = 171,
                        Name = "Lanturn",
                        Generation = 2,
                        Category = "Light",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };

                    var formlanturn = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 125,
                            Attack = 58,
                            Defense = 58,
                            SpecialAttack = 76,
                            SpecialDefense = 76,
                            Speed = 67
                        }
                    };

                    lanturn.Forms.Add(formlanturn);
                    db.Pokemons.Add(lanturn);


                    // #172 - Pichu
                    var pichu = new Pokemon
                    {
                        NationalDexNumber = 172,
                        Name = "Pichu",
                        Generation = 2,
                        Category = "Tiny Mouse",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formpichu = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 20,
                            Attack = 40,
                            Defense = 15,
                            SpecialAttack = 35,
                            SpecialDefense = 35,
                            Speed = 60
                        }
                    };

                    pichu.Forms.Add(formpichu);
                    db.Pokemons.Add(pichu);


                    // #173 - Cleffa
                    var cleffa = new Pokemon
                    {
                        NationalDexNumber = 173,
                        Name = "Cleffa",
                        Generation = 2,
                        Category = "Star Shape",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formcleffa = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 25,
                            Defense = 28,
                            SpecialAttack = 45,
                            SpecialDefense = 55,
                            Speed = 15
                        }
                    };

                    cleffa.Forms.Add(formcleffa);
                    db.Pokemons.Add(cleffa);


                    // #174 - Igglybuff
                    var igglybuff = new Pokemon
                    {
                        NationalDexNumber = 174,
                        Name = "Igglybuff",
                        Generation = 2,
                        Category = "Balloon",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formigglybuff = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 30,
                            Defense = 15,
                            SpecialAttack = 40,
                            SpecialDefense = 20,
                            Speed = 15
                        }
                    };

                    igglybuff.Forms.Add(formigglybuff);
                    db.Pokemons.Add(igglybuff);


                    // #175 - Togepi
                    var togepi = new Pokemon
                    {
                        NationalDexNumber = 175,
                        Name = "Togepi",
                        Generation = 2,
                        Category = "Spike Ball",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formtogepi = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 35,
                            Attack = 20,
                            Defense = 65,
                            SpecialAttack = 40,
                            SpecialDefense = 65,
                            Speed = 20
                        }
                    };

                    togepi.Forms.Add(formtogepi);
                    db.Pokemons.Add(togepi);

                    // #176 - Togetic
                    var togetic = new Pokemon
                    {
                        NationalDexNumber = 176,
                        Name = "Togetic",
                        Generation = 2,
                        Category = "Happiness",
                        EggGroups = new List<EggGroup> { EggGroup.Flying, EggGroup.Fairy }
                    };

                    var formtogetic = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fairy, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 55,
                            Attack = 40,
                            Defense = 85,
                            SpecialAttack = 80,
                            SpecialDefense = 105,
                            Speed = 40
                        }
                    };

                    togetic.Forms.Add(formtogetic);
                    db.Pokemons.Add(togetic);


                    // #177 - Natu
                    var natu = new Pokemon
                    {
                        NationalDexNumber = 177,
                        Name = "Natu",
                        Generation = 2,
                        Category = "Tiny Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formnatu = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 50,
                            Defense = 45,
                            SpecialAttack = 70,
                            SpecialDefense = 45,
                            Speed = 70
                        }
                    };

                    natu.Forms.Add(formnatu);
                    db.Pokemons.Add(natu);


                    // #178 - Xatu
                    var xatu = new Pokemon
                    {
                        NationalDexNumber = 178,
                        Name = "Xatu",
                        Generation = 2,
                        Category = "Mystic",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formxatu = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 75,
                            Defense = 70,
                            SpecialAttack = 95,
                            SpecialDefense = 70,
                            Speed = 95
                        }
                    };

                    xatu.Forms.Add(formxatu);
                    db.Pokemons.Add(xatu);


                    // #179 - Mareep
                    var mareep = new Pokemon
                    {
                        NationalDexNumber = 179,
                        Name = "Mareep",
                        Generation = 2,
                        Category = "Wool",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Field }
                    };

                    var formmareep = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 55,
                            Attack = 40,
                            Defense = 40,
                            SpecialAttack = 65,
                            SpecialDefense = 45,
                            Speed = 35
                        }
                    };

                    mareep.Forms.Add(formmareep);
                    db.Pokemons.Add(mareep);


                    // #180 - Flaaffy
                    var flaaffy = new Pokemon
                    {
                        NationalDexNumber = 180,
                        Name = "Flaaffy",
                        Generation = 2,
                        Category = "Wool",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Field }
                    };

                    var formflaaffy = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 55,
                            Defense = 55,
                            SpecialAttack = 80,
                            SpecialDefense = 60,
                            Speed = 45
                        }
                    };

                    flaaffy.Forms.Add(formflaaffy);
                    db.Pokemons.Add(flaaffy);


                    // #181 - Ampharos
                    var ampharos = new Pokemon
                    {
                        NationalDexNumber = 181,
                        Name = "Ampharos",
                        Generation = 2,
                        Category = "Light",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Field }
                    };

                    var formampharos = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 75,
                            Defense = 85,
                            SpecialAttack = 115,
                            SpecialDefense = 90,
                            Speed = 55
                        }
                    };

                    ampharos.Forms.Add(formampharos);
                    db.Pokemons.Add(ampharos);


                    // #182 - Bellossom
                    var bellossom = new Pokemon
                    {
                        NationalDexNumber = 182,
                        Name = "Bellossom",
                        Generation = 2,
                        Category = "Flower",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formbellossom = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 80,
                            Defense = 95,
                            SpecialAttack = 90,
                            SpecialDefense = 100,
                            Speed = 50
                        }
                    };

                    bellossom.Forms.Add(formbellossom);
                    db.Pokemons.Add(bellossom);


                    // #183 - Marill
                    var marill = new Pokemon
                    {
                        NationalDexNumber = 183,
                        Name = "Marill",
                        Generation = 2,
                        Category = "Aqua Mouse",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Fairy }
                    };

                    var formmarill = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 20,
                            Defense = 50,
                            SpecialAttack = 20,
                            SpecialDefense = 50,
                            Speed = 40
                        }
                    };

                    marill.Forms.Add(formmarill);
                    db.Pokemons.Add(marill);


                    // #184 - Azumarill
                    var azumarill = new Pokemon
                    {
                        NationalDexNumber = 184,
                        Name = "Azumarill",
                        Generation = 2,
                        Category = "Aqua Rabbit",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Fairy }
                    };

                    var formazumarill = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 50,
                            Defense = 80,
                            SpecialAttack = 60,
                            SpecialDefense = 80,
                            Speed = 50
                        }
                    };

                    azumarill.Forms.Add(formazumarill);
                    db.Pokemons.Add(azumarill);


                    // #185 - Sudowoodo
                    var sudowoodo = new Pokemon
                    {
                        NationalDexNumber = 185,
                        Name = "Sudowoodo",
                        Generation = 2,
                        Category = "Imitation",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formsudowoodo = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 100,
                            Defense = 115,
                            SpecialAttack = 30,
                            SpecialDefense = 65,
                            Speed = 30
                        }
                    };

                    sudowoodo.Forms.Add(formsudowoodo);
                    db.Pokemons.Add(sudowoodo);


                    // #186 - Politoed
                    var politoed = new Pokemon
                    {
                        NationalDexNumber = 186,
                        Name = "Politoed",
                        Generation = 2,
                        Category = "Frog",
                        EggGroups = new List<EggGroup> { EggGroup.Water1 }
                    };

                    var formpolitoed = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 75,
                            Defense = 75,
                            SpecialAttack = 90,
                            SpecialDefense = 100,
                            Speed = 70
                        }
                    };

                    politoed.Forms.Add(formpolitoed);
                    db.Pokemons.Add(politoed);


                    // #187 - Hoppip
                    var hoppip = new Pokemon
                    {
                        NationalDexNumber = 187,
                        Name = "Hoppip",
                        Generation = 2,
                        Category = "Cottonweed",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy, EggGroup.Grass }
                    };

                    var formhoppip = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 35,
                            Attack = 35,
                            Defense = 40,
                            SpecialAttack = 35,
                            SpecialDefense = 55,
                            Speed = 50
                        }
                    };

                    hoppip.Forms.Add(formhoppip);
                    db.Pokemons.Add(hoppip);


                    // #188 - Skiploom
                    var skiploom = new Pokemon
                    {
                        NationalDexNumber = 188,
                        Name = "Skiploom",
                        Generation = 2,
                        Category = "Cottonweed",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy, EggGroup.Grass }
                    };

                    var formskiploom = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 55,
                            Attack = 45,
                            Defense = 50,
                            SpecialAttack = 45,
                            SpecialDefense = 65,
                            Speed = 80
                        }
                    };

                    skiploom.Forms.Add(formskiploom);
                    db.Pokemons.Add(skiploom);


                    // #189 - Jumpluff
                    var jumpluff = new Pokemon
                    {
                        NationalDexNumber = 189,
                        Name = "Jumpluff",
                        Generation = 2,
                        Category = "Cottonweed",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy, EggGroup.Grass }
                    };

                    var formjumpluff = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 55,
                            Defense = 70,
                            SpecialAttack = 55,
                            SpecialDefense = 95,
                            Speed = 110
                        }
                    };

                    jumpluff.Forms.Add(formjumpluff);
                    db.Pokemons.Add(jumpluff);


                    // #190 - Aipom
                    var aipom = new Pokemon
                    {
                        NationalDexNumber = 190,
                        Name = "Aipom",
                        Generation = 2,
                        Category = "Long Tail",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formaipom = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 55,
                            Attack = 70,
                            Defense = 55,
                            SpecialAttack = 40,
                            SpecialDefense = 55,
                            Speed = 85
                        }
                    };

                    aipom.Forms.Add(formaipom);
                    db.Pokemons.Add(aipom);


                    // #191 - Sunkern
                    var sunkern = new Pokemon
                    {
                        NationalDexNumber = 191,
                        Name = "Sunkern",
                        Generation = 2,
                        Category = "Seed",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formsunkern = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 30,
                            Attack = 30,
                            Defense = 30,
                            SpecialAttack = 30,
                            SpecialDefense = 30,
                            Speed = 30
                        }
                    };

                    sunkern.Forms.Add(formsunkern);
                    db.Pokemons.Add(sunkern);


                    // #192 - Sunflora
                    var sunflora = new Pokemon
                    {
                        NationalDexNumber = 192,
                        Name = "Sunflora",
                        Generation = 2,
                        Category = "Sun",
                        EggGroups = new List<EggGroup> { EggGroup.Grass }
                    };

                    var formsunflora = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 75,
                            Defense = 55,
                            SpecialAttack = 105,
                            SpecialDefense = 85,
                            Speed = 30
                        }
                    };

                    sunflora.Forms.Add(formsunflora);
                    db.Pokemons.Add(sunflora);


                    // #193 - Yanma
                    var yanma = new Pokemon
                    {
                        NationalDexNumber = 193,
                        Name = "Yanma",
                        Generation = 2,
                        Category = "Clear Wing",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formyanma = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 65,
                            Defense = 45,
                            SpecialAttack = 75,
                            SpecialDefense = 45,
                            Speed = 95
                        }
                    };

                    yanma.Forms.Add(formyanma);
                    db.Pokemons.Add(yanma);


                    // #194 - Wooper
                    var wooper = new Pokemon
                    {
                        NationalDexNumber = 194,
                        Name = "Wooper",
                        Generation = 2,
                        Category = "Water Fish",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Field }
                    };

                    var formwooper = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 55,
                            Attack = 45,
                            Defense = 45,
                            SpecialAttack = 25,
                            SpecialDefense = 25,
                            Speed = 15
                        }
                    };

                    wooper.Forms.Add(formwooper);
                    db.Pokemons.Add(wooper);


                    // #195 - Quagsire
                    var quagsire = new Pokemon
                    {
                        NationalDexNumber = 195,
                        Name = "Quagsire",
                        Generation = 2,
                        Category = "Water Fish",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Field }
                    };

                    var formquagsire = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 95,
                            Attack = 85,
                            Defense = 85,
                            SpecialAttack = 65,
                            SpecialDefense = 65,
                            Speed = 35
                        }
                    };

                    quagsire.Forms.Add(formquagsire);
                    db.Pokemons.Add(quagsire);


                    // #196 - Espeon
                    var espeon = new Pokemon
                    {
                        NationalDexNumber = 196,
                        Name = "Espeon",
                        Generation = 2,
                        Category = "Sun",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formespeon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 65,
                            Defense = 60,
                            SpecialAttack = 130,
                            SpecialDefense = 95,
                            Speed = 110
                        }
                    };

                    espeon.Forms.Add(formespeon);
                    db.Pokemons.Add(espeon);


                    // #197 - Umbreon
                    var umbreon = new Pokemon
                    {
                        NationalDexNumber = 197,
                        Name = "Umbreon",
                        Generation = 2,
                        Category = "Moonlight",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formumbreon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dark },
                        Stats = new BaseStats
                        {
                            HP = 95,
                            Attack = 65,
                            Defense = 110,
                            SpecialAttack = 60,
                            SpecialDefense = 130,
                            Speed = 65
                        }
                    };

                    umbreon.Forms.Add(formumbreon);
                    db.Pokemons.Add(umbreon);


                    // #198 - Murkrow
                    var murkrow = new Pokemon
                    {
                        NationalDexNumber = 198,
                        Name = "Murkrow",
                        Generation = 2,
                        Category = "Darkness",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formmurkrow = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dark, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 85,
                            Defense = 42,
                            SpecialAttack = 85,
                            SpecialDefense = 42,
                            Speed = 91
                        }
                    };

                    murkrow.Forms.Add(formmurkrow);
                    db.Pokemons.Add(murkrow);


                    // #199 - Slowking
                    var slowking = new Pokemon
                    {
                        NationalDexNumber = 199,
                        Name = "Slowking",
                        Generation = 2,
                        Category = "Royal",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formslowking = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 95,
                            Attack = 75,
                            Defense = 80,
                            SpecialAttack = 100,
                            SpecialDefense = 110,
                            Speed = 30
                        }
                    };

                    slowking.Forms.Add(formslowking);
                    db.Pokemons.Add(slowking);


                    // #200 - Misdreavus
                    var misdreavus = new Pokemon
                    {
                        NationalDexNumber = 200,
                        Name = "Misdreavus",
                        Generation = 2,
                        Category = "Screech",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formmisdreavus = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ghost },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 60,
                            Defense = 60,
                            SpecialAttack = 85,
                            SpecialDefense = 85,
                            Speed = 85
                        }
                    };

                    misdreavus.Forms.Add(formmisdreavus);
                    db.Pokemons.Add(misdreavus);

                    // #201 - Unown
                    var unown = new Pokemon
                    {
                        NationalDexNumber = 201,
                        Name = "Unown",
                        Generation = 2,
                        Category = "Symbol",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formunown = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 48,
                            Attack = 72,
                            Defense = 48,
                            SpecialAttack = 72,
                            SpecialDefense = 48,
                            Speed = 48
                        }
                    };

                    unown.Forms.Add(formunown);
                    db.Pokemons.Add(unown);


                    // #202 - Wobbuffet
                    var wobbuffet = new Pokemon
                    {
                        NationalDexNumber = 202,
                        Name = "Wobbuffet",
                        Generation = 2,
                        Category = "Patient",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formwobbuffet = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 190,
                            Attack = 33,
                            Defense = 58,
                            SpecialAttack = 33,
                            SpecialDefense = 58,
                            Speed = 33
                        }
                    };

                    wobbuffet.Forms.Add(formwobbuffet);
                    db.Pokemons.Add(wobbuffet);


                    // #203 - Girafarig
                    var girafarig = new Pokemon
                    {
                        NationalDexNumber = 203,
                        Name = "Girafarig",
                        Generation = 2,
                        Category = "Long Neck",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formgirafarig = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 80,
                            Defense = 65,
                            SpecialAttack = 90,
                            SpecialDefense = 65,
                            Speed = 85
                        }
                    };

                    girafarig.Forms.Add(formgirafarig);
                    db.Pokemons.Add(girafarig);


                    // #204 - Pineco
                    var pineco = new Pokemon
                    {
                        NationalDexNumber = 204,
                        Name = "Pineco",
                        Generation = 2,
                        Category = "Bagworm",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formpineco = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 65,
                            Defense = 90,
                            SpecialAttack = 35,
                            SpecialDefense = 35,
                            Speed = 15
                        }
                    };

                    pineco.Forms.Add(formpineco);
                    db.Pokemons.Add(pineco);


                    // #205 - Forretress
                    var forretress = new Pokemon
                    {
                        NationalDexNumber = 205,
                        Name = "Forretress",
                        Generation = 2,
                        Category = "Bagworm",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formforretress = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Steel },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 90,
                            Defense = 140,
                            SpecialAttack = 60,
                            SpecialDefense = 60,
                            Speed = 40
                        }
                    };

                    forretress.Forms.Add(formforretress);
                    db.Pokemons.Add(forretress);


                    // #206 - Dunsparce
                    var dunsparce = new Pokemon
                    {
                        NationalDexNumber = 206,
                        Name = "Dunsparce",
                        Generation = 2,
                        Category = "Land Snake",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formdunsparce = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 70,
                            Defense = 70,
                            SpecialAttack = 65,
                            SpecialDefense = 65,
                            Speed = 45
                        }
                    };

                    dunsparce.Forms.Add(formdunsparce);
                    db.Pokemons.Add(dunsparce);


                    // #207 - Gligar
                    var gligar = new Pokemon
                    {
                        NationalDexNumber = 207,
                        Name = "Gligar",
                        Generation = 2,
                        Category = "Fly Scorpion",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formgligar = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 75,
                            Defense = 105,
                            SpecialAttack = 35,
                            SpecialDefense = 65,
                            Speed = 85
                        }
                    };

                    gligar.Forms.Add(formgligar);
                    db.Pokemons.Add(gligar);


                    // #208 - Steelix
                    var steelix = new Pokemon
                    {
                        NationalDexNumber = 208,
                        Name = "Steelix",
                        Generation = 2,
                        Category = "Iron Snake",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formsteelix = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 85,
                            Defense = 200,
                            SpecialAttack = 55,
                            SpecialDefense = 65,
                            Speed = 30
                        }
                    };

                    steelix.Forms.Add(formsteelix);
                    db.Pokemons.Add(steelix);


                    // #209 - Snubbull
                    var snubbull = new Pokemon
                    {
                        NationalDexNumber = 209,
                        Name = "Snubbull",
                        Generation = 2,
                        Category = "Fairy",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
                    };

                    var formsnubbull = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 80,
                            Defense = 50,
                            SpecialAttack = 40,
                            SpecialDefense = 40,
                            Speed = 30
                        }
                    };

                    snubbull.Forms.Add(formsnubbull);
                    db.Pokemons.Add(snubbull);


                    // #210 - Granbull
                    var granbull = new Pokemon
                    {
                        NationalDexNumber = 210,
                        Name = "Granbull",
                        Generation = 2,
                        Category = "Fairy",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
                    };

                    var formgranbull = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 120,
                            Defense = 75,
                            SpecialAttack = 60,
                            SpecialDefense = 60,
                            Speed = 45
                        }
                    };

                    granbull.Forms.Add(formgranbull);
                    db.Pokemons.Add(granbull);


                    // #211 - Qwilfish
                    var qwilfish = new Pokemon
                    {
                        NationalDexNumber = 211,
                        Name = "Qwilfish",
                        Generation = 2,
                        Category = "Balloon",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };

                    var formqwilfish = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Poison },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 95,
                            Defense = 85,
                            SpecialAttack = 55,
                            SpecialDefense = 55,
                            Speed = 85
                        }
                    };

                    qwilfish.Forms.Add(formqwilfish);
                    db.Pokemons.Add(qwilfish);


                    // #212 - Scizor
                    var scizor = new Pokemon
                    {
                        NationalDexNumber = 212,
                        Name = "Scizor",
                        Generation = 2,
                        Category = "Pincer",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formscizor = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Steel },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 130,
                            Defense = 100,
                            SpecialAttack = 55,
                            SpecialDefense = 80,
                            Speed = 65
                        }
                    };

                    scizor.Forms.Add(formscizor);
                    db.Pokemons.Add(scizor);


                    // #213 - Shuckle
                    var shuckle = new Pokemon
                    {
                        NationalDexNumber = 213,
                        Name = "Shuckle",
                        Generation = 2,
                        Category = "Mold",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formshuckle = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Rock },
                        Stats = new BaseStats
                        {
                            HP = 20,
                            Attack = 10,
                            Defense = 230,
                            SpecialAttack = 10,
                            SpecialDefense = 230,
                            Speed = 5
                        }
                    };

                    shuckle.Forms.Add(formshuckle);
                    db.Pokemons.Add(shuckle);


                    // #214 - Heracross
                    var heracross = new Pokemon
                    {
                        NationalDexNumber = 214,
                        Name = "Heracross",
                        Generation = 2,
                        Category = "Single Horn",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formheracross = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Fighting },
                        Stats = new BaseStats
                        {
                            HP = 80,
                            Attack = 125,
                            Defense = 75,
                            SpecialAttack = 40,
                            SpecialDefense = 95,
                            Speed = 85
                        }
                    };

                    heracross.Forms.Add(formheracross);
                    db.Pokemons.Add(heracross);


                    // #215 - Sneasel
                    var sneasel = new Pokemon
                    {
                        NationalDexNumber = 215,
                        Name = "Sneasel",
                        Generation = 2,
                        Category = "Sharp Claw",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formsneasel = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dark, PokemonType.Ice },
                        Stats = new BaseStats
                        {
                            HP = 55,
                            Attack = 95,
                            Defense = 55,
                            SpecialAttack = 35,
                            SpecialDefense = 75,
                            Speed = 115
                        }
                    };

                    sneasel.Forms.Add(formsneasel);
                    db.Pokemons.Add(sneasel);


                    // #216 - Teddiursa
                    var teddiursa = new Pokemon
                    {
                        NationalDexNumber = 216,
                        Name = "Teddiursa",
                        Generation = 2,
                        Category = "Little Bear",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formteddiursa = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 80,
                            Defense = 50,
                            SpecialAttack = 50,
                            SpecialDefense = 50,
                            Speed = 40
                        }
                    };

                    teddiursa.Forms.Add(formteddiursa);
                    db.Pokemons.Add(teddiursa);


                    // #217 - Ursaring
                    var ursaring = new Pokemon
                    {
                        NationalDexNumber = 217,
                        Name = "Ursaring",
                        Generation = 2,
                        Category = "Hibernator",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formursaring = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 130,
                            Defense = 75,
                            SpecialAttack = 75,
                            SpecialDefense = 75,
                            Speed = 55
                        }
                    };

                    ursaring.Forms.Add(formursaring);
                    db.Pokemons.Add(ursaring);


                    // #218 - Slugma
                    var slugma = new Pokemon
                    {
                        NationalDexNumber = 218,
                        Name = "Slugma",
                        Generation = 2,
                        Category = "Lava",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formslugma = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 40,
                            Defense = 40,
                            SpecialAttack = 70,
                            SpecialDefense = 40,
                            Speed = 20
                        }
                    };

                    slugma.Forms.Add(formslugma);
                    db.Pokemons.Add(slugma);


                    // #219 - Magcargo
                    var magcargo = new Pokemon
                    {
                        NationalDexNumber = 219,
                        Name = "Magcargo",
                        Generation = 2,
                        Category = "Lava",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formmagcargo = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire, PokemonType.Rock },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 50,
                            Defense = 120,
                            SpecialAttack = 90,
                            SpecialDefense = 80,
                            Speed = 30
                        }
                    };

                    magcargo.Forms.Add(formmagcargo);
                    db.Pokemons.Add(magcargo);


                    // #220 - Swinub
                    var swinub = new Pokemon
                    {
                        NationalDexNumber = 220,
                        Name = "Swinub",
                        Generation = 2,
                        Category = "Pig",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formswinub = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 50,
                            Defense = 40,
                            SpecialAttack = 30,
                            SpecialDefense = 30,
                            Speed = 50
                        }
                    };

                    swinub.Forms.Add(formswinub);
                    db.Pokemons.Add(swinub);


                    // #221 - Piloswine
                    var piloswine = new Pokemon
                    {
                        NationalDexNumber = 221,
                        Name = "Piloswine",
                        Generation = 2,
                        Category = "Swine",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formpiloswine = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 100,
                            Defense = 80,
                            SpecialAttack = 60,
                            SpecialDefense = 60,
                            Speed = 50
                        }
                    };

                    piloswine.Forms.Add(formpiloswine);
                    db.Pokemons.Add(piloswine);


                    // #222 - Corsola
                    var corsola = new Pokemon
                    {
                        NationalDexNumber = 222,
                        Name = "Corsola",
                        Generation = 2,
                        Category = "Coral",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water3 }
                    };

                    var formcorsola = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Rock },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 55,
                            Defense = 95,
                            SpecialAttack = 65,
                            SpecialDefense = 95,
                            Speed = 35
                        }
                    };

                    corsola.Forms.Add(formcorsola);
                    db.Pokemons.Add(corsola);


                    // #223 - Remoraid
                    var remoraid = new Pokemon
                    {
                        NationalDexNumber = 223,
                        Name = "Remoraid",
                        Generation = 2,
                        Category = "Jet",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water2 }
                    };

                    var formremoraid = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 35,
                            Attack = 65,
                            Defense = 35,
                            SpecialAttack = 65,
                            SpecialDefense = 35,
                            Speed = 65
                        }
                    };

                    remoraid.Forms.Add(formremoraid);
                    db.Pokemons.Add(remoraid);


                    // #224 - Octillery
                    var octillery = new Pokemon
                    {
                        NationalDexNumber = 224,
                        Name = "Octillery",
                        Generation = 2,
                        Category = "Jet",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water2 }
                    };

                    var formoctillery = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 105,
                            Defense = 75,
                            SpecialAttack = 105,
                            SpecialDefense = 75,
                            Speed = 45
                        }
                    };

                    octillery.Forms.Add(formoctillery);
                    db.Pokemons.Add(octillery);


                    // #225 - Delibird
                    var delibird = new Pokemon
                    {
                        NationalDexNumber = 225,
                        Name = "Delibird",
                        Generation = 2,
                        Category = "Delivery",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Field }
                    };

                    var formdelibird = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 45,
                            Attack = 55,
                            Defense = 45,
                            SpecialAttack = 65,
                            SpecialDefense = 45,
                            Speed = 75
                        }
                    };

                    delibird.Forms.Add(formdelibird);
                    db.Pokemons.Add(delibird);

                    // #226 - Mantine
                    var mantine = new Pokemon
                    {
                        NationalDexNumber = 226,
                        Name = "Mantine",
                        Generation = 2,
                        Category = "Kite",
                        EggGroups = new List<EggGroup> { EggGroup.Water1 }
                    };

                    var formmantine = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 85,
                            Attack = 40,
                            Defense = 70,
                            SpecialAttack = 80,
                            SpecialDefense = 140,
                            Speed = 70
                        }
                    };

                    mantine.Forms.Add(formmantine);
                    db.Pokemons.Add(mantine);


                    // #227 - Skarmory
                    var skarmory = new Pokemon
                    {
                        NationalDexNumber = 227,
                        Name = "Skarmory",
                        Generation = 2,
                        Category = "Armor Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formskarmory = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 80,
                            Defense = 140,
                            SpecialAttack = 40,
                            SpecialDefense = 70,
                            Speed = 70
                        }
                    };

                    skarmory.Forms.Add(formskarmory);
                    db.Pokemons.Add(skarmory);


                    // #228 - Houndour
                    var houndour = new Pokemon
                    {
                        NationalDexNumber = 228,
                        Name = "Houndour",
                        Generation = 2,
                        Category = "Dark",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formhoundour = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dark, PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 45,
                            Attack = 60,
                            Defense = 30,
                            SpecialAttack = 80,
                            SpecialDefense = 50,
                            Speed = 65
                        }
                    };

                    houndour.Forms.Add(formhoundour);
                    db.Pokemons.Add(houndour);


                    // #229 - Houndoom
                    var houndoom = new Pokemon
                    {
                        NationalDexNumber = 229,
                        Name = "Houndoom",
                        Generation = 2,
                        Category = "Dark",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formhoundoom = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dark, PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 90,
                            Defense = 50,
                            SpecialAttack = 110,
                            SpecialDefense = 80,
                            Speed = 95
                        }
                    };

                    houndoom.Forms.Add(formhoundoom);
                    db.Pokemons.Add(houndoom);


                    // #230 - Kingdra
                    var kingdra = new Pokemon
                    {
                        NationalDexNumber = 230,
                        Name = "Kingdra",
                        Generation = 2,
                        Category = "Dragon",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Dragon }
                    };

                    var formkingdra = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Dragon },
                        Stats = new BaseStats
                        {
                            HP = 75,
                            Attack = 95,
                            Defense = 95,
                            SpecialAttack = 95,
                            SpecialDefense = 95,
                            Speed = 85
                        }
                    };

                    kingdra.Forms.Add(formkingdra);
                    db.Pokemons.Add(kingdra);


                    // #231 - Phanpy
                    var phanpy = new Pokemon
                    {
                        NationalDexNumber = 231,
                        Name = "Phanpy",
                        Generation = 2,
                        Category = "Long Nose",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formphanpy = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 60,
                            Defense = 60,
                            SpecialAttack = 40,
                            SpecialDefense = 40,
                            Speed = 40
                        }
                    };

                    phanpy.Forms.Add(formphanpy);
                    db.Pokemons.Add(phanpy);


                    // #232 - Donphan
                    var donphan = new Pokemon
                    {
                        NationalDexNumber = 232,
                        Name = "Donphan",
                        Generation = 2,
                        Category = "Armor",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formdonphan = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 120,
                            Defense = 120,
                            SpecialAttack = 60,
                            SpecialDefense = 60,
                            Speed = 50
                        }
                    };

                    donphan.Forms.Add(formdonphan);
                    db.Pokemons.Add(donphan);


                    // #233 - Porygon2
                    var porygon2 = new Pokemon
                    {
                        NationalDexNumber = 233,
                        Name = "Porygon2",
                        Generation = 2,
                        Category = "Virtual",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formporygon2 = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 85,
                            Attack = 80,
                            Defense = 90,
                            SpecialAttack = 105,
                            SpecialDefense = 95,
                            Speed = 60
                        }
                    };

                    porygon2.Forms.Add(formporygon2);
                    db.Pokemons.Add(porygon2);


                    // #234 - Stantler
                    var stantler = new Pokemon
                    {
                        NationalDexNumber = 234,
                        Name = "Stantler",
                        Generation = 2,
                        Category = "Big Horn",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formstantler = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 73,
                            Attack = 95,
                            Defense = 62,
                            SpecialAttack = 85,
                            SpecialDefense = 65,
                            Speed = 85
                        }
                    };

                    stantler.Forms.Add(formstantler);
                    db.Pokemons.Add(stantler);


                    // #235 - Smeargle
                    var smeargle = new Pokemon
                    {
                        NationalDexNumber = 235,
                        Name = "Smeargle",
                        Generation = 2,
                        Category = "Painter",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formsmeargle = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 55,
                            Attack = 20,
                            Defense = 35,
                            SpecialAttack = 20,
                            SpecialDefense = 45,
                            Speed = 75
                        }
                    };

                    smeargle.Forms.Add(formsmeargle);
                    db.Pokemons.Add(smeargle);


                    // #236 - Tyrogue
                    var tyrogue = new Pokemon
                    {
                        NationalDexNumber = 236,
                        Name = "Tyrogue",
                        Generation = 2,
                        Category = "Scuffle",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formtyrogue = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats
                        {
                            HP = 35,
                            Attack = 35,
                            Defense = 35,
                            SpecialAttack = 35,
                            SpecialDefense = 35,
                            Speed = 35
                        }
                    };

                    tyrogue.Forms.Add(formtyrogue);
                    db.Pokemons.Add(tyrogue);


                    // #237 - Hitmontop
                    var hitmontop = new Pokemon
                    {
                        NationalDexNumber = 237,
                        Name = "Hitmontop",
                        Generation = 2,
                        Category = "Handstand",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formhitmontop = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 95,
                            Defense = 95,
                            SpecialAttack = 35,
                            SpecialDefense = 110,
                            Speed = 70
                        }
                    };

                    hitmontop.Forms.Add(formhitmontop);
                    db.Pokemons.Add(hitmontop);


                    // #238 - Smoochum
                    var smoochum = new Pokemon
                    {
                        NationalDexNumber = 238,
                        Name = "Smoochum",
                        Generation = 2,
                        Category = "Kiss",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formsmoochum = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice, PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 45,
                            Attack = 30,
                            Defense = 15,
                            SpecialAttack = 85,
                            SpecialDefense = 65,
                            Speed = 65
                        }
                    };

                    smoochum.Forms.Add(formsmoochum);
                    db.Pokemons.Add(smoochum);


                    // #239 - Elekid
                    var elekid = new Pokemon
                    {
                        NationalDexNumber = 239,
                        Name = "Elekid",
                        Generation = 2,
                        Category = "Electric",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formelekid = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 45,
                            Attack = 63,
                            Defense = 37,
                            SpecialAttack = 65,
                            SpecialDefense = 55,
                            Speed = 95
                        }
                    };

                    elekid.Forms.Add(formelekid);
                    db.Pokemons.Add(elekid);


                    // #240 - Magby
                    var magby = new Pokemon
                    {
                        NationalDexNumber = 240,
                        Name = "Magby",
                        Generation = 2,
                        Category = "Live Coal",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formmagby = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 45,
                            Attack = 75,
                            Defense = 37,
                            SpecialAttack = 70,
                            SpecialDefense = 55,
                            Speed = 83
                        }
                    };

                    magby.Forms.Add(formmagby);
                    db.Pokemons.Add(magby);


                    // #241 - Miltank
                    var miltank = new Pokemon
                    {
                        NationalDexNumber = 241,
                        Name = "Miltank",
                        Generation = 2,
                        Category = "Milk Cow",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formmiltank = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 95,
                            Attack = 80,
                            Defense = 105,
                            SpecialAttack = 40,
                            SpecialDefense = 70,
                            Speed = 100
                        }
                    };

                    miltank.Forms.Add(formmiltank);
                    db.Pokemons.Add(miltank);


                    // #242 - Blissey
                    var blissey = new Pokemon
                    {
                        NationalDexNumber = 242,
                        Name = "Blissey",
                        Generation = 2,
                        Category = "Happiness",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy }
                    };

                    var formblissey = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 255,
                            Attack = 10,
                            Defense = 10,
                            SpecialAttack = 75,
                            SpecialDefense = 135,
                            Speed = 55
                        }
                    };

                    blissey.Forms.Add(formblissey);
                    db.Pokemons.Add(blissey);


                    // #243 - Raikou
                    var raikou = new Pokemon
                    {
                        NationalDexNumber = 243,
                        Name = "Raikou",
                        Generation = 2,
                        Category = "Thunder",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formraikou = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 85,
                            Defense = 75,
                            SpecialAttack = 115,
                            SpecialDefense = 100,
                            Speed = 115
                        }
                    };

                    raikou.Forms.Add(formraikou);
                    db.Pokemons.Add(raikou);


                    // #244 - Entei
                    var entei = new Pokemon
                    {
                        NationalDexNumber = 244,
                        Name = "Entei",
                        Generation = 2,
                        Category = "Volcano",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formentei = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 115,
                            Attack = 115,
                            Defense = 85,
                            SpecialAttack = 90,
                            SpecialDefense = 75,
                            Speed = 100
                        }
                    };

                    entei.Forms.Add(formentei);
                    db.Pokemons.Add(entei);


                    // #245 - Suicune
                    var suicune = new Pokemon
                    {
                        NationalDexNumber = 245,
                        Name = "Suicune",
                        Generation = 2,
                        Category = "Aurora",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formsuicune = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 75,
                            Defense = 115,
                            SpecialAttack = 90,
                            SpecialDefense = 115,
                            Speed = 85
                        }
                    };

                    suicune.Forms.Add(formsuicune);
                    db.Pokemons.Add(suicune);


                    // #246 - Larvitar
                    var larvitar = new Pokemon
                    {
                        NationalDexNumber = 246,
                        Name = "Larvitar",
                        Generation = 2,
                        Category = "Rock Skin",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formlarvitar = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 64,
                            Defense = 50,
                            SpecialAttack = 45,
                            SpecialDefense = 50,
                            Speed = 41
                        }
                    };

                    larvitar.Forms.Add(formlarvitar);
                    db.Pokemons.Add(larvitar);


                    // #247 - Pupitar
                    var pupitar = new Pokemon
                    {
                        NationalDexNumber = 247,
                        Name = "Pupitar",
                        Generation = 2,
                        Category = "Hard Shell",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formpupitar = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 84,
                            Defense = 70,
                            SpecialAttack = 65,
                            SpecialDefense = 70,
                            Speed = 51
                        }
                    };

                    pupitar.Forms.Add(formpupitar);
                    db.Pokemons.Add(pupitar);


                    // #248 - Tyranitar
                    var tyranitar = new Pokemon
                    {
                        NationalDexNumber = 248,
                        Name = "Tyranitar",
                        Generation = 2,
                        Category = "Armor",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formtyranitar = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Dark },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 134,
                            Defense = 110,
                            SpecialAttack = 95,
                            SpecialDefense = 100,
                            Speed = 61
                        }
                    };

                    tyranitar.Forms.Add(formtyranitar);
                    db.Pokemons.Add(tyranitar);


                    // #249 - Lugia
                    var lugia = new Pokemon
                    {
                        NationalDexNumber = 249,
                        Name = "Lugia",
                        Generation = 2,
                        Category = "Diving",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formlugia = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 106,
                            Attack = 90,
                            Defense = 130,
                            SpecialAttack = 90,
                            SpecialDefense = 154,
                            Speed = 110
                        }
                    };

                    lugia.Forms.Add(formlugia);
                    db.Pokemons.Add(lugia);


                    // #250 - Ho-Oh
                    var hoOh = new Pokemon
                    {
                        NationalDexNumber = 250,
                        Name = "Ho-Oh",
                        Generation = 2,
                        Category = "Rainbow",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formhoOh = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 106,
                            Attack = 130,
                            Defense = 90,
                            SpecialAttack = 110,
                            SpecialDefense = 154,
                            Speed = 90
                        }
                    };

                    hoOh.Forms.Add(formhoOh);
                    db.Pokemons.Add(hoOh);

                    var celebi = new Pokemon
                    {
                        NationalDexNumber = 251,
                        Name = "Celebi",
                        Generation = 2,
                        Category = "Time Travel",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formcelebi = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic, PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 100,
                            Defense = 100,
                            SpecialAttack = 100,
                            SpecialDefense = 100,
                            Speed = 100
                        }
                    };

                    celebi.Forms.Add(formcelebi);
                    db.Pokemons.Add(celebi);


                    var treecko = new Pokemon
                    {
                        NationalDexNumber = 252,
                        Name = "Treecko",
                        Generation = 3,
                        Category = "Wood Gecko",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Dragon }
                    };

                    var formtreecko = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 45,
                            Defense = 35,
                            SpecialAttack = 65,
                            SpecialDefense = 55,
                            Speed = 70
                        }
                    };

                    treecko.Forms.Add(formtreecko);
                    db.Pokemons.Add(treecko);


                    var grovyle = new Pokemon
                    {
                        NationalDexNumber = 253,
                        Name = "Grovyle",
                        Generation = 3,
                        Category = "Wood Gecko",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Dragon }
                    };

                    var formgrovyle = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 65,
                            Defense = 45,
                            SpecialAttack = 85,
                            SpecialDefense = 65,
                            Speed = 95
                        }
                    };

                    grovyle.Forms.Add(formgrovyle);
                    db.Pokemons.Add(grovyle);


                    var sceptile = new Pokemon
                    {
                        NationalDexNumber = 254,
                        Name = "Sceptile",
                        Generation = 3,
                        Category = "Forest",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Dragon }
                    };

                    var formsceptile = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 85,
                            Defense = 65,
                            SpecialAttack = 105,
                            SpecialDefense = 85,
                            Speed = 120
                        }
                    };

                    sceptile.Forms.Add(formsceptile);
                    db.Pokemons.Add(sceptile);


                    var torchic = new Pokemon
                    {
                        NationalDexNumber = 255,
                        Name = "Torchic",
                        Generation = 3,
                        Category = "Chick",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formtorchic = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 45,
                            Attack = 60,
                            Defense = 40,
                            SpecialAttack = 70,
                            SpecialDefense = 50,
                            Speed = 45
                        }
                    };

                    torchic.Forms.Add(formtorchic);
                    db.Pokemons.Add(torchic);


                    var combusken = new Pokemon
                    {
                        NationalDexNumber = 256,
                        Name = "Combusken",
                        Generation = 3,
                        Category = "Young Fowl",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formcombusken = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire, PokemonType.Fighting },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 85,
                            Defense = 60,
                            SpecialAttack = 85,
                            SpecialDefense = 60,
                            Speed = 55
                        }
                    };

                    combusken.Forms.Add(formcombusken);
                    db.Pokemons.Add(combusken);


                    var blaziken = new Pokemon
                    {
                        NationalDexNumber = 257,
                        Name = "Blaziken",
                        Generation = 3,
                        Category = "Blaze",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formblaziken = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire, PokemonType.Fighting },
                        Stats = new BaseStats
                        {
                            HP = 80,
                            Attack = 120,
                            Defense = 70,
                            SpecialAttack = 110,
                            SpecialDefense = 70,
                            Speed = 80
                        }
                    };

                    blaziken.Forms.Add(formblaziken);
                    db.Pokemons.Add(blaziken);


                    var mudkip = new Pokemon
                    {
                        NationalDexNumber = 258,
                        Name = "Mudkip",
                        Generation = 3,
                        Category = "Mud Fish",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formmudkip = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 70,
                            Defense = 50,
                            SpecialAttack = 50,
                            SpecialDefense = 50,
                            Speed = 40
                        }
                    };

                    mudkip.Forms.Add(formmudkip);
                    db.Pokemons.Add(mudkip);


                    var marshtomp = new Pokemon
                    {
                        NationalDexNumber = 259,
                        Name = "Marshtomp",
                        Generation = 3,
                        Category = "Mud Fish",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formmarshtomp = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 85,
                            Defense = 70,
                            SpecialAttack = 60,
                            SpecialDefense = 70,
                            Speed = 50
                        }
                    };

                    marshtomp.Forms.Add(formmarshtomp);
                    db.Pokemons.Add(marshtomp);


                    var swampert = new Pokemon
                    {
                        NationalDexNumber = 260,
                        Name = "Swampert",
                        Generation = 3,
                        Category = "Mud Fish",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Water1 }
                    };

                    var formswampert = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 110,
                            Defense = 90,
                            SpecialAttack = 85,
                            SpecialDefense = 90,
                            Speed = 60
                        }
                    };

                    swampert.Forms.Add(formswampert);
                    db.Pokemons.Add(swampert);


                    var poochyena = new Pokemon
                    {
                        NationalDexNumber = 261,
                        Name = "Poochyena",
                        Generation = 3,
                        Category = "Bite",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formpoochyena = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dark },
                        Stats = new BaseStats
                        {
                            HP = 35,
                            Attack = 55,
                            Defense = 35,
                            SpecialAttack = 30,
                            SpecialDefense = 30,
                            Speed = 35
                        }
                    };

                    poochyena.Forms.Add(formpoochyena);
                    db.Pokemons.Add(poochyena);


                    var mightyena = new Pokemon
                    {
                        NationalDexNumber = 262,
                        Name = "Mightyena",
                        Generation = 3,
                        Category = "Bite",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formmightyena = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dark },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 90,
                            Defense = 70,
                            SpecialAttack = 60,
                            SpecialDefense = 60,
                            Speed = 70
                        }
                    };

                    mightyena.Forms.Add(formmightyena);
                    db.Pokemons.Add(mightyena);


                    var zigzagoon = new Pokemon
                    {
                        NationalDexNumber = 263,
                        Name = "Zigzagoon",
                        Generation = 3,
                        Category = "Tiny Raccoon",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formzigzagoon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 38,
                            Attack = 30,
                            Defense = 41,
                            SpecialAttack = 30,
                            SpecialDefense = 41,
                            Speed = 60
                        }
                    };

                    zigzagoon.Forms.Add(formzigzagoon);
                    db.Pokemons.Add(zigzagoon);


                    var linoone = new Pokemon
                    {
                        NationalDexNumber = 264,
                        Name = "Linoone",
                        Generation = 3,
                        Category = "Rushing",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formlinoone = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 78,
                            Attack = 70,
                            Defense = 61,
                            SpecialAttack = 50,
                            SpecialDefense = 61,
                            Speed = 100
                        }
                    };

                    linoone.Forms.Add(formlinoone);
                    db.Pokemons.Add(linoone);


                    var wurmple = new Pokemon
                    {
                        NationalDexNumber = 265,
                        Name = "Wurmple",
                        Generation = 3,
                        Category = "Worm",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formwurmple = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug },
                        Stats = new BaseStats
                        {
                            HP = 45,
                            Attack = 45,
                            Defense = 35,
                            SpecialAttack = 20,
                            SpecialDefense = 30,
                            Speed = 20
                        }
                    };

                    wurmple.Forms.Add(formwurmple);
                    db.Pokemons.Add(wurmple);


                    var silcoon = new Pokemon
                    {
                        NationalDexNumber = 266,
                        Name = "Silcoon",
                        Generation = 3,
                        Category = "Cocoon",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formsilcoon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 35,
                            Defense = 55,
                            SpecialAttack = 25,
                            SpecialDefense = 25,
                            Speed = 15
                        }
                    };

                    silcoon.Forms.Add(formsilcoon);
                    db.Pokemons.Add(silcoon);


                    var beautifly = new Pokemon
                    {
                        NationalDexNumber = 267,
                        Name = "Beautifly",
                        Generation = 3,
                        Category = "Butterfly",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formbeautifly = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 70,
                            Defense = 50,
                            SpecialAttack = 100,
                            SpecialDefense = 50,
                            Speed = 65
                        }
                    };

                    beautifly.Forms.Add(formbeautifly);
                    db.Pokemons.Add(beautifly);


                    var cascoon = new Pokemon
                    {
                        NationalDexNumber = 268,
                        Name = "Cascoon",
                        Generation = 3,
                        Category = "Cocoon",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formcascoon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 35,
                            Defense = 55,
                            SpecialAttack = 25,
                            SpecialDefense = 25,
                            Speed = 15
                        }
                    };

                    cascoon.Forms.Add(formcascoon);
                    db.Pokemons.Add(cascoon);


                    var dustox = new Pokemon
                    {
                        NationalDexNumber = 269,
                        Name = "Dustox",
                        Generation = 3,
                        Category = "Poison Moth",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formdustox = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Poison },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 50,
                            Defense = 70,
                            SpecialAttack = 50,
                            SpecialDefense = 90,
                            Speed = 65
                        }
                    };

                    dustox.Forms.Add(formdustox);
                    db.Pokemons.Add(dustox);


                    var lotad = new Pokemon
                    {
                        NationalDexNumber = 270,
                        Name = "Lotad",
                        Generation = 3,
                        Category = "Water Weed",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Grass }
                    };

                    var formlotad = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 30,
                            Defense = 30,
                            SpecialAttack = 40,
                            SpecialDefense = 50,
                            Speed = 30
                        }
                    };

                    lotad.Forms.Add(formlotad);
                    db.Pokemons.Add(lotad);


                    var lombre = new Pokemon
                    {
                        NationalDexNumber = 271,
                        Name = "Lombre",
                        Generation = 3,
                        Category = "Jolly",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Grass }
                    };

                    var formlombre = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 50,
                            Defense = 50,
                            SpecialAttack = 60,
                            SpecialDefense = 70,
                            Speed = 50
                        }
                    };

                    lombre.Forms.Add(formlombre);
                    db.Pokemons.Add(lombre);


                    var ludicolo = new Pokemon
                    {
                        NationalDexNumber = 272,
                        Name = "Ludicolo",
                        Generation = 3,
                        Category = "Carefree",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Grass }
                    };

                    var formludicolo = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 80,
                            Attack = 70,
                            Defense = 70,
                            SpecialAttack = 90,
                            SpecialDefense = 100,
                            Speed = 70
                        }
                    };

                    ludicolo.Forms.Add(formludicolo);
                    db.Pokemons.Add(ludicolo);


                    var seedot = new Pokemon
                    {
                        NationalDexNumber = 273,
                        Name = "Seedot",
                        Generation = 3,
                        Category = "Acorn",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Grass }
                    };

                    var formseedot = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 40,
                            Defense = 50,
                            SpecialAttack = 30,
                            SpecialDefense = 30,
                            Speed = 30
                        }
                    };

                    seedot.Forms.Add(formseedot);
                    db.Pokemons.Add(seedot);


                    var nuzleaf = new Pokemon
                    {
                        NationalDexNumber = 274,
                        Name = "Nuzleaf",
                        Generation = 3,
                        Category = "Wily",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Grass }
                    };

                    var formnuzleaf = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Dark },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 70,
                            Defense = 40,
                            SpecialAttack = 60,
                            SpecialDefense = 40,
                            Speed = 60
                        }
                    };

                    nuzleaf.Forms.Add(formnuzleaf);
                    db.Pokemons.Add(nuzleaf);


                    var shiftry = new Pokemon
                    {
                        NationalDexNumber = 275,
                        Name = "Shiftry",
                        Generation = 3,
                        Category = "Wicked",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Grass }
                    };

                    var formshiftry = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Dark },
                        Stats = new BaseStats
                        {
                            HP = 90,
                            Attack = 100,
                            Defense = 60,
                            SpecialAttack = 90,
                            SpecialDefense = 60,
                            Speed = 80
                        }
                    };

                    shiftry.Forms.Add(formshiftry);
                    db.Pokemons.Add(shiftry);

                    var taillow = new Pokemon
                    {
                        NationalDexNumber = 276,
                        Name = "Taillow",
                        Generation = 3,
                        Category = "Tiny Swallow",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formtaillow = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 55,
                            Defense = 30,
                            SpecialAttack = 30,
                            SpecialDefense = 30,
                            Speed = 85
                        }
                    };

                    taillow.Forms.Add(formtaillow);
                    db.Pokemons.Add(taillow);


                    var swellow = new Pokemon
                    {
                        NationalDexNumber = 277,
                        Name = "Swellow",
                        Generation = 3,
                        Category = "Swallow",
                        EggGroups = new List<EggGroup> { EggGroup.Flying }
                    };

                    var formswellow = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 85,
                            Defense = 60,
                            SpecialAttack = 75,
                            SpecialDefense = 50,
                            Speed = 125
                        }
                    };

                    swellow.Forms.Add(formswellow);
                    db.Pokemons.Add(swellow);


                    var wingull = new Pokemon
                    {
                        NationalDexNumber = 278,
                        Name = "Wingull",
                        Generation = 3,
                        Category = "Seagull",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Flying }
                    };

                    var formwingull = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 30,
                            Defense = 30,
                            SpecialAttack = 55,
                            SpecialDefense = 30,
                            Speed = 85
                        }
                    };

                    wingull.Forms.Add(formwingull);
                    db.Pokemons.Add(wingull);


                    var pelipper = new Pokemon
                    {
                        NationalDexNumber = 279,
                        Name = "Pelipper",
                        Generation = 3,
                        Category = "Water Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Flying }
                    };

                    var formpelipper = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 50,
                            Defense = 100,
                            SpecialAttack = 95,
                            SpecialDefense = 70,
                            Speed = 65
                        }
                    };

                    pelipper.Forms.Add(formpelipper);
                    db.Pokemons.Add(pelipper);


                    var ralts = new Pokemon
                    {
                        NationalDexNumber = 280,
                        Name = "Ralts",
                        Generation = 3,
                        Category = "Feeling",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formralts = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic, PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 28,
                            Attack = 25,
                            Defense = 25,
                            SpecialAttack = 45,
                            SpecialDefense = 35,
                            Speed = 40
                        }
                    };

                    ralts.Forms.Add(formralts);
                    db.Pokemons.Add(ralts);


                    var kirlia = new Pokemon
                    {
                        NationalDexNumber = 281,
                        Name = "Kirlia",
                        Generation = 3,
                        Category = "Emotion",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formkirlia = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic, PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 38,
                            Attack = 35,
                            Defense = 35,
                            SpecialAttack = 65,
                            SpecialDefense = 55,
                            Speed = 50
                        }
                    };

                    kirlia.Forms.Add(formkirlia);
                    db.Pokemons.Add(kirlia);


                    var gardevoir = new Pokemon
                    {
                        NationalDexNumber = 282,
                        Name = "Gardevoir",
                        Generation = 3,
                        Category = "Embrace",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formgardevoir = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic, PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 68,
                            Attack = 65,
                            Defense = 65,
                            SpecialAttack = 125,
                            SpecialDefense = 115,
                            Speed = 80
                        }
                    };

                    gardevoir.Forms.Add(formgardevoir);
                    db.Pokemons.Add(gardevoir);


                    var surskit = new Pokemon
                    {
                        NationalDexNumber = 283,
                        Name = "Surskit",
                        Generation = 3,
                        Category = "Pond Skater",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Bug }
                    };

                    var formsurskit = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 30,
                            Defense = 32,
                            SpecialAttack = 50,
                            SpecialDefense = 52,
                            Speed = 65
                        }
                    };

                    surskit.Forms.Add(formsurskit);
                    db.Pokemons.Add(surskit);


                    var masquerain = new Pokemon
                    {
                        NationalDexNumber = 284,
                        Name = "Masquerain",
                        Generation = 3,
                        Category = "Eyeball",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Bug }
                    };

                    var formmasquerain = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 60,
                            Defense = 62,
                            SpecialAttack = 100,
                            SpecialDefense = 82,
                            Speed = 80
                        }
                    };

                    masquerain.Forms.Add(formmasquerain);
                    db.Pokemons.Add(masquerain);


                    var shroomish = new Pokemon
                    {
                        NationalDexNumber = 285,
                        Name = "Shroomish",
                        Generation = 3,
                        Category = "Mushroom",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy, EggGroup.Grass }
                    };

                    var formshroomish = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 40,
                            Defense = 60,
                            SpecialAttack = 40,
                            SpecialDefense = 60,
                            Speed = 35
                        }
                    };

                    shroomish.Forms.Add(formshroomish);
                    db.Pokemons.Add(shroomish);


                    var breloom = new Pokemon
                    {
                        NationalDexNumber = 286,
                        Name = "Breloom",
                        Generation = 3,
                        Category = "Mushroom",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy, EggGroup.Grass }
                    };

                    var formbreloom = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Fighting },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 130,
                            Defense = 80,
                            SpecialAttack = 60,
                            SpecialDefense = 60,
                            Speed = 70
                        }
                    };

                    breloom.Forms.Add(formbreloom);
                    db.Pokemons.Add(breloom);


                    var slakoth = new Pokemon
                    {
                        NationalDexNumber = 287,
                        Name = "Slakoth",
                        Generation = 3,
                        Category = "Slacker",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formslakoth = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 60,
                            Defense = 60,
                            SpecialAttack = 35,
                            SpecialDefense = 35,
                            Speed = 30
                        }
                    };

                    slakoth.Forms.Add(formslakoth);
                    db.Pokemons.Add(slakoth);


                    var vigoroth = new Pokemon
                    {
                        NationalDexNumber = 288,
                        Name = "Vigoroth",
                        Generation = 3,
                        Category = "Wild Monkey",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formvigoroth = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 80,
                            Attack = 80,
                            Defense = 80,
                            SpecialAttack = 55,
                            SpecialDefense = 55,
                            Speed = 90
                        }
                    };

                    vigoroth.Forms.Add(formvigoroth);
                    db.Pokemons.Add(vigoroth);


                    var slaking = new Pokemon
                    {
                        NationalDexNumber = 289,
                        Name = "Slaking",
                        Generation = 3,
                        Category = "Lazy",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formslaking = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 150,
                            Attack = 160,
                            Defense = 100,
                            SpecialAttack = 95,
                            SpecialDefense = 65,
                            Speed = 100
                        }
                    };

                    slaking.Forms.Add(formslaking);
                    db.Pokemons.Add(slaking);


                    var nincada = new Pokemon
                    {
                        NationalDexNumber = 290,
                        Name = "Nincada",
                        Generation = 3,
                        Category = "Trainee",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formnincada = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 31,
                            Attack = 45,
                            Defense = 90,
                            SpecialAttack = 30,
                            SpecialDefense = 30,
                            Speed = 40
                        }
                    };

                    nincada.Forms.Add(formnincada);
                    db.Pokemons.Add(nincada);


                    var ninjask = new Pokemon
                    {
                        NationalDexNumber = 291,
                        Name = "Ninjask",
                        Generation = 3,
                        Category = "Ninja",
                        EggGroups = new List<EggGroup> { EggGroup.Bug }
                    };

                    var formninjask = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Flying },
                        Stats = new BaseStats
                        {
                            HP = 61,
                            Attack = 90,
                            Defense = 45,
                            SpecialAttack = 50,
                            SpecialDefense = 50,
                            Speed = 160
                        }
                    };

                    ninjask.Forms.Add(formninjask);
                    db.Pokemons.Add(ninjask);


                    var shedinja = new Pokemon
                    {
                        NationalDexNumber = 292,
                        Name = "Shedinja",
                        Generation = 3,
                        Category = "Shed",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formshedinja = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug, PokemonType.Ghost },
                        Stats = new BaseStats
                        {
                            HP = 1,
                            Attack = 90,
                            Defense = 45,
                            SpecialAttack = 30,
                            SpecialDefense = 30,
                            Speed = 40
                        }
                    };

                    shedinja.Forms.Add(formshedinja);
                    db.Pokemons.Add(shedinja);


                    var whismur = new Pokemon
                    {
                        NationalDexNumber = 293,
                        Name = "Whismur",
                        Generation = 3,
                        Category = "Whisper",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Monster }
                    };

                    var formwhismur = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 64,
                            Attack = 51,
                            Defense = 23,
                            SpecialAttack = 51,
                            SpecialDefense = 23,
                            Speed = 28
                        }
                    };

                    whismur.Forms.Add(formwhismur);
                    db.Pokemons.Add(whismur);


                    var loudred = new Pokemon
                    {
                        NationalDexNumber = 294,
                        Name = "Loudred",
                        Generation = 3,
                        Category = "Big Voice",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Monster }
                    };

                    var formloudred = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 84,
                            Attack = 71,
                            Defense = 43,
                            SpecialAttack = 71,
                            SpecialDefense = 43,
                            Speed = 48
                        }
                    };

                    loudred.Forms.Add(formloudred);
                    db.Pokemons.Add(loudred);


                    var exploud = new Pokemon
                    {
                        NationalDexNumber = 295,
                        Name = "Exploud",
                        Generation = 3,
                        Category = "Loud Noise",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Monster }
                    };

                    var formexploud = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 104,
                            Attack = 91,
                            Defense = 63,
                            SpecialAttack = 91,
                            SpecialDefense = 73,
                            Speed = 68
                        }
                    };

                    exploud.Forms.Add(formexploud);
                    db.Pokemons.Add(exploud);


                    var makuhita = new Pokemon
                    {
                        NationalDexNumber = 296,
                        Name = "Makuhita",
                        Generation = 3,
                        Category = "Guts",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formmakuhita = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats
                        {
                            HP = 72,
                            Attack = 60,
                            Defense = 30,
                            SpecialAttack = 20,
                            SpecialDefense = 30,
                            Speed = 25
                        }
                    };

                    makuhita.Forms.Add(formmakuhita);
                    db.Pokemons.Add(makuhita);


                    var hariyama = new Pokemon
                    {
                        NationalDexNumber = 297,
                        Name = "Hariyama",
                        Generation = 3,
                        Category = "Arm Thrust",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formhariyama = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting },
                        Stats = new BaseStats
                        {
                            HP = 144,
                            Attack = 120,
                            Defense = 60,
                            SpecialAttack = 40,
                            SpecialDefense = 60,
                            Speed = 50
                        }
                    };

                    hariyama.Forms.Add(formhariyama);
                    db.Pokemons.Add(hariyama);


                    var azurill = new Pokemon
                    {
                        NationalDexNumber = 298,
                        Name = "Azurill",
                        Generation = 3,
                        Category = "Polka Dot",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };

                    var formazurill = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 20,
                            Defense = 40,
                            SpecialAttack = 20,
                            SpecialDefense = 40,
                            Speed = 20
                        }
                    };

                    azurill.Forms.Add(formazurill);
                    db.Pokemons.Add(azurill);


                    var nosepass = new Pokemon
                    {
                        NationalDexNumber = 299,
                        Name = "Nosepass",
                        Generation = 3,
                        Category = "Compass",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };

                    var formnosepass = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock },
                        Stats = new BaseStats
                        {
                            HP = 30,
                            Attack = 45,
                            Defense = 135,
                            SpecialAttack = 45,
                            SpecialDefense = 90,
                            Speed = 30
                        }
                    };

                    nosepass.Forms.Add(formnosepass);
                    db.Pokemons.Add(nosepass);


                    var skitty = new Pokemon
                    {
                        NationalDexNumber = 300,
                        Name = "Skitty",
                        Generation = 3,
                        Category = "Kitten",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
                    };

                    var formskitty = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 45,
                            Defense = 45,
                            SpecialAttack = 35,
                            SpecialDefense = 35,
                            Speed = 50
                        }
                    };

                    skitty.Forms.Add(formskitty);
                    db.Pokemons.Add(skitty);

                    var delcatty = new Pokemon
                    {
                        NationalDexNumber = 301,
                        Name = "Delcatty",
                        Generation = 3,
                        Category = "Prim",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
                    };

                    var formdelcatty = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 65,
                            Defense = 65,
                            SpecialAttack = 55,
                            SpecialDefense = 55,
                            Speed = 70
                        }
                    };

                    delcatty.Forms.Add(formdelcatty);
                    db.Pokemons.Add(delcatty);


                    var sableye = new Pokemon
                    {
                        NationalDexNumber = 302,
                        Name = "Sableye",
                        Generation = 3,
                        Category = "Darkness",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formsableye = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dark, PokemonType.Ghost },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 75,
                            Defense = 75,
                            SpecialAttack = 65,
                            SpecialDefense = 65,
                            Speed = 50
                        }
                    };

                    sableye.Forms.Add(formsableye);
                    db.Pokemons.Add(sableye);


                    var mawile = new Pokemon
                    {
                        NationalDexNumber = 303,
                        Name = "Mawile",
                        Generation = 3,
                        Category = "Deceiver",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Fairy }
                    };

                    var formawile = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Fairy },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 85,
                            Defense = 85,
                            SpecialAttack = 55,
                            SpecialDefense = 55,
                            Speed = 50
                        }
                    };

                    mawile.Forms.Add(formawile);
                    db.Pokemons.Add(mawile);


                    var aron = new Pokemon
                    {
                        NationalDexNumber = 304,
                        Name = "Aron",
                        Generation = 3,
                        Category = "Iron Armor",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formaron = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Rock },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 70,
                            Defense = 100,
                            SpecialAttack = 40,
                            SpecialDefense = 40,
                            Speed = 30
                        }
                    };

                    aron.Forms.Add(formaron);
                    db.Pokemons.Add(aron);


                    var lairon = new Pokemon
                    {
                        NationalDexNumber = 305,
                        Name = "Lairon",
                        Generation = 3,
                        Category = "Iron Armor",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formlairon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Rock },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 90,
                            Defense = 140,
                            SpecialAttack = 50,
                            SpecialDefense = 50,
                            Speed = 40
                        }
                    };

                    lairon.Forms.Add(formlairon);
                    db.Pokemons.Add(lairon);


                    var aggron = new Pokemon
                    {
                        NationalDexNumber = 306,
                        Name = "Aggron",
                        Generation = 3,
                        Category = "Iron Armor",
                        EggGroups = new List<EggGroup> { EggGroup.Monster }
                    };

                    var formaggron = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Rock },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 110,
                            Defense = 180,
                            SpecialAttack = 60,
                            SpecialDefense = 60,
                            Speed = 50
                        }
                    };

                    aggron.Forms.Add(formaggron);
                    db.Pokemons.Add(aggron);


                    var meditite = new Pokemon
                    {
                        NationalDexNumber = 307,
                        Name = "Meditite",
                        Generation = 3,
                        Category = "Meditate",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formmeditite = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting, PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 30,
                            Attack = 40,
                            Defense = 55,
                            SpecialAttack = 40,
                            SpecialDefense = 55,
                            Speed = 60
                        }
                    };

                    meditite.Forms.Add(formmeditite);
                    db.Pokemons.Add(meditite);


                    var medicham = new Pokemon
                    {
                        NationalDexNumber = 308,
                        Name = "Medicham",
                        Generation = 3,
                        Category = "Meditate",
                        EggGroups = new List<EggGroup> { EggGroup.HumanLike }
                    };

                    var formedicham = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fighting, PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 60,
                            Defense = 75,
                            SpecialAttack = 60,
                            SpecialDefense = 75,
                            Speed = 80
                        }
                    };

                    medicham.Forms.Add(formedicham);
                    db.Pokemons.Add(medicham);


                    var electrike = new Pokemon
                    {
                        NationalDexNumber = 309,
                        Name = "Electrike",
                        Generation = 3,
                        Category = "Lightning",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formelectrike = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 40,
                            Attack = 45,
                            Defense = 40,
                            SpecialAttack = 65,
                            SpecialDefense = 40,
                            Speed = 65
                        }
                    };

                    electrike.Forms.Add(formelectrike);
                    db.Pokemons.Add(electrike);


                    var manectric = new Pokemon
                    {
                        NationalDexNumber = 310,
                        Name = "Manectric",
                        Generation = 3,
                        Category = "Discharge",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formmanectric = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 75,
                            Defense = 60,
                            SpecialAttack = 105,
                            SpecialDefense = 60,
                            Speed = 105
                        }
                    };

                    manectric.Forms.Add(formmanectric);
                    db.Pokemons.Add(manectric);


                    var plusle = new Pokemon
                    {
                        NationalDexNumber = 311,
                        Name = "Plusle",
                        Generation = 3,
                        Category = "Cheering",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy }
                    };

                    var formplusle = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 50,
                            Defense = 40,
                            SpecialAttack = 85,
                            SpecialDefense = 75,
                            Speed = 95
                        }
                    };

                    plusle.Forms.Add(formplusle);
                    db.Pokemons.Add(plusle);


                    var minun = new Pokemon
                    {
                        NationalDexNumber = 312,
                        Name = "Minun",
                        Generation = 3,
                        Category = "Cheering",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy }
                    };

                    var formminun = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Electric },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 40,
                            Defense = 50,
                            SpecialAttack = 75,
                            SpecialDefense = 85,
                            Speed = 95
                        }
                    };

                    minun.Forms.Add(formminun);
                    db.Pokemons.Add(minun);


                    var volbeat = new Pokemon
                    {
                        NationalDexNumber = 313,
                        Name = "Volbeat",
                        Generation = 3,
                        Category = "Firefly",
                        EggGroups = new List<EggGroup> { EggGroup.Bug, EggGroup.HumanLike }
                    };

                    var formvolbeat = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 73,
                            Defense = 75,
                            SpecialAttack = 47,
                            SpecialDefense = 85,
                            Speed = 85
                        }
                    };

                    volbeat.Forms.Add(formvolbeat);
                    db.Pokemons.Add(volbeat);


                    var illumise = new Pokemon
                    {
                        NationalDexNumber = 314,
                        Name = "Illumise",
                        Generation = 3,
                        Category = "Firefly",
                        EggGroups = new List<EggGroup> { EggGroup.Bug, EggGroup.HumanLike }
                    };

                    var formillumise = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Bug },
                        Stats = new BaseStats
                        {
                            HP = 65,
                            Attack = 47,
                            Defense = 75,
                            SpecialAttack = 73,
                            SpecialDefense = 85,
                            Speed = 85
                        }
                    };

                    illumise.Forms.Add(formillumise);
                    db.Pokemons.Add(illumise);


                    var roselia = new Pokemon
                    {
                        NationalDexNumber = 315,
                        Name = "Roselia",
                        Generation = 3,
                        Category = "Thorn",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy, EggGroup.Grass }
                    };

                    var formroselia = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Poison },
                        Stats = new BaseStats
                        {
                            HP = 50,
                            Attack = 60,
                            Defense = 45,
                            SpecialAttack = 100,
                            SpecialDefense = 80,
                            Speed = 65
                        }
                    };

                    roselia.Forms.Add(formroselia);
                    db.Pokemons.Add(roselia);


                    var gulpin = new Pokemon
                    {
                        NationalDexNumber = 316,
                        Name = "Gulpin",
                        Generation = 3,
                        Category = "Stomach",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formgulpin = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 43,
                            Defense = 53,
                            SpecialAttack = 43,
                            SpecialDefense = 53,
                            Speed = 40
                        }
                    };

                    gulpin.Forms.Add(formgulpin);
                    db.Pokemons.Add(gulpin);


                    var swalot = new Pokemon
                    {
                        NationalDexNumber = 317,
                        Name = "Swalot",
                        Generation = 3,
                        Category = "Poison Bag",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };

                    var formswalot = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats
                        {
                            HP = 100,
                            Attack = 73,
                            Defense = 83,
                            SpecialAttack = 73,
                            SpecialDefense = 83,
                            Speed = 55
                        }
                    };

                    swalot.Forms.Add(formswalot);
                    db.Pokemons.Add(swalot);


                    var carvanha = new Pokemon
                    {
                        NationalDexNumber = 318,
                        Name = "Carvanha",
                        Generation = 3,
                        Category = "Savage",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };

                    var formcarvanha = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Dark },
                        Stats = new BaseStats
                        {
                            HP = 45,
                            Attack = 90,
                            Defense = 20,
                            SpecialAttack = 65,
                            SpecialDefense = 20,
                            Speed = 65
                        }
                    };

                    carvanha.Forms.Add(formcarvanha);
                    db.Pokemons.Add(carvanha);


                    var sharpedo = new Pokemon
                    {
                        NationalDexNumber = 319,
                        Name = "Sharpedo",
                        Generation = 3,
                        Category = "Brutal",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };

                    var formsharpedo = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Dark },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 120,
                            Defense = 40,
                            SpecialAttack = 95,
                            SpecialDefense = 40,
                            Speed = 95
                        }
                    };

                    sharpedo.Forms.Add(formsharpedo);
                    db.Pokemons.Add(sharpedo);


                    var wailmer = new Pokemon
                    {
                        NationalDexNumber = 320,
                        Name = "Wailmer",
                        Generation = 3,
                        Category = "Ball Whale",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Water2 }
                    };

                    var formwailmer = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 130,
                            Attack = 70,
                            Defense = 35,
                            SpecialAttack = 70,
                            SpecialDefense = 35,
                            Speed = 60
                        }
                    };

                    wailmer.Forms.Add(formwailmer);
                    db.Pokemons.Add(wailmer);


                    var wailord = new Pokemon
                    {
                        NationalDexNumber = 321,
                        Name = "Wailord",
                        Generation = 3,
                        Category = "Float Whale",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Water2 }
                    };

                    var formwailord = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats
                        {
                            HP = 170,
                            Attack = 90,
                            Defense = 45,
                            SpecialAttack = 90,
                            SpecialDefense = 45,
                            Speed = 60
                        }
                    };

                    wailord.Forms.Add(formwailord);
                    db.Pokemons.Add(wailord);


                    var numel = new Pokemon
                    {
                        NationalDexNumber = 322,
                        Name = "Numel",
                        Generation = 3,
                        Category = "Numb",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formnumel = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 60,
                            Defense = 40,
                            SpecialAttack = 65,
                            SpecialDefense = 45,
                            Speed = 35
                        }
                    };

                    numel.Forms.Add(formnumel);
                    db.Pokemons.Add(numel);


                    var camerupt = new Pokemon
                    {
                        NationalDexNumber = 323,
                        Name = "Camerupt",
                        Generation = 3,
                        Category = "Eruption",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formcamerupt = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire, PokemonType.Ground },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 100,
                            Defense = 70,
                            SpecialAttack = 105,
                            SpecialDefense = 75,
                            Speed = 40
                        }
                    };

                    camerupt.Forms.Add(formcamerupt);
                    db.Pokemons.Add(camerupt);


                    var torkoal = new Pokemon
                    {
                        NationalDexNumber = 324,
                        Name = "Torkoal",
                        Generation = 3,
                        Category = "Coal",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formtorkoal = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Fire },
                        Stats = new BaseStats
                        {
                            HP = 70,
                            Attack = 85,
                            Defense = 140,
                            SpecialAttack = 85,
                            SpecialDefense = 70,
                            Speed = 20
                        }
                    };

                    torkoal.Forms.Add(formtorkoal);
                    db.Pokemons.Add(torkoal);


                    var spoink = new Pokemon
                    {
                        NationalDexNumber = 325,
                        Name = "Spoink",
                        Generation = 3,
                        Category = "Bounce",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };

                    var formspoink = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats
                        {
                            HP = 60,
                            Attack = 25,
                            Defense = 35,
                            SpecialAttack = 70,
                            SpecialDefense = 80,
                            Speed = 60
                        }
                    };

                    spoink.Forms.Add(formspoink);
                    db.Pokemons.Add(spoink);

                    var grumpig = new Pokemon
                    {
                        NationalDexNumber = 326,
                        Name = "Grumpig",
                        Generation = 3,
                        Category = "Manipulate",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };
                    var formgrumpig = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats { HP = 80, Attack = 45, Defense = 65, SpecialAttack = 90, SpecialDefense = 110, Speed = 80 }
                    };
                    grumpig.Forms.Add(formgrumpig);
                    db.Pokemons.Add(grumpig);


                    var spinda = new Pokemon
                    {
                        NationalDexNumber = 327,
                        Name = "Spinda",
                        Generation = 3,
                        Category = "Spot Panda",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.HumanLike }
                    };
                    var formspinda = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 60, Attack = 60, Defense = 60, SpecialAttack = 60, SpecialDefense = 60, Speed = 60 }
                    };
                    spinda.Forms.Add(formspinda);
                    db.Pokemons.Add(spinda);


                    var trapinch = new Pokemon
                    {
                        NationalDexNumber = 328,
                        Name = "Trapinch",
                        Generation = 3,
                        Category = "Ant Pit",
                        EggGroups = new List<EggGroup> { EggGroup.Bug, EggGroup.Dragon }
                    };
                    var formtrapinch = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats { HP = 45, Attack = 100, Defense = 45, SpecialAttack = 45, SpecialDefense = 45, Speed = 10 }
                    };
                    trapinch.Forms.Add(formtrapinch);
                    db.Pokemons.Add(trapinch);


                    var vibrava = new Pokemon
                    {
                        NationalDexNumber = 329,
                        Name = "Vibrava",
                        Generation = 3,
                        Category = "Vibration",
                        EggGroups = new List<EggGroup> { EggGroup.Bug, EggGroup.Dragon }
                    };
                    var formvibrava = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground, PokemonType.Dragon },
                        Stats = new BaseStats { HP = 50, Attack = 70, Defense = 50, SpecialAttack = 50, SpecialDefense = 50, Speed = 70 }
                    };
                    vibrava.Forms.Add(formvibrava);
                    db.Pokemons.Add(vibrava);


                    var flygon = new Pokemon
                    {
                        NationalDexNumber = 330,
                        Name = "Flygon",
                        Generation = 3,
                        Category = "Mystic",
                        EggGroups = new List<EggGroup> { EggGroup.Bug, EggGroup.Dragon }
                    };
                    var formflygon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground, PokemonType.Dragon },
                        Stats = new BaseStats { HP = 80, Attack = 100, Defense = 80, SpecialAttack = 80, SpecialDefense = 80, Speed = 100 }
                    };
                    flygon.Forms.Add(formflygon);
                    db.Pokemons.Add(flygon);


                    var cacnea = new Pokemon
                    {
                        NationalDexNumber = 331,
                        Name = "Cacnea",
                        Generation = 3,
                        Category = "Cactus",
                        EggGroups = new List<EggGroup> { EggGroup.Grass, EggGroup.HumanLike }
                    };
                    var formcacnea = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass },
                        Stats = new BaseStats { HP = 50, Attack = 85, Defense = 40, SpecialAttack = 85, SpecialDefense = 40, Speed = 35 }
                    };
                    cacnea.Forms.Add(formcacnea);
                    db.Pokemons.Add(cacnea);


                    var cacturne = new Pokemon
                    {
                        NationalDexNumber = 332,
                        Name = "Cacturne",
                        Generation = 3,
                        Category = "Scarecrow",
                        EggGroups = new List<EggGroup> { EggGroup.Grass, EggGroup.HumanLike }
                    };
                    var formcacturne = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Dark },
                        Stats = new BaseStats { HP = 70, Attack = 115, Defense = 60, SpecialAttack = 115, SpecialDefense = 60, Speed = 55 }
                    };
                    cacturne.Forms.Add(formcacturne);
                    db.Pokemons.Add(cacturne);


                    var swablu = new Pokemon
                    {
                        NationalDexNumber = 333,
                        Name = "Swablu",
                        Generation = 3,
                        Category = "Cotton Bird",
                        EggGroups = new List<EggGroup> { EggGroup.Flying, EggGroup.Dragon }
                    };
                    var formswablu = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal, PokemonType.Flying },
                        Stats = new BaseStats { HP = 45, Attack = 40, Defense = 60, SpecialAttack = 40, SpecialDefense = 75, Speed = 50 }
                    };
                    swablu.Forms.Add(formswablu);
                    db.Pokemons.Add(swablu);


                    var altaria = new Pokemon
                    {
                        NationalDexNumber = 334,
                        Name = "Altaria",
                        Generation = 3,
                        Category = "Humming",
                        EggGroups = new List<EggGroup> { EggGroup.Flying, EggGroup.Dragon }
                    };
                    var formaltaria = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon, PokemonType.Flying },
                        Stats = new BaseStats { HP = 75, Attack = 70, Defense = 90, SpecialAttack = 70, SpecialDefense = 105, Speed = 80 }
                    };
                    altaria.Forms.Add(formaltaria);
                    db.Pokemons.Add(altaria);


                    var zangoose = new Pokemon
                    {
                        NationalDexNumber = 335,
                        Name = "Zangoose",
                        Generation = 3,
                        Category = "Cat Ferret",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };
                    var formzangoose = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 73, Attack = 115, Defense = 60, SpecialAttack = 60, SpecialDefense = 60, Speed = 90 }
                    };
                    zangoose.Forms.Add(formzangoose);
                    db.Pokemons.Add(zangoose);


                    var seviper = new Pokemon
                    {
                        NationalDexNumber = 336,
                        Name = "Seviper",
                        Generation = 3,
                        Category = "Fang Snake",
                        EggGroups = new List<EggGroup> { EggGroup.Field, EggGroup.Dragon }
                    };
                    var formseviper = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Poison },
                        Stats = new BaseStats { HP = 73, Attack = 100, Defense = 60, SpecialAttack = 100, SpecialDefense = 60, Speed = 65 }
                    };
                    seviper.Forms.Add(formseviper);
                    db.Pokemons.Add(seviper);


                    var lunatone = new Pokemon
                    {
                        NationalDexNumber = 337,
                        Name = "Lunatone",
                        Generation = 3,
                        Category = "Meteorite",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };
                    var formlunatone = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 90, Attack = 55, Defense = 65, SpecialAttack = 95, SpecialDefense = 85, Speed = 70 }
                    };
                    lunatone.Forms.Add(formlunatone);
                    db.Pokemons.Add(lunatone);


                    var solrock = new Pokemon
                    {
                        NationalDexNumber = 338,
                        Name = "Solrock",
                        Generation = 3,
                        Category = "Meteorite",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };
                    var formsolrock = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 90, Attack = 95, Defense = 85, SpecialAttack = 55, SpecialDefense = 65, Speed = 70 }
                    };
                    solrock.Forms.Add(formsolrock);
                    db.Pokemons.Add(solrock);


                    var barboach = new Pokemon
                    {
                        NationalDexNumber = 339,
                        Name = "Barboach",
                        Generation = 3,
                        Category = "Whiskers",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };
                    var formbarboach = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Ground },
                        Stats = new BaseStats { HP = 50, Attack = 48, Defense = 43, SpecialAttack = 46, SpecialDefense = 41, Speed = 60 }
                    };
                    barboach.Forms.Add(formbarboach);
                    db.Pokemons.Add(barboach);


                    var whiscash = new Pokemon
                    {
                        NationalDexNumber = 340,
                        Name = "Whiscash",
                        Generation = 3,
                        Category = "Whiskers",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };
                    var formwhiscash = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Ground },
                        Stats = new BaseStats { HP = 110, Attack = 78, Defense = 73, SpecialAttack = 76, SpecialDefense = 71, Speed = 60 }
                    };
                    whiscash.Forms.Add(formwhiscash);
                    db.Pokemons.Add(whiscash);


                    var corphish = new Pokemon
                    {
                        NationalDexNumber = 341,
                        Name = "Corphish",
                        Generation = 3,
                        Category = "Ruffian",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water3 }
                    };
                    var formcorphish = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 43, Attack = 80, Defense = 65, SpecialAttack = 50, SpecialDefense = 35, Speed = 35 }
                    };
                    corphish.Forms.Add(formcorphish);
                    db.Pokemons.Add(corphish);


                    var crawdaunt = new Pokemon
                    {
                        NationalDexNumber = 342,
                        Name = "Crawdaunt",
                        Generation = 3,
                        Category = "Rogue",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water3 }
                    };
                    var formcrawdaunt = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Dark },
                        Stats = new BaseStats { HP = 63, Attack = 120, Defense = 85, SpecialAttack = 90, SpecialDefense = 55, Speed = 55 }
                    };
                    crawdaunt.Forms.Add(formcrawdaunt);
                    db.Pokemons.Add(crawdaunt);


                    var baltoy = new Pokemon
                    {
                        NationalDexNumber = 343,
                        Name = "Baltoy",
                        Generation = 3,
                        Category = "Clay Doll",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };
                    var formbaltoy = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 40, Attack = 40, Defense = 55, SpecialAttack = 40, SpecialDefense = 70, Speed = 55 }
                    };
                    baltoy.Forms.Add(formbaltoy);
                    db.Pokemons.Add(baltoy);


                    var claydol = new Pokemon
                    {
                        NationalDexNumber = 344,
                        Name = "Claydol",
                        Generation = 3,
                        Category = "Clay Doll",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };
                    var formclaydol = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 60, Attack = 70, Defense = 105, SpecialAttack = 70, SpecialDefense = 120, Speed = 75 }
                    };
                    claydol.Forms.Add(formclaydol);
                    db.Pokemons.Add(claydol);


                    var lileep = new Pokemon
                    {
                        NationalDexNumber = 345,
                        Name = "Lileep",
                        Generation = 3,
                        Category = "Sea Lily",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };
                    var formlileep = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Grass },
                        Stats = new BaseStats { HP = 66, Attack = 41, Defense = 77, SpecialAttack = 61, SpecialDefense = 87, Speed = 23 }
                    };
                    lileep.Forms.Add(formlileep);
                    db.Pokemons.Add(lileep);


                    var cradily = new Pokemon
                    {
                        NationalDexNumber = 346,
                        Name = "Cradily",
                        Generation = 3,
                        Category = "Barnacle",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };
                    var formcradily = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Grass },
                        Stats = new BaseStats { HP = 86, Attack = 81, Defense = 97, SpecialAttack = 81, SpecialDefense = 107, Speed = 43 }
                    };
                    cradily.Forms.Add(formcradily);
                    db.Pokemons.Add(cradily);


                    var anorith = new Pokemon
                    {
                        NationalDexNumber = 347,
                        Name = "Anorith",
                        Generation = 3,
                        Category = "Old Shrimp",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };
                    var formanorith = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Bug },
                        Stats = new BaseStats { HP = 45, Attack = 95, Defense = 50, SpecialAttack = 40, SpecialDefense = 50, Speed = 75 }
                    };
                    anorith.Forms.Add(formanorith);
                    db.Pokemons.Add(anorith);


                    var armaldo = new Pokemon
                    {
                        NationalDexNumber = 348,
                        Name = "Armaldo",
                        Generation = 3,
                        Category = "Plate",
                        EggGroups = new List<EggGroup> { EggGroup.Water3 }
                    };
                    var formarmaldo = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock, PokemonType.Bug },
                        Stats = new BaseStats { HP = 75, Attack = 125, Defense = 100, SpecialAttack = 70, SpecialDefense = 80, Speed = 45 }
                    };
                    armaldo.Forms.Add(formarmaldo);
                    db.Pokemons.Add(armaldo);


                    var feebas = new Pokemon
                    {
                        NationalDexNumber = 349,
                        Name = "Feebas",
                        Generation = 3,
                        Category = "Fish",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Dragon }
                    };
                    var formfeebas = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 20, Attack = 15, Defense = 20, SpecialAttack = 10, SpecialDefense = 55, Speed = 80 }
                    };
                    feebas.Forms.Add(formfeebas);
                    db.Pokemons.Add(feebas);


                    var milotic = new Pokemon
                    {
                        NationalDexNumber = 350,
                        Name = "Milotic",
                        Generation = 3,
                        Category = "Tender",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Dragon }
                    };
                    var formmilotic = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 95, Attack = 60, Defense = 79, SpecialAttack = 100, SpecialDefense = 125, Speed = 81 }
                    };
                    milotic.Forms.Add(formmilotic);
                    db.Pokemons.Add(milotic);


                    var castform = new Pokemon
                    {
                        NationalDexNumber = 351,
                        Name = "Castform",
                        Generation = 3,
                        Category = "Weather",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy, EggGroup.Amorphous }
                    };
                    var formcastform = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 70, Attack = 70, Defense = 70, SpecialAttack = 70, SpecialDefense = 70, Speed = 70 }
                    };
                    castform.Forms.Add(formcastform);
                    db.Pokemons.Add(castform);


                    var kecleon = new Pokemon
                    {
                        NationalDexNumber = 352,
                        Name = "Kecleon",
                        Generation = 3,
                        Category = "Color Swap",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };
                    var formkecleon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Normal },
                        Stats = new BaseStats { HP = 60, Attack = 90, Defense = 70, SpecialAttack = 60, SpecialDefense = 120, Speed = 40 }
                    };
                    kecleon.Forms.Add(formkecleon);
                    db.Pokemons.Add(kecleon);


                    var shuppet = new Pokemon
                    {
                        NationalDexNumber = 353,
                        Name = "Shuppet",
                        Generation = 3,
                        Category = "Puppet",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };
                    var formshuppet = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ghost },
                        Stats = new BaseStats { HP = 44, Attack = 75, Defense = 35, SpecialAttack = 63, SpecialDefense = 33, Speed = 45 }
                    };
                    shuppet.Forms.Add(formshuppet);
                    db.Pokemons.Add(shuppet);


                    var banette = new Pokemon
                    {
                        NationalDexNumber = 354,
                        Name = "Banette",
                        Generation = 3,
                        Category = "Marionette",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };
                    var formbanette = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ghost },
                        Stats = new BaseStats { HP = 64, Attack = 115, Defense = 65, SpecialAttack = 83, SpecialDefense = 63, Speed = 65 }
                    };
                    banette.Forms.Add(formbanette);
                    db.Pokemons.Add(banette);


                    var duskull = new Pokemon
                    {
                        NationalDexNumber = 355,
                        Name = "Duskull",
                        Generation = 3,
                        Category = "Requiem",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };
                    var formduskull = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ghost },
                        Stats = new BaseStats { HP = 20, Attack = 40, Defense = 90, SpecialAttack = 30, SpecialDefense = 90, Speed = 25 }
                    };
                    duskull.Forms.Add(formduskull);
                    db.Pokemons.Add(duskull);


                    var dusclops = new Pokemon
                    {
                        NationalDexNumber = 356,
                        Name = "Dusclops",
                        Generation = 3,
                        Category = "Beckon",
                        EggGroups = new List<EggGroup> { EggGroup.Amorphous }
                    };
                    var formdusclops = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ghost },
                        Stats = new BaseStats { HP = 40, Attack = 70, Defense = 130, SpecialAttack = 60, SpecialDefense = 130, Speed = 25 }
                    };
                    dusclops.Forms.Add(formdusclops);
                    db.Pokemons.Add(dusclops);


                    var tropius = new Pokemon
                    {
                        NationalDexNumber = 357,
                        Name = "Tropius",
                        Generation = 3,
                        Category = "Fruit",
                        EggGroups = new List<EggGroup> { EggGroup.Monster, EggGroup.Grass }
                    };
                    var formtropius = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Grass, PokemonType.Flying },
                        Stats = new BaseStats { HP = 99, Attack = 68, Defense = 83, SpecialAttack = 72, SpecialDefense = 87, Speed = 51 }
                    };
                    tropius.Forms.Add(formtropius);
                    db.Pokemons.Add(tropius);


                    var chingling = new Pokemon
                    {
                        NationalDexNumber = 358,
                        Name = "Chingling",
                        Generation = 4,
                        Category = "Bell",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formchingling = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats { HP = 45, Attack = 30, Defense = 50, SpecialAttack = 65, SpecialDefense = 50, Speed = 45 }
                    };
                    chingling.Forms.Add(formchingling);
                    db.Pokemons.Add(chingling);


                    var absol = new Pokemon
                    {
                        NationalDexNumber = 359,
                        Name = "Absol",
                        Generation = 3,
                        Category = "Disaster",
                        EggGroups = new List<EggGroup> { EggGroup.Field }
                    };
                    var formabsol = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dark },
                        Stats = new BaseStats { HP = 65, Attack = 130, Defense = 60, SpecialAttack = 75, SpecialDefense = 60, Speed = 75 }
                    };
                    absol.Forms.Add(formabsol);
                    db.Pokemons.Add(absol);


                    var wynaut = new Pokemon
                    {
                        NationalDexNumber = 360,
                        Name = "Wynaut",
                        Generation = 3,
                        Category = "Bright",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formwynaut = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats { HP = 95, Attack = 23, Defense = 48, SpecialAttack = 23, SpecialDefense = 48, Speed = 23 }
                    };
                    wynaut.Forms.Add(formwynaut);
                    db.Pokemons.Add(wynaut);


                    var snorunt = new Pokemon
                    {
                        NationalDexNumber = 361,
                        Name = "Snorunt",
                        Generation = 3,
                        Category = "Snow Hat",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy, EggGroup.Mineral }
                    };
                    var formsnorunt = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice },
                        Stats = new BaseStats { HP = 50, Attack = 50, Defense = 50, SpecialAttack = 50, SpecialDefense = 50, Speed = 50 }
                    };
                    snorunt.Forms.Add(formsnorunt);
                    db.Pokemons.Add(snorunt);


                    var glalie = new Pokemon
                    {
                        NationalDexNumber = 362,
                        Name = "Glalie",
                        Generation = 3,
                        Category = "Face",
                        EggGroups = new List<EggGroup> { EggGroup.Fairy, EggGroup.Mineral }
                    };
                    var formglalie = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice },
                        Stats = new BaseStats { HP = 80, Attack = 80, Defense = 80, SpecialAttack = 80, SpecialDefense = 80, Speed = 80 }
                    };
                    glalie.Forms.Add(formglalie);
                    db.Pokemons.Add(glalie);


                    var spheal = new Pokemon
                    {
                        NationalDexNumber = 363,
                        Name = "Spheal",
                        Generation = 3,
                        Category = "Clap",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Field }
                    };
                    var formspheal = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice, PokemonType.Water },
                        Stats = new BaseStats { HP = 70, Attack = 40, Defense = 50, SpecialAttack = 55, SpecialDefense = 50, Speed = 25 }
                    };
                    spheal.Forms.Add(formspheal);
                    db.Pokemons.Add(spheal);


                    var sealeo = new Pokemon
                    {
                        NationalDexNumber = 364,
                        Name = "Sealeo",
                        Generation = 3,
                        Category = "Ball Roll",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Field }
                    };
                    var formsealeo = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice, PokemonType.Water },
                        Stats = new BaseStats { HP = 90, Attack = 60, Defense = 70, SpecialAttack = 75, SpecialDefense = 70, Speed = 45 }
                    };
                    sealeo.Forms.Add(formsealeo);
                    db.Pokemons.Add(sealeo);


                    var walrein = new Pokemon
                    {
                        NationalDexNumber = 365,
                        Name = "Walrein",
                        Generation = 3,
                        Category = "Ice Break",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Field }
                    };
                    var formwalrein = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice, PokemonType.Water },
                        Stats = new BaseStats { HP = 110, Attack = 80, Defense = 90, SpecialAttack = 95, SpecialDefense = 90, Speed = 65 }
                    };
                    walrein.Forms.Add(formwalrein);
                    db.Pokemons.Add(walrein);


                    var clamperl = new Pokemon
                    {
                        NationalDexNumber = 366,
                        Name = "Clamperl",
                        Generation = 3,
                        Category = "Bivalve",
                        EggGroups = new List<EggGroup> { EggGroup.Water1 }
                    };
                    var formclamperl = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 35, Attack = 64, Defense = 85, SpecialAttack = 74, SpecialDefense = 55, Speed = 32 }
                    };
                    clamperl.Forms.Add(formclamperl);
                    db.Pokemons.Add(clamperl);


                    var huntail = new Pokemon
                    {
                        NationalDexNumber = 367,
                        Name = "Huntail",
                        Generation = 3,
                        Category = "Deep Sea",
                        EggGroups = new List<EggGroup> { EggGroup.Water1 }
                    };
                    var formhuntail = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 55, Attack = 104, Defense = 105, SpecialAttack = 94, SpecialDefense = 75, Speed = 52 }
                    };
                    huntail.Forms.Add(formhuntail);
                    db.Pokemons.Add(huntail);


                    var gorebyss = new Pokemon
                    {
                        NationalDexNumber = 368,
                        Name = "Gorebyss",
                        Generation = 3,
                        Category = "South Sea",
                        EggGroups = new List<EggGroup> { EggGroup.Water1 }
                    };
                    var formgorebyss = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 55, Attack = 84, Defense = 105, SpecialAttack = 114, SpecialDefense = 75, Speed = 52 }
                    };
                    gorebyss.Forms.Add(formgorebyss);
                    db.Pokemons.Add(gorebyss);


                    var relicanth = new Pokemon
                    {
                        NationalDexNumber = 369,
                        Name = "Relicanth",
                        Generation = 3,
                        Category = "Longevity",
                        EggGroups = new List<EggGroup> { EggGroup.Water1, EggGroup.Water2 }
                    };
                    var formrelicanth = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water, PokemonType.Rock },
                        Stats = new BaseStats { HP = 100, Attack = 90, Defense = 130, SpecialAttack = 45, SpecialDefense = 65, Speed = 55 }
                    };
                    relicanth.Forms.Add(formrelicanth);
                    db.Pokemons.Add(relicanth);


                    var luvdisc = new Pokemon
                    {
                        NationalDexNumber = 370,
                        Name = "Luvdisc",
                        Generation = 3,
                        Category = "Rendezvous",
                        EggGroups = new List<EggGroup> { EggGroup.Water2 }
                    };
                    var formluvdisc = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 43, Attack = 30, Defense = 55, SpecialAttack = 40, SpecialDefense = 65, Speed = 97 }
                    };
                    luvdisc.Forms.Add(formluvdisc);
                    db.Pokemons.Add(luvdisc);


                    var bagon = new Pokemon
                    {
                        NationalDexNumber = 371,
                        Name = "Bagon",
                        Generation = 3,
                        Category = "Rock Head",
                        EggGroups = new List<EggGroup> { EggGroup.Dragon }
                    };
                    var formbagon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon },
                        Stats = new BaseStats { HP = 45, Attack = 75, Defense = 60, SpecialAttack = 40, SpecialDefense = 30, Speed = 50 }
                    };
                    bagon.Forms.Add(formbagon);
                    db.Pokemons.Add(bagon);


                    var shelgon = new Pokemon
                    {
                        NationalDexNumber = 372,
                        Name = "Shelgon",
                        Generation = 3,
                        Category = "Endurance",
                        EggGroups = new List<EggGroup> { EggGroup.Dragon }
                    };
                    var formshelgon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon },
                        Stats = new BaseStats { HP = 65, Attack = 95, Defense = 100, SpecialAttack = 60, SpecialDefense = 50, Speed = 50 }
                    };
                    shelgon.Forms.Add(formshelgon);
                    db.Pokemons.Add(shelgon);


                    var salamence = new Pokemon
                    {
                        NationalDexNumber = 373,
                        Name = "Salamence",
                        Generation = 3,
                        Category = "Dragon",
                        EggGroups = new List<EggGroup> { EggGroup.Dragon }
                    };
                    var formsalamence = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon, PokemonType.Flying },
                        Stats = new BaseStats { HP = 95, Attack = 135, Defense = 80, SpecialAttack = 110, SpecialDefense = 80, Speed = 100 }
                    };
                    salamence.Forms.Add(formsalamence);
                    db.Pokemons.Add(salamence);


                    var beldum = new Pokemon
                    {
                        NationalDexNumber = 374,
                        Name = "Beldum",
                        Generation = 3,
                        Category = "Iron Ball",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };
                    var formbeldum = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 40, Attack = 55, Defense = 80, SpecialAttack = 35, SpecialDefense = 60, Speed = 30 }
                    };
                    beldum.Forms.Add(formbeldum);
                    db.Pokemons.Add(beldum);


                    var metang = new Pokemon
                    {
                        NationalDexNumber = 375,
                        Name = "Metang",
                        Generation = 3,
                        Category = "Iron Claw",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };
                    var formmetang = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 60, Attack = 75, Defense = 100, SpecialAttack = 55, SpecialDefense = 80, Speed = 50 }
                    };
                    metang.Forms.Add(formmetang);
                    db.Pokemons.Add(metang);


                    var metagross = new Pokemon
                    {
                        NationalDexNumber = 376,
                        Name = "Metagross",
                        Generation = 3,
                        Category = "Iron Leg",
                        EggGroups = new List<EggGroup> { EggGroup.Mineral }
                    };
                    var formmetagross = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 80, Attack = 135, Defense = 130, SpecialAttack = 95, SpecialDefense = 90, Speed = 70 }
                    };
                    metagross.Forms.Add(formmetagross);
                    db.Pokemons.Add(metagross);


                    var regirock = new Pokemon
                    {
                        NationalDexNumber = 377,
                        Name = "Regirock",
                        Generation = 3,
                        Category = "Rock Peak",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formregirock = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Rock },
                        Stats = new BaseStats { HP = 80, Attack = 100, Defense = 200, SpecialAttack = 50, SpecialDefense = 100, Speed = 50 }
                    };
                    regirock.Forms.Add(formregirock);
                    db.Pokemons.Add(regirock);


                    var regice = new Pokemon
                    {
                        NationalDexNumber = 378,
                        Name = "Regice",
                        Generation = 3,
                        Category = "Iceberg",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formregice = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ice },
                        Stats = new BaseStats { HP = 80, Attack = 50, Defense = 100, SpecialAttack = 100, SpecialDefense = 200, Speed = 50 }
                    };
                    regice.Forms.Add(formregice);
                    db.Pokemons.Add(regice);


                    var registeel = new Pokemon
                    {
                        NationalDexNumber = 379,
                        Name = "Registeel",
                        Generation = 3,
                        Category = "Iron",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formregisteel = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel },
                        Stats = new BaseStats { HP = 80, Attack = 75, Defense = 150, SpecialAttack = 75, SpecialDefense = 150, Speed = 50 }
                    };
                    registeel.Forms.Add(formregisteel);
                    db.Pokemons.Add(registeel);


                    var latias = new Pokemon
                    {
                        NationalDexNumber = 380,
                        Name = "Latias",
                        Generation = 3,
                        Category = "Eon",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formlatias = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 80, Attack = 80, Defense = 90, SpecialAttack = 110, SpecialDefense = 130, Speed = 110 }
                    };
                    latias.Forms.Add(formlatias);
                    db.Pokemons.Add(latias);


                    var latios = new Pokemon
                    {
                        NationalDexNumber = 381,
                        Name = "Latios",
                        Generation = 3,
                        Category = "Eon",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formlatios = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 80, Attack = 90, Defense = 80, SpecialAttack = 130, SpecialDefense = 110, Speed = 110 }
                    };
                    latios.Forms.Add(formlatios);
                    db.Pokemons.Add(latios);


                    var kyogre = new Pokemon
                    {
                        NationalDexNumber = 382,
                        Name = "Kyogre",
                        Generation = 3,
                        Category = "Sea Basin",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formkyogre = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Water },
                        Stats = new BaseStats { HP = 100, Attack = 100, Defense = 90, SpecialAttack = 150, SpecialDefense = 140, Speed = 90 }
                    };
                    kyogre.Forms.Add(formkyogre);
                    db.Pokemons.Add(kyogre);


                    var groudon = new Pokemon
                    {
                        NationalDexNumber = 383,
                        Name = "Groudon",
                        Generation = 3,
                        Category = "Continent",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formgroudon = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Ground },
                        Stats = new BaseStats { HP = 100, Attack = 150, Defense = 140, SpecialAttack = 100, SpecialDefense = 90, Speed = 90 }
                    };
                    groudon.Forms.Add(formgroudon);
                    db.Pokemons.Add(groudon);


                    var rayquaza = new Pokemon
                    {
                        NationalDexNumber = 384,
                        Name = "Rayquaza",
                        Generation = 3,
                        Category = "Sky High",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formrayquaza = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Dragon, PokemonType.Flying },
                        Stats = new BaseStats { HP = 105, Attack = 150, Defense = 90, SpecialAttack = 150, SpecialDefense = 90, Speed = 95 }
                    };
                    rayquaza.Forms.Add(formrayquaza);
                    db.Pokemons.Add(rayquaza);


                    var jirachi = new Pokemon
                    {
                        NationalDexNumber = 385,
                        Name = "Jirachi",
                        Generation = 3,
                        Category = "Wish",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formjirachi = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Steel, PokemonType.Psychic },
                        Stats = new BaseStats { HP = 100, Attack = 100, Defense = 100, SpecialAttack = 100, SpecialDefense = 100, Speed = 100 }
                    };
                    jirachi.Forms.Add(formjirachi);
                    db.Pokemons.Add(jirachi);


                    var deoxys = new Pokemon
                    {
                        NationalDexNumber = 386,
                        Name = "Deoxys",
                        Generation = 3,
                        Category = "DNA",
                        EggGroups = new List<EggGroup> { EggGroup.Undiscovered }
                    };
                    var formdeoxys = new PokemonForm
                    {
                        FormName = "Normal",
                        Types = new List<PokemonType> { PokemonType.Psychic },
                        Stats = new BaseStats { HP = 50, Attack = 150, Defense = 50, SpecialAttack = 150, SpecialDefense = 50, Speed = 150 }
                    };
                    deoxys.Forms.Add(formdeoxys);
                    db.Pokemons.Add(deoxys);

                    electrode.Forms.Add(formelectrode);
                    db.Pokemons.Add(electrode);

                    exeggcute.Forms.Add(formexeggcute);
                    db.Pokemons.Add(exeggcute);

                    exeggutor.Forms.Add(formexeggutor);
                    db.Pokemons.Add(exeggutor);

                    cubone.Forms.Add(formcubone);
                    db.Pokemons.Add(cubone);

                    marowak.Forms.Add(formmarowak);
                    db.Pokemons.Add(marowak);

                    hitmonlee.Forms.Add(formhitmonlee);
                    db.Pokemons.Add(hitmonlee);

                    hitmonchan.Forms.Add(formhitmonchan);
                    db.Pokemons.Add(hitmonchan);

                    lickitung.Forms.Add(formlickitung);
                    db.Pokemons.Add(lickitung);

                    koffing.Forms.Add(formkoffing);
                    db.Pokemons.Add(koffing);

                    weezing.Forms.Add(formweezing);
                    db.Pokemons.Add(weezing);

                    rhyhorn.Forms.Add(formrhyhorn);
                    db.Pokemons.Add(rhyhorn);

                    rhydon.Forms.Add(formrhydon);
                    db.Pokemons.Add(rhydon);

                    chansey.Forms.Add(formchansey);
                    db.Pokemons.Add(chansey);

                    tangela.Forms.Add(formtangela);
                    db.Pokemons.Add(tangela);

                    kangaskhan.Forms.Add(formkangaskhan);
                    db.Pokemons.Add(kangaskhan);

                    horsea.Forms.Add(formhorsea);
                    db.Pokemons.Add(horsea);

                    seadra.Forms.Add(formseadra);
                    db.Pokemons.Add(seadra);

                    goldeen.Forms.Add(formgoldeen);
                    db.Pokemons.Add(goldeen);

                    seaking.Forms.Add(formseaking);
                    db.Pokemons.Add(seaking);

                    staryu.Forms.Add(formstaryu);
                    db.Pokemons.Add(staryu);

                    starmie.Forms.Add(formstarmie);
                    db.Pokemons.Add(starmie);

                    mrMime.Forms.Add(formmrMime);
                    db.Pokemons.Add(mrMime);

                    scyther.Forms.Add(formscyther);
                    db.Pokemons.Add(scyther);

                    jynx.Forms.Add(formjynx);
                    db.Pokemons.Add(jynx);

                    electabuzz.Forms.Add(formelectabuzz);
                    db.Pokemons.Add(electabuzz);

                    golem.Forms.Add(formgolem);
                    db.Pokemons.Add(golem);

                    ponyta.Forms.Add(formponyta);
                    db.Pokemons.Add(ponyta);

                    rapidash.Forms.Add(formrapidash);
                    db.Pokemons.Add(rapidash);

                    slowpoke.Forms.Add(formslowpoke);
                    db.Pokemons.Add(slowpoke);

                    slowbro.Forms.Add(formslowbro);
                    db.Pokemons.Add(slowbro);

                    magnemite.Forms.Add(formmagnemite);
                    db.Pokemons.Add(magnemite);

                    magneton.Forms.Add(formmagneton);
                    db.Pokemons.Add(magneton);

                    farfetchd.Forms.Add(formfarfetchd);
                    db.Pokemons.Add(farfetchd);

                    doduo.Forms.Add(formdoduo);
                    db.Pokemons.Add(doduo);

                    dodrio.Forms.Add(formdodrio);
                    db.Pokemons.Add(dodrio);

                    seel.Forms.Add(formseel);
                    db.Pokemons.Add(seel);

                    dewgong.Forms.Add(formdewgong);
                    db.Pokemons.Add(dewgong);

                    grimer.Forms.Add(formgrimer);
                    db.Pokemons.Add(grimer);

                    muk.Forms.Add(formmuk);
                    db.Pokemons.Add(muk);

                    shellder.Forms.Add(formshellder);
                    db.Pokemons.Add(shellder);

                    cloyster.Forms.Add(formcloyster);
                    db.Pokemons.Add(cloyster);

                    gastly.Forms.Add(formgastly);
                    db.Pokemons.Add(gastly);

                    haunter.Forms.Add(formhaunter);
                    db.Pokemons.Add(haunter);

                    gengar.Forms.Add(formgengar);
                    db.Pokemons.Add(gengar);

                    onix.Forms.Add(formonix);
                    db.Pokemons.Add(onix);

                    drowzee.Forms.Add(formdrowzee);
                    db.Pokemons.Add(drowzee);

                    hypno.Forms.Add(formhypno);
                    db.Pokemons.Add(hypno);

                    krabby.Forms.Add(formkrabby);
                    db.Pokemons.Add(krabby);

                    kingler.Forms.Add(formkingler);
                    db.Pokemons.Add(kingler);

                    voltorb.Forms.Add(formvoltorb);
                    db.Pokemons.Add(voltorb);

                    dugtrio.Forms.Add(formdugtrio);
                    db.Pokemons.Add(dugtrio);

                    meowth.Forms.Add(formmeowth);
                    db.Pokemons.Add(meowth);

                    persian.Forms.Add(formpersian);
                    db.Pokemons.Add(persian);

                    psyduck.Forms.Add(formpsyduck);
                    db.Pokemons.Add(psyduck);

                    golduck.Forms.Add(formgolduck);
                    db.Pokemons.Add(golduck);

                    mankey.Forms.Add(formmankey);
                    db.Pokemons.Add(mankey);

                    primeape.Forms.Add(formprimeape);
                    db.Pokemons.Add(primeape);

                    growlithe.Forms.Add(formgrowlithe);
                    db.Pokemons.Add(growlithe);

                    arcanine.Forms.Add(formarcanine);
                    db.Pokemons.Add(arcanine);

                    poliwag.Forms.Add(formpoliwag);
                    db.Pokemons.Add(poliwag);

                    poliwhirl.Forms.Add(formpoliwhirl);
                    db.Pokemons.Add(poliwhirl);

                    poliwrath.Forms.Add(formpoliwrath);
                    db.Pokemons.Add(poliwrath);

                    abra.Forms.Add(formabra);
                    db.Pokemons.Add(abra);

                    kadabra.Forms.Add(formkadabra);
                    db.Pokemons.Add(kadabra);

                    alakazam.Forms.Add(formalakazam);
                    db.Pokemons.Add(alakazam);

                    machop.Forms.Add(formmachop);
                    db.Pokemons.Add(machop);

                    machoke.Forms.Add(formmachoke);
                    db.Pokemons.Add(machoke);

                    machamp.Forms.Add(formmachamp);
                    db.Pokemons.Add(machamp);

                    bellsprout.Forms.Add(formbellsprout);
                    db.Pokemons.Add(bellsprout);

                    weepinbell.Forms.Add(formweepinbell);
                    db.Pokemons.Add(weepinbell);

                    victreebel.Forms.Add(formvictreebel);
                    db.Pokemons.Add(victreebel);

                    tentacool.Forms.Add(formtentacool);
                    db.Pokemons.Add(tentacool);

                    tentacruel.Forms.Add(formtentacruel);
                    db.Pokemons.Add(tentacruel);

                    geodude.Forms.Add(formgeodude);
                    db.Pokemons.Add(geodude);

                    graveler.Forms.Add(formgraveler);
                    db.Pokemons.Add(graveler);

                    raichu.Forms.Add(formraichu);
                    db.Pokemons.Add(raichu);

                    sandshrew.Forms.Add(formsandshrew);
                    db.Pokemons.Add(sandshrew);

                    sandslash.Forms.Add(formsandslash);
                    db.Pokemons.Add(sandslash);

                    nidoranf.Forms.Add(formnidoranf);
                    db.Pokemons.Add(nidoranf);

                    nidorina.Forms.Add(formnidorina);
                    db.Pokemons.Add(nidorina);

                    nidoqueen.Forms.Add(formnidoqueen);
                    db.Pokemons.Add(nidoqueen);

                    nidoranm.Forms.Add(formnidoranm);
                    db.Pokemons.Add(nidoranm);

                    nidorino.Forms.Add(formnidorino);
                    db.Pokemons.Add(nidorino);

                    nidoking.Forms.Add(formnidoking);
                    db.Pokemons.Add(nidoking);

                    clefairy.Forms.Add(formclefairy);
                    db.Pokemons.Add(clefairy);

                    clefable.Forms.Add(formclefable);
                    db.Pokemons.Add(clefable);

                    vulpix.Forms.Add(formvulpix);
                    db.Pokemons.Add(vulpix);

                    ninetales.Forms.Add(formninetales);
                    db.Pokemons.Add(ninetales);

                    jigglypuff.Forms.Add(formjigglypuff);
                    db.Pokemons.Add(jigglypuff);

                    wigglytuff.Forms.Add(formwigglytuff);
                    db.Pokemons.Add(wigglytuff);

                    zubat.Forms.Add(formzubat);
                    db.Pokemons.Add(zubat);

                    golbat.Forms.Add(formgolbat);
                    db.Pokemons.Add(golbat);

                    oddish.Forms.Add(formoddish);
                    db.Pokemons.Add(oddish);

                    gloom.Forms.Add(formgloom);
                    db.Pokemons.Add(gloom);

                    vileplume.Forms.Add(formvileplume);
                    db.Pokemons.Add(vileplume);

                    paras.Forms.Add(formparas);
                    db.Pokemons.Add(paras);

                    parasect.Forms.Add(formparasect);
                    db.Pokemons.Add(parasect);

                    venonat.Forms.Add(formvenonat);
                    db.Pokemons.Add(venonat);

                    venomoth.Forms.Add(formvenomoth);
                    db.Pokemons.Add(venomoth);

                    diglett.Forms.Add(formdiglett);
                    db.Pokemons.Add(diglett);

                    pidgey.Forms.Add(formpidgey);
                    db.Pokemons.Add(pidgey);

                    pidgeotto.Forms.Add(formpidgeotto);
                    db.Pokemons.Add(pidgeotto);

                    pidgeot.Forms.Add(formpidgeot);
                    db.Pokemons.Add(pidgeot);

                    rattata.Forms.Add(formrattata);
                    db.Pokemons.Add(rattata);

                    raticate.Forms.Add(formraticate);
                    db.Pokemons.Add(raticate);

                    spearow.Forms.Add(formspearow);
                    db.Pokemons.Add(spearow);

                    fearow.Forms.Add(formfearow);
                    db.Pokemons.Add(fearow);

                    ekans.Forms.Add(formekans);
                    db.Pokemons.Add(ekans);

                    arbok.Forms.Add(formarbok);
                    db.Pokemons.Add(arbok);

                    pikachu.Forms.Add(formpikachu);
                    db.Pokemons.Add(pikachu);

                    bulbasaur.Forms.Add(form);
                    db.Pokemons.Add(bulbasaur);
                    ivysaur.Forms.Add(formivy);
                    db.Pokemons.Add(ivysaur);
                    venusaur.Forms.Add(formvenu);
                    db.Pokemons.Add(venusaur);
                    charmander.Forms.Add(formchar);
                    db.Pokemons.Add(charmander);
                    charmeleon.Forms.Add(formcharleon);
                    db.Pokemons.Add(charmeleon);
                    charizard.Forms.Add(formcharizard);
                    db.Pokemons.Add(charizard);
                    squirtle.Forms.Add(formsquirtle);
                    db.Pokemons.Add(squirtle);
                    wartortle.Forms.Add(formwartortle);
                    db.Pokemons.Add(wartortle);
                    blastoise.Forms.Add(formblastoise);
                    db.Pokemons.Add(blastoise);
                    caterpie.Forms.Add(formcaterpie);
                    db.Pokemons.Add(caterpie);
                    metapod.Forms.Add(formmetapod);
                    db.Pokemons.Add(metapod);
                    butterfree.Forms.Add(formbutterfree);
                    db.Pokemons.Add(butterfree);
                    weedle.Forms.Add(formweedle);
                    db.Pokemons.Add(weedle);
                    kakuna.Forms.Add(formkakuna);
                    db.Pokemons.Add(kakuna);
                    beedrill.Forms.Add(formbeedrill);
                    db.Pokemons.Add(beedrill);
                    Debug.WriteLine("Pokémons inseridos com sucesso!");
                }
                if (!db.Sprites.Any())
                {
                    LoadPokemonSprites(db);
                    Debug.WriteLine("Sprites inseridas com sucesso!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar banco:\n{ex.Message}\n\nDetalhes:\n{ex.InnerException?.Message}");
            }
            }
        }
    }
