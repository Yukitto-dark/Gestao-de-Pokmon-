using System;
using System.Collections.Generic;
using GestaoDePokemon.model;

namespace GestaoDePokemon.Data.Filters{
    /// <summary>
    /// Critérios combináveis para filtrar a Pokédex.
    /// Uma coleção de regiões nula ou vazia significa que nenhuma restrição
    /// regional deve ser aplicada. Uma coleção com itens limita os resultados
    /// às regiões selecionadas (operação OU entre elas).
    /// </summary>
    public sealed class PokemonFilter
    {
        public IReadOnlyCollection<PokemonType> Types { get; init; } = Array.Empty<PokemonType>();

        public IReadOnlyCollection<int>? OriginRegionIds { get; init; }

        public bool OnlyFavorites { get; init; }

        public int? NationalDexNumber { get; init; }

        public string? NameContains { get; init; }
    }
}
