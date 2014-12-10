using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectServer.Buildings {
    class Building : AbstractServerItem {
        public override bool implementAggroOrEffectEffects { get; set; }
        public override int Type { get; set; }
        public override int ID { get; set; }
        public override byte Faction { get; set; }
        public override Vector2f Position { get; set; }
        public override float Health { get; set; }
        public override bool UpdateNeeded { get; set; }
        public override Vector2f Target { get; set; }
        public override int TargetID { get; set; }

        public Building(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health) {
            //do something additional to the standard constructor
            implementAggroOrEffectEffects = false;
        }
    }
}
