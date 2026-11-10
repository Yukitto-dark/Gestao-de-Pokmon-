using System;
using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public static class PokemonTypeCatalog
    {
        private static readonly IReadOnlyDictionary<PokemonType, PokemonTypeInfo>
            Types = new Dictionary<PokemonType, PokemonTypeInfo>
        {
            { PokemonType.Normal, new("Normal", "images/types/normal.png") },
            { PokemonType.Fire, new("Fire", "images/types/fire.png") },
            { PokemonType.Fighting, new("Fighting", "images/types/fighting.png") },
            { PokemonType.Water, new("Water", "images/types/water.png") },
            { PokemonType.Flying, new("Flying", "images/types/flying.png") },
            { PokemonType.Grass, new("Grass", "images/types/grass.png") },
            { PokemonType.Poison, new("Poison", "images/types/poison.png") },
            { PokemonType.Electric, new("Electric", "images/types/electric.png") },
            { PokemonType.Ground, new("Ground", "images/types/ground.png") },
            { PokemonType.Psychic, new("Psychic", "images/types/psychic.png") },
            { PokemonType.Rock, new("Rock", "images/types/rock.png") },
            { PokemonType.Ice, new("Ice", "images/types/ice.png") },
            { PokemonType.Bug, new("Bug", "images/types/bug.png") },
            { PokemonType.Dragon, new("Dragon", "images/types/dragon.png") },
            { PokemonType.Ghost, new("Ghost", "images/types/ghost.png") },
            { PokemonType.Dark, new("Dark", "images/types/dark.png") },
            { PokemonType.Steel, new("Steel", "images/types/steel.png") }
        };

        public static PokemonTypeInfo GetInfo(PokemonType type)
        {
            if (!Types.TryGetValue(type, out var info))
                throw new ArgumentException("Tipo de Pokémon inválido.");

            return info;
        }
    }
}