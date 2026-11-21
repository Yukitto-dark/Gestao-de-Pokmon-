using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GestaoDePokemon.Controller;
using GestaoDePokemon.Data.Filters;
using GestaoDePokemon.Data.Repositories.Contracts;
using GestaoDePokemon.model;
using System.Windows.Controls.Primitives;

namespace GestaoDePokemon.View.Telas
{
    public partial class Box : Page
    {
        private readonly BoxController _controller = new();
        private readonly Image[] _slots;
        private readonly (CheckBox Caixa, int RegiaoId)[] _regioes;
        private bool _mostrarShiny;

        private static readonly BitmapImage CoracaoCheio = new(new Uri("pack://application:,,,/View/images/hert-full.png"));
        private static readonly BitmapImage CoracaoVazio = new(new Uri("pack://application:,,,/View/images/hert-full-vazio.png"));

        public Box()
        {
            InitializeComponent();

            var habilidades = new List<string> { string.Empty };
            habilidades.AddRange(_controller.ObterHabilidades());
            Filtro_Habilidade.ItemsSource = habilidades;
            Filtro_Habilidade.SelectedIndex = 0;

            _slots = new[]
            {
                _1, _2, _3, _4, _5, _6, _7, _8, _9, _10,
                _11, _12, _13, _14, _15, _16, _17, _18, _19, _20,
                _21, _22, _23, _24, _25, _26, _27, _28, _29, _30
            };

            // Id de cada região na tabela Regions.
            _regioes = new (CheckBox, int)[]
            {
                (Regiao_Kanto, 1), (Regiao_Johto, 2), (Regiao_Hoenn, 3),
                (Regiao_Sinnoh, 4), (Regiao_Unova, 5), (Regiao_Kalos, 6),
                (Regiao_Alola, 7), (Regiao_Galar, 8), (Regiao_Paldea, 9),
                (Regiao_Hisui, 10)
            };
            AtualizarPagina();
        }

        private void Navegar_Click(object sender, RoutedEventArgs e)
        {
            NavegacaoController.Navegar(this, sender);
        }

        private void Filtros_Click(object sender, RoutedEventArgs e) =>
            AlternarVisibilidade(PainelFiltros);

        private void Por_Type_Click(object sender, RoutedEventArgs e) =>
            AlternarVisibilidade(Types);

        private void Por_Regiao_Click(object sender, RoutedEventArgs e) =>
            AlternarVisibilidade(Regioes);

        private void Por_Favoritos_Click(object sender, RoutedEventArgs e) =>
            AlternarVisibilidade(Painel_Favoritos);

        private void Por_Party_Click(object sender, RoutedEventArgs e) =>
            AlternarVisibilidade(Painel_Party);

        private void Por_Habilidade_Click(object sender, RoutedEventArgs e) =>
            AlternarVisibilidade(Painel_Habilidade);

        private void Por_Formas_Click(object sender, RoutedEventArgs e) =>
            AlternarVisibilidade(Painel_Formas);

        private void Por_Id_Click(object sender, RoutedEventArgs e)
        {
            Por_Id.Visibility = Visibility.Collapsed;
            Id_TextBox.Visibility = Visibility.Visible;
            Id_TextBox.Focus();
        }

        private void Por_Nome_Click(object sender, RoutedEventArgs e)
        {
            Por_Nome.Visibility = Visibility.Collapsed;
            Nome_TextBox.Visibility = Visibility.Visible;
            Nome_TextBox.Focus();
        }

