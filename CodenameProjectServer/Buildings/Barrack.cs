using System;

using SFML;
using SFML.Window;
using SFML.Graphics;

namespace CodenameProjectServer
{
    class Barrack : SInterfaces.ISendable
    {
        //http://i2.kym-cdn.com/photos/images/original/000/234/739/fa5.jpg
        //works, though - 10/10
        public int Type { get; private set; }
        public int ID { get; set; }
        public byte Faction { get; set; }
        public Vector2f Position { get; set; }
        public float Health { get; set; }
        public int TargetID { get; set; }

        //set to true if you want messages concerning your aggro or effective rectangle
        public bool implementAggroOrEffectEffects { get; private set; }

        public Barrack(int _type, byte _faction, int _ID, Vector2f _position, float _health){
            Type = _type;
            ID = _ID;
            Faction = _faction;
            Position = _position;
            Health = _health;
        }

        public SFML.Window.Vector2f Center
        {
            get { throw new NotImplementedException(); }
        }

        public SFML.Window.Vector2f Target
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

        public void TakeEffect(int itemID) {
            //dont do anything - this is a building
            //if this had to do something, it would read the target id from TargetID to determine what to do
        }

        public void TargetAggro(int itemID) {
            //dont do anything - this is a building
        }

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

        public void Update()
        {
        }
    }
}
