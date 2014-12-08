using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer.Enteties
{
    class Peasent : SInterfaces.ISendable
    {
        public int Type { get; set; }

        public byte Faction { get; set; }

        public int ID { get; set; }

        public Vector2f Position { get; set; }

        public float Health { get; set; }
        public int TargetID { get; set; }

        Vector2f cTarget;

        float MovementSpeed = 1;

        //set to true if you want messages concerning your aggro or effective rectangle
        public bool implementAggroOrEffectEffects { get; private set; }

        public Peasent(int _type, byte _Faction, int _ID, Vector2f _Position, float health)
        {
            Type = _type;
            ID = _ID;
            Faction = _Faction;
            Position = _Position;
            Health = health;
            cTarget = _Position;
            implementAggroOrEffectEffects = true;
        }
        // will be 
        public Vector2f Center
        {
            get { throw new NotImplementedException(); }

        }

        public Vector2f Target
        {
            get
            {
                return cTarget;
            }
            set
            {
                cTarget = value;

            }
        }

        // will be implemented in time 
        public bool UpdateNeeded { get; set; }

        // stuff with moving and such stuff 
        public void Update()
        {
            Vector2f calc = cTarget;
            Vector2f diff = new Vector2f(calc.X - Position.X, calc.Y - Position.Y);
            if (diff.X == 0 && diff.Y == 0)
            {
                return;
            }
            float skalar = (float)Math.Sqrt((diff.X * diff.X) +( diff.Y * diff.Y));
            diff = new Vector2f(diff.X / skalar, diff.Y / skalar);
            Position = new Vector2f(Position.X + (diff.X * MovementSpeed), Position.Y + (diff.Y * MovementSpeed));


        }

        public FloatRect aggroRectangle {
            get {
                return new FloatRect(this.Position.X - SGlobal.AGGRO_RECT_SIZE / 2, this.Position.Y - SGlobal.AGGRO_RECT_SIZE / 2, SGlobal.AGGRO_RECT_SIZE, SGlobal.AGGRO_RECT_SIZE);
            }
        }

        public FloatRect effectiveRectangle {
            get {
                return new FloatRect(this.Position.X - SGlobal.EFFECTIVE_RECT_SIZE / 2, this.Position.Y - SGlobal.EFFECTIVE_RECT_SIZE / 2, SGlobal.EFFECTIVE_RECT_SIZE, SGlobal.EFFECTIVE_RECT_SIZE);
            }
        }

        public void TakeEffect(int itemID) {
            //okay since this aint no building you should do something here, for example stopping the movement so that the peasant stops glitching or something
            Server.Sendlist[itemID].Health -= 0.1f;
        }

        public void TargetAggro(int itemID) {
            //dont do anything - this is a building
        }
    }
}
