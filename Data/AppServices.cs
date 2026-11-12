namespace GestaoDePokemon.Data
{
    public static class AppServices
    {
        public static IFavoritosRepository Favoritos { get; } =
            new FavoritosMemoriaRepository();

        public static IPokemonRepository Pokemons { get; } =
            new PokemonMemoriaRepository();

        public static IRotaRepository Rotas { get; } =
            new RotaMemoriaRepository();
    }
}
