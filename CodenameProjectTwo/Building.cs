using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace CodenameProjectTwo
{
    class Building : CInterfaces.IDrawable
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

        public int Type
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

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public bool Faction
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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public float Health
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

        public float MaxHealth
        {
            get { throw new NotImplementedException(); }
        }

        public Vector2f Center
        {
            get { throw new NotImplementedException(); }
        }

        public FloatRect BoundingRectangle
        {
            get { throw new NotImplementedException(); }
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }


        string CInterfaces.IDrawable.Description
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

        string CInterfaces.IDrawable.Name
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

        public Texture Texture
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

        float CInterfaces.IDrawable.MaxHealth
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
    }
}
