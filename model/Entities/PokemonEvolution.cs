namespace GestaoDePokemon.model
{
    public class PokemonEvolution
    {
        public int Id { get; set; }

        // Espécie de origem
        public int FromPokemonId { get; set; }

        public Pokemon FromPokemon { get; set; } = null!;

        // Espécie de destino
        public int ToPokemonId { get; set; }

        public Pokemon ToPokemon { get; set; } = null!;

        // Formas de origem e de destino (opcionais). A relação acima é por espécie; estes campos restringem a
        // linha a uma forma: só o Meowth de Galar evolui para Perrserker, e o Rattata de Alola para o Raticate
        // de Alola. Para uma forma F, use as linhas com FromFormId == F.Id; se não houver nenhuma, use as
        // linhas com FromFormId nulo (valem para a forma padrão e para formas sem linha própria).
        public int? FromFormId { get; set; }

        public PokemonForm? FromForm { get; set; }

        public int? ToFormId { get; set; }

        public PokemonForm? ToForm { get; set; }

        // Método de evolução
        public EvolutionMethod Method { get; set; }

        public int? RequiredLevel { get; set; }

        public string? RequiredItem { get; set; }

        public EvolutionTime? RequiredTime { get; set; }

        public int? MinimumFriendship { get; set; }

        public int? MinimumBeauty { get; set; }

        public PokemonGender? RequiredGender { get; set; }

        public string? RequiredLocation { get; set; }

        public int? RequiredMoveId { get; set; }

        public PokemonMove? RequiredMove { get; set; }

        public string? GameVersion { get; set; }

        // Identificadores das espécies referenciadas usam Pokemon.Id.
        public int? RequiredPartySpeciesId { get; set; }
        public int? RequiredTradeSpeciesId { get; set; }

        // Condições tipadas expostas pela PokéAPI.
        public string? RequiredHeldItem { get; set; }
        public PokemonType? RequiredPartyType { get; set; }
        public PokemonType? RequiredMoveType { get; set; }
        public RelativePhysicalStats? RelativePhysicalStats { get; set; }
        public int? MinimumAffection { get; set; }
        public int? MinimumSteps { get; set; }
        public int? MinimumMoveCount { get; set; }
        public bool NeedsOverworldRain { get; set; }
        public bool TurnUpsideDown { get; set; }

        // Condições especiais documentadas fora do EvolutionDetail da PokéAPI.
        public bool RequiresEmptyPartySlot { get; set; }
        public bool RequiresPokeBallInBag { get; set; }
    }
}