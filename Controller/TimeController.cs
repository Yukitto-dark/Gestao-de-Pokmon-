using System.Collections.Generic;
using System.Linq;
using GestaoDePokemon.Data.Configuration;
using GestaoDePokemon.Data.Repositories.Contracts;

namespace GestaoDePokemon.Controller
{
    public class TimeController
    {
        private readonly IPartyRepository _party;
        private readonly IPokemonRepository _pokemons;

        public TimeController(
            IPartyRepository? party = null,
            IPokemonRepository? pokemons = null)
        {
            _party = party ?? AppServices.Party;
            _pokemons = pokemons ?? AppServices.Pokemons;
        }

        public IReadOnlyList<(int Id, bool Shiny)> ObterTime() => _party.Ids
            .Select(id => (id, _party.EhShiny(id)))
            .ToList();

        public string? ObterSpriteUrl(int id, bool shiny) =>
            _pokemons.ObterSpriteUrl(id, shiny);
    }
}
