using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace GestaoDePokemon
{
    public partial class MainWindow : Window
    {
        private bool _isFullScreen;
        private bool _f11Pressed;
        private bool _layoutRefreshPending;
        private bool _changingFullScreen;

        private Rect _windowedBounds;
        private WindowState _windowedState;
        private WindowStyle _windowedStyle;
        private ResizeMode _windowedResizeMode;

        public MainWindow()
        {
            InitializeComponent();

            var recurso = Application.GetResourceStream(
                new Uri("pack://application:,,,/View/cursors/pokecursor.cur"));

            if (recurso != null)
                Mouse.OverrideCursor = new Cursor(recurso.Stream);

            AtualizarBotaoMaximizarRestaurar();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F11 || _f11Pressed)
                return;

            _f11Pressed = true;
            e.Handled = true;
            AlternarTelaCheia();
        }

        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F11)
                return;

            _f11Pressed = false;
            e.Handled = true;
        }

        private void AlternarTelaCheia()
        {
            if (_isFullScreen)
                SairDaTelaCheia();
            else
                EntrarNaTelaCheia();
        }

        private void EntrarNaTelaCheia()
        {
            _changingFullScreen = true;
            try
            {
                _windowedState = WindowState == WindowState.Maximized
                    ? WindowState.Maximized
                    : WindowState.Normal;

                _windowedBounds = WindowState == WindowState.Maximized
                    ? RestoreBounds
                    : new Rect(Left, Top, ActualWidth, ActualHeight);

                if (_windowedBounds.Width <= 0 || _windowedBounds.Height <= 0)
                    _windowedBounds = new Rect(Left, Top, Width, Height);

                _windowedStyle = WindowStyle;
                _windowedResizeMode = ResizeMode;

                WindowState = WindowState.Normal;
                ResizeMode = ResizeMode.NoResize;
                TitleBar.Visibility = Visibility.Collapsed;
                WindowState = WindowState.Maximized;
                _isFullScreen = true;
            }
            finally
            {
                _changingFullScreen = false;
            }

            AgendarAtualizacaoLayout();
        }

        private void SairDaTelaCheia()
        {
            _changingFullScreen = true;
            try
            {
                WindowState = WindowState.Normal;
                WindowStyle = _windowedStyle;
                ResizeMode = _windowedResizeMode;

                Left = _windowedBounds.Left;
                Top = _windowedBounds.Top;
                Width = _windowedBounds.Width;
                Height = _windowedBounds.Height;

                TitleBar.Visibility = Visibility.Visible;

                if (_windowedState == WindowState.Maximized)
                    WindowState = WindowState.Maximized;

                _isFullScreen = false;
            }
            finally
            {
                _changingFullScreen = false;
            }

            AtualizarBotaoMaximizarRestaurar();
            AgendarAtualizacaoLayout();
        }

        // O botão de maximizar/restaurar é WPF, não o botão não-cliente do Wine.
        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            AlternarJanelaMaximizada();
        }

        private void AlternarJanelaMaximizada()
        {
            if (_isFullScreen)
                return;

            _changingFullScreen = true;
            try
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
            finally
            {
                _changingFullScreen = false;
            }

            AtualizarBotaoMaximizarRestaurar();
            AgendarAtualizacaoLayout();
        }

        private void AtualizarBotaoMaximizarRestaurar()
        {
            if (MaximizeRestoreGlyph == null || MaximizeRestoreButton == null)
                return;

            bool maximizada = WindowState == WindowState.Maximized;
            MaximizeRestoreGlyph.Text = maximizada ? "❐" : "□";
            MaximizeRestoreButton.ToolTip = maximizada ? "Restaurar" : "Maximizar";
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Duplo clique na área livre da barra tem o mesmo comportamento do botão.
        // O teste de ancestral evita tratar duplo clique nos próprios botões.
        private void TitleBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || OrigemDentroDeBotao(e.OriginalSource as DependencyObject))
                return;

            e.Handled = true;
            AlternarJanelaMaximizada();
        }

        private static bool OrigemDentroDeBotao(DependencyObject? elemento)
        {
            while (elemento != null)
            {
                if (elemento is ButtonBase)
                    return true;

                elemento = VisualTreeHelper.GetParent(elemento);
            }

            return false;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AtualizarBotaoMaximizarRestaurar();
            AgendarAtualizacaoLayout();
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            AtualizarBotaoMaximizarRestaurar();
            AgendarAtualizacaoLayout();
        }

        private void MainWindow_Activated(object? sender, EventArgs e)
        {
            AgendarAtualizacaoLayout();
        }

        private void AgendarAtualizacaoLayout()
        {
            if (_layoutRefreshPending || !IsLoaded)
                return;

            _layoutRefreshPending = true;
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
            {
                _layoutRefreshPending = false;

                if (!IsLoaded)
                    return;

                InvalidateMeasure();
                InvalidateArrange();
                MainFrame.InvalidateMeasure();
                MainFrame.InvalidateArrange();

                if (MainFrame.Content is FrameworkElement pagina)
                {
                    pagina.InvalidateMeasure();
                    pagina.InvalidateArrange();
                    pagina.InvalidateVisual();
                }

                MainFrame.UpdateLayout();
                UpdateLayout();
                Mouse.UpdateCursor();
            }));
        }
    }
}
