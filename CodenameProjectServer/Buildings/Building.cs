using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectServer.Buildings {
    class Building : AbstractServerBuilding {

        /*
            public Building(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            {
                Type = _type;
                ID = _ID;
                Faction = _faction;
                Position = _position;
                Health = _health;
            }
        }
        */
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
