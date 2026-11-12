using System.Collections.Generic;
using GestaoDePokemon.model;

namespace GestaoDePokemon.Data
{
    public interface IPokemonRepository
    {
        int Total { get; }

        Pokemon? Obter(int id);

        IReadOnlyList<int> ObterIds(int pagina, int tamanho);

        string? ObterSpriteUrl(int id);
    }
}
