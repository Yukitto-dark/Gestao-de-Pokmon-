using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace GestaoDePokemon.View.Controles
{
    internal static class EdicaoInline
    {
        public static void Nivel(Label label, Action<int> salvar)
        {
            TextBox? editor = null;

            label.Cursor = Cursors.Hand;
            label.MouseLeftButtonUp += (_, _) =>
            {
                if (label.Parent is not Grid grade)
                    return;

                if (editor is null)
                {
                    editor = CriarEditor(grade, label);
                    editor.KeyDown += (_, e) =>
                    {
                        if (e.Key == Key.Enter)
                        {
                            e.Handled = true;
                            Confirmar();
                        }
                        else if (e.Key == Key.Escape)
                        {
                            e.Handled = true;
                            Cancelar();
                        }
                    };
                    editor.LostFocus += (_, _) => Confirmar();
                }

                editor.Text = label.Content?.ToString() ?? string.Empty;
                label.Visibility = Visibility.Collapsed;
                editor.Visibility = Visibility.Visible;
                editor.Focus();
                editor.SelectAll();
            };

            void Confirmar()
            {
                if (editor is null || editor.Visibility != Visibility.Visible)
                    return;

                string texto = editor.Text.Trim();
                editor.Visibility = Visibility.Collapsed;
                label.Visibility = Visibility.Visible;

                // Mensagem, gravação e recarga das páginas só depois do evento de tecla/foco terminar.
                label.Dispatcher.BeginInvoke(() => Aplicar(texto), DispatcherPriority.Background);
            }

            void Aplicar(string texto)
            {
                if (!int.TryParse(texto, NumberStyles.Integer, CultureInfo.InvariantCulture, out int valor)
                    || valor < 1
                    || valor > 100)
                {
                    MessageBox.Show("O nível deve estar entre 1 e 100.", "Time",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (valor.ToString(CultureInfo.InvariantCulture) == label.Content?.ToString())
                    return;

                salvar(valor);
            }

            void Cancelar()
            {
                if (editor is null)
                    return;

                editor.Visibility = Visibility.Collapsed;
                label.Visibility = Visibility.Visible;
            }
        }

        private static TextBox CriarEditor(Grid grade, Label label)
        {
            TextBox editor = new()
            {
                Name = $"{label.Name}_editor",
                Visibility = Visibility.Collapsed,
                FontFamily = label.FontFamily,
                FontSize = label.FontSize,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromRgb(48, 48, 48)),
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1),
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0),
                HorizontalAlignment = label.HorizontalAlignment,
                VerticalAlignment = label.VerticalAlignment,
                Margin = label.Margin,
                Width = label.Width,
                Height = label.Height
            };
            grade.Children.Add(editor);
            return editor;
        }
    }
}
