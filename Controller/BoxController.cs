using System;
using System.Collections.Generic;
using System.Linq;
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

        // Todas as células que passam no filtro atual, já na ordem da Box. A lista é pequena
        // (uma entrada por forma) e só é refeita quando o filtro ou o resultado dele muda.
        private IReadOnlyList<PokemonBoxEntry>? _entradas;

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

        // Cada resultado é uma forma (a Normal e todas as alternativas) de uma espécie, em ordem de Pokédex.
        public int TotalResultados => Entradas.Count;

        public int TotalPaginas =>
            Math.Max(1, (TotalResultados + ItensPorPagina - 1) / ItensPorPagina);

        // Célula selecionada da Box (uma forma de uma espécie).
        public PokemonBoxEntry? EntradaSelecionada { get; private set; }

        // Número da Pokédex da espécie selecionada: os favoritos são por espécie.
        public int? IdSelecionado => EntradaSelecionada?.NationalDexNumber;

        private IReadOnlyList<PokemonBoxEntry> Entradas =>
            _entradas ??= _pokemons.ObterEntradas(_filtroAtual);

        public IReadOnlyList<PokemonBoxEntry> ObterEntradasDaPagina()
        {
            // Remover o último favorito/membro de uma página com o filtro ativo pode encurtar a lista.
            if (_paginaAtual > TotalPaginas)
                _paginaAtual = TotalPaginas;

            return Entradas
                .Skip((_paginaAtual - 1) * ItensPorPagina)
                .Take(ItensPorPagina)
                .ToList();
        }

        public void AplicarFiltros(PokemonFilter filtro)
        {
            ArgumentNullException.ThrowIfNull(filtro);
            _filtroAtual = filtro;
            _paginaAtual = 1;
            _entradas = null;
            EntradaSelecionada = null;
        }

        public void LimparFiltros() => AplicarFiltros(new PokemonFilter());

        public PokemonBoxEntry? Selecionar(int coluna, int linha)
        {
            if (coluna < 0 || coluna > 5 || linha < 0 || linha > 4)
                return null;

            int indice = linha * 6 + coluna;
            IReadOnlyList<PokemonBoxEntry> entradas = ObterEntradasDaPagina();

            if (indice >= entradas.Count)
            {
                EntradaSelecionada = null;
                return null;
            }

            EntradaSelecionada = entradas[indice];
            return EntradaSelecionada;
        }

        public void IrParaPagina(int direcao)
        {
            int pagina = _paginaAtual + direcao;

            if (pagina > TotalPaginas)
                pagina = 1;
            else if (pagina < 1)
                pagina = TotalPaginas;

            _paginaAtual = pagina;
            EntradaSelecionada = null;
        }

        public void LimparSelecao()
        {
            EntradaSelecionada = null;
        }

        public bool EhFavorito(int id) => _favoritos.EhFavorito(id);

        public bool EhNoTime(PokemonBoxEntry entrada) => _party.EhNoTime(entrada.FormId);

        // Sprite salvo como shiny: no time vale a forma; nos favoritos, a espécie.
        public bool EhShiny(PokemonBoxEntry entrada) =>
            _party.EhShiny(entrada.FormId) || _favoritos.EhShiny(entrada.NationalDexNumber);

        public Pokemon? ObterPokemon(int id) => _pokemons.Obter(id);

        public IReadOnlyList<Pokemon> ObterLinhaEvolutiva(int id) =>
            _pokemons.ObterLinhaEvolutiva(id);

        public string? ObterSpriteUrl(PokemonBoxEntry entrada) =>
            _pokemons.ObterSpriteFormaUrl(entrada.FormId);

        public string? ObterSpriteDetalheUrl(PokemonBoxEntry entrada, bool shiny) =>
            _pokemons.ObterSpriteDetalheFormaUrl(entrada.FormId, shiny);

        public string? ObterSpriteFormaUrl(int formId) =>
            _pokemons.ObterSpriteFormaUrl(formId);

        public IReadOnlyList<string> ObterHabilidades() =>
            _pokemons.ObterHabilidades();

        public ResultadoFavorito AlternarFavorito(bool shiny)
        {
            if (IdSelecionado is not int id)
                return ResultadoFavorito.SemSelecao;

            bool adicionado = _favoritos.Alternar(id, shiny);

            if (_filtroAtual.OnlyFavorites)
                _entradas = null;

            return adicionado
                ? ResultadoFavorito.Adicionado
                : ResultadoFavorito.Removido;
        }

        public ResultadoParty AlternarParty(bool shiny, out bool limiteAtingido)
        {
            limiteAtingido = false;

            if (EntradaSelecionada is not PokemonBoxEntry entrada)
                return ResultadoParty.SemSelecao;

            bool adicionado = _party.Alternar(entrada.FormId, shiny, out limiteAtingido);

            if (_filtroAtual.OnlyParty)
                _entradas = null;

            if (limiteAtingido)
                return ResultadoParty.LimiteAtingido;

            return adicionado
                ? ResultadoParty.Adicionado
                : ResultadoParty.Removido;
        }
    }
}
