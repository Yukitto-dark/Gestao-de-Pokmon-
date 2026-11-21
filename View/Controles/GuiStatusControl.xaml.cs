using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GestaoDePokemon.Controller;
using GestaoDePokemon.model;

namespace GestaoDePokemon.View.Controles
{
    public partial class GuiStatusControl : UserControl
    {
        private static readonly double[] LinhasRotulo = { 25, 90, 140, 194, 254, 308 };

        private readonly TextBox _editor;
        private readonly Label[] _bases;
        private readonly Label[] _stats;
        private readonly Label[] _evs;
        private Label? _labelEditando;
        private PokemonStat _statEditando;
        private PokemonSetController.SetInfo? _info;
        private int _membroId;
        private PokemonNature _naturezaInicial;
        private bool _editandoNatureza;
        private bool _carregandoNatureza;

        public event Action? IrResumo;
        public event Action? IrMoves;
        public event Action? VoltarTime;
        public event Action? SetAlterado;

        public GuiStatusControl()
        {
            InitializeComponent();
            Ir_summary.Click += (_, _) => IrResumo?.Invoke();
            Ir_moves.Click += (_, _) => IrMoves?.Invoke();
            Voltar_Time.Click += (_, _) => VoltarTime?.Invoke();

            // A fonte é copiada do rótulo ao abrir a edição (ver AbrirEdicao): um FontFamily criado em código
            // com caminho relativo ("/View/fonts/#...") não tem a URI base que o XAML resolve.
            _editor = new TextBox
            {
                Name = "EV_editor",
                Visibility = Visibility.Collapsed,
                FontSize = 30,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromRgb(48, 48, 48)),
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(1),
                TextAlignment = TextAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0)
            };
            _editor.KeyDown += Editor_KeyDown;
            _editor.LostFocus += (_, _) => ConfirmarEdicao();
            Raiz.Children.Add(_editor);

            // Cada linha: base stat da espécie, atributo com os EVs aplicados e os EVs investidos.
            // Clicar em qualquer um deles (ou no nome do atributo) edita os EVs.
            _bases = new[] { Base_hp, Base_attack, Base_defense, Base_special_attack, Base_special_defense, Base_speed };
            _stats = new[] { Stat_hp, Stat_attack, Stat_defense, Stat_special_attack, Stat_special_defense, Stat_speed };
            _evs = new[] { EV_hp, EV_attack, EV_defense, EV_special_attack, EV_special_defense, EV_speed };
            for (int i = 0; i < _evs.Length; i++)
            {
                Label label = _evs[i];
                PokemonStat stat = EffortValues.Estatisticas[i];

                foreach (Label clicavel in new[] { _bases[i], _stats[i], _evs[i] })
                {
                    clicavel.Cursor = Cursors.Hand;
                    clicavel.MouseLeftButtonUp += (_, _) => AbrirEdicao(label, stat);
                }

                Button hotspot = new()
                {
                    Name = $"EV_rotulo_{stat}",
                    Opacity = 0,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 30,
                    Width = 180,
                    Margin = new Thickness(268, LinhasRotulo[i], 0, 0),
                    ToolTip = $"Editar os EVs de {GuiDados.NomeCurto(stat)}"
                };
                hotspot.Click += (_, _) => AbrirEdicao(label, stat);
                Camada.Children.Add(hotspot);
            }

