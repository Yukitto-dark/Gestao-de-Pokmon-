using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDePokemon.model
{
    public class PokemonFavorites
    {
        private readonly List<Pokemon> _favorites = new();

        public IReadOnlyList<Pokemon> Favorites => _favorites.AsReadOnly();

        public int Count => _favorites.Count;

        public bool Add(Pokemon pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);

            if (_favorites.Any(existing => SamePokemon(existing, pokemon)))
                return false;

            _favorites.Add(pokemon);
            return true;
        }

        public bool Remove(Pokemon pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);

            Pokemon? favorite = _favorites.FirstOrDefault(
                existing => SamePokemon(existing, pokemon));

            return favorite != null && _favorites.Remove(favorite);
        }

        public bool IsFavorite(Pokemon pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);
            return _favorites.Any(existing => SamePokemon(existing, pokemon));
        }

        private static bool SamePokemon(Pokemon a, Pokemon b) =>
            ReferenceEquals(a, b) || (a.Id > 0 && b.Id > 0 && a.Id == b.Id);
    }
}
