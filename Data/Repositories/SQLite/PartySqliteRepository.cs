using System;
using System.Collections.Generic;
using System.Linq;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data.Repositories.SQLite
{
    public sealed class PartySqliteRepository : IPartyRepository
    {
        private const int MaximoMembros = 6;

        public IReadOnlyList<int> Ids
        {
            get
            {
                using var db = new AppDbContext();

                return db.Party
                    .AsNoTracking()
                    .OrderBy(member => member.AddedAt)
                    .ThenBy(member => member.Id)
                    .Select(member => member.PokemonId)
                    .ToList();
            }
        }

        public bool EhNoTime(int id)
        {
            using var db = new AppDbContext();

            return db.Party
                .AsNoTracking()
                .Any(member => member.PokemonId == id);
        }

        public bool EhShiny(int id)
        {
            using var db = new AppDbContext();

            return db.Party
                .AsNoTracking()
                .Any(member => member.PokemonId == id && member.IsShiny);
        }

        public bool Adicionar(int id, bool shiny, out bool limiteAtingido)
        {
            using var db = new AppDbContext();

            return TentarAdicionar(db, id, shiny, out limiteAtingido);
        }

        public bool Remover(int id)
        {
            using var db = new AppDbContext();

            PokemonPartyMember? existente = db.Party
                .SingleOrDefault(member => member.PokemonId == id);

            if (existente is null)
                return false;

            db.Party.Remove(existente);
            db.SaveChanges();

            return true;
        }

        public bool Alternar(int id, bool shiny, out bool limiteAtingido)
        {
            limiteAtingido = false;

            using var db = new AppDbContext();

            PokemonPartyMember? existente = db.Party
                .SingleOrDefault(member => member.PokemonId == id);

            if (existente is not null)
            {
                db.Party.Remove(existente);
                db.SaveChanges();

                    return false;
            }

            return TentarAdicionar(db, id, shiny, out limiteAtingido);
        }

        private bool TentarAdicionar(
            AppDbContext db,
            int id,
            bool shiny,
            out bool limiteAtingido)
        {
            limiteAtingido = false;

            if (id <= 0)
                return false;

            if (db.Party.Any(member => member.PokemonId == id))
                return false;

            bool pokemonExiste = db.Pokemons
                .Any(pokemon => pokemon.Id == id);

            if (!pokemonExiste)
                return false;

            if (db.Party.Count() >= MaximoMembros)
            {
                limiteAtingido = true;
                return false;
            }

            db.Party.Add(new PokemonPartyMember
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