        private void Limpar_Filtros_Click(object sender, RoutedEventArgs e)
        {
            Id_TextBox.Text = string.Empty;
            Nome_TextBox.Text = string.Empty;

            // Nenhuma região marcada significa mostrar todas as regiões.
            foreach ((CheckBox caixa, _) in _regioes)
                caixa.IsChecked = false;

            foreach (UIElement filho in Tipos_Hotspots.Children)
            {
                if (filho is System.Windows.Controls.Primitives.ToggleButton tipo)
                    tipo.IsChecked = false;
            }

            Types.Visibility = Visibility.Collapsed;
            Regioes.Visibility = Visibility.Collapsed;
            Painel_Favoritos.Visibility = Visibility.Collapsed;
            Filtro_Favoritos.IsChecked = false;
            Painel_Party.Visibility = Visibility.Collapsed;
            Filtro_Party.IsChecked = false;

            Painel_Habilidade.Visibility = Visibility.Collapsed;
            Filtro_Habilidade.SelectedIndex = 0;
            Painel_Formas.Visibility = Visibility.Collapsed;
            Filtro_Formas.IsChecked = false;

            Id_TextBox.Visibility = Visibility.Collapsed;
            Nome_TextBox.Visibility = Visibility.Collapsed;
            Por_Id.Visibility = Visibility.Visible;
            Por_Nome.Visibility = Visibility.Visible;

            _controller.LimparFiltros();
            AtualizarPagina();
        }

        private void Aplicar_Filtros_Click(object sender, RoutedEventArgs e)
        {
            int? numeroDex = null;
            string textoId = Id_TextBox.Text.Trim();

            if (textoId.Length > 0)
            {
                if (!int.TryParse(textoId, out int id) || id < 1)
                {
                    AvisarCampo("Informe um número de Pokédex válido.", "Filtro por número", Id_TextBox);
                    return;
                }

                numeroDex = id;
            }

            var tiposSelecionados = new List<PokemonType>();
            foreach (ToggleButton hotspot in Tipos_Hotspots.Children.OfType<ToggleButton>())
            {
                if (hotspot.IsChecked != true)
                    continue;

                string? nomeTipo = hotspot.ToolTip?.ToString();
                if (Enum.TryParse(nomeTipo, ignoreCase: true, out PokemonType tipo) &&
                    !tiposSelecionados.Contains(tipo))
                {
                    tiposSelecionados.Add(tipo);
                }
            }

            List<int> regioesSelecionadas = _regioes
                .Where(regiao => regiao.Caixa.IsChecked == true)
                .Select(regiao => regiao.RegiaoId)
                .ToList();

            string? nome = string.IsNullOrWhiteSpace(Nome_TextBox.Text)
                ? null
                : Nome_TextBox.Text.Trim();

            string? habilidade = Filtro_Habilidade.SelectedIndex > 0
                ? Filtro_Habilidade.SelectedValue as string
                : null;

            _controller.AplicarFiltros(new PokemonFilter
            {
                Types = tiposSelecionados,
                // Lista vazia = sem filtro regional; uma ou mais marcadas =
                // somente Pokémon das regiões selecionadas.
                OriginRegionIds = regioesSelecionadas,
                OnlyFavorites = Filtro_Favoritos.IsChecked == true,
                OnlyParty = Filtro_Party.IsChecked == true,
                NationalDexNumber = numeroDex,
                NameContains = nome,
                AbilityName = habilidade,
                HasAlternateForms = Filtro_Formas.IsChecked == true ? true : null
            });

            AtualizarPagina();
            PainelFiltros.Visibility = Visibility.Collapsed;
        }

        private void AtualizarPagina()
        {
            // Cada célula é uma forma: a espécie na ordem da Pokédex, a forma Normal e depois todas as outras.
            IReadOnlyList<PokemonBoxEntry> entradas = _controller.ObterEntradasDaPagina();

            int totalResultados = _controller.TotalResultados;
            Numeração_Box.Content = totalResultados == 0
                ? "0 - 0"
                : $"{_controller.PaginaAtual} - {_controller.TotalPaginas}";
            Numeração_Box.ToolTip = totalResultados == 0
                ? "Nenhum Pokémon encontrado com os filtros selecionados."
                : $"{totalResultados} Pokémon e formas encontrados.";

            for (int i = 0; i < _slots.Length; i++)
            {
                if (i < entradas.Count)
                {
                    _slots[i].Tag = entradas[i];
                    _slots[i].ToolTip = entradas[i].Descricao;
                    string? url = _controller.ObterSpriteUrl(entradas[i]);
                    _slots[i].Source = url is null
                        ? null
                        : new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    _slots[i].Tag = null;
                    _slots[i].ToolTip = null;
                    _slots[i].Source = null;
                }
            }

            _controller.LimparSelecao();
            LimparEstadoSemSelecao();
        }

