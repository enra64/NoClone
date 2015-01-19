using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo.Ressources
{
    class RessourceKeeper
    {
        public float Wood { get; set; }
        public float Stone { get; set; }
		public byte Faction {get; set;}

        public RessourceKeeper(byte _faction, float _stones, float _wood)
        {
            Faction = _faction;
            Stone = _stones;
            Wood = _wood;
        }
    }
}
