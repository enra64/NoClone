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

        Vector2f cTarget;

        float MovementSpeed = 1;

        public Peasent(int _type, byte _Faction, int _ID, Vector2f _Position, float health)
        {
            Type = _type;
            ID = _ID;
            Faction = _Faction;
            Position = _Position;
            Health = health;
            cTarget = _Position;

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

    }
}
