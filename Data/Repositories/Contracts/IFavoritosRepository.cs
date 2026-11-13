using System;
using System.Collections.Generic;

namespace GestaoDePokemon.Data.Repositories.Contracts
{
    public interface IFavoritosRepository
    {
        int Limite { get; }

        IReadOnlyList<int> Ids { get; }

        event Action? Alterado;

        bool Adicionar(int id, out bool limiteAtingido);

        bool Remover(int id);

        bool EhFavorito(int id);

        bool Alternar(int id, out bool limiteAtingido);
    }
}
