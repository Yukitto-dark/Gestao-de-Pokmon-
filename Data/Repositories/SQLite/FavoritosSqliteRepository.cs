using System;
using System.Collections.Generic;
using System.Linq;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data.Repositories.SQLite;

public sealed class FavoritosSqliteRepository : IFavoritosRepository
{
    public int Limite => 6;

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

    public event Action? Alterado;

    public bool EhFavorito(int id)
    {
        using var db = new AppDbContext();

        return db.Favorites
            .AsNoTracking()
            .Any(favorite => favorite.PokemonId == id);
    }

    public bool Alternar(int id, out bool limiteAtingido)
    {
        limiteAtingido = false;

        using var db = new AppDbContext();

        PokemonFavorite? existente = db.Favorites
            .SingleOrDefault(
                favorite => favorite.PokemonId == id);

        if (existente is not null)
        {
            db.Favorites.Remove(existente);
            db.SaveChanges();

            Alterado?.Invoke();
            return false;
        }

        bool pokemonExiste =
            db.Pokemons.Any(pokemon => pokemon.Id == id);

        if (!pokemonExiste)
            return false;

        if (db.Favorites.Count() >= Limite)
        {
            limiteAtingido = true;
            return false;
        }

        db.Favorites.Add(new PokemonFavorite
        {
            PokemonId = id,
            AddedAt = DateTime.UtcNow
        });

        db.SaveChanges();

        Alterado?.Invoke();
        return true;
    }
}
