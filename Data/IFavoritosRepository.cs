using System;
using System.Collections.Generic;

namespace GestaoDePokemon.Data
{
    public interface IFavoritosRepository
    {
        int Limite { get; }

        IReadOnlyList<int> Ids { get; }

        event Action? Alterado;

        bool EhFavorito(int id);

        bool Alternar(int id, out bool limiteAtingido);
    }
}
