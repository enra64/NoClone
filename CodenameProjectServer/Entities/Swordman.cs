using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer.Entities
{
    class Swordman : AbstractServerItem
    {
        private float MovementSpeed = 1;

        //call standard constructor of base class, see abstractserveritem
        public Swordman(int _type, byte _faction, int _ID, Vector2f _position, float _health)
            : base(_type, _faction, _ID, _position, _health)
        {
            //do something additional to the standard constructor
            implementAggroOrEffectEffects = true;
            IsTroop = true;
        }

        public override void TakeEffect(int itemID)
        {
            //okay since this aint no building you should do something here, for example stopping the movement so that the peasant stops glitching or something
            if (Server.Sendlist[itemID].Health <= 0 || Server.Sendlist[itemID].Faction != this.Faction)
                return;
            //Server.Sendlist[itemID].Health -= 0.1f;
            if (Server.Sendlist[itemID].IsBuilding)
            {
                Server.Sendlist[itemID].Health -= 0.1f; 
            }
            if (Server.Sendlist[itemID].IsTroop)
            {
                Server.Sendlist[itemID].Health -= 0.5f;
            }
        }

        public override void TargetAggro(int itemID)
        {
            //dont do anything - this is a building
        }

        public override void Update()
        {
            Vector2f calc = Target;
            Vector2f diff = new Vector2f(calc.X - Position.X, calc.Y - Position.Y);
            if (diff.X == 0 && diff.Y == 0)
            {
                return;
            }
            float skalar = (float)Math.Sqrt((diff.X * diff.X) + (diff.Y * diff.Y));
            diff = new Vector2f(diff.X / skalar, diff.Y / skalar);
            Position = new Vector2f(Position.X + (diff.X * MovementSpeed), Position.Y + (diff.Y * MovementSpeed));
        }
    }
}

