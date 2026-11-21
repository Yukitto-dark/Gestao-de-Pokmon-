using System.Collections.Generic;

namespace GestaoDePokemon.Data.Repositories.Contracts
{
    public interface IPartyRepository
    {
        IReadOnlyList<int> Ids { get; }

        bool Adicionar(int id, bool shiny, out bool limiteAtingido);

        bool Remover(int id);

        bool EhNoTime(int id);

        bool EhShiny(int id);

        bool Alternar(int id, bool shiny, out bool limiteAtingido);
    }
}
