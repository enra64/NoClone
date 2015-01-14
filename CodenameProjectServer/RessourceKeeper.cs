using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer
{
    class RessourceKeeper
    {
		public Int32 Wood {get; set;}
		public Int32 Stone {get; set;}
		public byte Faction {get; set;}

        public RessourceKeeper(byte _faction, int _stones, int _wood)
        {
            Faction = _faction;
            Stone = _stones;
            Wood = _wood;
        }

        public void subtract(int stone, int wood){
            Stone -= stone;
            Wood -= wood;
        }
    }
}
