using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using GestaoDePokemon.Data.Filters;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data.Repositories.SQLite;

public sealed class PokemonSqliteRepository : IPokemonRepository
{
    public Pokemon? Obter(int id)
    {
        using var db = new AppDbContext();

        // Consulta dividida: várias coleções incluídas em uma única consulta multiplicariam as linhas
        // (formas x sprites x golpes x habilidades), e os learnsets de todos os jogos são grandes.
        return db.Pokemons
            .AsNoTracking()
            .AsSplitQuery()
            .Include(pokemon => pokemon.OriginRegion)
            .Include(pokemon => pokemon.Forms)
                .ThenInclude(form => form.Sprites)
            .Include(pokemon => pokemon.Forms)
                .ThenInclude(form => form.Learnset)
                    .ThenInclude(learnable => learnable.Move)
            .Include(pokemon => pokemon.Forms)
                .ThenInclude(form => form.Abilities)
                    .ThenInclude(habilidade => habilidade.Ability)
            .Include(pokemon => pokemon.Forms)
                .ThenInclude(form => form.MegaEvolutions)
                    .ThenInclude(mega => mega.MegaForm)
                        .ThenInclude(form => form.Sprites)
            .Include(pokemon => pokemon.Evolutions)
                .ThenInclude(evolucao => evolucao.ToPokemon)
                    .ThenInclude(pokemon => pokemon.Forms)
                        .ThenInclude(form => form.Sprites)
            .Include(pokemon => pokemon.PreEvolutions)
                .ThenInclude(evolucao => evolucao.FromPokemon)
                    .ThenInclude(pokemon => pokemon.Forms)
                        .ThenInclude(form => form.Sprites)
            .SingleOrDefault(pokemon => pokemon.NationalDexNumber == id);
    }

    public IReadOnlyList<Pokemon> ObterLinhaEvolutiva(int id)
    {
        using var db = new AppDbContext();

        if (!db.Pokemons.AsNoTracking().Any(pokemon => pokemon.NationalDexNumber == id))
            return Array.Empty<Pokemon>();

        // Percorre só os pares (origem, destino); as espécies são carregadas no fim, com as formas.
        var relacoes = db.Evolutions
            .AsNoTracking()
            .Select(evolucao => new
            {
                De = evolucao.FromPokemon.NationalDexNumber,
                Para = evolucao.ToPokemon.NationalDexNumber
            })
            .ToList();

        var visitados = new HashSet<int> { id };
        var fila = new Queue<int>();
        fila.Enqueue(id);

        while (fila.Count > 0)
        {
            int atual = fila.Dequeue();

            foreach (var relacao in relacoes)
            {
                if (relacao.De == atual && visitados.Add(relacao.Para))
                    fila.Enqueue(relacao.Para);

                if (relacao.Para == atual && visitados.Add(relacao.De))
                    fila.Enqueue(relacao.De);
            }
        }

        int[] numeros = visitados.ToArray();

        return db.Pokemons
            .AsNoTracking()
            .AsSplitQuery()
            .Include(pokemon => pokemon.Forms)
                .ThenInclude(form => form.Sprites)
            .Where(pokemon => numeros.Contains(pokemon.NationalDexNumber))
            .OrderBy(pokemon => pokemon.NationalDexNumber)
            .ToList();
    }

    public IReadOnlyList<PokemonBoxEntry> ObterEntradas(PokemonFilter filtro)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        using var db = new AppDbContext();

        // Filtros por espécie (número, nome, região e favoritos) rodam no SQL; todas as formas
        // da espécie entram ou saem juntas.
        IQueryable<PokemonForm> formas = AplicarFiltrosEscalaveis(db, filtro)
            .SelectMany(pokemon => pokemon.Forms);

        // O filtro de time vale para a forma: só aparecem as formas que estão no time.
        if (filtro.OnlyParty)
        {
            formas = formas.Where(
                form => db.Party.Any(member => member.PokemonFormId == form.Id));
        }

        IOrderedQueryable<PokemonForm> ordenadas = formas
            .OrderBy(form => form.Pokemon.NationalDexNumber)
            .ThenBy(form => form.FormName == "Normal" ? 0 : 1)
            .ThenBy(form => form.Id);

        if (!RequerDetalhes(filtro))
        {
            return ordenadas
                .Select(form => new PokemonBoxEntry(
                    form.Id,
                    form.Pokemon.NationalDexNumber,
                    form.Pokemon.Name,
                    form.FormName))
                .ToList();
        }

        // Tipos (campo JSON), habilidades e formas alternativas são avaliados em memória.
        List<PokemonForm> candidatas = ordenadas
            .Include(form => form.Pokemon)
            .Include(form => form.Abilities)
                .ThenInclude(formAbility => formAbility.Ability)
            .AsSplitQuery()
            .ToList();

        HashSet<int> especiesComFormas = filtro.HasAlternateForms.HasValue
            ? EspeciesComFormasAlternativas(db, candidatas.Select(form => form.PokemonId).Distinct().ToArray())
            : new HashSet<int>();

