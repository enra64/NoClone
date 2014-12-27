using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer.Entities {
    class Peasant : AbstractServerItem {
        private float MovementSpeed = 1;
        private bool cancelMovement = false;
        private SGlobal.Direction CollisionDirection;

        //call standard constructor of base class, see abstractserveritem
        public Peasant(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health) {
            //do something additional to the standard constructor
            implementAggroOrEffectEffects = true;
            IsTroop = true;
        }

        public override void TakeEffect(int itemID) {
            //okay since this aint no building you should do something here, for example stopping the movement so that the peasant stops glitching or something
            if(Server.Sendlist[itemID].Health<=0)
                return;
            if (Server.Sendlist[itemID].IsRessource)
            {
                if (Server.Sendlist[itemID].Type == SGlobal.RESSOURCE_WOOD)
                    Server.RessourceList[Faction-1].Wood += SGlobal.RESSOURCE_INCREASE_WOOD;
                if (Server.Sendlist[itemID].Type == SGlobal.RESSOURCE_STONE)
                    Server.RessourceList[Faction - 1].Stone += SGlobal.RESSOURCE_INCREASE_STONE;
            }
            CollisionDirection = this.checkCollisionDirection(itemID);
        }

        public override void TargetAggro(int itemID) {
            //dont do anything - this is a building
        }

        public override void Update() {
                Vector2f calc = Target;
                Vector2f diff = new Vector2f(calc.X - Position.X, calc.Y - Position.Y);
                if (diff.X == 0 && diff.Y == 0) {
                    return;
                }
                float skalar = (float)Math.Sqrt((diff.X * diff.X) + (diff.Y * diff.Y));
                diff = new Vector2f(diff.X / skalar, diff.Y / skalar);
                diff = CancelMovement(diff);
                Position = new Vector2f(Position.X + (diff.X * MovementSpeed), Position.Y + (diff.Y * MovementSpeed));
            
        }

        private Vector2f CancelMovement(Vector2f _nextmove) {
            //abort if unknown direction
            if (CollisionDirection == null || CollisionDirection == SGlobal.Direction.Uhhhh)
                return _nextmove;
            //if we have a collision moving upwards, cancel out that part, if rightwards, cancel out x etc
            Vector2f cleanedVector=new Vector2f(_nextmove.X, _nextmove.Y);
            if (CollisionDirection == SGlobal.Direction.Top && _nextmove.Y < 0)
                cleanedVector.Y = 0;
            if (CollisionDirection == SGlobal.Direction.Bottom && _nextmove.Y > 0)
                cleanedVector.Y = 0;
            if (CollisionDirection == SGlobal.Direction.Left && _nextmove.X < 0)
                cleanedVector.X = 0;
            if (CollisionDirection == SGlobal.Direction.Right && _nextmove.X > 0)
                cleanedVector.X = 0;
            return cleanedVector;
        }
    }
}

