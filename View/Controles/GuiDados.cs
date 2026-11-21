using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GestaoDePokemon.Controller;
using GestaoDePokemon.model;

namespace GestaoDePokemon.View.Controles
{
    internal static class GuiDados
    {
        public static string NaturezaEfeito(PokemonNature nature)
        {
            NatureModifiers modificadores = PokemonNatureRules.GetModifiers(nature);

            if (modificadores.IncreasedStat is not PokemonStat aumentado)
                return "sem efeito";

            return $"+{NomeCurto(aumentado)} / -{NomeCurto(modificadores.DecreasedStat!.Value)}";
        }

        public static string NomeCurto(PokemonStat stat) => stat switch
        {
            PokemonStat.HP => "HP",
            PokemonStat.Attack => "Ataque",
            PokemonStat.Defense => "Defesa",
            PokemonStat.SpecialAttack => "Sp.Atk",
            PokemonStat.SpecialDefense => "Sp.Def",
            PokemonStat.Speed => "Vel",
            _ => stat.ToString()
        };

        public static void Sprite(Image imagem, string? url)
        {
            imagem.Source = string.IsNullOrEmpty(url)
                ? null
                : new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        public static void Tipo(Image imagem, PokemonType tipo)
        {
            imagem.Source = new BitmapImage(new Uri(
                $"pack://application:,,,/{PokemonTypeCatalog.GetInfo(tipo).SpritePath}",
                UriKind.Absolute));
        }

        public static void Tipos(
            Image primeiro,
            Image segundo,
            IReadOnlyList<PokemonType> tipos,
            double spriteEsquerda,
            double spriteLargura)
        {
            const double largura = 78;
            const double espaco = 4;

            double total = tipos.Count > 1 ? largura * 2 + espaco : largura;
            double x = spriteEsquerda + (spriteLargura - total) / 2;

            primeiro.Margin = new Thickness(x, primeiro.Margin.Top, 0, 0);
            segundo.Margin = new Thickness(x + largura + espaco, segundo.Margin.Top, 0, 0);

            primeiro.Source = null;
            segundo.Source = null;
            primeiro.Visibility = tipos.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            segundo.Visibility = tipos.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

            if (tipos.Count > 0)
                Tipo(primeiro, tipos[0]);

            if (tipos.Count > 1)
                Tipo(segundo, tipos[1]);
        }

        public static int HpMaximo(int baseHp, int iv, int ev, int level) =>
            ((2 * baseHp + iv + ev / 4) * level / 100) + level + 10;

        public static int BaseStat(BaseStats stats, PokemonStat stat) => stat switch
        {
            PokemonStat.HP => stats.HP,
            PokemonStat.Attack => stats.Atk,
            PokemonStat.Defense => stats.Def,
            PokemonStat.SpecialAttack => stats.SpAtk,
            PokemonStat.SpecialDefense => stats.SpDef,
            PokemonStat.Speed => stats.Speed,
            _ => 0
        };

        public static int Iv(IndividualValues ivs, PokemonStat stat) => ivs.GetValue(stat);

        /// <summary>
        /// Valor real do atributo no nível atual (fórmula da Geração III em diante): base stat, IV, EVs,
        /// nível e natureza (+10% / -10%, que nunca afeta o HP).
        /// </summary>
        public static int StatNoNivel(PokemonSetController.SetInfo info, PokemonStat stat)
        {
            int baseStat = BaseStat(info.Stats, stat);
            int iv = Iv(info.IVs, stat);
            int ev = info.EVs.GetValue(stat);

            if (stat == PokemonStat.HP)
                return HpMaximo(baseStat, iv, ev, info.Level);

            int valor = (2 * baseStat + iv + ev / 4) * info.Level / 100 + 5;
            NatureModifiers natureza = PokemonNatureRules.GetModifiers(info.Nature);

            if (natureza.IncreasedStat == stat)
                return valor * 110 / 100;

            if (natureza.DecreasedStat == stat)
                return valor * 90 / 100;

            return valor;
        }

        public static string QuebrarLinhas(string texto, int maximoPorLinha)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            var resultado = new System.Text.StringBuilder();
            int atual = 0;

            foreach (string palavra in texto.Split(' '))
            {
                if (atual > 0 && atual + 1 + palavra.Length > maximoPorLinha)
                {
                    resultado.Append('\n');
                    atual = 0;
                }
                else if (atual > 0)
                {
                    resultado.Append(' ');
                    atual++;
                }

                resultado.Append(palavra);
                atual += palavra.Length;
            }

            return resultado.ToString();
        }

        public static string Id(PokemonSetController.SetInfo info) =>
            info.PokemonId.ToString("000", CultureInfo.InvariantCulture);

        public static string Nome(PokemonSetController.SetInfo info) => info.Nome;

        // Nome com a forma, para dicas de tela: "Vulpix (Alolan Form)".
        public static string NomeCompleto(PokemonSetController.SetInfo info) =>
            string.Equals(info.FormName, "Normal", StringComparison.OrdinalIgnoreCase)
                ? info.Nome
                : $"{info.Nome} ({info.FormName})";
    }
}
