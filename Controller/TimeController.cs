using System.Collections.Generic;
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

        public IReadOnlyList<MembroTime> ObterTime() => _party.Membros;

        // Ícone da Box da forma que está no time (Alola, Galar, Mega...), normal ou shiny.
        public string? ObterSpriteUrl(MembroTime membro) =>
            _pokemons.ObterSpriteFormaUrl(membro.FormId, membro.Shiny);
    }
}
