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
            Position = cTarget;
            return;
            Vector2f DifferenzVektor = new Vector2f(cTarget.X - Position.X, cTarget.Y - Position.Y);
            float skalar = (float)Math.Sqrt(Math.Pow(DifferenzVektor.X, 2) + Math.Pow(DifferenzVektor.Y, 2));
            DifferenzVektor = new Vector2f(DifferenzVektor.X / skalar, DifferenzVektor.Y / skalar);
            Position = new Vector2f(Position.X +DifferenzVektor.X*MovementSpeed, Position.Y+DifferenzVektor.Y*MovementSpeed);
        }

    }
}
