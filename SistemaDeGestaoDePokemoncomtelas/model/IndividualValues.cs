namespace GestaoDePokemon.model
{
    public readonly record struct IndividualValues
    {
        public int HP { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int SpecialAttack { get; }
        public int SpecialDefense { get; }
        public int Speed { get; }

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

        private static void Validate(int value, string name)
        {
            if (value < 0 || value > 31)
                throw new ArgumentOutOfRangeException(
                    name,
                    "O IV deve estar entre 0 e 31."
                );
        }

        public static IndividualValues GenerateRandom()
        {
            return new IndividualValues(
                Random.Shared.Next(32),
                Random.Shared.Next(32),
                Random.Shared.Next(32),
                Random.Shared.Next(32),
                Random.Shared.Next(32),
                Random.Shared.Next(32)
            );
        }
    }
}