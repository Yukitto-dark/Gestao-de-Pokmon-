using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public class Pokemon
    {
        public int Id { get; set; }

        public int NationalDexNumber { get; set; }

        public int EvolutionLineId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Generation { get; set; }

        public string Category { get; set; } = string.Empty;

        private readonly List<EggGroup> _eggGroups = new();

        public IReadOnlyList<EggGroup> EggGroups =>
            _eggGroups.AsReadOnly();
        public List<PokemonForm> Forms { get; set; } = new();
        
        public List<LearnableMove> Learnset { get; set; } = new();

        public void AddEggGroup(EggGroup group)
        {
            if (!Enum.IsDefined(group))
                throw new ArgumentException("Egg Group inválido.");

            if (_eggGroups.Contains(group))
                throw new ArgumentException(
                    "Esse Egg Group já foi adicionado."
                );

            bool exclusiveGroup =
                group == EggGroup.Ditto ||
                group == EggGroup.Undiscovered;

            bool alreadyHasExclusiveGroup =
                _eggGroups.Contains(EggGroup.Ditto) ||
                _eggGroups.Contains(EggGroup.Undiscovered);

            if (_eggGroups.Count > 0 &&
                (exclusiveGroup || alreadyHasExclusiveGroup))
            {
                throw new InvalidOperationException(
                    "Ditto e Undiscovered não podem ser combinados com outros Egg Groups."
                );
            }

            if (_eggGroups.Count >= 2)
                throw new InvalidOperationException(
                    "Um Pokémon não pode possuir mais de dois Egg Groups."
                );

            _eggGroups.Add(group);
        }
    }
}
