using System;
using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public class PokemonTeam
    {
        private const int MaxMembers = 6;

        private readonly List<PokemonInstance> _members = new();

        public int Id { get; set; }

        public string Name { get; private set; }

        public IReadOnlyList<PokemonInstance> Members =>
            _members.AsReadOnly();

        public int MemberCount => _members.Count;

        public PokemonTeam(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome do time inválido.");

            Name = name;
        }

        public void AddPokemon(PokemonInstance pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));

            if (_members.Count >= MaxMembers)
                throw new InvalidOperationException(
                    "Um time não pode possuir mais de seis Pokémon."
                );

            if (_members.Contains(pokemon))
                throw new InvalidOperationException(
                    "Esse Pokémon já pertence ao time."
                );

            _members.Add(pokemon);
        }

        public bool RemovePokemon(PokemonInstance pokemon)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));

            return _members.Remove(pokemon);
        }

        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Nome do time inválido.");

            Name = newName;
        }
    }
}