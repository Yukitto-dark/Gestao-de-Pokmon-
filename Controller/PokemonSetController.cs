using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GestaoDePokemon.Data.Configuration;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Controller
{
    public class PokemonSetController
    {
        public sealed record SetInfo(
            int PokemonId,
            string Nome,
            string? SpriteUrl,
            int Level,
            PokemonNature Nature,
            IndividualValues IVs,
            EffortValues EVs,
            int? AbilityId,
            IReadOnlyList<int> MoveIds,
            IReadOnlyList<PokemonAbility> Abilities,
            IReadOnlyList<PokemonMove> TodosOsMoves);

        private readonly IPokemonRepository _pokemons;

        public PokemonSetController(IPokemonRepository? pokemons = null)
        {
            _pokemons = pokemons ?? AppServices.Pokemons;
        }

        public SetInfo? Obter(int pokemonId, bool shiny)
        {
            using var db = new AppDbContext();

            PokemonPartyMember? membro = db.Party
                .AsNoTracking()
                .Include(member => member.Pokemon)
                    .ThenInclude(pokemon => pokemon.Forms)
                        .ThenInclude(form => form.Abilities)
                            .ThenInclude(entry => entry.Ability)
                .Include(member => member.Ability)
                .FirstOrDefault(member => member.PokemonId == pokemonId);

            if (membro?.Pokemon is null)
                return null;

            PokemonForm? form = ObterForma(membro.Pokemon);

            var habilidades = form is null
                ? new List<PokemonAbility>()
                : form.Abilities
                    .Where(entry => entry.Ability is not null)
                    .Select(entry => entry.Ability!)
                    .GroupBy(ability => ability.Id)
                    .Select(group => group.First())
                    .ToList();

            string nome = membro.Pokemon.Name;

            EffortValues evs = GarantirEVsMinimos(db, pokemonId, membro.EVs);

            List<int> moveIds = GarantirMovesUnicos(
                db, pokemonId, LerMoveIds(membro.MoveIdsJson));

            return new SetInfo(
                pokemonId,
                nome,
                _pokemons.ObterSpriteDetalheUrl(pokemonId, shiny),
                membro.Level,
                membro.Nature,
                membro.IVs,
                evs,
                membro.AbilityId,
                moveIds,
                habilidades,
                db.Moves
                    .AsNoTracking()
                    .OrderBy(move => move.Name)
                    .ToList());
        }

        private static List<int> GarantirMovesUnicos(
            AppDbContext db,
            int pokemonId,
            List<int> ids)
        {
            List<int> normalizados = new();

            foreach (int id in ids)
            {
                if (id != 0 && normalizados.Contains(id))
                    continue;

                normalizados.Add(id);
            }

            RemoverZerosFinais(normalizados);

            if (ids.SequenceEqual(normalizados))
                return ids;

            PokemonPartyMember? membro = db.Party
                .FirstOrDefault(member => member.PokemonId == pokemonId);

            if (membro is not null)
            {
                membro.MoveIdsJson = JsonSerializer.Serialize(normalizados);
                db.SaveChanges();
            }

            return normalizados;
        }

        private static EffortValues GarantirEVsMinimos(
            AppDbContext db,
            int pokemonId,
            EffortValues atuais)
        {
            PokemonPartyMember? membro = db.Party
                .FirstOrDefault(member => member.PokemonId == pokemonId);

            if (membro is null)
                return atuais;

            if (!membro.EVs.GarantirMinimo())
                return atuais;

            db.SaveChanges();
            return membro.EVs;
        }

        public void AtualizarNivel(int pokemonId, int level)
        {
            if (level < 1 || level > 100)
                throw new ArgumentOutOfRangeException(nameof(level), "O nível deve estar entre 1 e 100.");

            using var db = new AppDbContext();

            PokemonPartyMember? membro = db.Party
                .FirstOrDefault(member => member.PokemonId == pokemonId);

            if (membro is null)
                throw new ArgumentException("Pokémon não encontrado no time.", nameof(pokemonId));

            membro.Level = level;
            db.SaveChanges();
        }

        public void AtualizarNatureza(int pokemonId, PokemonNature nature)
        {
            if (!Enum.IsDefined(nature))
                throw new ArgumentException("Natureza inválida.", nameof(nature));

            using var db = new AppDbContext();

            PokemonPartyMember? membro = db.Party
                .FirstOrDefault(member => member.PokemonId == pokemonId);

            if (membro is null)
                throw new ArgumentException("Pokémon não encontrado no time.", nameof(pokemonId));

            membro.Nature = nature;
            db.SaveChanges();
        }

        public void AtualizarMoves(int pokemonId, IReadOnlyList<int> moveIds)
        {
            if (moveIds is null || moveIds.Count > 4)
                throw new ArgumentException("O Pokémon deve ter no máximo quatro golpes.", nameof(moveIds));

            List<int> ids = Enumerable.Range(0, 4)
                .Select(i => i < moveIds.Count ? moveIds[i] : 0)
                .ToList();

            List<int> preenchidos = ids.Where(id => id != 0).ToList();
            if (preenchidos.Count != preenchidos.Distinct().Count())
                throw new ArgumentException("Não é permitido repetir o mesmo golpe.", nameof(moveIds));

            using var db = new AppDbContext();

            PokemonPartyMember? membro = db.Party
                .FirstOrDefault(member => member.PokemonId == pokemonId);

            if (membro is null)
                throw new ArgumentException("Pokémon não encontrado no time.", nameof(pokemonId));

            if (preenchidos.Count > 0)
            {
                HashSet<int> validos = db.Moves
                    .AsNoTracking()
                    .Where(move => preenchidos.Contains(move.Id))
                    .Select(move => move.Id)
                    .ToHashSet();

                if (preenchidos.Any(id => !validos.Contains(id)))
                    throw new ArgumentException("Golpe inválido.", nameof(moveIds));
            }

            RemoverZerosFinais(ids);

            membro.MoveIdsJson = JsonSerializer.Serialize(ids);
            db.SaveChanges();
        }

        public EffortValues AtualizarEV(int pokemonId, PokemonStat stat, int valor)
        {
            using var db = new AppDbContext();

            PokemonPartyMember? membro = db.Party
                .FirstOrDefault(member => member.PokemonId == pokemonId);

            if (membro is null)
                throw new ArgumentException("Pokémon não encontrado no time.", nameof(pokemonId));

            if (valor < EffortValues.MinPerStat)
                throw new ArgumentOutOfRangeException(
                    nameof(valor), $"O mínimo por estatística é {EffortValues.MinPerStat} EV.");

            membro.EVs.Set(stat, valor);
            db.SaveChanges();
            return membro.EVs;
        }

        internal static PokemonForm? ObterForma(Pokemon? pokemon) =>
            pokemon?.Forms.FirstOrDefault(form => form.FormName == "Normal")
            ?? pokemon?.Forms.FirstOrDefault();

        private static void RemoverZerosFinais(List<int> ids)
        {
            while (ids.Count > 1 && ids[^1] == 0)
                ids.RemoveAt(ids.Count - 1);
        }

        private static List<int> LerMoveIds(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<int>();

            try
            {
                return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }
            catch (JsonException)
            {
                return new List<int>();
            }
        }
    }
}