        private void Grade_Click(object sender, MouseButtonEventArgs e)
        {
            Point ponto = e.GetPosition((IInputElement)sender);

            if (ponto.X < 247 || ponto.X >= 626 || ponto.Y < 121 || ponto.Y >= 417)
                return;

            int coluna = Math.Clamp((int)((ponto.X - 247) / 63), 0, 5);
            int linha = Math.Clamp((int)((ponto.Y - 121) / 59), 0, 4);
            PokemonBoxEntry? entrada = _controller.Selecionar(coluna, linha);

            if (entrada is null)
            {
                LimparEstadoSemSelecao();
                return;
            }

            _mostrarShiny = _controller.EhShiny(entrada);
            AtualizarImagemDetalhe();
            Shine.IsEnabled = true;
            Shine.ToolTip = _mostrarShiny
                ? "Exibir sprite normal"
                : "Exibir sprite shiny";
            Botao_Favorito.IsEnabled = true;
            Equipar_Party.IsEnabled = true;
            Selecao_Slot.Margin = new Thickness(247 + coluna * 63, 121 + linha * 59, 0, 0);
            Selecao_Slot.Visibility = Visibility.Visible;
            AtualizarPainelSelecao();
        }

        private void Shine_Click(object sender, RoutedEventArgs e)
        {
            if (_controller.IdSelecionado is null)
                return;

            _mostrarShiny = !_mostrarShiny;
            AtualizarImagemDetalhe();
            Shine.ToolTip = _mostrarShiny
                ? "Exibir sprite normal"
                : "Exibir sprite shiny";
        }

        private void AtualizarImagemDetalhe()
        {
            if (_controller.EntradaSelecionada is not PokemonBoxEntry entrada)
            {
                Imagem_Pokemon.Source = null;
                Imagem_Pokemon.ToolTip = null;
                return;
            }

            Imagem_Pokemon.ToolTip = entrada.Descricao;
            string? url = _controller.ObterSpriteDetalheUrl(entrada, _mostrarShiny);
            Imagem_Pokemon.Source = url is null
                ? null
                : new BitmapImage(new Uri(url, UriKind.Absolute));
        }

