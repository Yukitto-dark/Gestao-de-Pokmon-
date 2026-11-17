namespace GestaoDePokemon.Data.Repositories.Contracts
{
    /// <summary>
    /// Uma célula da Box: uma forma de uma espécie (a forma "Normal" ou qualquer forma alternativa).
    /// </summary>
    /// <param name="FormId">Id da forma (PokemonForm.Id); identifica os sprites da célula.</param>
    /// <param name="NationalDexNumber">Número da Pokédex da espécie.</param>
    /// <param name="PokemonName">Nome da espécie.</param>
    /// <param name="FormName">Nome da forma ("Normal", "Alolan Form", "Mega Charizard X"...).</param>
    public sealed record PokemonBoxEntry(
        int FormId,
        int NationalDexNumber,
        string PokemonName,
        string FormName)
    {
        /// <summary>Texto curto para dicas de tela: "#006 Charizard" ou "#006 Charizard - Mega Charizard X".</summary>
        public string Descricao =>
            string.Equals(FormName, "Normal", System.StringComparison.OrdinalIgnoreCase)
                ? $"#{NationalDexNumber:000} {PokemonName}"
                : $"#{NationalDexNumber:000} {PokemonName} - {FormName}";
    }
}
