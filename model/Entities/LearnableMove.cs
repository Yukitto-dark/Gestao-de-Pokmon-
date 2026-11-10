namespace GestaoDePokemon.model
{
    public class LearnableMove
    {
        public required PokemonMove Move { get; init; }

        public MoveLearnMethod Method { get; set; }

        public int? Level { get; set; }

        // null indica que a condição não está limitada a uma versão específica.
        public string? GameVersion { get; set; }
    }
}
