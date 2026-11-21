using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public class RotaInfo
    {
        public List<int> PokemonIds { get; set; } = new();

        public string Informacoes { get; set; } = string.Empty;
    }
}
