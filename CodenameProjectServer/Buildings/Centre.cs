using System;

using SFML;
using SFML.Graphics;
using SFML.Window;

namespace CodenameProjectServer
{
    class Centre : CodenameProjectServer.SInterfaces.ISendable
    {
        //http://i2.kym-cdn.com/photos/images/original/000/234/739/fa5.jpg
        public int Type { get; private set; }
        public int ID { get; set; }
        public byte Faction { get; set; }
        public Vector2f Position { get; set; }
        public float Health { get; set; }

        public Centre(int _type, byte _faction, int _ID, Vector2f _position, float _health)
        {
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
