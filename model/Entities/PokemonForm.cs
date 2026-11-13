using System;
using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public class PokemonForm
    {
        private List<PokemonType> _types = new();

        public int Id { get; set; }

        public int PokemonId { get; set; }

        public Pokemon Pokemon { get; set; } = null!;

        public string FormName { get; set; } = "Normal";

        public IReadOnlyList<PokemonType> Types => _types.AsReadOnly();

        public BaseStats Stats { get; set; }

        public List<PokemonSprite> Sprites { get; set; } = new();

        public List<PokemonFormAbility> Abilities { get; set; } = new();

        // O conjunto de movimentos pode variar entre formas.
        public List<LearnableMove> Learnset { get; set; } = new();

        public void AddType(PokemonType type)
        {
            if (!Enum.IsDefined(type))
                throw new ArgumentException("Tipo de Pokémon inválido.", nameof(type));

            if (_types.Contains(type))
                throw new InvalidOperationException("Esse tipo já foi adicionado.");

            if (_types.Count >= 2)
                throw new InvalidOperationException(
                    "Uma forma de Pokémon não pode possuir mais de dois tipos.");

            _types.Add(type);
        }
    }
}
