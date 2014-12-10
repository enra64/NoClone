﻿using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

namespace CodenameProjectTwo.Ressources
{
    class Stone : AbstractClientItem {
        //http://i2.kym-cdn.com/photos/images/original/000/234/739/fa5.jpg
        public override int Type { get; set; }
        public override int ID { get; set; }
        public override byte Faction { get; set; }
        public override Vector2f Position { get; set; }
        public override float Health { get; set; }
        public override SFML.Graphics.Sprite Sprite { get; set; }
        public override SFML.Graphics.Texture Texture { get; set; }
        public override Vector2f Size { get; set; }
        public override string Description { get; set; }

        //this should instance the base class; now you only need to set the variables specific to this building, and potentially override methods
        public Stone(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health) {
            Name = "Stone";
        }
    }
}
