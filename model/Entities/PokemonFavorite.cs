using System;

namespace GestaoDePokemon.model
{
    public class PokemonFavorite
    {
        public int Id { get; set; }

        public int PokemonId { get; set; }

        public Pokemon Pokemon { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
