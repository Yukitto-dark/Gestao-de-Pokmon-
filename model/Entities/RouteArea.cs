using System.Collections.Generic;

namespace GestaoDePokemon.model;

/// <summary>
/// Subárea de uma rota, conforme a divisão de áreas da PokéAPI.
/// </summary>
public class RouteArea
{
    public int Id { get; set; }

    public int RouteId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string PokeApiName { get; set; } = string.Empty;

    public int? PokeApiAreaId { get; set; }

    public Route Route { get; set; } = null!;

    public List<RoutePokemonEncounter> Encounters { get; set; } = new();
}
