using System;

using SFML;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectServer.Ressources {
    class Stone : AbstractServerItem {
        public override bool implementAggroOrEffectEffects { get; set; }
        public override int Type { get; set; }
        public override int ID { get; set; }
        public override byte Faction { get; set; }
        public override Vector2f Position { get; set; }
        public override float Health { get; set; }
        public override bool UpdateNeeded { get; set; }
        public override Vector2f Target { get; set; }
        public override int TargetID { get; set; }

        public Stone(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health) {
            //do something additional to the standard constructor, in this case disable checking for actions from this item
            implementAggroOrEffectEffects = false;
        }
    }
}

