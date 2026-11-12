using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GestaoDePokemon.Controller;

namespace GestaoDePokemon.View.Telas
{
    /// <summary>
    /// Interação lógica para Box.xam
    /// </summary>
    public partial class Box : Page
    {
        private readonly BoxController _controller = new();
        private readonly Image[] _slots;

        private static readonly BitmapImage CoracaoCheio = new(new Uri("pack://application:,,,/View/images/hert-full.png"));
        private static readonly BitmapImage CoracaoVazio = new(new Uri("pack://application:,,,/View/images/hert-full-vazio.png"));

        public Box()
        {
            InitializeComponent();
            _slots = new[]
            {
                _1, _2, _3, _4, _5, _6, _7, _8, _9, _10,
                _11, _12, _13, _14, _15, _16, _17, _18, _19, _20,
                _21, _22, _23, _24, _25, _26, _27, _28, _29, _30
            };
            AtualizarPagina();
        }

        private void Navegar_Click(object sender, RoutedEventArgs e)
        {
            NavegacaoController.Navegar(this, sender);
        }

        private void Filtros_Click(object sender, RoutedEventArgs e)
        {
            PainelFiltros.Visibility = PainelFiltros.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Por_Type_Click(object sender, RoutedEventArgs e)
        {
            Types.Visibility = Types.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Por_Regiao_Click(object sender, RoutedEventArgs e)
        {
            Regioes.Visibility = Regioes.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Por_Favoritos_Click(object sender, RoutedEventArgs e)
        {
            Painel_Favoritos.Visibility = Painel_Favoritos.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Por_Id_Click(object sender, RoutedEventArgs e)
        {
            Por_Id.Visibility = Visibility.Collapsed;
            Id_TextBox.Visibility = Visibility.Visible;
            Id_TextBox.Focus();
        }

        private void Por_Nome_Click(object sender, RoutedEventArgs e)
        {
            Por_Nome.Visibility = Visibility.Collapsed;
            Nome_TextBox.Visibility = Visibility.Visible;
            Nome_TextBox.Focus();
        }

        private void Limpar_Filtros_Click(object sender, RoutedEventArgs e)
        {
            Id_TextBox.Text = string.Empty;
            Nome_TextBox.Text = string.Empty;

            Regiao_Kanto.IsChecked = true;
            Regiao_Johto.IsChecked = true;
            Regiao_Hoenn.IsChecked = true;

            foreach (UIElement filho in Tipos_Hotspots.Children)
            {
                if (filho is System.Windows.Controls.Primitives.ToggleButton tipo)
                    tipo.IsChecked = false;
            }

            Types.Visibility = Visibility.Collapsed;
            Regioes.Visibility = Visibility.Collapsed;
            Painel_Favoritos.Visibility = Visibility.Collapsed;
            Filtro_Favoritos.IsChecked = false;

            Id_TextBox.Visibility = Visibility.Collapsed;
            Nome_TextBox.Visibility = Visibility.Collapsed;
            Por_Id.Visibility = Visibility.Visible;
            Por_Nome.Visibility = Visibility.Visible;
        }

        private void Aplicar_Filtros_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AtualizarPagina()
        {
            Numeração_Box.Content = $"{_controller.PaginaAtual} - {_controller.TotalPaginas}";

            IReadOnlyList<int> ids = _controller.ObterIdsDaPagina();

            for (int i = 0; i < _slots.Length; i++)
            {
                if (i < ids.Count)
                {
                    _slots[i].Tag = ids[i];
                    string? url = _controller.ObterSpriteUrl(ids[i]);
                    _slots[i].Source = url is null
                        ? null
                        : new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    _slots[i].Tag = null;
                    _slots[i].Source = null;
                }
            }

            _controller.LimparSelecao();
            Imagem_Pokemon.Source = null;
            Botao_Favorito.IsEnabled = false;
            Selecao_Slot.Visibility = Visibility.Collapsed;
            AtualizarCoracao();
        }

        private void Grade_Click(object sender, MouseButtonEventArgs e)
        {
            Point ponto = e.GetPosition((IInputElement)sender);

            if (ponto.X < 247 || ponto.X >= 626 || ponto.Y < 121 || ponto.Y >= 417)
                return;

            int coluna = Math.Clamp((int)((ponto.X - 247) / 63), 0, 5);
            int linha = Math.Clamp((int)((ponto.Y - 121) / 59), 0, 4);
            int indice = linha * 6 + coluna;

            int? id = _controller.Selecionar(coluna, linha);

            if (id is null)
                return;

            Imagem_Pokemon.Source = _slots[indice].Source;
            Botao_Favorito.IsEnabled = true;
            Selecao_Slot.Margin = new Thickness(247 + coluna * 63, 121 + linha * 59, 0, 0);
            Selecao_Slot.Visibility = Visibility.Visible;
            AtualizarCoracao();
        }

        private void Favorito_Click(object sender, RoutedEventArgs e)
        {
            ResultadoFavorito resultado = _controller.AlternarFavorito(out _);

            if (resultado is ResultadoFavorito.SemSelecao)
            {
                Botao_Favorito.IsChecked = false;
                return;
            }

            if (resultado is ResultadoFavorito.LimiteAtingido)
            {
                Botao_Favorito.IsChecked = false;
                MessageBox.Show("Limite de 6 favoritos atingido.", "Favoritos", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            AtualizarCoracao();
        }

        private void AtualizarCoracao()
        {
            if (_controller.IdSelecionado is not int id)
            {
                Botao_Favorito.IsChecked = false;
                Icone_Favorito.Source = CoracaoVazio;
                Botao_Favorito.ToolTip = "Selecione um Pokémon";
                return;
            }

            bool favorito = _controller.EhFavorito(id);
            Botao_Favorito.IsChecked = favorito;
            Icone_Favorito.Source = favorito ? CoracaoCheio : CoracaoVazio;
            Botao_Favorito.ToolTip = favorito ? $"Remover {id} dos favoritos" : $"Favoritar {id}";
        }

        private void Prox_Box_Click(object sender, RoutedEventArgs e)
        {
            _controller.IrParaPagina(1);
            AtualizarPagina();
        }

        private void Ant_Box_Click(object sender, RoutedEventArgs e)
        {
            _controller.IrParaPagina(-1);
            AtualizarPagina();
        }
    }
}
