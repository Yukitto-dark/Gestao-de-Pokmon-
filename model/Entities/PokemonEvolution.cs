using System;
using System.Linq;
namespace GestaoDePokemon.model
{
    public class PokemonEvolution
    {
        public int Id { get; set; }

        // Espécie de origem
        public int FromPokemonId { get; set; }

        public Pokemon FromPokemon { get; set; } = null!;

        // Espécie de destino
        public int ToPokemonId { get; set; }

        public Pokemon ToPokemon { get; set; } = null!;

        // Método de evolução
        public EvolutionMethod Method { get; set; }

        public int? RequiredLevel { get; set; }

        public string? RequiredItem { get; set; }

        public EvolutionTime? RequiredTime { get; set; }

        public int? MinimumFriendship { get; set; }

        public int? MinimumBeauty { get; set; }

        public PokemonGender? RequiredGender { get; set; }

        public string? RequiredLocation { get; set; }

        public int? RequiredMoveId { get; set; }

        public PokemonMove? RequiredMove { get; set; }

        public string? GameVersion { get; set; }
    }
}