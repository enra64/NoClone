using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo.Entities {
    class Peasant : AbstractClientItem {
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

        //this should instance the base class; now you only need to set the variables specific to this item, and potentially override methods
        public Peasant(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health, true /*says we want to load from people array*/) {
            //do stuff additional to the base constructor
            Name = "Billy";
        }

        /*
         * If you want to change a method of the base class (AbstractClientItem), you can do it like this:
         *         
         * public override void Draw() {
         *      //hyper advanced super drawing
         * }
         */
    }
}
