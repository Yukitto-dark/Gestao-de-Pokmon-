namespace GestaoDePokemon.model
{
    public class PokemonFormAbility
    {
        public int Id { get; set; }

        public int PokemonFormId { get; set; }

        public PokemonForm PokemonForm { get; set; } = null!;

        public int AbilityId { get; set; }

        public PokemonAbility Ability { get; set; } = null!;

        public AbilitySlot Slot { get; set; }
    }
}
