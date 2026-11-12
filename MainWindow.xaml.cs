using System;
using System.Windows;
using System.Windows.Input;

namespace GestaoDePokemon
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var recurso = Application.GetResourceStream(new Uri("pack://application:,,,/View/cursors/pokecursor.cur"));
            if (recurso != null)
                Mouse.OverrideCursor = new Cursor(recurso.Stream);
        }
    }
}
