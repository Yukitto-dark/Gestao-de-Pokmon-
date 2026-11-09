using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public class PokemonForm
    {
        public int Id { get; set; }

        public string FormName { get; set; } = "Normal";

        private readonly List<PokemonType> _type= new();
        
        public IReadOnlyList<PokemonType> Type => _type; 

        public PokemonForm()
        {
    _       typesView = _types.AsReadOnly();
        }

        public IReadOnlyList<PokemonType> Types => _typesView;

        public BaseStats Stats { get; set; }

        public List<PokemonSprite> Sprites { get; set; } = new();

        public List<PokemonFormAbility> Abilities { get; set; } = new();
            
        public void AddType(PokemonType type)
        {
            if (!Enum.IsDefined(type))
                throw new ArgumentException("Tipo de Pokémon inválido.");

            if (_types.Contains(type))
                throw new ArgumentException("Esse tipo já foi adicionado.");

            if (_types.Count >= 2)
                throw new InvalidOperationException(
                    "Um Pokémon não pode possuir mais de dois tipos.");

                _types.Add(type);
        }
    }
}
