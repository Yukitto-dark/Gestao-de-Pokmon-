using System;
using System.Collections.Generic;
using GestaoDePokemon.Data;

namespace GestaoDePokemon.Controller
{
    public enum ResultadoFavorito
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
        private readonly IPokemonRepository _pokemons;

        private int _paginaAtual = 1;

        public BoxController(
            IPokemonRepository? pokemons = null,
            IFavoritosRepository? favoritos = null)
        {
            _pokemons = pokemons ?? AppServices.Pokemons;
            _favoritos = favoritos ?? AppServices.Favoritos;
        }

        public int PaginaAtual => _paginaAtual;

        public int TotalPaginas =>
            Math.Max(1, (_pokemons.Total + ItensPorPagina - 1) / ItensPorPagina);

        public int? IdSelecionado { get; private set; }

        public IReadOnlyList<int> ObterIdsDaPagina() =>
            _pokemons.ObterIds(_paginaAtual, ItensPorPagina);

        public int? Selecionar(int coluna, int linha)
        {
            if (coluna < 0 || coluna > 5 || linha < 0 || linha > 4)
                return null;

            int indice = linha * 6 + coluna;
            IdSelecionado = (_paginaAtual - 1) * ItensPorPagina + indice + 1;
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
            IdSelecionado = null;
        }

        public void LimparSelecao()
        {
            IdSelecionado = null;
        }

        public bool EhFavorito(int id) => _favoritos.EhFavorito(id);

        public string? ObterSpriteUrl(int id) => _pokemons.ObterSpriteUrl(id);

        public ResultadoFavorito AlternarFavorito(out bool limiteAtingido)
        {
            limiteAtingido = false;

            if (IdSelecionado is not int id)
                return ResultadoFavorito.SemSelecao;

            bool adicionado = _favoritos.Alternar(id, out limiteAtingido);

            if (limiteAtingido)
                return ResultadoFavorito.LimiteAtingido;

            return adicionado
                ? ResultadoFavorito.Adicionado
                : ResultadoFavorito.Removido;
        }
    }
}
