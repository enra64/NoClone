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
        #region variables
        //use like variables
        /// <summary>
        /// type of the building; you can use either one class for multiple types or
        /// one type per class; the classes will be instanced via a switch() listening
        /// to the type
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// the id of this instance; dont change it, because the server
        /// ensures that it is unique. 
        /// if you do change it, the game will crash
        /// because of you. how does that make you feel?
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// False->client 1; true-> client 2. probably will bite us in the ass
        /// as soon as we want more than two players.
        /// </summary>
        public bool Faction { get; set; }

        /// <summary>
        /// gets set on instanciation, is the position of your building
        /// </summary>
        public Vector2f Position { get; set; }

        /// <summary>
        /// gets set to 100 on instanciation; please stay between 0 and 100.
        /// </summary>
        public float Health { get; set; }

        /*
         * VARIABLES END HERE
         */
        #endregion

        public Building(int _type, bool _faction, int _ID, Vector2f _position, float _health)
        {
            Type = _type;
            ID = _ID;
            Faction = _faction;
            Position = _position;
            Health = _health;
        }

        /// <summary>
        /// this could access the cglobal descriptions; the arrayposition will be type
        /// </summary>
        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// a short name that is displayed by the id
        /// btw dont use umlauts
        /// </summary>
        public string Name
        {
            get { return "so heisse ich"; }
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
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Draw your building...
        /// </summary>
        public void Draw()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the texture (not the path to texture) of your building here
        /// </summary>
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
    }
}
