namespace GestaoDePokemon.model
{
    public enum MoveCategory
    {
        Physical,
        Special,
        Status
    }

    public class PokemonMove
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public PokemonType Type { get; set; }

        public MoveCategory Category { get; set; }

        public int? Power { get; set; }

        public int? Accuracy { get; set; }

        public int PP { get; set; }
    }
}