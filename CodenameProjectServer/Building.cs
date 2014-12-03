using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectServer
{
    class Building : SInterfaces.ISendable
    {
        public int Type { get; private set; }
        public int ID { get; set; }
        public bool Faction { get; set; }
        public Vector2f Position { get; set; }
        public float Health { get; set; }

        public Building(int _type, bool _faction, int _ID, Vector2f _position, float _health)
        {
            Type = _type;
            ID = _ID;
            Faction = _faction;
            Position = _position;
            Health = _health;
        }
        //look at the nonimplementedness

        public Vector2f Center
        {
            get { throw new NotImplementedException(); }
        }

        public Vector2f Target
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool UpdateNeeded
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Update()
        {
            //throw new NotImplementedException();
        }
    }
}
