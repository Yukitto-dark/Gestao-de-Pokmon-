using System;
using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public static class PokemonTypeCatalog
    {
        private static readonly IReadOnlyDictionary<PokemonType, PokemonTypeInfo>
            Types = new Dictionary<PokemonType, PokemonTypeInfo>
        {
            { PokemonType.Normal, new("Normal", "View/images/types/Type-Normal.png") },
            { PokemonType.Fire, new("Fire", "View/images/types/Type-Fire.png") },
            { PokemonType.Fighting, new("Fighting", "View/images/types/Type-Fight.png") },
            { PokemonType.Water, new("Water", "View/images/types/Type-Water.png") },
            { PokemonType.Flying, new("Flying", "View/images/types/Type-Flying.png") },
            { PokemonType.Grass, new("Grass", "View/images/types/Type-Grass.png") },
            { PokemonType.Poison, new("Poison", "View/images/types/Type-Poison.png") },
            { PokemonType.Electric, new("Electric", "View/images/types/Type-Electr.png") },
            { PokemonType.Ground, new("Ground", "View/images/types/Type-Ground.png") },
            { PokemonType.Psychic, new("Psychic", "View/images/types/Type-Psychc.png") },
            { PokemonType.Rock, new("Rock", "View/images/types/Type-Rock.png") },
            { PokemonType.Ice, new("Ice", "View/images/types/Type-Ice.png") },
            { PokemonType.Bug, new("Bug", "View/images/types/Type-Bug.png") },
            { PokemonType.Dragon, new("Dragon", "View/images/types/Type-Dragon.png") },
            { PokemonType.Ghost, new("Ghost", "View/images/types/Type-Ghost.png") },
            { PokemonType.Dark, new("Dark", "View/images/types/Type-Dark.png") },
            { PokemonType.Steel, new("Steel", "View/images/types/Type-Steel.png") },
            { PokemonType.Fairy, new("Fairy", "View/images/types/Type-Fairy.png") }
        };

        public static PokemonTypeInfo GetInfo(PokemonType type)
        {
            if (!Types.TryGetValue(type, out var info))
                throw new ArgumentException("Tipo de Pokémon inválido.");

            return info;
        }
    }
}