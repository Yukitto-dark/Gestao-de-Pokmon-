using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GestaoDePokemon.Controller;
using GestaoDePokemon.Data.Configuration;
using GestaoDePokemon.Data.Filters;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.Data.Repositories.InMemory;
using GestaoDePokemon.Data.Repositories.SQLite;
using GestaoDePokemon.model;
using System.Windows.Controls.Primitives;

namespace GestaoDePokemon.View.Telas
{
    /// <summary>
    /// Interação lógica para Box.xam
    /// </summary>
    public partial class Box : Page
    {
        private readonly BoxController _controller = new();
        private readonly Image[] _slots;
        private bool _mostrarShiny;

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

            // Nenhuma região marcada significa mostrar todas as regiões.
            Regiao_Kanto.IsChecked = false;
            Regiao_Johto.IsChecked = false;
            Regiao_Hoenn.IsChecked = false;

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

            _controller.LimparFiltros();
            AtualizarPagina();
        }

        private void Aplicar_Filtros_Click(object sender, RoutedEventArgs e)
        {
            int? numeroDex = null;
            string textoId = Id_TextBox.Text.Trim();

            if (textoId.Length > 0)
            {
                if (!int.TryParse(textoId, out int id) || id < 1)
                {
                    MessageBox.Show(
                        "Informe um número de Pokédex válido.",
                        "Filtro por número",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    Id_TextBox.Focus();
                    return;
                }

                numeroDex = id;
            }

            var tiposSelecionados = new List<PokemonType>();
            foreach (ToggleButton hotspot in Tipos_Hotspots.Children.OfType<ToggleButton>())
            {
                if (hotspot.IsChecked != true)
                    continue;

                string? nomeTipo = hotspot.ToolTip?.ToString();
                if (Enum.TryParse(nomeTipo, ignoreCase: true, out PokemonType tipo) &&
                    !tiposSelecionados.Contains(tipo))
                {
                    tiposSelecionados.Add(tipo);
                }
            }

            var regioesSelecionadas = new List<int>();
            if (Regiao_Kanto.IsChecked == true) regioesSelecionadas.Add(1);
            if (Regiao_Johto.IsChecked == true) regioesSelecionadas.Add(2);
            if (Regiao_Hoenn.IsChecked == true) regioesSelecionadas.Add(3);

            string? nome = string.IsNullOrWhiteSpace(Nome_TextBox.Text)
                ? null
                : Nome_TextBox.Text.Trim();

            _controller.AplicarFiltros(new PokemonFilter
            {
                Types = tiposSelecionados,
                // Lista vazia = sem filtro regional; uma ou mais marcadas =
                // somente Pokémon das regiões selecionadas.
                OriginRegionIds = regioesSelecionadas,
                OnlyFavorites = Filtro_Favoritos.IsChecked == true,
                NationalDexNumber = numeroDex,
                NameContains = nome
            });

            AtualizarPagina();
            PainelFiltros.Visibility = Visibility.Collapsed;
        }

        private void AtualizarPagina()
        {
            int totalResultados = _controller.TotalResultados;
            Numeração_Box.Content = totalResultados == 0
                ? "0 - 0"
                : $"{_controller.PaginaAtual} - {_controller.TotalPaginas}";
            Numeração_Box.ToolTip = totalResultados == 0
                ? "Nenhum Pokémon encontrado com os filtros selecionados."
                : $"{totalResultados} Pokémon encontrados.";

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
            _mostrarShiny = false;
            Imagem_Pokemon.Source = null;
            Shine.IsEnabled = false;
            Shine.ToolTip = "Selecione um Pokémon";
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
            int? id = _controller.Selecionar(coluna, linha);

            if (id is null)
            {
                _mostrarShiny = false;
                Imagem_Pokemon.Source = null;
                Shine.IsEnabled = false;
                Shine.ToolTip = "Selecione um Pokémon";
                Botao_Favorito.IsEnabled = false;
                Selecao_Slot.Visibility = Visibility.Collapsed;
                AtualizarCoracao();
                return;
            }

            _mostrarShiny = false;
            AtualizarImagemDetalhe();
            Shine.IsEnabled = true;
            Shine.ToolTip = "Exibir sprite shiny";
            Botao_Favorito.IsEnabled = true;
            Selecao_Slot.Margin = new Thickness(247 + coluna * 63, 121 + linha * 59, 0, 0);
            Selecao_Slot.Visibility = Visibility.Visible;
            AtualizarCoracao();
        }

        private void Shine_Click(object sender, RoutedEventArgs e)
        {
            if (_controller.IdSelecionado is null)
                return;

            _mostrarShiny = !_mostrarShiny;
            AtualizarImagemDetalhe();
            Shine.ToolTip = _mostrarShiny
                ? "Exibir sprite normal"
                : "Exibir sprite shiny";
        }

        private void AtualizarImagemDetalhe()
        {
            if (_controller.IdSelecionado is not int id)
            {
                Imagem_Pokemon.Source = null;
                return;
            }

            string? url = _controller.ObterSpriteDetalheUrl(id, _mostrarShiny);
            Imagem_Pokemon.Source = url is null
                ? null
                : new BitmapImage(new Uri(url, UriKind.Absolute));
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

            if (Filtro_Favoritos.IsChecked == true)
                AtualizarPagina();
            else
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