        private void AtualizarInformacoesPokemon()
        {
            Informação_Conteudo.Children.Clear();
            Informação_Pokemon.ScrollToTop();

            if (_controller.EntradaSelecionada is not PokemonBoxEntry entrada ||
                _controller.ObterPokemon(entrada.NationalDexNumber) is not Pokemon pokemon)
                return;

            // Dados da forma da célula selecionada (tipos, habilidades e atributos mudam entre formas).
            PokemonForm? normal = pokemon.Forms.FirstOrDefault(form => form.FormName == "Normal")
                ?? pokemon.Forms.FirstOrDefault();
            PokemonForm? form = pokemon.Forms.FirstOrDefault(form => form.Id == entrada.FormId)
                ?? normal;

            IReadOnlyList<PokemonType> tiposPokemon = form is null
                ? Array.Empty<PokemonType>()
                : form.Types;

            string ovos = pokemon.EggGroups.Count == 0
                ? "-"
                : string.Join(" / ", pokemon.EggGroups);

            string regiao = string.IsNullOrWhiteSpace(pokemon.OriginRegion?.Name)
                ? "-"
                : pokemon.OriginRegion.Name;

            BaseStats stats = form?.Stats ?? default;
            // Formas sem golpes próprios (Megas, formas cosméticas) usam os da forma Normal.
            IReadOnlyList<LearnableMove> learnset = form is { Learnset.Count: > 0 }
                ? form.Learnset
                : normal?.Learnset ?? (IReadOnlyList<LearnableMove>)Array.Empty<LearnableMove>();

            // Os níveis mudam entre os jogos: a lista usa o mesmo jogo de referência da build do time.
            string? jogoReferencia = PokemonSetController.JogoReferencia(learnset);
            List<(int Nivel, string Nome, PokemonType Tipo)> golpesNivel = learnset
                .Where(golpe => golpe.Method == MoveLearnMethod.LevelUp &&
                                (jogoReferencia is null || golpe.GameVersion == jogoReferencia))
                .GroupBy(golpe => golpe.Move.Name)
                .Select(grupo => (
                    Nivel: grupo.Min(golpe => golpe.Level ?? int.MaxValue),
                    Nome: grupo.Key,
                    Tipo: grupo.First().Move.Type))
                .OrderBy(golpe => golpe.Nivel)
                .ThenBy(golpe => golpe.Nome)
                .ToList();

            List<(string Nome, PokemonType Tipo)> tms = learnset
                .Where(golpe => golpe.Method == MoveLearnMethod.TM)
                .GroupBy(golpe => golpe.Move.Name)
                .Select(grupo => (Nome: grupo.Key, Tipo: grupo.First().Move.Type))
                .OrderBy(golpe => golpe.Nome)
                .ToList();

            AdicionarTextoInformacao($"#{pokemon.NationalDexNumber:000} {pokemon.Name}");
            if (form is not null && !string.Equals(form.FormName, "Normal", StringComparison.OrdinalIgnoreCase))
                AdicionarTextoInformacao($"Forma: {form.FormName}");
            AdicionarTextoInformacao($"Categoria: {pokemon.Category}");
            AdicionarTextoInformacao("Tipo:");

            if (tiposPokemon.Count == 0)
                AdicionarTextoInformacao("-");
            else
                Informação_Conteudo.Children.Add(CriarLinhaTipos(tiposPokemon));

            AdicionarTextoInformacao($"Região: {regiao}");
            AdicionarTextoInformacao($"Geração: {pokemon.Generation}");
            AdicionarTextoInformacao($"Ovos: {ovos}");

            AdicionarTituloDetalhe("Dados físicos", 8);
            AdicionarTextoDetalhe($"Altura: {(form?.Height is double h ? h.ToString("0.0", CultureInfo.InvariantCulture) + " m" : "-")}");
            AdicionarTextoDetalhe($"Peso: {(form?.Weight is double w ? w.ToString("0.0", CultureInfo.InvariantCulture) + " kg" : "-")}");

            AdicionarTituloDetalhe("Habilidades", 8);
            if (form is null || form.Abilities.Count == 0)
            {
                AdicionarTextoDetalhe("-");
            }
            else
            {
                foreach (PokemonFormAbility habilidade in form.Abilities.OrderBy(a => a.Slot))
                {
                    string rotulo = habilidade.Slot switch
                    {
                        AbilitySlot.Hidden => "Oculta",
                        AbilitySlot.Second => "Segunda",
                        _ => "Primeira"
                    };
                    TextBlock bloco = AdicionarTextoDetalhe($"{rotulo}: {habilidade.Ability.Name}");
                    bloco.ToolTip = habilidade.Ability.Description;
                }
            }

            AdicionarTituloDetalhe("Atributos (base / Lv.100)", 8);
            AdicionarTextoDetalhe($"HP: {stats.HP} ({StatMinimo(stats.HP, true)} - {StatMaximo(stats.HP, true)})");
            AdicionarTextoDetalhe($"Atk: {stats.Atk} ({StatMinimo(stats.Atk, false)} - {StatMaximo(stats.Atk, false)})");
            AdicionarTextoDetalhe($"Def: {stats.Def} ({StatMinimo(stats.Def, false)} - {StatMaximo(stats.Def, false)})");
            AdicionarTextoDetalhe($"SpA: {stats.SpAtk} ({StatMinimo(stats.SpAtk, false)} - {StatMaximo(stats.SpAtk, false)})");
            AdicionarTextoDetalhe($"SpD: {stats.SpDef} ({StatMinimo(stats.SpDef, false)} - {StatMaximo(stats.SpDef, false)})");
            AdicionarTextoDetalhe($"Spe: {stats.Speed} ({StatMinimo(stats.Speed, false)} - {StatMaximo(stats.Speed, false)})");
            AdicionarTextoDetalhe($"Total: {stats.Total}");

            AdicionarTituloDetalhe("Linha evolutiva", 8);
            AdicionarLinhaEvolutiva(pokemon);

            AdicionarTituloDetalhe("Formas", 8);
            foreach (PokemonForm forma in pokemon.Forms
                .OrderBy(f => f.FormName == "Normal" ? 0 : 1)
                .ThenBy(f => f.Id))
            {
                AdicionarLinhaDetalhe(
                    forma.FormName,
                    _controller.ObterSpriteFormaUrl(forma.Id),
                    MontarTooltipForma(forma));
            }

            AdicionarAtributosPorForma(pokemon);

            AdicionarTituloDetalhe(jogoReferencia is null
                ? "Ao subir de nível"
                : $"Ao subir de nível ({PokemonSetController.NomeJogo(jogoReferencia)})", 8);

            if (golpesNivel.Count == 0)
            {
                AdicionarTextoInformacao("- Nenhum");
            }
            else
            {
                foreach ((int nivel, string nome, PokemonType tipo) in golpesNivel)
                {
                    string texto = nivel == int.MaxValue
                        ? $"? - {nome}"
                        : $"Nv {nivel} - {nome}";

                    Informação_Conteudo.Children.Add(CriarLinhaGolpe(texto, tipo));
                }
            }

            AdicionarTituloDetalhe("TMs", 8);

            if (tms.Count == 0)
            {
                AdicionarTextoInformacao("- Nenhum");
            }
            else
            {
                foreach ((string nome, PokemonType tipo) in tms)
                    Informação_Conteudo.Children.Add(CriarLinhaGolpe(nome, tipo));
            }

            Informação_Pokemon.ScrollToTop();
        }

