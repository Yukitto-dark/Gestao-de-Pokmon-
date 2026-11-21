using System;
using System.Collections.Generic;
using GestaoDePokemon.model;

using GestaoDePokemon.Data.Filters;
namespace GestaoDePokemon.Data.Repositories.Contracts{
    public interface IPokemonRepository
    {
        Pokemon? Obter(int id);

        IReadOnlyList<Pokemon> ObterLinhaEvolutiva(int id) => Array.Empty<Pokemon>();

        /// <summary>Conta os Pokémon que correspondem aos critérios informados.</summary>
        int Contar(PokemonFilter filtro);

        /// <summary>Retorna uma página dos Pokémon que correspondem aos critérios.</summary>
        IReadOnlyList<int> ObterIds(PokemonFilter filtro, int pagina, int tamanho);

        // Sprite pequeno usado nas células da Box (mini); shiny seleciona miniShiny.
        string? ObterSpriteUrl(int id, bool shiny = false);

        // Sprite frontal usado no painel de detalhes; shiny seleciona frontShiny.
        string? ObterSpriteDetalheUrl(int id, bool shiny);

        // Sprite da caixa para uma forma específica (linha evolutiva, formas e megas).
        string? ObterSpriteFormaUrl(int formId) => null;

        // Nomes de habilidades disponíveis para o filtro da Box.
        IReadOnlyList<string> ObterHabilidades() => Array.Empty<string>();
    }
}
