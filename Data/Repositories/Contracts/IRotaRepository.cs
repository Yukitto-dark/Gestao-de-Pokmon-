using System.Collections.Generic;
using System.Text.RegularExpressions;
using GestaoDePokemon.model;
namespace GestaoDePokemon.Data.Repositories.Contracts
{
    /// <summary>
    /// Uma rota da região. Kanto a Galar têm rotas numeradas ("Route 201"); Paldea e Hisui usam locais
    /// com nome ("Asado Desert", "Obsidian Fieldlands"), numerados só para a ordem.
    /// </summary>
    public sealed record RotaResumo(int Numero, string Nome)
    {
        private static readonly Regex RotaNumerada = new(@"^(Sea )?Route \d+$");

        /// <summary>Texto do botão e do título: "Rota 201" para rotas numeradas, o nome nos demais.</summary>
        public string Rotulo => RotaNumerada.IsMatch(Nome) ? $"Rota {Numero}" : Nome;
    }

    public interface IRotaRepository
    {
        IReadOnlyList<string> ObterRegioes();

        IReadOnlyList<RotaResumo> ObterRotas(string regiao);

        RotaInfo? ObterRota(string regiao, int numero);
    }
}
