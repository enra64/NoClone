﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo.Ressources
{
    class RessourceKeeper
    {
		public int Wood {get; set;}
		public int Stone {get; set;}
		public byte Faction {get; set;}

        public RessourceKeeper(byte _faction, int _stones, int _wood)
        {
            Faction = _faction;
            Stone = _stones;
            Wood = _wood;
        }
    }
}