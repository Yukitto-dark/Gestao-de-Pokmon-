using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GestaoDePokemon.Database;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data
{
    /// <summary>
    /// Consulta as espécies e seus sprites diretamente no banco SQLite.
    /// Cada operação cria seu próprio DbContext para evitar compartilhar
    /// uma unidade de trabalho entre chamadas da interface.
    /// </summary>
    public sealed class PokemonSqliteRepository : IPokemonRepository
    {
        public int Total
        {
            get
            {
                using var db = new AppDbContext();
                return db.Pokemons.AsNoTracking().Count();
            }
        }

        public Pokemon? Obter(int id)
        {
            using var db = new AppDbContext();

            return db.Pokemons
                .AsNoTracking()
                .Include(pokemon => pokemon.Forms)
                    .ThenInclude(form => form.Sprites)
                .SingleOrDefault(pokemon => pokemon.NationalDexNumber == id);
        }

        public IReadOnlyList<int> ObterIds(int pagina, int tamanho) =>
            ObterIds(new PokemonFilter(), pagina, tamanho);

        public int Contar(PokemonFilter filtro)
        {
            ArgumentNullException.ThrowIfNull(filtro);
            using var db = new AppDbContext();

            IQueryable<Pokemon> consulta = AplicarFiltrosEscalaveis(db, filtro);
            PokemonType[] tipos = filtro.Types.Distinct().ToArray();

            if (tipos.Length == 0)
                return consulta.Count();

            return FiltrarPorTipos(consulta, tipos).Count;
        }

        public IReadOnlyList<int> ObterIds(PokemonFilter filtro, int pagina, int tamanho)
        {
            ArgumentNullException.ThrowIfNull(filtro);

            if (pagina < 1 || tamanho < 1)
                return Array.Empty<int>();

            int deslocamento;
            try
            {
                deslocamento = checked((pagina - 1) * tamanho);
            }
            catch (OverflowException)
            {
                return Array.Empty<int>();
            }

            using var db = new AppDbContext();
            IQueryable<Pokemon> consulta = AplicarFiltrosEscalaveis(db, filtro);
            PokemonType[] tipos = filtro.Types.Distinct().ToArray();

            if (tipos.Length == 0)
            {
                return consulta
                    .OrderBy(pokemon => pokemon.NationalDexNumber)
                    .Skip(deslocamento)
                    .Take(tamanho)
                    .Select(pokemon => pokemon.NationalDexNumber)
                    .ToList();
            }

            return FiltrarPorTipos(consulta, tipos)
                .OrderBy(pokemon => pokemon.NationalDexNumber)
                .Skip(deslocamento)
                .Take(tamanho)
                .Select(pokemon => pokemon.NationalDexNumber)
                .ToList();
        }

        private static IQueryable<Pokemon> AplicarFiltrosEscalaveis(
            AppDbContext db,
            PokemonFilter filtro)
        {
            IQueryable<Pokemon> consulta = db.Pokemons.AsNoTracking();

            if (filtro.NationalDexNumber is int numero)
                consulta = consulta.Where(pokemon => pokemon.NationalDexNumber == numero);

            if (!string.IsNullOrWhiteSpace(filtro.NameContains))
            {
                string nome = filtro.NameContains.Trim();
                consulta = consulta.Where(pokemon =>
                    EF.Functions.Like(pokemon.Name, $"%{nome}%"));
            }

            // Sem regiões marcadas, não restringe a Pokédex. Quando uma ou
            // mais regiões estão marcadas, retorna apenas as espécies cuja
            // região de origem corresponde a pelo menos uma delas.
            if (filtro.OriginRegionIds is { Count: > 0 })
            {
                int[] regioes = filtro.OriginRegionIds.Distinct().ToArray();
                consulta = consulta.Where(pokemon => regioes.Contains(pokemon.OriginRegionId));
            }

            if (filtro.OnlyFavorites)
            {
                consulta = consulta.Where(pokemon =>
                    db.Favorites.Any(favorite => favorite.PokemonId == pokemon.Id));
            }

            return consulta;
        }

        /// <summary>
        /// Os tipos são armazenados como JSON na coluna Types. Como essa coleção
        /// é convertida por ValueConverter, o filtro é aplicado após materializar
        /// as formas. O conjunto atual (386 espécies) é pequeno e essa abordagem
        /// mantém o mapeamento do domínio simples e confiável.
        /// Todos os tipos selecionados precisam estar presentes na mesma forma.
        /// </summary>
        private static List<Pokemon> FiltrarPorTipos(
            IQueryable<Pokemon> consulta,
            IReadOnlyCollection<PokemonType> tipos)
        {
            PokemonType[] tiposSelecionados = tipos.ToArray();

            return consulta
                .Include(pokemon => pokemon.Forms)
                .OrderBy(pokemon => pokemon.NationalDexNumber)
                .ToList()
                .Where(pokemon => pokemon.Forms.Any(form =>
                    tiposSelecionados.All(tipo => form.Types.Contains(tipo))))
                .ToList();
        }

        public string? ObterSpriteUrl(int id)
        {
            using var db = new AppDbContext();

            string? relativePath = db.Sprites
                .AsNoTracking()
                .Where(sprite => sprite.PokemonForm.Pokemon.NationalDexNumber == id)
                .OrderBy(sprite => sprite.Gender == null ? 0 : 1)
                .ThenBy(sprite => sprite.Id)
                .Select(sprite => sprite.BoxDefault ?? sprite.ExhibitionDefault)
                .FirstOrDefault();

            return ResolveAssetUri(relativePath);
        }

        /// <summary>
        /// Obtém a arte frontal grande do Pokémon selecionado.
        /// A Box usa BoxDefault (mini), enquanto o painel de detalhes usa
        /// ExhibitionDefault (frontNormal) ou ExhibitionShiny (frontShiny).
        /// Se o sprite shiny não existir, volta para a arte normal.
        /// </summary>
        public string? ObterSpriteDetalheUrl(int id, bool shiny)
        {
            using var db = new AppDbContext();

            string? relativePath = db.Sprites
                .AsNoTracking()
                .Where(sprite => sprite.PokemonForm.Pokemon.NationalDexNumber == id)
                .OrderBy(sprite => sprite.Gender == null ? 0 : 1)
                .ThenBy(sprite => sprite.Id)
                .Select(sprite => shiny
                    ? (sprite.ExhibitionShiny ?? sprite.ExhibitionDefault)
                    : (sprite.ExhibitionDefault ?? sprite.BoxDefault))
                .FirstOrDefault();

            return ResolveAssetUri(relativePath);
        }

        private static string? ResolveAssetUri(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            string normalizedPath = relativePath
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            string fullPath = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, normalizedPath));

            return File.Exists(fullPath)
                ? new Uri(fullPath).AbsoluteUri
                : null;
        }
    }
}
