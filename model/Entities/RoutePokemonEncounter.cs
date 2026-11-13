namespace GestaoDePokemon.model;

/// <summary>
/// Registra um Pokémon encontrado em uma área de rota, sem simular encontros.
/// </summary>
public class RoutePokemonEncounter
{
    public int Id { get; set; }

    public int RouteAreaId { get; set; }

    public int PokemonId { get; set; }

    public string EncounterMethod { get; set; } = string.Empty;

    public int MinimumLevel { get; set; }

    public int MaximumLevel { get; set; }

    /// <summary>Percentual informado pela fonte para este detalhe de encontro.</summary>
    public int ChancePercent { get; set; }

    /// <summary>Versão específica do jogo, por exemplo ruby ou emerald.</summary>
    public string GameVersion { get; set; } = string.Empty;

    /// <summary>Condições exigidas pelo encontro, armazenadas como JSON.</summary>
    public string ConditionsJson { get; set; } = "[]";

    public RouteArea RouteArea { get; set; } = null!;

    public Pokemon Pokemon { get; set; } = null!;
}
