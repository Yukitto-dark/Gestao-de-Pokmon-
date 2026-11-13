using System.Collections.Generic;

namespace GestaoDePokemon.model;

/// <summary>
/// Região geográfica cadastrada na Pokédex.
/// </summary>
public class Region
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Generation { get; set; }

    /// <summary>Descrição geral da região.</summary>
    public string? Description { get; set; }

    /// <summary>Lista JSON das versões principais associadas à região.</summary>
    public string MainGames { get; set; } = "[]";

    public List<Route> Routes { get; set; } = new();
}
