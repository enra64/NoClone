using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer
{
    class Building : SInterfaces.ISendable
    {
        //look at the nonimplementedness

        public int Type
        {
            get { throw new NotImplementedException(); }
        }

        public int Subtype
        {
            get { throw new NotImplementedException(); }
        }

        public int ID
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

        public SFML.Window.Vector2f Position
        {
            get { throw new NotImplementedException(); }
        }

        public float Health
        {
            get { throw new NotImplementedException(); }
        }

        public SFML.Window.Vector2f Center
        {
            get { throw new NotImplementedException(); }
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
