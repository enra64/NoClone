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
        private SGlobal.Direction CollisionDirection = SGlobal.Direction.Uhhhh;

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
            //peasant log:
            //day 715: they still havent noticed i am a peasant
            //day 826: I think the default building suspects something. Must destroy him.
            //         Am thinking about transforming him into another building.
        }

        public override void Update() {
                Vector2f calc = Target;
                Vector2f diff = new Vector2f(calc.X - Position.X, calc.Y - Position.Y);
                if (diff.X == 0 && diff.Y == 0)
                    return;
                float skalar = (float)Math.Sqrt((diff.X * diff.X) + (diff.Y * diff.Y));
                diff = new Vector2f(diff.X / skalar, diff.Y / skalar);
                diff = CancelMovement(diff);
                Position = new Vector2f(Position.X + (diff.X * MovementSpeed), Position.Y + (diff.Y * MovementSpeed));
                //clear collisiondirection to avoid bugging
                CollisionDirection = SGlobal.Direction.Uhhhh;
        }

        private Vector2f CancelMovement(Vector2f _nextmove) {
            //abort if unknown direction
            if (CollisionDirection == SGlobal.Direction.Uhhhh)
                return _nextmove;
            //if we have a collision moving upwards, cancel out y, if rightwards, cancel out x etc
            Vector2f cleanedVector=new Vector2f(_nextmove.X, _nextmove.Y);
            if (CollisionDirection == SGlobal.Direction.Top && _nextmove.Y < 0) {
                //cancel movement into object
                cleanedVector.Y = 0;
                //speed up the movement along the side, so that we arent
                //glued to the building
                if (0 < cleanedVector.X && cleanedVector.X < 2f)
                    cleanedVector.X = 2f;
                if (-2f < cleanedVector.X && cleanedVector.X < 0)
                    cleanedVector.X = -2f;
            }
            if (CollisionDirection == SGlobal.Direction.Bottom && _nextmove.Y > 0) {
                cleanedVector.Y = 0;
                if (0 < cleanedVector.X && cleanedVector.X < 2f)
                    cleanedVector.X = 2f;
                if (-2f < cleanedVector.X && cleanedVector.X < 0)
                    cleanedVector.X = - 2f;
            }
            if (CollisionDirection == SGlobal.Direction.Left && _nextmove.X < 0) {
                cleanedVector.X = 0;
                if (0 < cleanedVector.Y && cleanedVector.Y < 2f)
                    cleanedVector.Y = 2f;
                if (-2f < cleanedVector.Y && cleanedVector.Y < 0)
                    cleanedVector.Y = -2f;
            }
            if (CollisionDirection == SGlobal.Direction.Right && _nextmove.X > 0) {
                cleanedVector.X = 0;
                if (0 < cleanedVector.Y && cleanedVector.Y < 2f)
                    cleanedVector.Y = 2f;
                if (-2f < cleanedVector.Y && cleanedVector.Y < 0)
                    cleanedVector.Y = -2f;
            }
            return cleanedVector;
        }
    }
}