            EdicaoInline.Nivel(lvl_pokemon, valor =>
            {
                try
                {
                    new PokemonSetController().AtualizarNivel(_membroId, valor);
                    lvl_pokemon.Content = valor;
                    SetAlterado?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Não foi possível salvar o nível: {ex.Message}", "Time",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            Nature_select.ItemsSource = Enum.GetValues<PokemonNature>();
            Nature_pokemon.Cursor = Cursors.Hand;
            Nature_pokemon.MouseLeftButtonUp += (_, _) => AbrirNatureza();
            // O ComboBox é escondido e a natureza é gravada depois que o evento dele termina: esconder o
            // controle e recarregar as páginas dentro do próprio SelectionChanged/DropDownClosed (com o
            // popup ainda fechando) pode derrubar o aplicativo, principalmente no Wine.
            Nature_select.DropDownClosed += (_, _) => Adiar(FecharNatureza);
            Nature_select.SelectionChanged += (_, _) =>
            {
                if (_editandoNatureza && !_carregandoNatureza)
                    Adiar(FecharNatureza);
            };
        }

        public void Carregar(PokemonSetController.SetInfo info, bool shiny)
        {
            _info = info;
            _membroId = info.MembroId;

            Id_pokemon.Content = GuiDados.Id(info);
            Nome_pokemon.Content = GuiDados.Nome(info);
            Nome_pokemon.ToolTip = GuiDados.NomeCompleto(info);
            lvl_pokemon.Content = info.Level.ToString();

            for (int i = 0; i < _evs.Length; i++)
            {
                PokemonStat stat = EffortValues.Estatisticas[i];
                int baseStat = GuiDados.BaseStat(info.Stats, stat);
                int ev = info.EVs.GetValue(stat);
                int valor = GuiDados.StatNoNivel(info, stat);

                _bases[i].Content = baseStat;
                _stats[i].Content = valor;
                _evs[i].Content = $"+{ev}";

                string dica =
                    $"{GuiDados.NomeCurto(stat)}: base {baseStat}, {ev} EVs, IV {GuiDados.Iv(info.IVs, stat)}, " +
                    $"natureza {info.Nature}\n" +
                    $"Valor no nível {info.Level}: {valor}\n" +
                    $"Clique para mudar os EVs (0 a {EffortValues.MaxPerStat}).";
                _bases[i].ToolTip = $"Base stat de {GuiDados.NomeCurto(stat)}\n{dica}";
                _stats[i].ToolTip = $"{GuiDados.NomeCurto(stat)} com os EVs aplicados\n{dica}";
                _evs[i].ToolTip = $"EVs investidos em {GuiDados.NomeCurto(stat)}\n{dica}";
            }
            AtualizarDisponivel(info.EVs);

            Nature_pokemon.Content = info.Nature.ToString();
            Nature_effect.Content = GuiDados.NaturezaEfeito(info.Nature);
            happiness_pokemon.Content = "—";
            Size_A.Content = info.Height is double altura
                ? altura.ToString("0.0", CultureInfo.InvariantCulture) + " m"
                : "—";
            Size_subA.Content = info.Weight is double peso
                ? peso.ToString("0.0", CultureInfo.InvariantCulture) + " kg"
                : "—";

            GuiDados.Tipos(Type_pokemon, Type_pokemon2, info.Types, 37, 172);

            GuiDados.Sprite(Imagem_pokemon, info.SpriteUrl);
        }

        private void AbrirEdicao(Label label, PokemonStat stat)
        {
            if (_info is null)
                return;

            ConfirmarEdicao();

            _labelEditando = label;
            _statEditando = stat;
            // O editor abre sobre o número de EVs da linha.
            _editor.Text = _info.EVs.GetValue(stat).ToString(CultureInfo.InvariantCulture);
            _editor.FontFamily = label.FontFamily;
            _editor.FontSize = label.FontSize + 4;
            _editor.HorizontalAlignment = label.HorizontalAlignment;
            _editor.VerticalAlignment = label.VerticalAlignment;
            _editor.Margin = label.Margin;
            _editor.Width = label.Width;
            _editor.Height = label.Height;
            label.Visibility = Visibility.Collapsed;
            _editor.Visibility = Visibility.Visible;
            _editor.Focus();
            _editor.SelectAll();
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                ConfirmarEdicao();
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CancelarEdicao();
            }
        }

        // Executa a ação depois que o evento atual (tecla, foco, seleção) termina de ser processado.
        private void Adiar(Action acao) =>
            Dispatcher.BeginInvoke(acao, DispatcherPriority.Background);

        private void ConfirmarEdicao()
        {
            if (_labelEditando is null || _info is null)
                return;

            Label label = _labelEditando;
            PokemonStat stat = _statEditando;
            string texto = _editor.Text.Trim();

            _labelEditando = null;
            _editor.Visibility = Visibility.Collapsed;
            label.Visibility = Visibility.Visible;

            // Validação, gravação e recarga das páginas ficam para depois do evento de tecla/foco:
            // abrir uma mensagem ou recarregar as páginas no meio dele derrubava o aplicativo.
            Adiar(() => AplicarEV(label, stat, texto));
        }

        private void AplicarEV(Label label, PokemonStat stat, string texto)
        {
            if (_info is null)
                return;

            // O valor digitado são os EVs do atributo (0 a 252), com o limite de 510 no total.
            string nome = GuiDados.NomeCurto(stat);

            if (!int.TryParse(texto, NumberStyles.Integer, CultureInfo.InvariantCulture, out int ev)
                || ev < 0
                || ev > EffortValues.MaxPerStat)
            {
                MessageBox.Show(
                    $"Os EVs de {nome} devem ficar entre 0 e {EffortValues.MaxPerStat}.",
                    "Time", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ev == _info.EVs.GetValue(stat))
                return;

            int disponivel = EffortValues.MaxTotal - _info.EVs.Total;
            if (ev - _info.EVs.GetValue(stat) > disponivel)
            {
                MessageBox.Show(
                    $"O total de EVs não pode ultrapassar {EffortValues.MaxTotal}. " +
                    $"Ainda há {disponivel} EVs disponíveis: {nome} pode ter até {_info.EVs.GetValue(stat) + disponivel} EVs.",
                    "Time", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                EffortValues evs = new PokemonSetController()
                    .AtualizarEV(_info.MembroId, stat, ev);
                label.Content = $"+{ev}";
                AtualizarDisponivel(evs);
                SetAlterado?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Não foi possível salvar o EV: {ex.Message}", "Time",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelarEdicao()
        {
            if (_labelEditando is null)
                return;

            _labelEditando.Visibility = Visibility.Visible;
            _labelEditando = null;
            _editor.Visibility = Visibility.Collapsed;
        }

        private void AbrirNatureza()
        {
            if (_info is null)
                return;

            _naturezaInicial = _info.Nature;
            _editandoNatureza = true;
            _carregandoNatureza = true;
            Nature_select.SelectedItem = _naturezaInicial;
            _carregandoNatureza = false;
            Nature_pokemon.Visibility = Visibility.Collapsed;
            Nature_select.Visibility = Visibility.Visible;
            Nature_select.Focus();
            Nature_select.IsDropDownOpen = true;
        }

        private void FecharNatureza()
        {
            if (!_editandoNatureza)
                return;

            _editandoNatureza = false;
            Nature_select.IsDropDownOpen = false;
            Nature_select.Visibility = Visibility.Collapsed;
            Nature_pokemon.Visibility = Visibility.Visible;

            if (_info is null || Nature_select.SelectedItem is not PokemonNature nature)
                return;

            if (nature == _naturezaInicial)
                return;

            try
            {
                new PokemonSetController().AtualizarNatureza(_membroId, nature);
                Nature_pokemon.Content = nature;
                SetAlterado?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Não foi possível salvar a natureza: {ex.Message}", "Time",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AtualizarDisponivel(EffortValues evs) =>
            EVs_disponiveis.Content = $"EVs disponíveis: {EffortValues.MaxTotal - evs.Total}";
    }
}
