using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDePokemon.model
{
    public class PokemonTeam
    {
        private const int MaxMembers = 6;
        private List<PokemonBuild> _members = new();

        public int Id { get; set; }

        public string Name { get; private set; }

        public IReadOnlyList<PokemonBuild> Members => _members.AsReadOnly();

        public int MemberCount => _members.Count;

        // Construtor privado utilizado pelo Entity Framework Core.
        private PokemonTeam()
        {
            Name = string.Empty;
        }

        public PokemonTeam(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome do time inválido.", nameof(name));

            Name = name.Trim();
        }

        public void AddPokemon(PokemonBuild pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);

            if (_members.Count >= MaxMembers)
                throw new InvalidOperationException(
                    "Um time não pode possuir mais de seis Pokémon.");

            if (_members.Any(existing => SameBuild(existing, pokemon)))
                throw new InvalidOperationException(
                    "Essa configuração já pertence ao time.");

            _members.Add(pokemon);
        }

        public bool RemovePokemon(PokemonBuild pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);

            PokemonBuild? existing = _members.FirstOrDefault(
                member => SameBuild(member, pokemon));

            return existing != null && _members.Remove(existing);
        }

        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Nome do time inválido.", nameof(newName));

            Name = newName.Trim();
        }

        private static bool SameBuild(PokemonBuild a, PokemonBuild b) =>
            ReferenceEquals(a, b) || (a.Id > 0 && b.Id > 0 && a.Id == b.Id);
    }
}
