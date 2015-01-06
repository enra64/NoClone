using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

namespace CodenameProjectTwo.Ressources
{
    class Wood: AbstractClientItem 
    
        {
        //http://i2.kym-cdn.com/photos/images/original/000/234/739/fa5.jpg
        //this should instance the base class; now you only need to set the variables specific to this building, and potentially override methods
        public Wood(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health) 
            {
            Name = "Wood";
            }
    
        }
}

