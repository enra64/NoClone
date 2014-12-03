using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

using CodenameProjectTwo;

namespace CodenameProjectTwo.Buildings
{
    class Centre  : CodenameProjectTwo.CInterfaces.IDrawable

    {
        Sprite cSprite;
        Texture cTexture;
        Vector2f cSize;

        public Centre(int _type, bool _faction, int _ID, Vector2f _position, float _health)
        {
            Type = _type;
            ID = _ID;
            Faction = _faction;
            Position = _position;
            Health = _health;
            cTexture = CGlobal.BUILDING_TEXTURES[Type];
            cSprite = new Sprite(cTexture);
            cSprite.Position = this.Position;
            cSize = new Vector2f(Texture.Size.X, Texture.Size.Y);
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

        public Texture Texture
        {
            get
            {
                return this.cTexture;
            }
            set
            {
                this.cTexture = value;
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
