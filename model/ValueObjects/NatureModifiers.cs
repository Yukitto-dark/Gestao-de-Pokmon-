namespace GestaoDePokemon.model
{
    public readonly record struct NatureModifiers(
        PokemonStat? IncreasedStat,
        PokemonStat? DecreasedStat
    );
}