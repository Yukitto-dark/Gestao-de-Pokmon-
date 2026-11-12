using System;
using System.Windows.Controls;

namespace GestaoDePokemon.Controller
{
    public static class NavegacaoController
    {
        public static string RegiaoSelecionada { get; private set; } = "Kanto";

        public static void Navegar(Page page, object sender)
        {
            if (sender is not Button button || string.IsNullOrWhiteSpace(button.Name))
                return;

            string nome = button.Name.ToLowerInvariant();
            string? destino = null;

            if (nome.Contains("home"))
                destino = "/View/Telas/Home.xaml";
            else if (nome.Contains("box"))
                destino = "/View/Telas/Box.xaml";
            else if (nome.Contains("favoritos"))
                destino = "/View/Telas/Favoritos.xaml";
            else if (nome.Contains("mapas"))
                destino = "/View/Telas/Mapas.xaml";

            if (destino != null)
                page.NavigationService?.Navigate(new Uri(destino, UriKind.Relative));
        }

        public static void NavegarParaRegiao(Page page, string regiao)
        {
            if (string.IsNullOrWhiteSpace(regiao))
                return;

            RegiaoSelecionada = regiao;
            page.NavigationService?.Navigate(
                new Uri("/View/Telas/Regioes.xaml", UriKind.Relative));
        }
    }
}
