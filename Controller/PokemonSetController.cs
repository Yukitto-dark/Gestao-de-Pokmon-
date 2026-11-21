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
        public const int NivelMinimo = 1;
        public const int NivelMaximo = 100;

        /// <summary>
        /// Um golpe que o Pokémon pode aprender e a forma de aprender: "Nv 36", "TM", "Tutor", "Ovo"...
        /// O nível é o do jogo de referência da forma; golpes que só eram aprendidos por nível em jogos
        /// mais antigos usam o menor nível entre eles e não entram nos golpes padrão.
        /// </summary>
        public sealed record GolpeDisponivel(PokemonMove Move, int? Nivel, string Origem, bool NivelNoJogoReferencia)
        {
            public string Descricao => $"{Move.Name} ({Origem})";
        }

        // Jogos usados como referência para os níveis, do mais recente ao mais antigo. Os jogos com
        // progressão própria (Let's Go, Legends e Champions) só são usados quando não há outro.
        private static readonly string[] JogosReferencia =
        {
            "scarlet-violet", "sword-shield", "brilliant-diamond-shining-pearl", "ultra-sun-ultra-moon",
            "sun-moon", "omega-ruby-alpha-sapphire", "x-y", "black-2-white-2", "black-white",
            "heartgold-soulsilver", "platinum", "diamond-pearl", "firered-leafgreen", "emerald",
            "ruby-sapphire", "crystal", "gold-silver", "yellow", "red-blue",
            "lets-go-pikachu-lets-go-eevee", "legends-za", "legends-arceus", "champions"
        };

        public sealed record SetInfo(
            int MembroId,
            int PokemonId,
            string Nome,
            string FormName,
            string? SpriteUrl,
            IReadOnlyList<PokemonType> Types,
            BaseStats Stats,
            double? Height,
            double? Weight,
            int Level,
            PokemonNature Nature,
            IndividualValues IVs,
            EffortValues EVs,
            int? AbilityId,
            IReadOnlyList<int> MoveIds,
            bool MovesPadrao,
            IReadOnlyList<PokemonAbility> Abilities,
            IReadOnlyList<GolpeDisponivel> GolpesDisponiveis);

        private readonly IPokemonRepository _pokemons;

        public PokemonSetController(IPokemonRepository? pokemons = null)
        {
            _pokemons = pokemons ?? AppServices.Pokemons;
        }

        public SetInfo? Obter(int membroId, bool shiny)
        {
            using var db = new AppDbContext();

            PokemonPartyMember? membro = db.Party
                .AsNoTracking()
                .Include(member => member.Pokemon)
                    .ThenInclude(pokemon => pokemon.Forms)
                        .ThenInclude(form => form.Abilities)
                            .ThenInclude(entry => entry.Ability)
                .AsSplitQuery()
                .FirstOrDefault(member => member.Id == membroId);

            if (membro?.Pokemon is null)
                return null;

            PokemonForm? form = ObterForma(membro.Pokemon, membro.PokemonFormId);

            var habilidades = form is null
                ? new List<PokemonAbility>()
                : form.Abilities
                    .OrderBy(entry => entry.Slot)
                    .Where(entry => entry.Ability is not null)
                    .Select(entry => entry.Ability!)
                    .GroupBy(ability => ability.Id)
                    .Select(group => group.First())
                    .ToList();

            IReadOnlyList<GolpeDisponivel> golpes = ObterGolpesDisponiveis(db, membro.Pokemon, form);

            EffortValues evs = GarantirEVsMinimos(db, membroId, membro.EVs);

            List<int> salvos = GarantirMovesValidos(
                db, membroId, LerMoveIds(membro.MoveIdsJson), golpes);

            // Sem golpes salvos, o Pokémon aparece com os quatro últimos golpes aprendidos por nível
            // até o nível atual, como nos jogos. Eles só são gravados na primeira edição.
            bool padrao = salvos.Count == 0;
            List<int> moveIds = padrao
                ? GolpesPadrao(golpes, membro.Level)
                : salvos;

            return new SetInfo(
                membroId,
                membro.Pokemon.NationalDexNumber,
                membro.Pokemon.Name,
                form?.FormName ?? "Normal",
                form is null
                    ? _pokemons.ObterSpriteDetalheUrl(membro.Pokemon.NationalDexNumber, shiny)
                    : _pokemons.ObterSpriteDetalheFormaUrl(form.Id, shiny),
                form?.Types ?? Array.Empty<PokemonType>(),
                form?.Stats ?? default,
                form?.Height,
                form?.Weight,
                membro.Level,
                membro.Nature,
                membro.IVs,
                evs,
                membro.AbilityId,
                moveIds,
                padrao,
                habilidades,
                golpes);
        }

        /// <summary>
        /// Golpes que a forma do membro aprende (por nível, TM, tutor, ovo ou evolução) em qualquer jogo.
        /// Formas sem learnset próprio (Megas e formas cosméticas) usam o da forma Normal.
        /// </summary>
        private static IReadOnlyList<GolpeDisponivel> ObterGolpesDisponiveis(
            AppDbContext db,
            Pokemon pokemon,
            PokemonForm? form)
        {
            List<LearnableMove> learnset = form is null
                ? new List<LearnableMove>()
                : CarregarLearnset(db, form.Id);

            if (learnset.Count == 0 && ObterForma(pokemon, null) is PokemonForm normal && normal.Id != form?.Id)
                learnset = CarregarLearnset(db, normal.Id);

            string? referencia = JogoReferencia(learnset);

            return learnset
                .GroupBy(learnable => learnable.MoveId)
                .Select(grupo =>
                {
                    PokemonMove move = grupo.First().Move;
                    List<LearnableMove> porNivel = grupo
                        .Where(learnable => learnable.Method == MoveLearnMethod.LevelUp)
                        .ToList();
                    List<LearnableMove> noJogo = porNivel
                        .Where(learnable => learnable.GameVersion == referencia)
                        .ToList();

                    if (porNivel.Count > 0)
                    {
                        // Nível 0 é o golpe aprendido ao evoluir (jogos recentes): conta como nível 1.
                        int nivel = Math.Max(1, (noJogo.Count > 0 ? noJogo : porNivel)
                            .Min(learnable => learnable.Level ?? 1));
                        return new GolpeDisponivel(move, nivel, $"Nv {nivel}", noJogo.Count > 0);
                    }

                    MoveLearnMethod metodo = grupo
                        .Select(learnable => learnable.Method)
                        .OrderBy(OrdemMetodo)
                        .First();

                    return new GolpeDisponivel(move, null, NomeMetodo(metodo), false);
                })
                .OrderBy(golpe => golpe.Move.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static readonly Dictionary<string, string> NomesJogos = new(StringComparer.Ordinal)
        {
            ["scarlet-violet"] = "Scarlet/Violet",
            ["sword-shield"] = "Sword/Shield",
            ["brilliant-diamond-shining-pearl"] = "Brilliant Diamond/Shining Pearl",
            ["ultra-sun-ultra-moon"] = "Ultra Sun/Ultra Moon",
            ["sun-moon"] = "Sun/Moon",
            ["omega-ruby-alpha-sapphire"] = "Omega Ruby/Alpha Sapphire",
            ["x-y"] = "X/Y",
            ["black-2-white-2"] = "Black 2/White 2",
            ["black-white"] = "Black/White",
            ["heartgold-soulsilver"] = "HeartGold/SoulSilver",
            ["platinum"] = "Platinum",
            ["diamond-pearl"] = "Diamond/Pearl",
            ["firered-leafgreen"] = "FireRed/LeafGreen",
            ["emerald"] = "Emerald",
            ["ruby-sapphire"] = "Ruby/Sapphire",
            ["crystal"] = "Crystal",
            ["gold-silver"] = "Gold/Silver",
            ["yellow"] = "Yellow",
            ["red-blue"] = "Red/Blue",
            ["lets-go-pikachu-lets-go-eevee"] = "Let's Go",
            ["legends-za"] = "Legends: Z-A",
            ["legends-arceus"] = "Legends: Arceus",
            ["champions"] = "Champions"
        };

        public static string NomeJogo(string jogo) =>
            NomesJogos.TryGetValue(jogo, out string? nome) ? nome : jogo;

        /// <summary>Jogo mais recente em que a forma aprende golpes por nível (referência para os níveis).</summary>
        public static string? JogoReferencia(IReadOnlyCollection<LearnableMove> learnset)
        {
            HashSet<string> jogos = learnset
                .Where(learnable => learnable.Method == MoveLearnMethod.LevelUp && learnable.GameVersion is not null)
                .Select(learnable => learnable.GameVersion!)
                .ToHashSet(StringComparer.Ordinal);

            return JogosReferencia.FirstOrDefault(jogos.Contains)
                ?? jogos
                    .OrderByDescending(jogo => learnset.Count(learnable => learnable.GameVersion == jogo))
                    .FirstOrDefault();
        }

        private static List<LearnableMove> CarregarLearnset(AppDbContext db, int formId) =>
            db.LearnableMoves
                .AsNoTracking()
                .Include(learnable => learnable.Move)
                .Where(learnable => learnable.PokemonFormId == formId)
                .ToList();

        private static int OrdemMetodo(MoveLearnMethod metodo) => metodo switch
        {
            MoveLearnMethod.Evolution => 0,
            MoveLearnMethod.TM => 1,
            MoveLearnMethod.Tutor => 2,
            MoveLearnMethod.Egg => 3,
            _ => 4
        };

        private static string NomeMetodo(MoveLearnMethod metodo) => metodo switch
        {
            MoveLearnMethod.Evolution => "Evolução",
            MoveLearnMethod.TM => "TM",
            MoveLearnMethod.Tutor => "Tutor",
            MoveLearnMethod.Egg => "Ovo",
            _ => metodo.ToString()
        };

        private static List<int> GolpesPadrao(IReadOnlyList<GolpeDisponivel> golpes, int nivel) =>
            golpes
                .Where(golpe => golpe.NivelNoJogoReferencia && golpe.Nivel is int aprendido && aprendido <= nivel)
                .OrderBy(golpe => golpe.Nivel)
                .ThenBy(golpe => golpe.Move.Name, StringComparer.OrdinalIgnoreCase)
                .TakeLast(4)
                .Select(golpe => golpe.Move.Id)
                .ToList();

        /// <summary>
        /// Remove golpes repetidos e golpes que a forma não aprende (gravados por versões anteriores,
        /// que ofereciam o catálogo inteiro), mantendo a posição dos demais.
        /// </summary>
        private static List<int> GarantirMovesValidos(
            AppDbContext db,
            int membroId,
            List<int> ids,
            IReadOnlyList<GolpeDisponivel> golpes)
        {
            HashSet<int> permitidos = golpes.Select(golpe => golpe.Move.Id).ToHashSet();
            List<int> normalizados = new();

            foreach (int id in ids.Take(4))
            {
                if (id == 0 || normalizados.Contains(id) || !permitidos.Contains(id))
                {
                    normalizados.Add(0);
                    continue;
                }

                normalizados.Add(id);
            }

            RemoverZerosFinais(normalizados);
            if (normalizados.All(id => id == 0))
                normalizados.Clear();

            if (ids.SequenceEqual(normalizados))
                return ids;

            PokemonPartyMember? membro = db.Party
                .FirstOrDefault(member => member.Id == membroId);

            if (membro is not null)
            {
                membro.MoveIdsJson = JsonSerializer.Serialize(normalizados);
                db.SaveChanges();
            }

            return normalizados;
        }

        private static EffortValues GarantirEVsMinimos(
            AppDbContext db,
            int membroId,
            EffortValues atuais)
        {
            PokemonPartyMember? membro = db.Party
                .FirstOrDefault(member => member.Id == membroId);

            if (membro is null)
                return atuais;

            if (!membro.EVs.GarantirMinimo())
                return atuais;

            db.SaveChanges();
            return membro.EVs;
        }

        public void AtualizarNivel(int membroId, int level)
        {
            if (level < NivelMinimo || level > NivelMaximo)
                throw new ArgumentOutOfRangeException(
                    nameof(level), $"O nível deve estar entre {NivelMinimo} e {NivelMaximo}.");

            using var db = new AppDbContext();

            PokemonPartyMember membro = ObterMembro(db, membroId);
            membro.Level = level;
            db.SaveChanges();
        }

        public void AtualizarNatureza(int membroId, PokemonNature nature)
        {
            if (!Enum.IsDefined(nature))
                throw new ArgumentException("Natureza inválida.", nameof(nature));

            using var db = new AppDbContext();

            PokemonPartyMember membro = ObterMembro(db, membroId);
            membro.Nature = nature;
            db.SaveChanges();
        }

        public void AtualizarMoves(int membroId, IReadOnlyList<int> moveIds)
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

            PokemonPartyMember membro = db.Party
                .Include(member => member.Pokemon)
                    .ThenInclude(pokemon => pokemon.Forms)
                .FirstOrDefault(member => member.Id == membroId)
                ?? throw new ArgumentException("Pokémon não encontrado no time.", nameof(membroId));

            if (preenchidos.Count > 0)
            {
                PokemonForm? form = ObterForma(membro.Pokemon, membro.PokemonFormId);
                HashSet<int> permitidos = ObterGolpesDisponiveis(db, membro.Pokemon, form)
                    .Select(golpe => golpe.Move.Id)
                    .ToHashSet();

                int? invalido = preenchidos.FirstOrDefault(id => !permitidos.Contains(id));
                if (invalido is > 0)
                {
                    string nome = db.Moves.AsNoTracking()
                        .Where(move => move.Id == invalido)
                        .Select(move => move.Name)
                        .FirstOrDefault() ?? $"#{invalido}";

                    throw new ArgumentException(
                        $"{membro.Pokemon.Name} não aprende o golpe {nome}.", nameof(moveIds));
                }
            }

            RemoverZerosFinais(ids);

            membro.MoveIdsJson = JsonSerializer.Serialize(ids);
            db.SaveChanges();
        }

        public EffortValues AtualizarEV(int membroId, PokemonStat stat, int valor)
        {
            using var db = new AppDbContext();

            PokemonPartyMember membro = ObterMembro(db, membroId);

            membro.EVs.Set(stat, valor);
            db.SaveChanges();
            return membro.EVs;
        }

        private static PokemonPartyMember ObterMembro(AppDbContext db, int membroId) =>
            db.Party.FirstOrDefault(member => member.Id == membroId)
            ?? throw new ArgumentException("Pokémon não encontrado no time.", nameof(membroId));

        /// <summary>A forma gravada no membro do time ou, se não houver, a forma Normal da espécie.</summary>
        internal static PokemonForm? ObterForma(Pokemon? pokemon, int? formId = null) =>
            (formId is int id ? pokemon?.Forms.FirstOrDefault(form => form.Id == id) : null)
            ?? pokemon?.Forms.FirstOrDefault(form => form.FormName == "Normal")
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
