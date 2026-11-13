using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.Data.Repositories.SQLite;
using GestaoDePokemon.Data.Repositories.InMemory;
namespace GestaoDePokemon.Data.Configuration
{
    /// <summary>
    /// Ponto central de composição dos repositórios usados pelos Controllers.
    /// Os dados de Pokémon e favoritos agora são persistidos no SQLite.
    /// Rotas continuam em memória até existir uma entidade/tabela de rotas.
    /// </summary>
    public static class AppServices
    {
        public static IFavoritosRepository Favoritos { get; } =
            new FavoritosSqliteRepository();

        public static IPokemonRepository Pokemons { get; } =
            new PokemonSqliteRepository();

        public static IRotaRepository Rotas { get; } =
            new RotaMemoriaRepository();
    }
}
