using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoDePokemon.model
{
    internal class PokemonBase
    {
        public int idPokedex;
        public int nome_base {  get; set; }
        public string categoria { get; set; }
        public int geracao_origem { get; set; }
        public enum GroupEgg
        {
            monsterGroup,
            waterGroup1,
            bugGroup,
            flyingGroup,
            fielGroup,
            fairyGroup,
            grassGroup,
            HumanLikeGroup,
            waterGroup3,
            mineralGroup,
            amorphousGroup,
            waterGroup2,
            dittoGroup,
            dragonGroup,
            noEgssDiscoveredGroup
        }
        public int id_linha_evolutiva;
    }
}
