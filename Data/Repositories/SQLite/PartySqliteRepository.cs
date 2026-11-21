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

        public IReadOnlyList<MembroTime> Membros
        {
            get
            {
                using var db = new AppDbContext();

                return db.Party
                    .AsNoTracking()
                    .OrderBy(member => member.AddedAt)
                    .ThenBy(member => member.Id)
                    .Select(member => new
                    {
                        member.Id,
                        member.Pokemon.NationalDexNumber,
                        member.PokemonFormId,
                        member.PokemonId,
                        member.IsShiny
                    })
                    .AsEnumerable()
                    .Select(member => new MembroTime(
                        member.Id,
                        member.NationalDexNumber,
                        member.PokemonFormId ?? FormaNormal(db, member.PokemonId),
                        member.IsShiny))
                    .ToList();
            }
        }

        public bool EhNoTime(int formId)
        {
            using var db = new AppDbContext();

            return db.Party
                .AsNoTracking()
                .Any(member => member.PokemonFormId == formId);
        }

        public bool EhShiny(int formId)
        {
            using var db = new AppDbContext();

            return db.Party
                .AsNoTracking()
                .Any(member => member.PokemonFormId == formId && member.IsShiny);
        }

        public bool Alternar(int formId, bool shiny, out bool limiteAtingido)
        {
            limiteAtingido = false;

            using var db = new AppDbContext();

            PokemonPartyMember? existente = db.Party
                .FirstOrDefault(member => member.PokemonFormId == formId);

            if (existente is not null)
            {
                db.Party.Remove(existente);
                db.SaveChanges();
                return false;
            }

            var forma = db.PokemonForms
                .AsNoTracking()
                .Where(form => form.Id == formId)
                .Select(form => new { form.Id, form.PokemonId })
                .FirstOrDefault();

            if (forma is null)
                return false;

            if (db.Party.Count() >= MaximoMembros)
            {
                limiteAtingido = true;
                return false;
            }

            db.Party.Add(new PokemonPartyMember
            {
                PokemonId = forma.PokemonId,
                PokemonFormId = forma.Id,
                IsShiny = shiny,
                AddedAt = DateTime.UtcNow
            });

            db.SaveChanges();
            return true;
        }

        private static int FormaNormal(AppDbContext db, int pokemonId) =>
            db.PokemonForms
                .AsNoTracking()
                .Where(form => form.PokemonId == pokemonId)
                .OrderBy(form => form.FormName == "Normal" ? 0 : 1)
                .ThenBy(form => form.Id)
                .Select(form => form.Id)
                .FirstOrDefault();
    }
}
