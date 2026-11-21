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

        return db.Pokemons
            .AsNoTracking()
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

        Pokemon? inicial = db.Pokemons
            .AsNoTracking()
            .Include(pokemon => pokemon.Forms)
                .ThenInclude(form => form.Sprites)
            .SingleOrDefault(pokemon => pokemon.NationalDexNumber == id);

        if (inicial is null)
            return Array.Empty<Pokemon>();

        List<PokemonEvolution> relacoes = db.Evolutions
            .AsNoTracking()
            .Include(evolucao => evolucao.FromPokemon)
                .ThenInclude(pokemon => pokemon.Forms)
                    .ThenInclude(form => form.Sprites)
            .Include(evolucao => evolucao.ToPokemon)
                .ThenInclude(pokemon => pokemon.Forms)
                    .ThenInclude(form => form.Sprites)
            .ToList();

        var especies = new Dictionary<int, Pokemon> { [id] = inicial };
        var visitados = new HashSet<int> { id };
        var fila = new Queue<int>();
        fila.Enqueue(id);

        while (fila.Count > 0)
        {
            int atual = fila.Dequeue();

            foreach (PokemonEvolution relacao in relacoes)
            {
                if (relacao.FromPokemonId == atual && relacao.ToPokemon is not null)
                    Adicionar(relacao.ToPokemon);

                if (relacao.ToPokemonId == atual && relacao.FromPokemon is not null)
                    Adicionar(relacao.FromPokemon);
            }
        }

        return especies.Values
            .OrderBy(pokemon => pokemon.NationalDexNumber)
            .ToList();

        void Adicionar(Pokemon candidato)
        {
            if (!visitados.Add(candidato.NationalDexNumber))
                return;

            especies[candidato.NationalDexNumber] = candidato;
            fila.Enqueue(candidato.NationalDexNumber);
        }
    }

    public int Contar(PokemonFilter filtro)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        using var db = new AppDbContext();

        return ObterIdsFiltrados(db, filtro).Count;
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

        return ObterIdsFiltrados(db, filtro)
            .Skip(deslocamento)
            .Take(tamanho)
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

    private static IReadOnlyList<int> ObterIdsFiltrados(
        AppDbContext db,
        PokemonFilter filtro)
    {
        IQueryable<Pokemon> consulta =
            AplicarFiltrosEscalaveis(db, filtro);

        if (!RequerDetalhes(filtro))
        {
            return consulta
                .OrderBy(pokemon => pokemon.NationalDexNumber)
                .Select(pokemon => pokemon.NationalDexNumber)
                .ToList();
        }

        return consulta
            .Include(pokemon => pokemon.Forms)
                .ThenInclude(form => form.Abilities)
                    .ThenInclude(formAbility => formAbility.Ability)
            .Include(pokemon => pokemon.Forms)
                .ThenInclude(form => form.MegaEvolutionOrigin)
            .AsEnumerable()
            .Where(pokemon => Corresponde(pokemon, filtro))
            .OrderBy(pokemon => pokemon.NationalDexNumber)
            .Select(pokemon => pokemon.NationalDexNumber)
            .ToList();
    }

    private static bool RequerDetalhes(PokemonFilter filtro) =>
        filtro.Types.Count > 0 ||
        filtro.AbilityName is not null ||
        filtro.HasAlternateForms.HasValue;

    private static bool Corresponde(Pokemon pokemon, PokemonFilter filtro)
    {
        PokemonType[] tipos = filtro.Types.Distinct().ToArray();

        if (tipos.Length > 0 &&
            !pokemon.Forms.Any(form => tipos.All(tipo => form.Types.Contains(tipo))))
            return false;

        if (filtro.AbilityName is string nome &&
            !pokemon.Forms.Any(form => form.Abilities.Any(formAbility =>
                string.Equals(
                    formAbility.Ability.Name,
                    nome,
                    StringComparison.OrdinalIgnoreCase))))
            return false;

        if (filtro.HasAlternateForms is bool comFormas)
        {
            bool temFormas = pokemon.Forms.Any(form =>
                form.MegaEvolutionOrigin is null &&
                !string.Equals(
                    form.FormName,
                    "Normal",
                    StringComparison.OrdinalIgnoreCase));

            if (temFormas != comFormas)
                return false;
        }

        return true;
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

        if (filtro.OnlyParty)
        {
            consulta = consulta.Where(
                pokemon => db.Party.Any(
                    member => member.PokemonId == pokemon.Id));
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

    public string? ObterSpriteFormaUrl(int formId) => ObterSpriteUri(
        sprite => sprite.PokemonFormId == formId,
        sprite => sprite.BoxDefault ?? sprite.ExhibitionDefault);

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
