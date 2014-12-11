using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

namespace CodenameProjectServer.Buildings {
    //http://i2.kym-cdn.com/photos/images/original/000/234/739/fa5.jpg
    class Centre : AbstractServerItem {
        public Centre(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health) {
            //do something additional to the standard constructor, in this case disable checking for actions from this item
            implementAggroOrEffectEffects = false;
        }
    }
}

