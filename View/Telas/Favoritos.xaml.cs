using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GestaoDePokemon.Controller;

namespace GestaoDePokemon.View.Telas
{
    /// <summary>
    /// Interação lógica para Favoritos.xam
    /// </summary>
    public partial class Favoritos : Page
    {
        private readonly FavoritosController _controller = new();

        public Favoritos()
        {
            InitializeComponent();
            Loaded += Favoritos_Loaded;
        }

        private void Favoritos_Loaded(object sender, RoutedEventArgs e)
        {
            Image[] imagens =
            {
                Favorito_1, Favorito_2, Favorito_3,
                Favorito_4, Favorito_5, Favorito_6
            };

            IReadOnlyList<int> ids = _controller.ObterFavoritos();

            for (int i = 0; i < imagens.Length; i++)
            {
                if (i >= ids.Count)
                {
                    imagens[i].Source = null;
                    continue;
                }

                string? url = _controller.ObterSpriteUrl(ids[i]);
                imagens[i].Source = url is null
                    ? null
                    : new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            }
        }

        private void Navegar_Click(object sender, RoutedEventArgs e)
        {
            NavegacaoController.Navegar(this, sender);
        }
    }
}
