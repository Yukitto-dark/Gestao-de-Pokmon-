using System.Collections.Generic;

namespace GestaoDePokemon.model
{
    public class RotaInfo
    {
        public int Numero { get; set; }

        public string Regiao { get; set; } = string.Empty;

        public List<int> PokemonIds { get; set; } = new();

        public string Informacoes { get; set; } = string.Empty;

        public string? MapaImagem { get; set; }
    }
}
