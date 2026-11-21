using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GestaoDePokemon.Controller;
using GestaoDePokemon.Data.Repositories.Contracts;

namespace GestaoDePokemon.View.Telas
{
    public partial class Time : Page
    {
        private readonly TimeController _controller = new();
        private IReadOnlyList<MembroTime> _membros = Array.Empty<MembroTime>();

        public Time()
        {
            InitializeComponent();
            Loaded += Time_Loaded;
            SetPopup.Fechado += () => SetPopup.Visibility = Visibility.Collapsed;
        }

        private void Time_Loaded(object sender, RoutedEventArgs e)
        {
            Image[] imagens =
            {
                Party_1, Party_2, Party_3,
                Party_4, Party_5, Party_6
            };

            _membros = _controller.ObterTime();

            for (int i = 0; i < imagens.Length; i++)
            {
                if (i >= _membros.Count)
                {
                    imagens[i].Source = null;
                    imagens[i].ToolTip = null;
                    continue;
                }

                MembroTime membro = _membros[i];
                string? url = _controller.ObterSpriteUrl(membro);
                imagens[i].Source = url is null
                    ? null
                    : new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
                imagens[i].ToolTip = membro.Shiny
                    ? $"#{membro.NationalDexNumber} (shiny)"
                    : $"#{membro.NationalDexNumber}";
            }
        }

        private void Party_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image imagem ||
                !int.TryParse(imagem.Tag?.ToString(), out int indice))
                return;

            if (indice < 1 || indice > _membros.Count)
                return;

            SetPopup.Abrir(_membros[indice - 1]);
        }

        private void Navegar_Click(object sender, RoutedEventArgs e)
        {
            NavegacaoController.Navegar(this, sender);
        }
    }
}
