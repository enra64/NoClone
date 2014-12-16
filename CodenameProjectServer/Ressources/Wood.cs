using System;

using SFML;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectServer.Ressources
{
    class Wood : AbstractServerItem
    {
        public Wood(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health)
        {
            //do something additional to the standard constructor, in this case disable checking for actions from this item
            implementAggroOrEffectEffects = false;
        }
    }
}