        return candidatas
            .Where(form => Corresponde(form, filtro, especiesComFormas))
            .Select(form => new PokemonBoxEntry(
                form.Id,
                form.Pokemon.NationalDexNumber,
                form.Pokemon.Name,
                form.FormName))
            .ToList();
    }

    public IReadOnlyList<string> ObterHabilidades()
    {
        using var db = new AppDbContext();

        return db.Abilities
            .AsNoTracking()
            .OrderBy(ability => ability.Name)
            .Select(ability => ability.Name)
            .ToList();
    }

    private static bool RequerDetalhes(PokemonFilter filtro) =>
        filtro.Types.Count > 0 ||
        filtro.AbilityName is not null ||
        filtro.HasAlternateForms.HasValue;

    // Os filtros de tipo e habilidade valem para cada forma (uma forma de Alola pode ter outro tipo).
    private static bool Corresponde(
        PokemonForm form,
        PokemonFilter filtro,
        HashSet<int> especiesComFormas)
    {
        PokemonType[] tipos = filtro.Types.Distinct().ToArray();

        if (tipos.Length > 0 && !tipos.All(tipo => form.Types.Contains(tipo)))
            return false;

        if (filtro.AbilityName is string nome &&
            !form.Abilities.Any(formAbility =>
                string.Equals(
                    formAbility.Ability.Name,
                    nome,
                    StringComparison.OrdinalIgnoreCase)))
            return false;

        if (filtro.HasAlternateForms is bool comFormas &&
            especiesComFormas.Contains(form.PokemonId) != comFormas)
            return false;

        return true;
    }

    // Espécies que possuem alguma forma alternativa que não seja Mega Evolução.
    private static HashSet<int> EspeciesComFormasAlternativas(AppDbContext db, int[] especies) =>
        db.PokemonForms
            .AsNoTracking()
            .Where(form => especies.Contains(form.PokemonId)
                           && form.FormName != "Normal"
                           && !db.MegaEvolutions.Any(mega => mega.MegaFormId == form.Id))
            .Select(form => form.PokemonId)
            .Distinct()
            .AsEnumerable()
            .ToHashSet();

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

    public string? ObterSpriteUrl(int id, bool shiny = false) => ObterSpriteUri(
        sprite => sprite.PokemonForm.Pokemon.NationalDexNumber == id,
        sprite => shiny
            ? (sprite.BoxShiny ?? sprite.BoxDefault ??
               sprite.ExhibitionShiny ?? sprite.ExhibitionDefault)
            : (sprite.BoxDefault ?? sprite.ExhibitionDefault));

    public string? ObterSpriteDetalheUrl(int id, bool shiny) => ObterSpriteUri(
        sprite => sprite.PokemonForm.Pokemon.NationalDexNumber == id,
        sprite => shiny
            ? (sprite.ExhibitionShiny ?? sprite.ExhibitionDefault)
            : (sprite.ExhibitionDefault ?? sprite.BoxDefault));

    public string? ObterSpriteFormaUrl(int formId, bool shiny = false)
    {
        string? url = ObterSpriteUri(
            sprite => sprite.PokemonFormId == formId,
            sprite => shiny
                ? (sprite.BoxShiny ?? sprite.BoxDefault ??
                   sprite.ExhibitionShiny ?? sprite.ExhibitionDefault)
                : (sprite.BoxDefault ?? sprite.ExhibitionDefault));

        // Forma sem arquivo de sprite próprio: usa o ícone da espécie.
        return url ?? ObterSpriteDaEspecieDaForma(formId, id => ObterSpriteUrl(id, shiny));
    }

    public string? ObterSpriteDetalheFormaUrl(int formId, bool shiny)
    {
        string? url = ObterSpriteUri(
            sprite => sprite.PokemonFormId == formId,
            sprite => shiny
                ? (sprite.ExhibitionShiny ?? sprite.ExhibitionDefault)
                : (sprite.ExhibitionDefault ?? sprite.BoxDefault));

        return url ?? ObterSpriteDaEspecieDaForma(formId, id => ObterSpriteDetalheUrl(id, shiny));
    }

    private static string? ObterSpriteDaEspecieDaForma(int formId, Func<int, string?> obter)
    {
        using var db = new AppDbContext();

        int? numero = db.PokemonForms
            .AsNoTracking()
            .Where(form => form.Id == formId)
            .Select(form => (int?)form.Pokemon.NationalDexNumber)
            .FirstOrDefault();

        return numero is int id ? obter(id) : null;
    }

    private static string? ObterSpriteUri(
        Expression<Func<PokemonSprite, bool>> filtro,
        Expression<Func<PokemonSprite, string?>> selecao)
    {
        using var db = new AppDbContext();

        string? relativePath = db.Sprites
            .AsNoTracking()
            .Where(filtro)
            .OrderBy(sprite => sprite.Gender == null ? 0 : 1)
            .ThenBy(sprite => sprite.Id)
            .Select(selecao)
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
