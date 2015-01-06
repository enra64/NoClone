using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo.Buildings {
    class Barrack : AbstractClientItem {
        //this should instance the base class; now you only need to set the variables specific to this building, and potentially override methods
        public Barrack(int _type, byte _faction, int _ID, Vector2f _position, float _health) : base(_type, _faction, _ID, _position, _health) {
            Name = "Barrack";
        }
    }
}