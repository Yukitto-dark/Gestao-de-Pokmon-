using System;
using System.Collections.Generic;
using GestaoDePokemon.model;

using GestaoDePokemon.Data.Filters;
namespace GestaoDePokemon.Data.Repositories.Contracts{
    public interface IPokemonRepository
    {
        Pokemon? Obter(int id);

        IReadOnlyList<Pokemon> ObterLinhaEvolutiva(int id) => Array.Empty<Pokemon>();

        /// <summary>
        /// Células da Box que correspondem aos critérios, em ordem de Pokédex: cada espécie, a forma
        /// "Normal" primeiro e depois todas as outras formas dela (ordem de cadastro).
        /// </summary>
        IReadOnlyList<PokemonBoxEntry> ObterEntradas(PokemonFilter filtro);

        // Sprite pequeno usado nas células da Box (mini); shiny seleciona miniShiny.
        string? ObterSpriteUrl(int id, bool shiny = false);

        // Sprite frontal usado no painel de detalhes; shiny seleciona frontShiny.
        string? ObterSpriteDetalheUrl(int id, bool shiny);

        // Sprite da caixa para uma forma específica (Box, Time, linha evolutiva, formas e megas).
        // Sem sprite próprio, usa o da espécie.
        string? ObterSpriteFormaUrl(int formId, bool shiny = false) => null;

        // Sprite frontal de uma forma específica (painel de detalhes da Box e tela de set).
        string? ObterSpriteDetalheFormaUrl(int formId, bool shiny) => null;

        // Nomes de habilidades disponíveis para o filtro da Box.
        IReadOnlyList<string> ObterHabilidades() => Array.Empty<string>();
    }
}
