using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

namespace CodenameProjectTwo.Ressources
{
    class Stone : CInterfaces.IDrawable
    {
       public int Type { get; set; }

        public int ID { get; set; }

        public byte Faction { get; set; }

        public Vector2f Position { get; set; }

        public float Health { get; set; }

        
        Sprite sSprite;
        Texture sTexture;
        Vector2f sSize;

        public Stone(int _type, byte _faction, int _ID, Vector2f _position, float _health)
        {
            Type = _type;
            ID = _ID;
            Faction = _faction;
            Position = _position;
            Health = _health;
            Texture = CGlobal.RESSOURCE_TEXTURES[Type];
            sSprite = new Sprite(Texture);
            sSprite.Position = this.Position;
            sSize = new Vector2f(Texture.Size.X, Texture.Size.Y);
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
            get { return "Stone"; }
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
                return new FloatRect(this.Position.X, this.Position.Y, this.sSize.X, this.sSize.Y);
            }
        }

        /// <summary>
        /// Draw your building...
        /// </summary>
        public void Draw()
        {
            Client.cRenderWindow.Draw(sSprite);
        }

        /// <summary>
        /// Return the texture (not the path to texture) of your building here
        /// </summary>
        public Texture Texture
        {
            get
            {
                return this.sTexture;
            }
            set
            {
                this.sTexture = value;
            }
        }
    }
}
