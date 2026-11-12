using System;
using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public static class PokemonTypeCatalog
    {
        private static readonly IReadOnlyDictionary<PokemonType, PokemonTypeInfo>
            Types = new Dictionary<PokemonType, PokemonTypeInfo>
        {
            { PokemonType.Normal, new("Normal", "View/images/types/normal.png") },
            { PokemonType.Fire, new("Fire", "View/images/types/fire.png") },
            { PokemonType.Fighting, new("Fighting", "View/images/types/fighting.png") },
            { PokemonType.Water, new("Water", "View/images/types/water.png") },
            { PokemonType.Flying, new("Flying", "View/images/types/flying.png") },
            { PokemonType.Grass, new("Grass", "View/images/types/grass.png") },
            { PokemonType.Poison, new("Poison", "View/images/types/poison.png") },
            { PokemonType.Electric, new("Electric", "View/images/types/electric.png") },
            { PokemonType.Ground, new("Ground", "View/images/types/ground.png") },
            { PokemonType.Psychic, new("Psychic", "View/images/types/psychic.png") },
            { PokemonType.Rock, new("Rock", "View/images/types/rock.png") },
            { PokemonType.Ice, new("Ice", "View/images/types/ice.png") },
            { PokemonType.Bug, new("Bug", "View/images/types/bug.png") },
            { PokemonType.Dragon, new("Dragon", "View/images/types/dragon.png") },
            { PokemonType.Ghost, new("Ghost", "View/images/types/ghost.png") },
            { PokemonType.Dark, new("Dark", "View/images/types/dark.png") },
            { PokemonType.Steel, new("Steel", "View/images/types/steel.png") }
        };

        public static PokemonTypeInfo GetInfo(PokemonType type)
        {
            if (!Types.TryGetValue(type, out var info))
                throw new ArgumentException("Tipo de Pokémon inválido.");

            return info;
        }
    }
}