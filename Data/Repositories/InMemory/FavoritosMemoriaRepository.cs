using System;
using System.Collections.Generic;
using GestaoDePokemon.Data.Repositories.Contracts;

namespace GestaoDePokemon.Data.Repositories.InMemory
{
    public sealed class FavoritosMemoriaRepository : IFavoritosRepository
    {
        private const int MaximoFavoritos = 6;

        private readonly List<int> _ids = new();

        public int Limite => MaximoFavoritos;

        public IReadOnlyList<int> Ids => _ids.AsReadOnly();

        public event Action? Alterado;

        public bool EhFavorito(int id)
        {
            return _ids.Contains(id);
        }

        public bool Adicionar(int id, out bool limiteAtingido)
        {
            limiteAtingido = false;

            if (id <= 0 || EhFavorito(id))
                return false;

            if (_ids.Count >= Limite)
            {
                limiteAtingido = true;
                return false;
            }

            _ids.Add(id);
            Alterado?.Invoke();

            return true;
        }

        public bool Remover(int id)
        {
            if (!_ids.Remove(id))
                return false;

            Alterado?.Invoke();
            return true;
        }

        public bool Alternar(int id, out bool limiteAtingido)
        {
            limiteAtingido = false;

            if (Remover(id))
                return false;

            return Adicionar(id, out limiteAtingido);
        }
    }
}
