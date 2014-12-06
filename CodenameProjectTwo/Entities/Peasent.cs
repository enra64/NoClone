using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo.Entities
{
    class Peasent : CInterfaces.IDrawable
    {
        public int Type { get; set; }

        public byte Faction { get; set; }

        public int ID { get; set; }

        public Vector2f Position { get; set; }

        public float Health { get; set; }

        Vector2f cTarget;
        Sprite pSprite;
        Texture Texture;
        Vector2f pSize;
        public Peasent(int _type, byte _Faction, int _ID, Vector2f _Position, float health)
        {
            Type = _type;
            ID = _ID;
            Faction = _Faction;
            Position = _Position;
            Health = health;

            Texture = CGlobal.PEOPLE_TEXTURES[CGlobal.PEOPLE_PEASANT - CGlobal.PEOPLE_ID_OFFSET];
            pSprite = new Sprite(Texture);
            pSprite.Position = this.Position;
            pSize = new Vector2f(Texture.Size.X, Texture.Size.Y);

        }
        // will be 
        public Vector2f Center
        {
            get
            {
                throw new NotImplementedException();
            }
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
        public void Draw()
        {
            pSprite.Position = Position;
            Client.cRenderWindow.Draw(pSprite);
        }



        public string Description
        {
            get { return CGlobal.PEOPLE_DESCRIPTIONS[1]; }
        }

        public string Name
        {
            get { return "Billy"; }
        }

        Texture CInterfaces.IDrawable.Texture
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


        public FloatRect BoundingRectangle
        {
            get {
                return new FloatRect(this.Position.X, this.Position.Y, this.pSize.X, this.pSize.Y);
            }
        }


    }
}
