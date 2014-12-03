using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

using CodenameProjectTwo;

namespace CodenameProjectTwo.buildings
{
    class VillageCentre : CodenameProjectTwo.CInterfaces.IDrawable
    {
   
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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                Description = "The centre and the main building of a faction is the village centre. If it gets destroyed the whole fation would scatter and lose its power!";
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                Name = "Village Centre";
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
                Texture = new Texture("assets/graphics/buildings/hqred.png");
                throw new NotImplementedException();
            }
        }

        public bool Faction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                Faction = true;
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
                ID = 2;
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
                Position = new Vector2f(100,100);
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
                Health = 1000;
                throw new NotImplementedException();
            }
        }

        public float MaxHealth
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                MaxHealth = 1000;
                throw new NotImplementedException();
            }
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
    }
}
