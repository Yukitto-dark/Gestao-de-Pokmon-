using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GestaoDePokemon.Controller;
using GestaoDePokemon.Data.Repositories.Contracts;

namespace GestaoDePokemon.View.Controles
{
    public partial class PokemonSetPopup : UserControl
    {
        private static readonly (int Largura, int Altura)[] Dimensoes =
        {
            (827, 657), (703, 559), (895, 664)
        };

        private readonly GuiSummaryControl _resumo = new();
        private readonly GuiStatusControl _status = new();
        private readonly GuiMovesControl _moves = new();

        private int _membroId;
        private bool _shiny;
        private int _paginaAtual;

        public event Action? Fechado;

        public PokemonSetPopup()
        {
            InitializeComponent();

            _resumo.IrStatus += () => MostrarPagina(1);
            _resumo.IrMoves += () => MostrarPagina(2);
            _resumo.VoltarTime += Fechar;
            _resumo.SetAlterado += Recarregar;

            _status.IrResumo += () => MostrarPagina(0);
            _status.IrMoves += () => MostrarPagina(2);
            _status.VoltarTime += Fechar;
            _status.SetAlterado += Recarregar;

            _moves.IrResumo += () => MostrarPagina(0);
            _moves.IrStatus += () => MostrarPagina(1);
            _moves.VoltarTime += Fechar;
            _moves.SetAlterado += Recarregar;
        }

        public bool Abrir(MembroTime membro)
        {
            _resumo.ConfirmarNivelPendente();

            _membroId = membro.MembroId;
            _shiny = membro.Shiny;

            PokemonSetController.SetInfo? info = new PokemonSetController().Obter(_membroId, _shiny);

            if (info is null)
            {
                MessageBox.Show("Não foi possível abrir os dados deste Pokémon.", "Time",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            Carregar(info);
            _paginaAtual = 0;
            MostrarPagina(0);
            Visibility = Visibility.Visible;
            return true;
        }

        private void Recarregar()
        {
            PokemonSetController.SetInfo? info = new PokemonSetController().Obter(_membroId, _shiny);
            if (info is null)
                return;

            Carregar(info);
            MostrarPagina(_paginaAtual);
        }

        private void Carregar(PokemonSetController.SetInfo info)
        {
            _resumo.Carregar(info, _shiny);
            _status.Carregar(info, _shiny);
            _moves.Carregar(info, _shiny);
        }

        private void MostrarPagina(int indice)
        {
            _paginaAtual = indice;
            (int largura, int altura) = Dimensoes[indice];
            object pagina = indice switch
            {
                1 => _status,
                2 => _moves,
                _ => _resumo
            };
            if (!ReferenceEquals(Conteudo.Content, pagina))
                Conteudo.Content = pagina;
            Painel.Width = largura;
            Painel.Height = altura;
        }

        private void Fechar()
        {
            _resumo.ConfirmarNivelPendente();
            Fechado?.Invoke();
        }

        private void Fundo_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == Raiz)
                Fechar();
        }
    }
}
