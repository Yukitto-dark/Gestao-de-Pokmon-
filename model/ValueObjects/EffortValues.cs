using System;

namespace GestaoDePokemon.model
{
    public class EffortValues
    {
        public const int MaxPerStat = 252;
        public const int MaxTotal = 510;

        public int HP { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int SpecialAttack { get; private set; }
        public int SpecialDefense { get; private set; }
        public int Speed { get; private set; }

        public int Total =>
            HP + Attack + Defense + SpecialAttack + SpecialDefense + Speed;

        // Adiciona EVs à distribuição atual.
        public void Add(PokemonStat stat, int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "A quantidade deve ser positiva.");

            ValidateStat(stat);

            int current = GetValue(stat);
            if (amount > MaxPerStat - current)
                throw new InvalidOperationException(
                    $"Um atributo não pode ultrapassar {MaxPerStat} EVs.");

            Set(stat, current + amount);
        }

        // Define o valor exato de um atributo, permitindo também reduzi-lo ou zerá-lo.
        public void Set(PokemonStat stat, int value)
        {
            ValidateStat(stat);

            if (value < 0 || value > MaxPerStat)
                throw new ArgumentOutOfRangeException(
                    nameof(value), $"Cada atributo deve possuir entre 0 e {MaxPerStat} EVs.");

            int newTotal = Total - GetValue(stat) + value;
            if (newTotal > MaxTotal)
                throw new InvalidOperationException(
                    $"O total de EVs não pode ultrapassar {MaxTotal}.");

            SetValue(stat, value);
        }

        public void Reset()
        {
            HP = 0;
            Attack = 0;
            Defense = 0;
            SpecialAttack = 0;
            SpecialDefense = 0;
            Speed = 0;
        }

        public int GetValue(PokemonStat stat)
        {
            ValidateStat(stat);

            return stat switch
            {
                PokemonStat.HP => HP,
                PokemonStat.Attack => Attack,
                PokemonStat.Defense => Defense,
                PokemonStat.SpecialAttack => SpecialAttack,
                PokemonStat.SpecialDefense => SpecialDefense,
                PokemonStat.Speed => Speed,
                _ => throw new ArgumentOutOfRangeException(nameof(stat))
            };
        }

        private void SetValue(PokemonStat stat, int value)
        {
            switch (stat)
            {
                case PokemonStat.HP: HP = value; break;
                case PokemonStat.Attack: Attack = value; break;
                case PokemonStat.Defense: Defense = value; break;
                case PokemonStat.SpecialAttack: SpecialAttack = value; break;
                case PokemonStat.SpecialDefense: SpecialDefense = value; break;
                case PokemonStat.Speed: Speed = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }

        private static void ValidateStat(PokemonStat stat)
        {
            if (!Enum.IsDefined(stat))
                throw new ArgumentException("Atributo inválido.", nameof(stat));
        }
    }
}
