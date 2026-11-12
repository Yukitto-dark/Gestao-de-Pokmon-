using System.Collections.Generic;
using GestaoDePokemon.Data;

namespace GestaoDePokemon.Controller
{
    public class FavoritosController
    {
        private readonly IFavoritosRepository _favoritos;
        private readonly IPokemonRepository _pokemons;

        public FavoritosController(
            IFavoritosRepository? favoritos = null,
            IPokemonRepository? pokemons = null)
        {
            _favoritos = favoritos ?? AppServices.Favoritos;
            _pokemons = pokemons ?? AppServices.Pokemons;
        }

        public IReadOnlyList<int> ObterFavoritos() => _favoritos.Ids;

        public string? ObterSpriteUrl(int id) => _pokemons.ObterSpriteUrl(id);
    }
}
