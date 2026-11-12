using System;
using System.Collections.Generic;

namespace GestaoDePokemon.Data
{
    public class FavoritosMemoriaRepository : IFavoritosRepository
    {
        private readonly List<int> _ids = new();

        public int Limite => 6;

        public IReadOnlyList<int> Ids => _ids;

        public event Action? Alterado;

        public bool EhFavorito(int id) => _ids.Contains(id);

        public bool Alternar(int id, out bool limiteAtingido)
        {
            limiteAtingido = false;

            if (_ids.Remove(id))
            {
                Alterado?.Invoke();
                return false;
            }

            if (_ids.Count >= Limite)
            {
                limiteAtingido = true;
                return false;
            }

            _ids.Add(id);
            Alterado?.Invoke();
            return true;
        }
    }
}
