using System.Collections.Generic;

namespace GestaoDePokemon.Data.Repositories.Contracts
{
    /// <summary>
    /// Um Pokémon do time: a espécie, a forma escolhida na Box e se foi colocado com o sprite shiny.
    /// </summary>
    /// <param name="MembroId">Id da linha do time (PokemonPartyMember.Id); identifica o set salvo.</param>
    /// <param name="NationalDexNumber">Número da Pokédex da espécie.</param>
    /// <param name="FormId">Id da forma (PokemonForm.Id).</param>
    /// <param name="Shiny">Se o Pokémon foi colocado no time com o sprite shiny.</param>
    public sealed record MembroTime(int MembroId, int NationalDexNumber, int FormId, bool Shiny);

    public interface IPartyRepository
    {
        /// <summary>Membros do time na ordem em que foram adicionados.</summary>
        IReadOnlyList<MembroTime> Membros { get; }

        /// <summary>Se a forma informada está no time.</summary>
        bool EhNoTime(int formId);

        /// <summary>Se a forma informada está no time com o sprite shiny.</summary>
        bool EhShiny(int formId);

        /// <summary>Retira a forma do time se ela já estiver lá; caso contrário, adiciona.</summary>
        bool Alternar(int formId, bool shiny, out bool limiteAtingido);
    }
}
