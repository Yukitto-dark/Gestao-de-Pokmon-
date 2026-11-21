using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data.Repositories.SQLite
{
    public sealed class RotaSqliteRepository : IRotaRepository
    {
        // Versões com encontros no seed de rotas, em ordem de lançamento.
        private static readonly (string Chave, string Nome)[] Versoes =
        {
            ("red", "Red"),
            ("blue", "Blue"),
            ("yellow", "Yellow"),
            ("gold", "Gold"),
            ("silver", "Silver"),
            ("crystal", "Crystal"),
            ("ruby", "Ruby"),
            ("sapphire", "Sapphire"),
            ("emerald", "Emerald"),
            ("firered", "FireRed"),
            ("leafgreen", "LeafGreen"),
            ("diamond", "Diamond"),
            ("pearl", "Pearl"),
            ("platinum", "Platinum"),
            ("heartgold", "HeartGold"),
            ("soulsilver", "SoulSilver"),
            ("black", "Black"),
            ("white", "White"),
            ("black-2", "Black 2"),
            ("white-2", "White 2"),
            ("x", "X"),
            ("y", "Y"),
            ("omega-ruby", "Omega Ruby"),
            ("alpha-sapphire", "Alpha Sapphire"),
            ("sun", "Sun"),
            ("moon", "Moon"),
            ("ultra-sun", "Ultra Sun"),
            ("ultra-moon", "Ultra Moon"),
            ("lets-go-pikachu", "Let's Go, Pikachu!"),
            ("lets-go-eevee", "Let's Go, Eevee!"),
            ("sword", "Sword"),
            ("shield", "Shield"),
            ("brilliant-diamond", "Brilliant Diamond"),
            ("shining-pearl", "Shining Pearl"),
            ("legends-arceus", "Legends: Arceus"),
            ("scarlet", "Scarlet"),
            ("violet", "Violet"),
            ("scarlet-violet", "Scarlet/Violet")
        };

        public IReadOnlyList<string> ObterRegioes()
        {
            using var db = new AppDbContext();

            return db.Regions
                .AsNoTracking()
                .OrderBy(region => region.Id)
                .Select(region => region.Name)
                .ToList();
        }

        public IReadOnlyList<RotaResumo> ObterRotas(string regiao)
        {
            if (string.IsNullOrWhiteSpace(regiao))
                return Array.Empty<RotaResumo>();

            using var db = new AppDbContext();

            return db.Routes
                .AsNoTracking()
                .Where(route => route.Region.Name == regiao)
                .OrderBy(route => route.Number)
                .Select(route => new { route.Number, route.Name })
                .AsEnumerable()
                .Select(route => new RotaResumo(route.Number, route.Name))
                .ToList();
        }

        public RotaInfo? ObterRota(string regiao, int numero)
        {
            if (string.IsNullOrWhiteSpace(regiao))
                return null;

            using var db = new AppDbContext();

            Route? route = db.Routes
                .AsNoTracking()
                .Include(existing => existing.Region)
                .Include(existing => existing.Areas)
                    .ThenInclude(area => area.Encounters)
                        .ThenInclude(encounter => encounter.Pokemon)
                .FirstOrDefault(existing =>
                    existing.Region.Name == regiao &&
                    existing.Number == numero);

            if (route is null)
                return null;

            List<RoutePokemonEncounter> encontros = route.Areas
                .SelectMany(area => area.Encounters)
                .ToList();

            List<Pokemon> pokemons = encontros
                .Select(encounter => encounter.Pokemon)
                .Where(pokemon => pokemon is not null)
                .GroupBy(pokemon => pokemon.NationalDexNumber)
                .OrderBy(group => group.Key)
                .Select(group => group.First())
                .ToList();

            return new RotaInfo
            {
                PokemonIds = pokemons
                    .Select(pokemon => pokemon.NationalDexNumber)
                    .ToList(),
                Informacoes = MontarInformacoes(route, encontros, pokemons)
            };
        }

        private static string MontarInformacoes(
            Route route,
            IReadOnlyCollection<RoutePokemonEncounter> encontros,
            IReadOnlyCollection<Pokemon> pokemons)
        {
            List<string> versoes = ObterVersoes(route, encontros);
            List<string> areas = route.Areas
                .Select(area => area.Name)
                .Where(nome => !string.IsNullOrWhiteSpace(nome))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(nome => nome)
                .ToList();

            var texto = new StringBuilder();
            texto.AppendLine($"{new RotaResumo(route.Number, route.Name).Rotulo} - {route.Region.Name}");

            if (versoes.Count > 0)
                texto.AppendLine($"Jogos: {string.Join(", ", versoes)}");

            if (areas.Count > 1)
                texto.AppendLine($"Áreas: {string.Join(", ", areas)}");

            texto.AppendLine(
                $"{encontros.Count} encontros registrados para {pokemons.Count} Pokémon.");

            if (pokemons.Count > 0)
            {
                texto.AppendLine(
                    $"Pokémon: {string.Join(", ", pokemons.Select(pokemon => pokemon.Name))}");
            }

            return texto.ToString().TrimEnd();
        }

        private static List<string> ObterVersoes(
            Route route,
            IReadOnlyCollection<RoutePokemonEncounter> encontros)
        {
            List<string> versoes = LerVersoes(route.GameVersions);

            if (versoes.Count == 0)
            {
                versoes = encontros
                    .Select(encounter => encounter.GameVersion)
                    .Where(versao => !string.IsNullOrWhiteSpace(versao))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            return versoes
                .OrderBy(versao =>
                {
                    int indice = Array.FindIndex(
                        Versoes,
                        item => string.Equals(
                            item.Chave,
                            versao,
                            StringComparison.OrdinalIgnoreCase));
                    return indice < 0 ? int.MaxValue : indice;
                })
                .ThenBy(versao => versao)
                .Select(FormatarVersao)
                .ToList();
        }

        private static List<string> LerVersoes(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json)
                    ?? new List<string>();
            }
            catch (JsonException)
            {
                return new List<string>();
            }
        }

        private static string FormatarVersao(string versao)
        {
            var encontrada = Array.Find(
                Versoes,
                item => string.Equals(
                    item.Chave,
                    versao,
                    StringComparison.OrdinalIgnoreCase));

            return encontrada.Chave is null ? versao : encontrada.Nome;
        }
    }
}
