using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

using CodenameProjectTwo;

namespace CodenameProjectTwo.Buildings
{
    class Centre  : CInterfaces.IDrawable
    {
        //http://i2.kym-cdn.com/photos/images/original/000/234/739/fa5.jpg
        public int Type { get; set; }

        public int ID { get; set; }

        public bool Faction { get; set; }

        public Vector2f Position { get; set; }

        public float Health { get; set; }

        
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
            Texture = CGlobal.BUILDING_TEXTURES[Type];
            cSprite = new Sprite(Texture);
            cSprite.Position = this.Position;
            cSize = new Vector2f(Texture.Size.X, Texture.Size.Y);
        }

        /// <summary>
        /// this could access the cglobal descriptions; the arrayposition will be type
        /// </summary>
        public string Description
        {
            get { return CGlobal.BUILDING_DESCRIPTIONS[Type]; }
        }

        /// <summary>
        /// a short name that is displayed by the id
        /// btw dont use umlauts
        /// </summary>
        public string Name
        {
            get { return "Red Centre"; }
        }

        /// <summary>
        /// return the center of your building here
        /// </summary>
        public Vector2f Center
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// return a floatrect around your building
        /// </summary>
        public FloatRect BoundingRectangle
        {
            get {
                return new FloatRect(this.Position.X, this.Position.Y, this.cSize.X, this.cSize.Y);
            }
        }

        /// <summary>
        /// Draw your building...
        /// </summary>
        public void Draw()
        {
            Client.cRenderWindow.Draw(cSprite);
        }

        /// <summary>
        /// Return the texture (not the path to texture) of your building here
        /// </summary>
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
    }
}