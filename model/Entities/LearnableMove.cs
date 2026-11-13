namespace GestaoDePokemon.model
{
    public class LearnableMove
    {
        public int Id { get; set; }

        public int PokemonFormId { get; set; }

        public PokemonForm PokemonForm { get; set; } = null!;

        public int MoveId { get; set; }

        public required PokemonMove Move { get; set; }

        public MoveLearnMethod Method { get; set; }

        public int? Level { get; set; }

        // null indica que a condição não está limitada a uma versão específica.
        public string? GameVersion { get; set; }
    }
}
