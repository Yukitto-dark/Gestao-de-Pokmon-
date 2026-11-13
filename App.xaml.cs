using GestaoDePokemon.Database;
using System;
using System.Windows;

namespace GestaoDePokemon
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                DatabaseInitializer.Inicializar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Não foi possível inicializar o banco de dados.\n\n{ex.Message}",
                    "Gestão de Pokémon - Banco de dados",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            base.OnStartup(e);
        }
    }
}
