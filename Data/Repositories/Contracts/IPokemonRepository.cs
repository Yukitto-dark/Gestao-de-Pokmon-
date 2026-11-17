using System.Collections.Generic;
using GestaoDePokemon.model;

using GestaoDePokemon.Data.Filters;
namespace GestaoDePokemon.Data.Repositories.Contracts{
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

        /// <summary>
        /// Conta as células da Box que correspondem aos critérios: uma por forma (a Normal e todas as
        /// alternativas). A implementação padrão conta só as espécies.
        /// </summary>
        int ContarEntradas(PokemonFilter filtro) => Contar(filtro);

        /// <summary>
        /// Retorna uma página da Box em ordem de Pokédex: cada espécie, a forma Normal primeiro e depois todas as
        /// suas outras formas. A implementação padrão devolve só a forma Normal de cada espécie.
        /// </summary>
        IReadOnlyList<PokemonBoxEntry> ObterEntradas(PokemonFilter filtro, int pagina, int tamanho)
        {
            var entradas = new List<PokemonBoxEntry>();

            foreach (int id in ObterIds(filtro, pagina, tamanho))
                entradas.Add(new PokemonBoxEntry(id, id, $"#{id}", "Normal"));

            return entradas;
        }

        // Sprite pequeno de uma célula da Box (a forma dela). Sem sprite próprio, usa o da espécie.
        string? ObterSpriteUrl(PokemonBoxEntry entrada) => ObterSpriteUrl(entrada.NationalDexNumber);

        // Sprite frontal de uma célula da Box (a forma dela) no painel de detalhes.
        string? ObterSpriteDetalheUrl(PokemonBoxEntry entrada, bool shiny) =>
            ObterSpriteDetalheUrl(entrada.NationalDexNumber, shiny);

        // Sprite pequeno usado nas células da Box (mini).
        string? ObterSpriteUrl(int id);

        // Sprite frontal usado no painel de detalhes; shiny seleciona frontShiny.
        string? ObterSpriteDetalheUrl(int id, bool shiny);
    }
}
