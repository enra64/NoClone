using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo {
    
    abstract class AbstractClientItem {

        #region variables
        //use like variables
        /// <summary>
        /// type of the building; you can use either one class for multiple types or
        /// one type per class; the classes will be instanced via a switch() listening
        /// to the type
        /// </summary>
        public virtual int Type { get; set; }

        /// <summary>
        /// the id of this instance; dont change it, because the server
        /// ensures that it is unique. 
        /// if you do change it, the game will crash
        /// because of you. how does that make you feel?
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        /// False->client 1; true-> client 2. probably will bite us in the ass
        /// as soon as we want more than two players.
        /// </summary>
        public virtual byte Faction { get; set; }

        /// <summary>
        /// gets set on instanciation, is the position of your building
        /// </summary>
        public virtual Vector2f Position { get; set; }

        /// <summary>
        /// gets set to 100 on instanciation; please stay between 0 and 100.
        /// </summary>
        public virtual float Health { get; set; }
        public virtual Sprite Sprite { get; set; }
        public virtual Texture Texture { get; set; }
        private Vector2f size;

        /// <summary>
        /// this could access the cglobal descriptions; the arrayposition will be type
        /// </summary>
        public virtual string Description { get; set; }

        private RectangleShape outline;
        /*
         * VARIABLES END HERE
         */
        #endregion
        bool isAnim = false;
        bool People;
        /// <summary>
        /// Constructor for buildings and ressources
        /// </summary>
        protected AbstractClientItem(int _type, byte _faction, int _ID, Vector2f _position, float _health) {
            CommonConstructor(_type, _faction, _ID, _position, _health, false);
        }

        /// <summary>
        /// People Constructor
        /// </summary>
        protected AbstractClientItem(int _type, byte _faction, int _ID, Vector2f _position, float _health, bool people) {
            CommonConstructor(_type, _faction, _ID, _position, _health, people);
        }

        private void CommonConstructor(int _type, byte _faction, int _ID, Vector2f _position, float _health, bool people) {
            Type = _type;
            ID = _ID;
            Faction = _faction;
            Position = _position;
            Health = _health;
            People = people;
            if (people) {
                Description = CGlobal.PEOPLE_DESCRIPTIONS[Type - CGlobal.PEOPLE_ID_OFFSET];
                Texture = CGlobal.PEOPLE_TEXTURES[Type - CGlobal.PEOPLE_ID_OFFSET];
            }
            else {
                Description = CGlobal.BUILDING_DESCRIPTIONS[Type];
                Texture = CGlobal.BUILDING_TEXTURES[Type];
            }
            Sprite = new Sprite(Texture);
            
            Sprite.Position = this.Position;
            outline = new RectangleShape(new Vector2f(Sprite.Scale.X * Texture.Size.X, Sprite.Scale.Y * Texture.Size.Y));
            outline.OutlineColor = Color.White;
            outline.FillColor = Color.Transparent;
            outline.OutlineThickness = 1.5f;
        }

        /// <summary>
        /// a short name that is displayed by the id
        /// btw dont use umlauts
        /// </summary>
        public string Name { get; set; }

        public Vector2f Size {
            get {
                return new Vector2f(Texture.Size.X * Sprite.Scale.X, Texture.Size.Y * Sprite.Scale.Y);
            }
        }

        /// <summary>
        /// return the center of your building here
        /// </summary>
        public Vector2f Center {
            get { return new Vector2f(this.Position.X + (float)this.Size.X / 2f, this.Position.Y + (float)this.Size.Y / 2f); }
        }

        public void DrawOutline() {
            outline.Position = this.Position;
            Client.cRenderWindow.Draw(outline);
        }

        /// <summary>
        /// return a floatrect around your building
        /// </summary>
        public FloatRect BoundingRectangle {
            get { return new FloatRect(this.Position.X, this.Position.Y, this.Size.X, this.Size.Y); }
        }

        public int WoodCost() {
            if (Type < CGlobal.PEOPLE_ID_OFFSET)
                return CGlobal.BUILDING_COSTS_WOOD[Type];
            else
                return CGlobal.BUILDING_COSTS_WOOD[Type - CGlobal.PEOPLE_ID_OFFSET];

        }

        public int StoneCost() {
            if (Type < CGlobal.PEOPLE_ID_OFFSET)
                return CGlobal.BUILDING_COSTS_STONE[Type];
            else
                return CGlobal.BUILDING_COSTS_STONE[Type - CGlobal.PEOPLE_ID_OFFSET];
        }

        /// <summary>
        /// Draw your building...
        /// </summary>
        public void Draw() {
            Sprite.Position = this.Position;
            Client.cRenderWindow.Draw(Sprite);

            
            if (isAnim == false)
            {
                isAnim = true;
                DelayUtil.delayUtil(150, () => CGlobal.textureVector.X = 1);
                DelayUtil.delayUtil(300, () => CGlobal.textureVector.X = 0);
                DelayUtil.delayUtil(450, () => isAnim = false);
            }
            if (People && Type == 100)
            {
                Sprite.TextureRect = new IntRect(CGlobal.textureVector.X * 124, CGlobal.textureVector.Y * 173, 124, 173);
            }

            if (People && Type == 101)
            {
                Sprite.TextureRect = new IntRect(CGlobal.textureVector.X * 146, CGlobal.textureVector.Y * 184, 146, 184);
            }
        }

    }
}
