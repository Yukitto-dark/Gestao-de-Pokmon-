using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private int _pokemonId;

        public GuiSummaryControl()
        {
            InitializeComponent();
            Ir_stats.Click += (_, _) => IrStatus?.Invoke();
            Ir_Moves.Click += (_, _) => IrMoves?.Invoke();
            Voltar_Time.Click += (_, _) => VoltarTime?.Invoke();
            Ir_Move.Cursor = Cursors.Hand;
            Ir_Move.MouseLeftButtonUp += (_, _) => IrMoves?.Invoke();

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
            _pokemonId = info.PokemonId;
            PokemonForm? form = GuiDados.ObterForma(info.PokemonId);

            Id_pokemon.Content = GuiDados.Id(info);
            Nome_pokemon.Content = GuiDados.Nome(info);
            lvl_pokemon.Content = info.Level.ToString();

            int hpMax = GuiDados.HpMaximo(
                form?.Stats.HP ?? 0,
                info.IVs.HP,
                info.EVs.GetValue(PokemonStat.HP),
                info.Level);
            Vida.Content = $"{hpMax}/{hpMax}";

            Experiencia.Content = "—";
            XP_faltando.Content = "—";

            PokemonAbility? habilidade = info.Abilities
                .FirstOrDefault(ability => ability.Id == info.AbilityId)
                ?? info.Abilities.FirstOrDefault();
            Ability.Content = habilidade?.Name ?? "—";
            Info_ability.Text = GuiDados.QuebrarLinhas(habilidade?.Description ?? "—", 55);

            GuiDados.Sprite(Imagem_pokemon, info.SpriteUrl);
            GuiDados.Tipos(Type_pokemon, Type_pokemon2, form?.Types ?? Array.Empty<PokemonType>(), 56, 180);
        }
    }
}
