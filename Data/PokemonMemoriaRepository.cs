using System;
using System.Collections.Generic;
using GestaoDePokemon.model;

namespace GestaoDePokemon.Data
{
    public class PokemonMemoriaRepository : IPokemonRepository
    {
        private const int TotalPokemons = 70 * 30;

        public int Total => TotalPokemons;

        public Pokemon? Obter(int id) => null;

        public IReadOnlyList<int> ObterIds(int pagina, int tamanho)
        {
            if (pagina < 1 || tamanho < 1)
                return Array.Empty<int>();

            int idBase = (pagina - 1) * tamanho + 1;
            var ids = new int[tamanho];

            for (int i = 0; i < tamanho; i++)
                ids[i] = idBase + i;

            return ids;
        }

        public string? ObterSpriteUrl(int id) => null;
    }
}
