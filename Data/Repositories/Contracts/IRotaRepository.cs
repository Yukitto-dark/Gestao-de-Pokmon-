using System.Collections.Generic;
using GestaoDePokemon.model;
namespace GestaoDePokemon.Data.Repositories.Contracts
{
    public interface IRotaRepository
    {
        IReadOnlyList<int> ObterRotas(string regiao);

        RotaInfo? ObterRota(string regiao, int numero);
    }
}
