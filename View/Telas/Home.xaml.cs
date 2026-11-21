using System.Windows;
using System.Windows.Controls;
using GestaoDePokemon.Controller;

namespace GestaoDePokemon.View.Telas
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Navegar_Click(object sender, RoutedEventArgs e)
        {
            NavegacaoController.Navegar(this, sender);
        }
    }
}
