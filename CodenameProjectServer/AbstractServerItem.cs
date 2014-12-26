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
        public virtual bool IsRessource { get; set; }
        public virtual bool IsBuilding { get; set; }
        public virtual bool IsTroop { get; set; }

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
            get {
                SizeKeeper match = SGlobal.SizeList.Find(i => i.Type == this.Type);
                if (match == null)
                    return new FloatRect();
                else
                    return new FloatRect(this.Position.X - 4, this.Position.Y - 4, match.X + 8, match.Y + 8);
            }
        }

        /// <summary>
        /// If the server knows the size of the object, it returns a floatrect slightly larger than the actual size.
        /// If the server does _not_ know the size, an empty floatrect will be returned; be aware!
        /// </summary>
        public FloatRect effectiveRectangle {
            get {
                SizeKeeper match = SGlobal.SizeList.Find(i => i.Type == this.Type);
                if (match == null)
                    return new FloatRect();
                else
                    return new FloatRect(this.Position.X, this.Position.Y, match.X, match.Y);
            }
        }

        /// <summary>
        /// Update your building...
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// called when the smaller target rectangle is hit
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void TakeEffect(int itemID) { }

        /// <summary>
        /// called when the larger rectangle is hit;
        /// this is allowed to change the target, for example if a swordsman is just
        /// standing around, and then "sees" an enemy.
        /// </summary>
        /// <param name="itemID"></param>
        public virtual void TargetAggro(int itemID) { }
    }
}