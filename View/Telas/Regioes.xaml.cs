using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GestaoDePokemon.Controller;
using GestaoDePokemon.model;

namespace GestaoDePokemon.View.Telas
{
    public partial class Regioes : Page
    {
        private readonly RegioesController _controller = new();
        private bool _inicializado;

        public Regioes()
        {
            InitializeComponent();
            Loaded += Regioes_Loaded;
        }

        private void Regioes_Loaded(object sender, RoutedEventArgs e)
        {
            if (_inicializado)
                return;

            _inicializado = true;

            foreach (string regiao in _controller.ObterRegioes())
            {
                var aba = new RadioButton
                {
                    Content = regiao,
                    Tag = regiao,
                    Style = (Style)FindResource("TabRegiao"),
                    Width = 150,
                    Height = 38,
                    Margin = new Thickness(8, 0, 8, 0),
                    GroupName = "Regiao"
                };
                aba.Checked += Regiao_Checked;
                Abas_Regioes.Children.Add(aba);
            }

            if (Abas_Regioes.Children.Count == 0)
                return;

            string selecionada = NavegacaoController.RegiaoSelecionada;
            RadioButton ativa = Abas_Regioes.Children
                .OfType<RadioButton>()
                .FirstOrDefault(aba => string.Equals(
                    aba.Tag as string, selecionada, StringComparison.OrdinalIgnoreCase))
                ?? (RadioButton)Abas_Regioes.Children[0];

            ativa.IsChecked = true;
        }

        private void Regiao_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton aba && Lista_Rotas != null)
                PopularRotas(aba.Tag as string ?? "");
        }

        private void PopularRotas(string regiao)
        {
            Lista_Rotas.Children.Clear();

            Style estilo = (Style)FindResource("BotaoRota");

            foreach (int numero in _controller.ObterRotas(regiao))
            {
                var botao = new Button
                {
                    Content = $"Rota {numero}",
                    Style = estilo,
                    Tag = numero
                };
                botao.Click += Rota_Click;
                Lista_Rotas.Children.Add(botao);
            }

            LimparRota();
            Rotas_Scroll.ScrollToTop();
            Info_Scroll.ScrollToTop();
        }

        private void Rota_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button botao || botao.Tag is not int numero)
                return;

            RotaInfo? rota = _controller.ObterRota(numero);

            Titulo_Rota.Text = $"Rota {numero}";
            Pokemons_Rota.Children.Clear();

            if (rota != null)
            {
                foreach (string url in _controller.ObterSpritesDaRota(rota))
                {
                    Pokemons_Rota.Children.Add(new Image
                    {
                        Source = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute)),
                        Width = 48,
                        Height = 48,
                        Margin = new Thickness(0, 0, 6, 6),
                        Stretch = Stretch.Uniform
                    });
                }
            }

            Informacoes_Rota.Text = rota?.Informacoes ?? string.Empty;
            Info_Scroll.ScrollToTop();
        }

        private void LimparRota()
        {
            Titulo_Rota.Text = "Informações da rota";
            Pokemons_Rota.Children.Clear();
            Informacoes_Rota.Text = string.Empty;
        }

        private void Navegar_Click(object sender, RoutedEventArgs e)
        {
            NavegacaoController.Navegar(this, sender);
        }
    }
}
