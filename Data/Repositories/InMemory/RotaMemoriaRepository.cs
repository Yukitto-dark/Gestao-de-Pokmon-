using System;
using System.Collections.Generic;
using GestaoDePokemon.model;

using GestaoDePokemon.Data.Repositories.Contracts;
namespace GestaoDePokemon.Data.Repositories.InMemory
{
    public class RotaMemoriaRepository : IRotaRepository
    {
        private static readonly Dictionary<string, (int Inicio, int Fim)> Faixas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Kanto"] = (1, 28),
                ["Johto"] = (29, 46),
                ["Hoenn"] = (101, 134)
            };

        public IReadOnlyList<int> ObterRotas(string regiao)
        {
            if (string.IsNullOrWhiteSpace(regiao) ||
                !Faixas.TryGetValue(regiao, out var faixa))
            {
                return Array.Empty<int>();
            }

            var rotas = new int[faixa.Fim - faixa.Inicio + 1];

            for (int i = 0; i < rotas.Length; i++)
                rotas[i] = faixa.Inicio + i;

            return rotas;
        }

        public RotaInfo? ObterRota(string regiao, int numero)
        {
            if (string.IsNullOrWhiteSpace(regiao) ||
                !Faixas.TryGetValue(regiao, out var faixa))
            {
                return null;
            }

            if (numero < faixa.Inicio || numero > faixa.Fim)
                return null;

            return new RotaInfo
            {
                Numero = numero,
                Regiao = regiao
            };
        }
    }
}
