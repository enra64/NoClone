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
        private int type, id;
        private bool faction;
        private Vector2f position;
        private float health;

        public Building(int _type, bool _faction, int _ID, Vector2f _position, float _health)
        {
            type = _type;
            id = _ID;
            faction = _faction;
            position = _position;
            health = _health;
        }
        //look at the nonimplementedness
        public int Type
        {
            get { throw new NotImplementedException(); }
        }

        public bool Faction
        {
            get { throw new NotImplementedException(); }
        }

        public int ID
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

        public Vector2f Position
        {
            get { throw new NotImplementedException(); }
        }

        public float Health
        {
            get { throw new NotImplementedException(); }
        }

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
            throw new NotImplementedException();
        }
    }
}
