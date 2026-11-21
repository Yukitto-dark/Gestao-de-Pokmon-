using System;
using System.IO;
using System.Linq;
using GestaoDePokemon.Data.Persistence;
using GestaoDePokemon.model;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePokemon.Data.Persistence.Initialization;

/// <summary>
/// Ícone da Box próprio de uma forma. As espécies não precisam disto (o caminho mini/{dex}.png não
/// muda; o arquivo é trocado no lugar), mas uma forma que antes não tinha ícone próprio foi gravada
/// apontando para o ícone da espécie. Se agora existe forms/{dex}/{forma}/box.png ao lado da arte
/// frontal dela, BoxDefault passa a apontar para esse arquivo. Formas que já têm ícone próprio ou
/// que continuam sem arquivo próprio não são alteradas.
/// </summary>
public static class BoxIconsInitializer
{
    private const string SpeciesIconFolder = "Data/images/pokemon/mini/";

    public static void Inicializar(AppDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);

        var sprites = db.Sprites
            .Include(sprite => sprite.PokemonForm)
            .Where(sprite => sprite.ExhibitionDefault != null
                             && sprite.PokemonForm.FormName != "Normal"
                             && (sprite.BoxDefault == null || sprite.BoxDefault.StartsWith(SpeciesIconFolder)))
            .ToList();

        foreach (PokemonSprite sprite in sprites)
        {
            string front = sprite.ExhibitionDefault!;
            int slash = front.LastIndexOf('/');
            if (slash <= 0)
                continue;

            string ownIcon = front[..slash] + "/box.png";
            string diskPath = Path.Combine(
                AppContext.BaseDirectory,
                ownIcon.Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(diskPath))
                sprite.BoxDefault = ownIcon;
        }
    }
}
