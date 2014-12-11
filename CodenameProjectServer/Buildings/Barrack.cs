using System;

using SFML;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectServer {
    //http://i2.kym-cdn.com/photos/images/original/000/234/739/fa5.jpg
    //works, though - 10/10
    class Barrack : AbstractServerItem {
        public Barrack(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health) {
            //do something additional to the standard constructor, in this case disable checking for actions from this item
            implementAggroOrEffectEffects = false;
        }
    }
}
