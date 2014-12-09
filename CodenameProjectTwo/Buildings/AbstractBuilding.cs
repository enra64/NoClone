using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo.Buildings {
    abstract class AbstractBuilding : CInterfaces.IDrawable {
         #region variables
        //use like variables
        /// <summary>
        /// type of the building; you can use either one class for multiple types or
        /// one type per class; the classes will be instanced via a switch() listening
        /// to the type
        /// </summary>
        public abstract int Type { get; set; }

        /// <summary>
        /// the id of this instance; dont change it, because the server
        /// ensures that it is unique. 
        /// if you do change it, the game will crash
        /// because of you. how does that make you feel?
        /// </summary>
        public abstract int ID { get; set; }

        /// <summary>
        /// False->client 1; true-> client 2. probably will bite us in the ass
        /// as soon as we want more than two players.
        /// </summary>
        public abstract byte Faction { get; set; }

        /// <summary>
        /// gets set on instanciation, is the position of your building
        /// </summary>
        public abstract Vector2f Position { get; set; }

        /// <summary>
        /// gets set to 100 on instanciation; please stay between 0 and 100.
        /// </summary>
        public abstract float Health { get; set; }
        public abstract Sprite Sprite { get; set; }
        public abstract Texture Texture { get; set; }
        public abstract Vector2f Size {get; set;}

        /// <summary>
        /// this could access the cglobal descriptions; the arrayposition will be type
        /// </summary>
        public abstract string Description { get; set; }

        /*
         * VARIABLES END HERE
         */
        #endregion

        protected AbstractBuilding(int _type, byte _faction, int _ID, Vector2f _position, float _health){
            Type = _type;
            ID = _ID;
            Faction = _faction;
            Position = _position;
            Health = _health;
            Description = CGlobal.BUILDING_DESCRIPTIONS[Type];
            Texture = CGlobal.BUILDING_TEXTURES[Type];
            Sprite = new Sprite(Texture);
            Sprite.Position = this.Position;
            Size = new Vector2f(Texture.Size.X, Texture.Size.Y);
        }

        /// <summary>
        /// a short name that is displayed by the id
        /// btw dont use umlauts
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// return the center of your building here
        /// </summary>
        public Vector2f Center {
            get { return new Vector2f(this.Position.X + (float)this.Size.X / 2f, this.Position.Y + (float)this.Size.Y / 2f); }
        }

        /// <summary>
        /// return a floatrect around your building
        /// </summary>
        public FloatRect BoundingRectangle {
            get { return new FloatRect(this.Position.X, this.Position.Y, this.Size.X, this.Size.Y); }
        }

        /// <summary>
        /// Draw your building...
        /// </summary>
        public void Draw(){
            Client.cRenderWindow.Draw(Sprite);
        }
    }
}
