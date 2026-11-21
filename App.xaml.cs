using GestaoDePokemon.Data.Persistence.Initialization;
using System;
using System.IO;
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

                if (DatabaseInitializer.Avisos.Count > 0)
                {
                    MessageBox.Show(
                        string.Join("\n\n", DatabaseInitializer.Avisos),
                        "Gestão de Pokémon - Banco de dados",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                string? log = GravarLog(ex);

                MessageBox.Show(
                    $"Não foi possível inicializar o banco de dados.\n\n{ex.Message}" +
                    (log is null ? string.Empty : $"\n\nDetalhes completos em:\n{log}"),
                    "Gestão de Pokémon - Banco de dados",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            base.OnStartup(e);
        }

        // Grava a exceção completa (com as internas e a pilha) ao lado do banco de dados.
        private static string? GravarLog(Exception ex)
        {
            try
            {
                string pasta = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "GestaoDePokemon");
                Directory.CreateDirectory(pasta);

                string caminho = Path.Combine(pasta, "erro-banco.txt");
                File.WriteAllText(caminho, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n{ex}");
                return caminho;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
