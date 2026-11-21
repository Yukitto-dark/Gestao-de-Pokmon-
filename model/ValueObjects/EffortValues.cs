using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDePokemon.model
{
    public class EffortValues
    {
        // Sem mínimo artificial: na build o valor exibido é o base stat da espécie mais os EVs investidos,
        // então o próprio base stat é o piso (EV 0). Antes era 10 para todas as espécies.
        public const int MinPerStat = 0;
        public const int MaxPerStat = 252;
        public const int MaxTotal = 510;

        public static readonly IReadOnlyList<PokemonStat> Estatisticas = new[]
        {
            PokemonStat.HP, PokemonStat.Attack, PokemonStat.Defense,
            PokemonStat.SpecialAttack, PokemonStat.SpecialDefense, PokemonStat.Speed
        };

        public int HP { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int SpecialAttack { get; private set; }
        public int SpecialDefense { get; private set; }
        public int Speed { get; private set; }

        public int Total =>
            HP + Attack + Defense + SpecialAttack + SpecialDefense + Speed;

        public bool GarantirMinimo()
        {
            int[] valores = Estatisticas
                .Select(stat => Math.Max(GetValue(stat), MinPerStat))
                .ToArray();

            int excesso = valores.Sum() - MaxTotal;
            while (excesso > 0)
            {
                int indice = -1;
                for (int i = 0; i < valores.Length; i++)
                {
                    if (valores[i] > MinPerStat &&
                        (indice < 0 || valores[i] > valores[indice]))
                        indice = i;
                }

                if (indice < 0)
                    break;

                valores[indice]--;
                excesso--;
            }

            if (valores.SequenceEqual(Estatisticas.Select(GetValue)))
                return false;

            Reset();
            for (int i = 0; i < Estatisticas.Count; i++)
                Set(Estatisticas[i], valores[i]);

            return true;
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
