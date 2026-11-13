using System.Collections.Generic;
using GestaoDePokemon.model;

namespace GestaoDePokemon.Data
{
    public interface IPokemonRepository
    {
        int Total { get; }

        Pokemon? Obter(int id);

        IReadOnlyList<int> ObterIds(int pagina, int tamanho);

        /// <summary>Conta os Pokémon que correspondem aos critérios informados.</summary>
        int Contar(PokemonFilter filtro) => Total;

        /// <summary>Retorna uma página dos Pokémon que correspondem aos critérios.</summary>
        IReadOnlyList<int> ObterIds(PokemonFilter filtro, int pagina, int tamanho) =>
            ObterIds(pagina, tamanho);

        // Sprite pequeno usado nas células da Box (mini).
        string? ObterSpriteUrl(int id);

        // Sprite frontal usado no painel de detalhes; shiny seleciona frontShiny.
        string? ObterSpriteDetalheUrl(int id, bool shiny);
    }
}
