using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectServer
{
    class Building : SInterfaces.ISendable
    {
        public int Type { get; private set; }
        public int ID { get; set; }
        public byte Faction { get; set; }
        public Vector2f Position { get; set; }
        public float Health { get; set; }
        public int TargetID { get; set; }

        /*
         FOR HELP LOOK TO CLIENT BUILDING.CS; SERVER ONLY STUFF GETS EXPLAINED HERE
         */

        public Building(int _type, byte _faction, int _ID, Vector2f _position, float _health)
        {
            Type = _type;
            ID = _ID;
            Faction = _faction;
            Position = _position;
            Health = _health;
        }
        //look at the nonimplementedness

        public Vector2f Center
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// this should set a target the building walks to (hint: more for bauers or schwertkaempfers...)
        /// </summary>
        public Vector2f Target
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

        /// <summary>
        /// Keep a record whether the server needs to update the clients on what you are currently doing.
        /// </summary>
        public bool UpdateNeeded
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

        /// <summary>
        /// _only_ update here; the clients must not move or change their health etc; it will be overwritten by the server anyway
        /// </summary>
        public void Update()
        {
            //throw new NotImplementedException();
        }

        public FloatRect aggroRectangle {
            get {
                return new FloatRect(this.Position.X - SGlobal.AGGRO_RECT_SIZE / 2, this.Position.Y - SGlobal.AGGRO_RECT_SIZE / 2, SGlobal.AGGRO_RECT_SIZE, SGlobal.AGGRO_RECT_SIZE);
            }
        }

        public FloatRect effectiveRectangle {
            get {
                return new FloatRect(this.Position.X - SGlobal.EFFECTIVE_RECT_SIZE / 2, this.Position.Y - SGlobal.EFFECTIVE_RECT_SIZE / 2, SGlobal.EFFECTIVE_RECT_SIZE, SGlobal.EFFECTIVE_RECT_SIZE);
            }
        }

        public void TakeEffect() {
            //dont do anything - this is a building
            //if this had to do something, it would read the target id from TargetID to determine what to do
        }

        public void TargetAggro() {
            //dont do anything - this is a building
        }
    }
}
