using System.Text.Json.Serialization;

namespace GestaoDePokemon.model
{
    public readonly record struct IndividualValues
    {
        public const int Min = 0;
        public const int Max = 31;

        public int HP { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int SpecialAttack { get; }
        public int SpecialDefense { get; }
        public int Speed { get; }

        [JsonConstructor]
        public IndividualValues(
            int hp,
            int attack,
            int defense,
            int specialAttack,
            int specialDefense,
            int speed)
        {
            Validate(hp, nameof(hp));
            Validate(attack, nameof(attack));
            Validate(defense, nameof(defense));
            Validate(specialAttack, nameof(specialAttack));
            Validate(specialDefense, nameof(specialDefense));
            Validate(speed, nameof(speed));

            HP = hp;
            Attack = attack;
            Defense = defense;
            SpecialAttack = specialAttack;
            SpecialDefense = specialDefense;
            Speed = speed;
        }

        public int GetValue(PokemonStat stat) => stat switch
        {
            PokemonStat.HP => HP,
            PokemonStat.Attack => Attack,
            PokemonStat.Defense => Defense,
            PokemonStat.SpecialAttack => SpecialAttack,
            PokemonStat.SpecialDefense => SpecialDefense,
            PokemonStat.Speed => Speed,
            _ => throw new ArgumentOutOfRangeException(nameof(stat))
        };

        /// <summary>Cópia com o IV de um atributo trocado (o tipo é imutável).</summary>
        public IndividualValues Com(PokemonStat stat, int value) => stat switch
        {
            PokemonStat.HP => new(value, Attack, Defense, SpecialAttack, SpecialDefense, Speed),
            PokemonStat.Attack => new(HP, value, Defense, SpecialAttack, SpecialDefense, Speed),
            PokemonStat.Defense => new(HP, Attack, value, SpecialAttack, SpecialDefense, Speed),
            PokemonStat.SpecialAttack => new(HP, Attack, Defense, value, SpecialDefense, Speed),
            PokemonStat.SpecialDefense => new(HP, Attack, Defense, SpecialAttack, value, Speed),
            PokemonStat.Speed => new(HP, Attack, Defense, SpecialAttack, SpecialDefense, value),
            _ => throw new ArgumentOutOfRangeException(nameof(stat))
        };

        private static void Validate(int value, string name)
        {
            if (value < Min || value > Max)
                throw new ArgumentOutOfRangeException(
                    name,
                    "O IV deve estar entre 0 e 31."
                );
        }
    }
}
