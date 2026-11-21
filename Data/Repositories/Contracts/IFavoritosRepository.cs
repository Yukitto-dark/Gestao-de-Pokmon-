using System.Collections.Generic;

namespace GestaoDePokemon.Data.Repositories.Contracts
{
    public interface IFavoritosRepository
    {
        IReadOnlyList<int> Ids { get; }

        bool Adicionar(int id, bool shiny);

        bool Remover(int id);

        bool EhFavorito(int id);

        bool EhShiny(int id);

        bool Alternar(int id, bool shiny);
    }
}
