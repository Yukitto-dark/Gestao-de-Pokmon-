namespace GestaoDePokemon.model
{
    public class EffortValues
    {
        public int HP { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int SpecialAttack { get; private set; }
        public int SpecialDefense { get; private set; }
        public int Speed { get; private set; }

        public int Total =>
            HP + Attack + Defense +
            SpecialAttack + SpecialDefense + Speed;

        public void Add(PokemonStat stat, int amount)
        {
            if (!Enum.IsDefined(stat))
                throw new ArgumentException("Atributo inválido.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (Total + amount > 510)
                throw new InvalidOperationException(
                    "O Pokémon não pode ultrapassar 510 EVs."
                );

            int current = GetValue(stat);

            if (current + amount > 252)
                throw new InvalidOperationException(
                    "Um atributo não pode ultrapassar 252 EVs."
                );

            switch (stat)
            {
                case PokemonStat.HP:
                    HP += amount;
                    break;

                case PokemonStat.Attack:
                    Attack += amount;
                    break;

                case PokemonStat.Defense:
                    Defense += amount;
                    break;

                case PokemonStat.SpecialAttack:
                    SpecialAttack += amount;
                    break;

                case PokemonStat.SpecialDefense:
                    SpecialDefense += amount;
                    break;

                case PokemonStat.Speed:
                    Speed += amount;
                    break;
            }
        }

        private int GetValueEv(PokemonStat stat)
        {
            return stat switch
            {
                PokemonStat.HP => HP,
                PokemonStat.Attack => Attack,
                PokemonStat.Defense => Defense,
                PokemonStat.SpecialAttack => SpecialAttack,
                PokemonStat.SpecialDefense => SpecialDefense,
                PokemonStat.Speed => Speed,
                _ => throw new ArgumentException("Atributo inválido.")
            };
        }
    }
}