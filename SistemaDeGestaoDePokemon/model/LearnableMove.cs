namespace GestaoDePokemon.model
{
    public class LearnableMove
    {
        public PokemonMove Move { get; set; }

        public MoveLearnMethod Method { get; set; }

        public int? Level { get; set; }

        public string? GameVersion { get; set; }
    }
}