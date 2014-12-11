using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer {
    abstract class AbstractServerItem : SInterfaces.ISendable {
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
        public virtual bool UpdateNeeded { get; set; }
        public virtual int TargetID { get; set; }
        public virtual Vector2f Target { get; set; }
        public virtual bool implementAggroOrEffectEffects { get; set; }

        /*
         * VARIABLES END HERE
         */
        #endregion

        protected AbstractServerItem(int _type, byte _faction, int _ID, Vector2f _position, float _health) {
            Type = _type;
            ID = _ID;
            Faction = _faction;
            Position = _position;
            Health = _health;
        }

        /// <summary>
        /// a short name that is displayed by the id
        /// btw dont use umlauts
        /// </summary>
        public string Name { get; set; }

        public FloatRect aggroRectangle {
            get { return new FloatRect(this.Position.X - SGlobal.AGGRO_RECT_SIZE / 2, this.Position.Y - SGlobal.AGGRO_RECT_SIZE / 2, SGlobal.AGGRO_RECT_SIZE, SGlobal.AGGRO_RECT_SIZE); }
        }

        public FloatRect effectiveRectangle {
            get {
                return new FloatRect(this.Position.X - SGlobal.EFFECTIVE_RECT_SIZE / 2, this.Position.Y - SGlobal.EFFECTIVE_RECT_SIZE / 2, SGlobal.EFFECTIVE_RECT_SIZE, SGlobal.EFFECTIVE_RECT_SIZE);
            }
        }

        /// <summary>
        /// Draw your building...
        /// </summary>
        public virtual void Update() { }

        public virtual void TakeEffect(int itemID) { }

        public virtual void TargetAggro(int itemID) { }
    }
}