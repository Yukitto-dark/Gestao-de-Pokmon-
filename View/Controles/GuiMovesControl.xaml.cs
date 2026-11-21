using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GestaoDePokemon.Controller;
using GestaoDePokemon.model;

namespace GestaoDePokemon.View.Controles
{
    public partial class GuiMovesControl : UserControl
    {
        private sealed record OpcaoGolpe(int? Id, string Nome);

        public event Action? IrResumo;
        public event Action? IrStatus;
        public event Action? VoltarTime;
        public event Action? SetAlterado;

        private readonly Label[] _nomes;
        private readonly ComboBox[] _seletores;
        private PokemonMove[] _selecionados = Array.Empty<PokemonMove>();
        private int[] _moveIdsAtuais = new int[4];
        private OpcaoGolpe[] _opcoes = Array.Empty<OpcaoGolpe>();
        private PokemonSetController.SetInfo? _info;
        private int _pokemonId;
        private int _slotEditando = -1;
        private int? _moveInicial;
        private bool _carregando;

        public GuiMovesControl()
        {
            InitializeComponent();
            Ir_summary.Click += (_, _) => IrResumo?.Invoke();
            Ir_stats.Click += (_, _) => IrStatus?.Invoke();
            Voltar_Time.Click += (_, _) => VoltarTime?.Invoke();

            _nomes = new[] { Move_name1, Move_name1_Copiar, Move_name1_Copiar1, Move_name1_Copiar2 };
            _seletores = new[] { Move_select1, Move_select2, Move_select3, Move_select4 };

            for (int i = 0; i < _nomes.Length; i++)
            {
                int indice = i;
                _nomes[i].Cursor = Cursors.Hand;
                _nomes[i].MouseLeftButtonUp += (_, _) =>
                {
                    if (indice < _selecionados.Length)
                        MostrarDetalhes(_selecionados[indice]);
                    AbrirSlot(indice);
                };

                _seletores[i].SelectionChanged += (_, _) =>
                {
                    if (indice == _slotEditando && !_carregando)
                        ConfirmarSlot(indice);
                };
                _seletores[i].DropDownClosed += (_, _) =>
                {
                    if (indice != _slotEditando || _carregando)
                        return;

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (indice == _slotEditando)
                            ConfirmarSlot(indice);
                    }));
                };
            }

            EdicaoInline.Nivel(lvl_pokemon, valor =>
            {
                try
                {
                    new PokemonSetController().AtualizarNivel(_pokemonId, valor);
                    lvl_pokemon.Content = valor;
                    SetAlterado?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Não foi possível salvar o nível: {ex.Message}", "Time",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        public void Carregar(PokemonSetController.SetInfo info, bool shiny)
        {
            _info = info;
            _pokemonId = info.PokemonId;
            PokemonForm? form = GuiDados.ObterForma(info.PokemonId);

            Id_pokemon.Content = GuiDados.Id(info);
            Nome_pokemon.Content = GuiDados.Nome(info);
            lvl_pokemon.Content = info.Level.ToString();

            GuiDados.Sprite(Imagem_pokemon1, info.SpriteUrl);
            GuiDados.Tipos(Type_pokemon, Type_pokemon2, form?.Types ?? Array.Empty<PokemonType>(), 65, 196);

            _opcoes = new[] { new OpcaoGolpe(null, "—") }
                .Concat(info.TodosOsMoves.Select(move => new OpcaoGolpe(move.Id, move.Name)))
                .ToArray();

            _carregando = true;
            foreach (ComboBox seletor in _seletores)
                seletor.ItemsSource = _opcoes;
            _carregando = false;

            _selecionados = info.MoveIds.Count == 0
                ? info.TodosOsMoves.Take(4).ToArray()
                : Enumerable.Range(0, 4)
                    .Select(i => i < info.MoveIds.Count
                        ? info.TodosOsMoves.FirstOrDefault(move => move.Id == info.MoveIds[i])
                        : null)
                    .Select(move => move ?? new PokemonMove())
                    .ToArray();

            _moveIdsAtuais = _selecionados
                .Select(move => move?.Id ?? 0)
                .Concat(Enumerable.Repeat(0, 4))
                .Take(4)
                .ToArray();

            Label[] pps = { PP_move1, PP_move2, PP_move3, PP_move4 };
            Image[] tipos = { Move_type1, Move_type2, Move_type3, Move_type4 };

            for (int i = 0; i < _nomes.Length; i++)
            {
                PokemonMove? move = i < _selecionados.Length && _selecionados[i].Id != 0
                    ? _selecionados[i]
                    : null;
                _nomes[i].Content = move?.Name ?? "—";
                pps[i].Content = move is null ? "—" : $"{move.PP}/{move.PP}";

                if (move is null)
                    tipos[i].Source = null;
                else
                    GuiDados.Tipo(tipos[i], move.Type);
            }

            MostrarDetalhes(_selecionados.FirstOrDefault(move => move.Id != 0));
        }

        private void AbrirSlot(int indice)
        {
            if (_opcoes.Length == 0)
                return;

            if (_slotEditando >= 0)
                FecharSlot(_slotEditando);

            _slotEditando = indice;
            int idAtual = _moveIdsAtuais[indice];
            _moveInicial = idAtual > 0 ? idAtual : null;

            HashSet<int> outros = _moveIdsAtuais
                .Where((id, i) => i != indice && id != 0)
                .ToHashSet();

            OpcaoGolpe[] opcoesSlot = _opcoes
                .Where(opcao => opcao.Id is null
                    || opcao.Id == _moveInicial
                    || !outros.Contains(opcao.Id.Value))
                .ToArray();

            ComboBox seletor = _seletores[indice];
            _carregando = true;
            seletor.ItemsSource = opcoesSlot;
            seletor.SelectedItem = opcoesSlot.FirstOrDefault(opcao => opcao.Id == _moveInicial) ?? opcoesSlot[0];
            _carregando = false;
            _nomes[indice].Visibility = Visibility.Collapsed;
            seletor.Visibility = Visibility.Visible;
            seletor.Focus();
            seletor.IsDropDownOpen = true;
        }

        private void ConfirmarSlot(int indice)
        {
            if (_seletores[indice].SelectedItem is not OpcaoGolpe opcao)
                return;

            ComboBox seletor = _seletores[indice];
            seletor.Visibility = Visibility.Collapsed;
            _nomes[indice].Visibility = Visibility.Visible;
            _slotEditando = -1;

            int anterior = _moveIdsAtuais[indice];
            int novo = opcao.Id ?? 0;
            if (novo == anterior)
                return;

            _moveIdsAtuais[indice] = novo;

            try
            {
                new PokemonSetController().AtualizarMoves(_pokemonId, _moveIdsAtuais);
                PokemonMove? move = opcao.Id is int id
                    ? _info?.TodosOsMoves.FirstOrDefault(m => m.Id == id)
                    : null;
                MostrarDetalhes(move);
                SetAlterado?.Invoke();
            }
            catch (Exception ex)
            {
                _moveIdsAtuais[indice] = anterior;
                MessageBox.Show($"Não foi possível salvar o golpe: {ex.Message}", "Time",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FecharSlot(int indice)
        {
            _seletores[indice].IsDropDownOpen = false;
            _seletores[indice].Visibility = Visibility.Collapsed;
            _nomes[indice].Visibility = Visibility.Visible;
            _slotEditando = -1;
        }

        private void MostrarDetalhes(PokemonMove? move)
        {
            Move_power.Content = move?.Power?.ToString(CultureInfo.InvariantCulture) ?? "—";
            Move_accuracy.Content = move?.Accuracy?.ToString(CultureInfo.InvariantCulture) ?? "—";
            Move_typeofhit.Content = move?.Category.ToString() ?? "—";
            Move_description.Text = GuiDados.QuebrarLinhas(move is null
                ? "—"
                : $"{move.Name} · {move.Type} · {move.Category}", 27);
        }
    }
}
