namespace GestaoDePokemon.model
{
    public class PokemonSprite
    {
        public int Id { get; set; }

        public int PokemonFormId { get; set; }

        public PokemonForm PokemonForm { get; set; } = null!;

        // null representa um sprite que não varia por gênero.
        public PokemonGender? Gender { get; set; }

        public string? ExhibitionDefault { get; set; }

        public string? BoxDefault { get; set; }

        public string? ExhibitionShiny { get; set; }

        public string? BoxShiny { get; set; }
    }
}
