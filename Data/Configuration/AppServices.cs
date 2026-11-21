using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.Data.Repositories.SQLite;
namespace GestaoDePokemon.Data.Configuration
{
    /// <summary>
    /// Ponto central de composição dos repositórios usados pelos Controllers.
    /// Pokémon, favoritos e rotas são persistidos no SQLite.
    /// </summary>
    public static class AppServices
    {
        public static IFavoritosRepository Favoritos { get; } =
            new FavoritosSqliteRepository();

        public static IPartyRepository Party { get; } =
            new PartySqliteRepository();

        public static IPokemonRepository Pokemons { get; } =
            new PokemonSqliteRepository();

        public static IRotaRepository Rotas { get; } =
            new RotaSqliteRepository();
    }
}
