using System;
using System.Collections.Generic;
using System.Linq;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data.Repositories.SQLite
{
    public sealed class FavoritosSqliteRepository : IFavoritosRepository
    {
        public IReadOnlyList<int> Ids
        {
            get
            {
                using var db = new AppDbContext();

                return db.Favorites
                    .AsNoTracking()
                    .OrderBy(favorite => favorite.AddedAt)
                    .ThenBy(favorite => favorite.Id)
                    .Select(favorite => favorite.PokemonId)
                    .ToList();
            }
        }

        public bool EhFavorito(int id)
        {
            using var db = new AppDbContext();

            return db.Favorites
                .AsNoTracking()
                .Any(favorite => favorite.PokemonId == id);
        }

        public bool EhShiny(int id)
        {
            using var db = new AppDbContext();

            return db.Favorites
                .AsNoTracking()
                .Any(favorite => favorite.PokemonId == id && favorite.IsShiny);
        }

        public bool Adicionar(int id, bool shiny)
        {
            using var db = new AppDbContext();

            return TentarAdicionar(db, id, shiny);
        }

        public bool Remover(int id)
        {
            using var db = new AppDbContext();

            PokemonFavorite? existente = db.Favorites
                .SingleOrDefault(favorite => favorite.PokemonId == id);

            if (existente is null)
                return false;

            db.Favorites.Remove(existente);
            db.SaveChanges();

            return true;
        }

        public bool Alternar(int id, bool shiny)
        {
            using var db = new AppDbContext();

            PokemonFavorite? existente = db.Favorites
                .SingleOrDefault(favorite => favorite.PokemonId == id);

            if (existente is not null)
            {
                db.Favorites.Remove(existente);
                db.SaveChanges();

                    return false;
            }

            return TentarAdicionar(db, id, shiny);
        }

        private bool TentarAdicionar(AppDbContext db, int id, bool shiny)
        {
            if (id <= 0)
                return false;

            if (db.Favorites.Any(favorite => favorite.PokemonId == id))
                return false;

            bool pokemonExiste = db.Pokemons
                .Any(pokemon => pokemon.Id == id);

            if (!pokemonExiste)
                return false;

            db.Favorites.Add(new PokemonFavorite
            {
                PokemonId = id,
                IsShiny = shiny,
                AddedAt = DateTime.UtcNow
            });

            db.SaveChanges();

            return true;
        }
    }
}
