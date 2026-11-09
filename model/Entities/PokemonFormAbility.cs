namespace GestaoDePokemon.model
{
    public class PokemonFormAbility
    {
        public PokemonAbility Ability { get; set; } = null!;

        public AbilitySlot Slot { get; set; }
    }
}