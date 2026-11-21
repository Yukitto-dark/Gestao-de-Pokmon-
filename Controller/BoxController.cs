using System;
using System.Collections.Generic;
using GestaoDePokemon.Data.Configuration;
using GestaoDePokemon.Data.Filters;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.model;

namespace GestaoDePokemon.Controller
{
    public enum ResultadoFavorito
    {
        SemSelecao,
        Adicionado,
        Removido
    }

    public enum ResultadoParty
    {
        SemSelecao,
        Adicionado,
        Removido,
        LimiteAtingido
    }

    public class BoxController
    {
        public const int ItensPorPagina = 30;

        private readonly IFavoritosRepository _favoritos;
        private readonly IPartyRepository _party;
        private readonly IPokemonRepository _pokemons;
        private PokemonFilter _filtroAtual = new();
        private int _paginaAtual = 1;
        private IReadOnlyList<int>? _idsDaPagina;

        public BoxController(
            IPokemonRepository? pokemons = null,
            IFavoritosRepository? favoritos = null,
            IPartyRepository? party = null)
        {
            _pokemons = pokemons ?? AppServices.Pokemons;
            _favoritos = favoritos ?? AppServices.Favoritos;
            _party = party ?? AppServices.Party;
        }

        public int PaginaAtual => _paginaAtual;

        public int TotalResultados => _pokemons.Contar(_filtroAtual);

        public int TotalPaginas =>
            Math.Max(1, (TotalResultados + ItensPorPagina - 1) / ItensPorPagina);

        public int? IdSelecionado { get; private set; }

        public IReadOnlyList<int> ObterIdsDaPagina() =>
            _idsDaPagina ??= _pokemons.ObterIds(_filtroAtual, _paginaAtual, ItensPorPagina);

        public void AplicarFiltros(PokemonFilter filtro)
        {
            ArgumentNullException.ThrowIfNull(filtro);
            _filtroAtual = filtro;
            _paginaAtual = 1;
            _idsDaPagina = null;
            IdSelecionado = null;
        }

        public void LimparFiltros() => AplicarFiltros(new PokemonFilter());

        public int? Selecionar(int coluna, int linha)
        {
            if (coluna < 0 || coluna > 5 || linha < 0 || linha > 4)
                return null;

            int indice = linha * 6 + coluna;
            IReadOnlyList<int> ids = ObterIdsDaPagina();

            if (indice >= ids.Count)
            {
                IdSelecionado = null;
                return null;
            }

            IdSelecionado = ids[indice];
            return IdSelecionado;
        }

        public void IrParaPagina(int direcao)
        {
            int pagina = _paginaAtual + direcao;

            if (pagina > TotalPaginas)
                pagina = 1;
            else if (pagina < 1)
                pagina = TotalPaginas;

            _paginaAtual = pagina;
            _idsDaPagina = null;
            IdSelecionado = null;
        }

        public void LimparSelecao()
        {
            IdSelecionado = null;
        }

        public bool EhFavorito(int id) => _favoritos.EhFavorito(id);

        public bool EhNoTime(int id) => _party.EhNoTime(id);

        public bool EhShiny(int id) =>
            _favoritos.EhShiny(id) || _party.EhShiny(id);

        public Pokemon? ObterPokemon(int id) => _pokemons.Obter(id);

        public IReadOnlyList<Pokemon> ObterLinhaEvolutiva(int id) =>
            _pokemons.ObterLinhaEvolutiva(id);

        public string? ObterSpriteUrl(int id, bool shiny = false) =>
            _pokemons.ObterSpriteUrl(id, shiny);

        public string? ObterSpriteDetalheUrl(int id, bool shiny) =>
            _pokemons.ObterSpriteDetalheUrl(id, shiny);

        public string? ObterSpriteFormaUrl(int formId) =>
            _pokemons.ObterSpriteFormaUrl(formId);

        public IReadOnlyList<string> ObterHabilidades() =>
            _pokemons.ObterHabilidades();

        public ResultadoFavorito AlternarFavorito(bool shiny)
        {
            if (IdSelecionado is not int id)
                return ResultadoFavorito.SemSelecao;

            bool adicionado = _favoritos.Alternar(id, shiny);
            _idsDaPagina = null;

            return adicionado
                ? ResultadoFavorito.Adicionado
                : ResultadoFavorito.Removido;
        }

        public ResultadoParty AlternarParty(bool shiny, out bool limiteAtingido)
        {
            limiteAtingido = false;

            if (IdSelecionado is not int id)
                return ResultadoParty.SemSelecao;

            bool adicionado = _party.Alternar(id, shiny, out limiteAtingido);
            _idsDaPagina = null;

            if (limiteAtingido)
                return ResultadoParty.LimiteAtingido;

            return adicionado
                ? ResultadoParty.Adicionado
                : ResultadoParty.Removido;
        }
    }
}
