using System.Windows;
using System.Windows.Controls;
using GestaoDePokemon.Controller;

namespace GestaoDePokemon.View.Telas
{
    public partial class Mapas : Page
    {
        public Mapas()
        {
            InitializeComponent();
        }

        private void Navegar_Click(object sender, RoutedEventArgs e)
        {
            NavegacaoController.Navegar(this, sender);
        }

        private void Regiao_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botao && botao.Tag is string regiao)
                NavegacaoController.NavegarParaRegiao(this, regiao);
        }
    }
}
