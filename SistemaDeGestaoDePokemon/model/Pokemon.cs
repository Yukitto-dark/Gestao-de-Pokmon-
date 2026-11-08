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

        public List<EggGroup> EggGroups { get; set; } = new();

        public List<PokemonForm> Forms { get; set; } = new();
        
        public List<LearnableMove> Learnset { get; set; } = new();
    }
}