        private TextBlock AdicionarTexto(
            string texto,
            string nomeEstilo,
            double margemSuperior = 0,
            double margemInferior = 0)
        {
            TextBlock bloco = CriarTexto(texto, nomeEstilo);

            if (margemSuperior > 0 || margemInferior > 0)
                bloco.Margin = new Thickness(0, margemSuperior, 0, margemInferior);

            Informação_Conteudo.Children.Add(bloco);
            return bloco;
        }

        private void AdicionarTextoInformacao(string texto, double margemSuperior = 0) =>
            AdicionarTexto(texto, "TextoInformacao", margemSuperior);

        private TextBlock CriarTextoInformacao(string texto) => CriarTexto(texto, "TextoInformacao");

        private FrameworkElement CriarLinhaGolpe(string texto, PokemonType tipo)
        {
            var linha = new Grid { Margin = new Thickness(0, 0, 0, 2) };
            linha.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            linha.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Image imagem = CriarImagemTipo(tipo, 60);
            imagem.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(imagem, 0);
            linha.Children.Add(imagem);

            TextBlock bloco = CriarTextoInformacao(texto);
            Grid.SetColumn(bloco, 1);
            linha.Children.Add(bloco);

            return linha;
        }

        private FrameworkElement CriarLinhaTipos(IReadOnlyList<PokemonType> tipos)
        {
            var linha = new WrapPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 4)
            };

            foreach (PokemonType tipo in tipos)
                linha.Children.Add(CriarImagemTipo(tipo, 60));

