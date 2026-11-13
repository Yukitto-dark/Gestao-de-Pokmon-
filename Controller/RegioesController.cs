using System.Collections.Generic;
using GestaoDePokemon.Data.Configuration;
using GestaoDePokemon.Data.Filters;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.Data.Repositories.InMemory;
using GestaoDePokemon.Data.Repositories.SQLite;
using GestaoDePokemon.model;

namespace GestaoDePokemon.Controller
{
    public class RegioesController
    {
        private readonly IRotaRepository _rotas;
        private readonly IPokemonRepository _pokemons;

        public RegioesController(
            IRotaRepository? rotas = null,
            IPokemonRepository? pokemons = null)
        {
            _rotas = rotas ?? AppServices.Rotas;
            _pokemons = pokemons ?? AppServices.Pokemons;
        }

        public string RegiaoAtual { get; private set; } = "Kanto";

        public void SelecionarRegiao(string regiao)
        {
            if (!string.IsNullOrWhiteSpace(regiao))
                RegiaoAtual = regiao;
        }

        public IReadOnlyList<int> ObterRotas(string regiao)
        {
            SelecionarRegiao(regiao);
            return _rotas.ObterRotas(RegiaoAtual);
        }

        public RotaInfo? ObterRota(int numero) =>
            _rotas.ObterRota(RegiaoAtual, numero);

        public IReadOnlyList<string> ObterSpritesDaRota(RotaInfo rota)
        {
            var sprites = new List<string>();

            foreach (int id in rota.PokemonIds)
            {
                string? url = _pokemons.ObterSpriteUrl(id);

                if (url != null)
                    sprites.Add(url);
            }

            return sprites;
        }
    }
}
