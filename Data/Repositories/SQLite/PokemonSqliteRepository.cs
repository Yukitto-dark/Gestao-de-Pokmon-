using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GestaoDePokemon.Data.Filters;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data.Repositories.SQLite;

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

        IQueryable<Pokemon> consulta =
            AplicarFiltrosEscalaveis(db, filtro);

        PokemonType[] tipos = filtro.Types.Distinct().ToArray();

        if (tipos.Length == 0)
            return consulta.Count();

        return FiltrarPorTipos(consulta, tipos).Count;
    }

    public IReadOnlyList<int> ObterIds(
        PokemonFilter filtro,
        int pagina,
        int tamanho)
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

        IQueryable<Pokemon> consulta =
            AplicarFiltrosEscalaveis(db, filtro);

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

    /// <summary>
    /// Conta as células da Box: uma por forma. Os filtros de número, nome, região e favoritos valem para a
    /// espécie (todas as formas dela entram ou saem juntas); o filtro de tipos vale para cada forma.
    /// </summary>
    public int ContarEntradas(PokemonFilter filtro)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        using var db = new AppDbContext();

        IQueryable<PokemonForm> formas = FormasDasEspecies(db, filtro);
        PokemonType[] tipos = filtro.Types.Distinct().ToArray();

        if (tipos.Length == 0)
            return formas.Count();

        return FiltrarEntradasPorTipos(formas, tipos).Count;
    }

    /// <summary>
    /// Página da Box em ordem de Pokédex: cada espécie, a forma "Normal" primeiro e depois todas as outras
    /// formas dela (ordem de cadastro), sem exceção.
    /// </summary>
    public IReadOnlyList<PokemonBoxEntry> ObterEntradas(
        PokemonFilter filtro,
        int pagina,
        int tamanho)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        if (pagina < 1 || tamanho < 1)
            return Array.Empty<PokemonBoxEntry>();

        int deslocamento;

        try
        {
            deslocamento = checked((pagina - 1) * tamanho);
        }
        catch (OverflowException)
        {
            return Array.Empty<PokemonBoxEntry>();
        }

        using var db = new AppDbContext();

        IQueryable<PokemonForm> formas = FormasDasEspecies(db, filtro);
        PokemonType[] tipos = filtro.Types.Distinct().ToArray();

        if (tipos.Length == 0)
        {
            return formas
                .OrderBy(form => form.Pokemon.NationalDexNumber)
                .ThenBy(form => form.FormName == "Normal" ? 0 : 1)
                .ThenBy(form => form.Id)
                .Skip(deslocamento)
                .Take(tamanho)
                .Select(form => new PokemonBoxEntry(
                    form.Id,
                    form.Pokemon.NationalDexNumber,
                    form.Pokemon.Name,
                    form.FormName))
                .ToList();
        }

        return FiltrarEntradasPorTipos(formas, tipos)
            .Skip(deslocamento)
            .Take(tamanho)
            .ToList();
    }

    // Todas as formas das espécies que passam nos filtros de espécie (os mesmos usados na lista por espécie).
    private static IQueryable<PokemonForm> FormasDasEspecies(
        AppDbContext db,
        PokemonFilter filtro)
    {
        return AplicarFiltrosEscalaveis(db, filtro)
            .SelectMany(pokemon => pokemon.Forms);
    }

    // Os tipos ficam em um campo JSON da forma e não são traduzidos para SQL: o filtro roda em memória
    // (são no máximo algumas centenas de formas), já na ordem final da Box.
    private static List<PokemonBoxEntry> FiltrarEntradasPorTipos(
        IQueryable<PokemonForm> formas,
        IReadOnlyCollection<PokemonType> tipos)
    {
        PokemonType[] tiposSelecionados = tipos.ToArray();

        return formas
            .OrderBy(form => form.Pokemon.NationalDexNumber)
            .ThenBy(form => form.FormName == "Normal" ? 0 : 1)
            .ThenBy(form => form.Id)
            .Select(form => new
            {
                Forma = form,
                form.Pokemon.NationalDexNumber,
                form.Pokemon.Name
            })
            .ToList()
            .Where(item => tiposSelecionados.All(
                tipo => item.Forma.Types.Contains(tipo)))
            .Select(item => new PokemonBoxEntry(
                item.Forma.Id,
                item.NationalDexNumber,
                item.Name,
                item.Forma.FormName))
            .ToList();
    }

    private static IQueryable<Pokemon> AplicarFiltrosEscalaveis(
        AppDbContext db,
        PokemonFilter filtro)
    {
        IQueryable<Pokemon> consulta =
            db.Pokemons.AsNoTracking();

        if (filtro.NationalDexNumber is int numero)
        {
            consulta = consulta.Where(
                pokemon => pokemon.NationalDexNumber == numero);
        }

        if (!string.IsNullOrWhiteSpace(filtro.NameContains))
        {
            string nome = filtro.NameContains.Trim();

            consulta = consulta.Where(
                pokemon => EF.Functions.Like(
                    pokemon.Name,
                    $"%{nome}%"));
        }

        if (filtro.OriginRegionIds is { Count: > 0 })
        {
            int[] regioes =
                filtro.OriginRegionIds.Distinct().ToArray();

            consulta = consulta.Where(
                pokemon => regioes.Contains(pokemon.OriginRegionId));
        }

        if (filtro.OnlyFavorites)
        {
            consulta = consulta.Where(
                pokemon => db.Favorites.Any(
                    favorite => favorite.PokemonId == pokemon.Id));
        }

        return consulta;
    }

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
                tiposSelecionados.All(
                    tipo => form.Types.Contains(tipo))))
            .ToList();
    }

    public string? ObterSpriteUrl(int id)
    {
        using var db = new AppDbContext();

        string? relativePath = db.Sprites
            .AsNoTracking()
            .Where(sprite =>
                sprite.PokemonForm.Pokemon.NationalDexNumber == id)
            .OrderBy(sprite => sprite.Gender == null ? 0 : 1)
            .ThenBy(sprite => sprite.Id)
            .Select(sprite =>
                sprite.BoxDefault ?? sprite.ExhibitionDefault)
            .FirstOrDefault();

        return ResolveAssetUri(relativePath);
    }

    public string? ObterSpriteDetalheUrl(int id, bool shiny)
    {
        using var db = new AppDbContext();

        string? relativePath = db.Sprites
            .AsNoTracking()
            .Where(sprite =>
                sprite.PokemonForm.Pokemon.NationalDexNumber == id)
            .OrderBy(sprite => sprite.Gender == null ? 0 : 1)
            .ThenBy(sprite => sprite.Id)
            .Select(sprite => shiny
                ? (sprite.ExhibitionShiny ?? sprite.ExhibitionDefault)
                : (sprite.ExhibitionDefault ?? sprite.BoxDefault))
            .FirstOrDefault();

        return ResolveAssetUri(relativePath);
    }

    public string? ObterSpriteUrl(PokemonBoxEntry entrada)
    {
        ArgumentNullException.ThrowIfNull(entrada);

        using var db = new AppDbContext();

        string? relativePath = db.Sprites
            .AsNoTracking()
            .Where(sprite => sprite.PokemonFormId == entrada.FormId)
            .OrderBy(sprite => sprite.Gender == null ? 0 : 1)
            .ThenBy(sprite => sprite.Id)
            .Select(sprite =>
                sprite.BoxDefault ?? sprite.ExhibitionDefault)
            .FirstOrDefault();

        // Forma sem arquivo de sprite próprio: a célula continua aparecendo, com o sprite da espécie.
        return ResolveAssetUri(relativePath)
            ?? ObterSpriteUrl(entrada.NationalDexNumber);
    }

    public string? ObterSpriteDetalheUrl(PokemonBoxEntry entrada, bool shiny)
    {
        ArgumentNullException.ThrowIfNull(entrada);

        using var db = new AppDbContext();

        string? relativePath = db.Sprites
            .AsNoTracking()
            .Where(sprite => sprite.PokemonFormId == entrada.FormId)
            .OrderBy(sprite => sprite.Gender == null ? 0 : 1)
            .ThenBy(sprite => sprite.Id)
            .Select(sprite => shiny
                ? (sprite.ExhibitionShiny ?? sprite.ExhibitionDefault)
                : (sprite.ExhibitionDefault ?? sprite.BoxDefault))
            .FirstOrDefault();

        return ResolveAssetUri(relativePath)
            ?? ObterSpriteDetalheUrl(entrada.NationalDexNumber, shiny);
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
