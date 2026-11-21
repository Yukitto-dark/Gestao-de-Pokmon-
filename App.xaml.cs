using GestaoDePokemon.Data.Persistence.Initialization;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace GestaoDePokemon
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Um erro inesperado em uma tela não fecha mais o aplicativo: é registrado em erro-app.txt,
            // mostrado ao usuário e o aplicativo continua aberto.
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                    GravarLog(ex, "erro-app.txt");
            };

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
                string? log = GravarLog(ex, "erro-banco.txt");

                MessageBox.Show(
                    $"Não foi possível inicializar o banco de dados.\n\n{ex.Message}" +
                    (log is null ? string.Empty : $"\n\nDetalhes completos em:\n{log}"),
                    "Gestão de Pokémon - Banco de dados",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            string? log = GravarLog(e.Exception, "erro-app.txt");

            try
            {
                MessageBox.Show(
                    $"Ocorreu um erro inesperado, mas o aplicativo continua aberto.\n\n" +
                    $"{DatabaseInitializer.MensagemCompleta(e.Exception)}" +
                    (log is null ? string.Empty : $"\n\nDetalhes completos em:\n{log}"),
                    "Gestão de Pokémon",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception)
            {
                // Se nem a mensagem puder ser exibida (por exemplo, durante o layout), o log já foi gravado.
            }
        }

        // Grava a exceção completa (com as internas e a pilha) ao lado do banco de dados.
        private static string? GravarLog(Exception ex, string arquivo)
        {
            try
            {
                string pasta = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "GestaoDePokemon");
                Directory.CreateDirectory(pasta);

                string caminho = Path.Combine(pasta, arquivo);
                File.AppendAllText(caminho, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n{ex}\n\n");
                return caminho;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
