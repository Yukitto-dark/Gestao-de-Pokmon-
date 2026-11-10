using System;
using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public class PokemonForm
    {
        private readonly List<PokemonType> _types = new();
        private readonly IReadOnlyList<PokemonType> _typesView;

        public int Id { get; set; }

        public string FormName { get; set; } = "Normal";

        public IReadOnlyList<PokemonType> Types => _typesView;

        public BaseStats Stats { get; set; }

        public List<PokemonSprite> Sprites { get; set; } = new();

        public List<PokemonFormAbility> Abilities { get; set; } = new();

        // O conjunto de movimentos pode variar entre formas.
        public List<LearnableMove> Learnset { get; set; } = new();

        public PokemonForm()
        {
            _typesView = _types.AsReadOnly();
        }

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
