using System.Collections.Generic;

namespace GestaoDePokemon.model;

/// <summary>
/// Rota numerada pertencente a uma região.
/// </summary>
public class Route
{
    public int Id { get; set; }

    public int RegionId { get; set; }

    public int Number { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Identificador da localização na PokéAPI.</summary>
    public string PokeApiName { get; set; } = string.Empty;

    public int? PokeApiLocationId { get; set; }

    /// <summary>Versões em que a rota possui dados de encontro (JSON).</summary>
    public string GameVersions { get; set; } = "[]";

    public Region Region { get; set; } = null!;

    public List<RouteArea> Areas { get; set; } = new();
}