            return linha;
        }

        private static Image CriarImagemTipo(PokemonType tipo, double largura)
        {
            PokemonTypeInfo info = PokemonTypeCatalog.GetInfo(tipo);

            return new Image
            {
                Source = new BitmapImage(
                    new Uri($"pack://application:,,,/{info.SpritePath}", UriKind.Absolute)),
                Width = largura,
                Height = Math.Round(largura * 0.4),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(0, 0, 4, 2),
                ToolTip = info.Name
            };
        }

        private void AdicionarTituloDetalhe(string texto, double margemSuperior = 0) =>
            AdicionarTexto(texto, "TituloDetalhe", margemSuperior, margemSuperior > 0 ? 2 : 0);

        private TextBlock AdicionarTextoDetalhe(string texto, double margemSuperior = 0) =>
            AdicionarTexto(texto, "TextoDetalhe", margemSuperior);

        private TextBlock CriarTextoDetalhe(string texto) => CriarTexto(texto, "TextoDetalhe");

        private TextBlock CriarTexto(string texto, string nomeEstilo) => new()
        {
            Style = (Style)FindResource(nomeEstilo),
            Text = texto
        };

        private void AdicionarLinhaDetalhe(
            string texto,
            string? spriteUrl,
            string? toolTip = null)
        {
            var linha = new Grid { Margin = new Thickness(0, 0, 0, 3) };
            linha.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            linha.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Image imagem = CriarImagemForma(spriteUrl, 32);
            imagem.VerticalAlignment = VerticalAlignment.Center;
            imagem.Margin = new Thickness(0, 0, 6, 0);
            Grid.SetColumn(imagem, 0);
            linha.Children.Add(imagem);

            TextBlock bloco = CriarTextoDetalhe(texto);
            bloco.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(bloco, 1);
            linha.Children.Add(bloco);

            linha.ToolTip = toolTip;
            Informação_Conteudo.Children.Add(linha);
        }

        private void AdicionarLinhaEvolutiva(Pokemon pokemon)
        {
            List<Pokemon> especies = _controller
                .ObterLinhaEvolutiva(pokemon.NationalDexNumber)
                .ToList();

            if (especies.Count == 0)
                return;

            var linha = new WrapPanel { Orientation = Orientation.Horizontal };

            for (int i = 0; i < especies.Count; i++)
            {
                if (i > 0)
                {
                    TextBlock seta = CriarTextoDetalhe("→");
                    seta.VerticalAlignment = VerticalAlignment.Center;
                    linha.Children.Add(seta);
                }

                Pokemon especie = especies[i];
                PokemonForm? forma = especie.Forms.FirstOrDefault(f => f.FormName == "Normal")
                    ?? especie.Forms.FirstOrDefault();

                linha.Children.Add(CriarItemEvolucao(
                    especie.Name,
                    forma is null ? null : _controller.ObterSpriteFormaUrl(forma.Id)));
            }

            Informação_Conteudo.Children.Add(linha);
        }

        private FrameworkElement CriarItemEvolucao(string nome, string? spriteUrl)
        {
            var item = new StackPanel
            {
                Width = 56,
                Margin = new Thickness(0, 0, 2, 4),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Image imagem = CriarImagemForma(spriteUrl, 40);
            imagem.HorizontalAlignment = HorizontalAlignment.Center;
            item.Children.Add(imagem);

            TextBlock rotulo = CriarTextoDetalhe(nome);
            rotulo.FontSize = 11;
            rotulo.TextAlignment = TextAlignment.Center;
            item.Children.Add(rotulo);

            return item;
        }

        private void AdicionarAtributosPorForma(Pokemon pokemon)
        {
            if (pokemon.Forms.Count < 2)
                return;

            List<IGrouping<BaseStats, PokemonForm>> grupos = pokemon.Forms
                .GroupBy(forma => forma.Stats)
                .ToList();

            if (grupos.Count < 2)
                return;

            AdicionarTituloDetalhe("Atributos por forma", 8);

            foreach (IGrouping<BaseStats, PokemonForm> grupo in grupos)
            {
                string nomes = string.Join(", ", grupo
                    .Select(forma => forma.FormName)
                    .OrderBy(nome => nome));
                BaseStats stats = grupo.Key;

                TextBlock bloco = AdicionarTextoDetalhe(
                    $"{nomes}: {stats.HP}/{stats.Atk}/{stats.Def}/{stats.SpAtk}/{stats.SpDef}/{stats.Speed} ({stats.Total})");
                bloco.ToolTip = MontarTooltipAtributos(stats, grupo.ToList());
            }
        }

        private static string MontarTooltipForma(PokemonForm forma)
        {
            var texto = new StringBuilder();
            texto.AppendLine($"Forma: {forma.FormName}");

            if (forma.Height is double altura)
                texto.AppendLine($"Altura: {altura.ToString("0.0", CultureInfo.InvariantCulture)} m");

            if (forma.Weight is double peso)
                texto.AppendLine($"Peso: {peso.ToString("0.0", CultureInfo.InvariantCulture)} kg");

            if (forma.Types.Count > 0)
                texto.AppendLine("Tipos: " + string.Join(", ",
                    forma.Types.Select(tipo => PokemonTypeCatalog.GetInfo(tipo).Name)));

            if (forma.Abilities.Count > 0)
                texto.AppendLine("Habilidades: " + string.Join(", ", forma.Abilities
                    .OrderBy(habilidade => habilidade.Slot)
                    .Select(habilidade => habilidade.Ability.Name)));

            texto.AppendLine();
            texto.Append(MontarTooltipAtributos(forma.Stats, new[] { forma }));
            return texto.ToString().TrimEnd();
        }

        private static string MontarTooltipAtributos(
            BaseStats stats,
            IReadOnlyList<PokemonForm> formas)
        {
            var texto = new StringBuilder();
            texto.AppendLine("Atributos base (intervalo estimado em Lv.100, IV 0 a 31, sem EVs):");
            texto.AppendLine(FormatarStat("HP", stats.HP, true));
            texto.AppendLine(FormatarStat("Atk", stats.Atk, false));
            texto.AppendLine(FormatarStat("Def", stats.Def, false));
            texto.AppendLine(FormatarStat("SpA", stats.SpAtk, false));
            texto.AppendLine(FormatarStat("SpD", stats.SpDef, false));
            texto.AppendLine(FormatarStat("Spe", stats.Speed, false));
            texto.AppendLine($"Total: {stats.Total}");

            List<string> alturas = formas
                .Where(forma => forma.Height is double)
                .Select(forma => forma.Height!.Value.ToString("0.0", CultureInfo.InvariantCulture) + " m")
                .Distinct()
                .ToList();
            List<string> pesos = formas
                .Where(forma => forma.Weight is double)
                .Select(forma => forma.Weight!.Value.ToString("0.0", CultureInfo.InvariantCulture) + " kg")
                .Distinct()
                .ToList();

            if (alturas.Count > 0)
                texto.AppendLine("Altura: " + string.Join(" / ", alturas));

            if (pesos.Count > 0)
                texto.Append("Peso: " + string.Join(" / ", pesos));

            return texto.ToString().TrimEnd();
        }

        private static string FormatarStat(string rotulo, int baseStat, bool hp) =>
            $"{rotulo}: {baseStat} ({StatMinimo(baseStat, hp)} - {StatMaximo(baseStat, hp)})";

        private static Image CriarImagemForma(string? spriteUrl, double tamanho) => new()
        {
            Source = spriteUrl is null
                ? null
                : new BitmapImage(new Uri(spriteUrl, UriKind.Absolute)),
            Width = tamanho,
            Height = tamanho,
            Stretch = Stretch.Uniform
        };

        private static int StatMinimo(int baseStat, bool hp) =>
            hp ? 2 * baseStat + 110 : 2 * baseStat + 5;

        private static int StatMaximo(int baseStat, bool hp) =>
            hp
                ? 2 * baseStat + 204
                : (int)Math.Floor((2 * baseStat + 99) * 1.1);

        private static void AlternarVisibilidade(UIElement painel) =>
            painel.Visibility = painel.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;

        private static void AvisarCampo(string mensagem, string titulo, TextBox campo)
        {
            MessageBox.Show(mensagem, titulo, MessageBoxButton.OK, MessageBoxImage.Warning);
            campo.Focus();
        }

        private void LimparEstadoSemSelecao()
        {
            _mostrarShiny = false;
            Imagem_Pokemon.Source = null;
            Imagem_Pokemon.ToolTip = null;
            Shine.IsEnabled = false;
            Shine.ToolTip = "Selecione um Pokémon";
            Botao_Favorito.IsEnabled = false;
            Equipar_Party.IsEnabled = false;
            Selecao_Slot.Visibility = Visibility.Collapsed;
            AtualizarPainelSelecao();
        }

        private void AtualizarPainelSelecao()
        {
            AtualizarCoracao();
            AtualizarBotaoParty();
            AtualizarInformacoesPokemon();
        }

        private void Favorito_Click(object sender, RoutedEventArgs e)
        {
            ResultadoFavorito resultado = _controller.AlternarFavorito(_mostrarShiny);

            if (resultado is ResultadoFavorito.SemSelecao)
            {
                Botao_Favorito.IsChecked = false;
                return;
            }

            if (Filtro_Favoritos.IsChecked == true)
                AtualizarPagina();
            else
                AtualizarCoracao();
        }

        private void Equipar_Party_Click(object sender, RoutedEventArgs e)
        {
            ResultadoParty resultado = _controller.AlternarParty(
                _mostrarShiny, out bool limiteAtingido);

            if (resultado is ResultadoParty.SemSelecao)
                return;

            if (limiteAtingido)
            {
                MessageBox.Show(
                    "Limite de 6 Pokémon no time atingido.",
                    "Time",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                AtualizarBotaoParty();
                return;
            }

            if (Filtro_Party.IsChecked == true)
                AtualizarPagina();
            else
                AtualizarBotaoParty();
        }

        private void AtualizarBotaoParty()
        {
            if (_controller.EntradaSelecionada is not PokemonBoxEntry entrada)
            {
                Equipar_Party.IsEnabled = false;
                Equipar_Party.Content = "Por na party";
                Equipar_Party.ToolTip = "Selecione um Pokémon";
                return;
            }

            bool noTime = _controller.EhNoTime(entrada);
            string shiny = _controller.EhShiny(entrada) ? " (shiny)" : string.Empty;
            Equipar_Party.IsEnabled = true;
            Equipar_Party.Content = noTime ? "Remover da party" : "Por na party";
            Equipar_Party.ToolTip = noTime
                ? $"Remover {entrada.Descricao} do time{shiny}"
                : $"Adicionar {entrada.Descricao} ao time{shiny}";
        }

        private void AtualizarCoracao()
        {
            if (_controller.EntradaSelecionada is not PokemonBoxEntry entrada)
            {
                Botao_Favorito.IsChecked = false;
                Icone_Favorito.Source = CoracaoVazio;
                Botao_Favorito.ToolTip = "Selecione um Pokémon";
                return;
            }

            // Os favoritos são por espécie: todas as formas dela compartilham o coração.
            int id = entrada.NationalDexNumber;
            bool favorito = _controller.EhFavorito(id);
            string shiny = _controller.EhShiny(entrada) ? " (shiny)" : string.Empty;
            Botao_Favorito.IsChecked = favorito;
            Icone_Favorito.Source = favorito ? CoracaoCheio : CoracaoVazio;
            Botao_Favorito.ToolTip = favorito
                ? $"Remover {id} dos favoritos{shiny}"
                : $"Favoritar {id}{shiny}";
        }

        private void Prox_Box_Click(object sender, RoutedEventArgs e)
        {
            _controller.IrParaPagina(1);
            AtualizarPagina();
        }

        private void Ant_Box_Click(object sender, RoutedEventArgs e)
        {
            _controller.IrParaPagina(-1);
            AtualizarPagina();
        }
    }
}
