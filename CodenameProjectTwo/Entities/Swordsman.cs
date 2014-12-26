using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo
{
    class Swordsman : AbstractClientItem
    {

        public Swordsman(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health, true)
        {
            Name = "Garrosh Hellscream";
        }
    }
}