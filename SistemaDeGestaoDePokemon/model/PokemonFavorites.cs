using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDePokemon.model
{
    public class PokemonFavorites
    {
        private readonly List<Pokemon> _favorites = new();

        public IReadOnlyList<Pokemon> Favorites =>
            _favorites.AsReadOnly();

        public int Count => _favorites.Count;

        public bool Add(Pokemon pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));

            if (_favorites.Any(p => p.Id == pokemon.Id))
                return false;

            _favorites.Add(pokemon);

            return true;
        }

        public bool Remove(Pokemon pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));

            Pokemon? favorite = _favorites.FirstOrDefault(
                p => p.Id == pokemon.Id
            );

            if (favorite == null)
                return false;

            return _favorites.Remove(favorite);
        }

        public bool IsFavorite(Pokemon pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));

            return _favorites.Any(p => p.Id == pokemon.Id);
        }
    }
}