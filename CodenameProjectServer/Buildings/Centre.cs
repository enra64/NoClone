using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

namespace CodenameProjectServer.Buildings {
    //http://i2.kym-cdn.com/photos/images/original/000/234/739/fa5.jpg
    class Centre : AbstractServerItem {
        public override bool implementAggroOrEffectEffects { get; set; }
        public override int Type { get; set; }
        public override int ID { get; set; }
        public override byte Faction { get; set; }
        public override Vector2f Position { get; set; }
        public override float Health { get; set; }
        public override bool UpdateNeeded { get; set; }
        public override Vector2f Target { get; set; }
        public override int TargetID { get; set; }

        public Centre(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health) {
            //do something additional to the standard constructor, in this case disable checking for actions from this item
            implementAggroOrEffectEffects = false;
        }

        public override void TakeEffect(int itemID) {
            //dont do anything - this is a building
            //if this had to do something, it would read the target id from TargetID to determine what to do
        }

        public override void TargetAggro(int itemID) {
            //dont do anything - this is a building
        }

        public override void Update() {

        }
    }
}

