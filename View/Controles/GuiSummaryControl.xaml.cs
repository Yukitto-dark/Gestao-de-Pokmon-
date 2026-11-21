using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using GestaoDePokemon.Controller;
using GestaoDePokemon.model;

namespace GestaoDePokemon.View.Controles
{
    public partial class GuiSummaryControl : UserControl
    {
        public event Action? IrStatus;
        public event Action? IrMoves;
        public event Action? VoltarTime;
        public event Action? SetAlterado;

        // Espera o usuário parar de arrastar a barra antes de gravar o nível.
        private readonly DispatcherTimer _salvarNivel = new() { Interval = TimeSpan.FromMilliseconds(350) };

        private PokemonSetController.SetInfo? _info;
        private int _membroId;
        private bool _carregando;

        public GuiSummaryControl()
        {
            InitializeComponent();
            Ir_stats.Click += (_, _) => IrStatus?.Invoke();
            Ir_Moves.Click += (_, _) => IrMoves?.Invoke();
            Voltar_Time.Click += (_, _) => VoltarTime?.Invoke();
            Ir_Move.Cursor = Cursors.Hand;
            Ir_Move.MouseLeftButtonUp += (_, _) => IrMoves?.Invoke();

            EdicaoInline.Nivel(lvl_pokemon, SalvarNivel);

            Nivel_menos.Click += (_, _) => Nivel_slider.Value = Math.Max(Nivel_slider.Minimum, Nivel_slider.Value - 1);
            Nivel_mais.Click += (_, _) => Nivel_slider.Value = Math.Min(Nivel_slider.Maximum, Nivel_slider.Value + 1);
            Nivel_slider.ValueChanged += (_, _) =>
            {
                int nivel = (int)Math.Round(Nivel_slider.Value);
                Nivel_valor.Content = $"Nv {nivel}";
                AtualizarBotoesNivel(nivel);

                if (_carregando)
                    return;

                // Prévia imediata do nível e do HP; a gravação acontece quando a barra para.
                lvl_pokemon.Content = nivel.ToString();
                AtualizarVida(nivel);
                _salvarNivel.Stop();
                _salvarNivel.Start();
            };
            _salvarNivel.Tick += (_, _) =>
            {
                _salvarNivel.Stop();
                SalvarNivel((int)Math.Round(Nivel_slider.Value));
            };
        }

        public void Carregar(PokemonSetController.SetInfo info, bool shiny)
        {
            _salvarNivel.Stop();
            _info = info;
            _membroId = info.MembroId;

            Id_pokemon.Content = GuiDados.Id(info);
            Nome_pokemon.Content = GuiDados.Nome(info);
            Nome_pokemon.ToolTip = GuiDados.NomeCompleto(info);
            lvl_pokemon.Content = info.Level.ToString();

            _carregando = true;
            Nivel_slider.Minimum = PokemonSetController.NivelMinimo;
            Nivel_slider.Maximum = PokemonSetController.NivelMaximo;
            Nivel_slider.Value = info.Level;
            Nivel_valor.Content = $"Nv {info.Level}";
            AtualizarBotoesNivel(info.Level);
            _carregando = false;

            AtualizarVida(info.Level);

            Experiencia.Content = "—";

            PokemonAbility? habilidade = info.Abilities
                .FirstOrDefault(ability => ability.Id == info.AbilityId)
                ?? info.Abilities.FirstOrDefault();
            Ability.Content = habilidade?.Name ?? "—";
            Info_ability.Text = GuiDados.QuebrarLinhas(habilidade?.Description ?? "—", 55);

            GuiDados.Sprite(Imagem_pokemon, info.SpriteUrl);
            GuiDados.Tipos(Type_pokemon, Type_pokemon2, info.Types, 56, 180);
        }

        /// <summary>Grava na hora um nível ainda esperando a barra parar (ao fechar ou trocar de Pokémon).</summary>
        public void ConfirmarNivelPendente()
        {
            if (!_salvarNivel.IsEnabled)
                return;

            _salvarNivel.Stop();
            SalvarNivel((int)Math.Round(Nivel_slider.Value));
        }

        private void AtualizarVida(int nivel)
        {
            if (_info is null)
                return;

            int hpMax = GuiDados.HpMaximo(
                _info.Stats.HP,
                _info.IVs.HP,
                _info.EVs.GetValue(PokemonStat.HP),
                nivel);
            Vida.Content = $"{hpMax}/{hpMax}";
        }

        private void AtualizarBotoesNivel(int nivel)
        {
            Nivel_menos.IsEnabled = nivel > PokemonSetController.NivelMinimo;
            Nivel_mais.IsEnabled = nivel < PokemonSetController.NivelMaximo;
        }

        private void SalvarNivel(int valor)
        {
            if (_info is null || valor == _info.Level)
                return;

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
        }
    }
}
