using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDePokemon.model
{
    /// <summary>
    /// Configuração planejada de uma espécie para compor um time.
    /// Não representa um Pokémon capturado nem simula treinamento no jogo.
    /// </summary>
    public class PokemonBuild
    {
        private List<PokemonMove> _moves = new();

        public int Id { get; set; }

        public int PokemonTeamId { get; private set; }

        public PokemonTeam Team { get; private set; } = null!;

        public int SpeciesId { get; private set; }

        public Pokemon Species { get; private set; } = null!;

        public int PokemonFormId { get; private set; }

        public PokemonForm Form { get; private set; } = null!;

        public PokemonGender Gender { get; private set; }

        public int Level { get; private set; }

        public IReadOnlyList<PokemonMove> Moves => _moves.AsReadOnly();

        public IndividualValues IVs { get; private set; }

        public EffortValues EVs { get; private set; }

        public PokemonNature Nature { get; private set; }

        public int AbilityId { get; private set; }

        public PokemonAbility Ability { get; private set; } = null!;

        public AbilitySlot AbilitySlot { get; private set; }

        // Opcional: permite validar o Learnset contra um jogo específico.
        public string? GameVersion { get; private set; }

        // Construtor privado utilizado pelo Entity Framework Core.
        private PokemonBuild()
        {
            Species = null!;
            Form = null!;
            Ability = null!;
            Team = null!;
            EVs = new EffortValues();
        }

        public PokemonBuild(
            Pokemon species,
            PokemonForm form,
            PokemonGender gender,
            int level,
            IndividualValues ivs,
            PokemonNature nature,
            PokemonAbility ability,
            AbilitySlot abilitySlot,
            params PokemonMove[] moves)
        {
            Species = species ?? throw new ArgumentNullException(nameof(species));
            Form = form ?? throw new ArgumentNullException(nameof(form));
            SpeciesId = species.Id;
            PokemonFormId = form.Id;

            if (!species.Forms.Any(f => SameForm(f, form)))
                throw new ArgumentException("Essa forma não pertence à espécie.", nameof(form));

            if (!Enum.IsDefined(gender))
                throw new ArgumentException("Gênero inválido.", nameof(gender));

            ValidateLevel(level);

            if (!Enum.IsDefined(nature))
                throw new ArgumentException("Natureza inválida.", nameof(nature));

            ValidateAbility(form, ability, abilitySlot);

            if (moves == null)
                throw new ArgumentNullException(nameof(moves));

            if (moves.Length > 4)
                throw new ArgumentException("Um Pokémon não pode possuir mais de quatro movimentos.", nameof(moves));

            if (Array.Exists(moves, move => move == null))
                throw new ArgumentException("Não são permitidos movimentos nulos.", nameof(moves));

            Gender = gender;
            Level = level;
            IVs = ivs;
            EVs = new EffortValues();
            Nature = nature;
            Ability = ability;
            AbilityId = ability.Id;
            AbilitySlot = abilitySlot;

            foreach (PokemonMove move in moves)
                LearnMove(move);
        }

        public void SetLevel(int level)
        {
            ValidateLevel(level);

            if (_moves.Any(move => !CanLearnMove(move, level, GameVersion)))
                throw new InvalidOperationException(
                    "O novo nível é incompatível com um ou mais movimentos selecionados.");

            Level = level;
        }

        public void SetNature(PokemonNature nature)
        {
            if (!Enum.IsDefined(nature))
                throw new ArgumentException("Natureza inválida.", nameof(nature));

            Nature = nature;
        }

        public void SetIndividualValues(IndividualValues ivs)
        {
            IVs = ivs;
        }

        public void SetAbility(PokemonAbility ability, AbilitySlot slot)
        {
            ValidateAbility(Form, ability, slot);
            Ability = ability;
            AbilityId = ability.Id;
            AbilitySlot = slot;
        }

        public void SetGameVersion(string? gameVersion)
        {
            string? normalized = string.IsNullOrWhiteSpace(gameVersion)
                ? null
                : gameVersion.Trim();

            if (_moves.Any(move => !CanLearnMove(move, Level, normalized)))
                throw new InvalidOperationException(
                    "Um ou mais movimentos selecionados não estão disponíveis nessa versão do jogo.");

            GameVersion = normalized;
        }

        public bool CanLearnMove(PokemonMove move)
        {
            ArgumentNullException.ThrowIfNull(move);
            return CanLearnMove(move, Level, GameVersion);
        }

        public void LearnMove(PokemonMove move)
        {
            ArgumentNullException.ThrowIfNull(move);

            if (_moves.Count >= 4)
                throw new InvalidOperationException(
                    "Um Pokémon não pode possuir mais de quatro movimentos.");

            if (_moves.Any(existing => SameMove(existing, move)))
                throw new InvalidOperationException("Esse movimento já está selecionado.");

            if (!CanLearnMove(move, Level, GameVersion))
                throw new InvalidOperationException(
                    $"{move.Name} não está disponível para essa forma, nível ou versão do jogo.");

            _moves.Add(move);
        }

        public bool ForgetMove(PokemonMove move)
        {
            ArgumentNullException.ThrowIfNull(move);

            int index = _moves.FindIndex(existing => SameMove(existing, move));
            if (index < 0)
                return false;

            _moves.RemoveAt(index);
            return true;
        }

        public void ReplaceMove(PokemonMove currentMove, PokemonMove newMove)
        {
            ArgumentNullException.ThrowIfNull(currentMove);
            ArgumentNullException.ThrowIfNull(newMove);

            int index = _moves.FindIndex(existing => SameMove(existing, currentMove));
            if (index < 0)
                throw new InvalidOperationException("O movimento a ser substituído não está selecionado.");

            if (_moves.Any(existing => SameMove(existing, newMove)))
                throw new InvalidOperationException("O novo movimento já está selecionado.");

            if (!CanLearnMove(newMove, Level, GameVersion))
                throw new InvalidOperationException(
                    $"{newMove.Name} não está disponível para essa forma, nível ou versão do jogo.");

            _moves[index] = newMove;
        }

        public void AddEffortValues(PokemonStat stat, int amount)
        {
            EVs.Add(stat, amount);
        }

        public void SetEffortValue(PokemonStat stat, int value)
        {
            EVs.Set(stat, value);
        }

        private bool CanLearnMove(PokemonMove move, int level, string? gameVersion)
        {
            IEnumerable<LearnableMove> entries = Form.Learnset.Where(entry =>
                entry.Move != null && SameMove(entry.Move, move));

            if (gameVersion != null)
            {
                entries = entries.Where(entry =>
                    entry.GameVersion == null ||
                    string.Equals(entry.GameVersion, gameVersion, StringComparison.OrdinalIgnoreCase));
            }

            return entries.Any(entry =>
                Enum.IsDefined(entry.Method) &&
                (entry.Method != MoveLearnMethod.LevelUp ||
                 !entry.Level.HasValue ||
                 entry.Level.Value <= level));
        }

        private static void ValidateAbility(
            PokemonForm form, PokemonAbility ability, AbilitySlot slot)
        {
            ArgumentNullException.ThrowIfNull(ability);

            if (!Enum.IsDefined(slot))
                throw new ArgumentException("Slot de habilidade inválido.", nameof(slot));

            bool available = form.Abilities.Any(entry =>
                entry.Ability != null &&
                SameAbility(entry.Ability, ability) &&
                entry.Slot == slot);

            if (!available)
                throw new ArgumentException(
                    "Habilidade não disponível para essa forma.", nameof(ability));
        }

        private static bool SameForm(PokemonForm a, PokemonForm b) =>
            ReferenceEquals(a, b) || (a.Id > 0 && b.Id > 0 && a.Id == b.Id);

        private static bool SameMove(PokemonMove a, PokemonMove b) =>
            ReferenceEquals(a, b) || (a.Id > 0 && b.Id > 0 && a.Id == b.Id);

        private static bool SameAbility(PokemonAbility a, PokemonAbility b) =>
            ReferenceEquals(a, b) || (a.Id > 0 && b.Id > 0 && a.Id == b.Id);

        private static void ValidateLevel(int level)
        {
            if (level < 1 || level > 100)
                throw new ArgumentOutOfRangeException(nameof(level), "O nível deve estar entre 1 e 100.");
        }
    }
}
