namespace GestaoDePokemon.model
{
    public class PokemonMegaEvolution
    {
        public int Id { get; set; }

        // Forma base
        public int BaseFormId { get; set; }

        public PokemonForm BaseForm { get; set; } = null!;

        // Forma Mega
        public int MegaFormId { get; set; }

        public PokemonForm MegaForm { get; set; } = null!;

        // Método utilizado para Mega Evoluir
        public MegaEvolutionMethod Method { get; set; }

        // Pedra Mega necessária, quando aplicável
        public string? RequiredItem { get; set; }

        // Movimento necessário, quando aplicável
        public int? RequiredMoveId { get; set; }

        public PokemonMove? RequiredMove { get; set; }
    }
}