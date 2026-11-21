using System;

namespace GestaoDePokemon.model
{
    public class PokemonPartyMember
    {
        public int Id { get; set; }

        public int PokemonId { get; set; }

        public bool IsShiny { get; set; }

        public Pokemon Pokemon { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public int Level { get; set; } = 100;

        public PokemonNature Nature { get; set; } = PokemonNature.Hardy;

        public int? AbilityId { get; set; }

        public PokemonAbility? Ability { get; set; }

        public IndividualValues IVs { get; set; } = new(31, 31, 31, 31, 31, 31);

        public EffortValues EVs { get; set; } = new();

        public string MoveIdsJson { get; set; } = "[]";
    }
}
