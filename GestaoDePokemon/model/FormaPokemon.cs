using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoDePokemon.model
{
    public struct baseStats
    {
        public int HP { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int SpAtk { get; set; }
        public int SpDef { get; set; }
        public int Speed { get; set; }

        public baseStats(int hp, int atk, int def, int spAtk, int spDef, int speed)
        {
            HP = hp;
            Atk = atk;
            Def = def;
            SpAtk = spAtk;
            SpDef = spDef;
            Speed = speed;
        }
        public override string ToString()
        {
            return $"HP: {HP} | Atk: {Atk} | Def: {Def} | SpAtk: {SpAtk} | SpDef: {SpDef} | Speed: {Speed}";
        }
    }
    internal class FormaPokemon
    {
        public int id_linha_evolutiva;
        public int idPokedex;
        public string nomeDaForma {  get; set; }
        public enum tipo
        {
            normal,
            fogo,
            lutador,
            agua,
            voador,
            grama,
            venenoso,
            eletrico,
            terra,
            pisiquico,
            pedra,
            gelo,
            bug,
            dragao,
            fantasma,
            sombrio,
            aco,
            fada
        }

        public baseStats status { get; set; }
        


    }
}
