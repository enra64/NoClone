using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer
{
    class SInterfaces
    {
        /// <summary>
        /// interface everything you want to send
        /// has to implement
        /// </summary>
        public interface ISendable
        {
            //idea: WHAT, ID (NOT WHAT), WHERE, HEALTH (possibly reassigned correlating to WHAT parameter)
            /// <summary>
            /// Used for transmitting state
            /// </summary>
            int Type { get; }

            /// <summary>
            /// Used for transmitting state; contains the faction
            /// </summary>
            bool Faction { get; }

            /// <summary>
            /// This _MUST_ be set to the value of SGlobal.ID_COUNTER at time of creation,
            /// which _HAS_ to be increased afterwards for my server protocol to work!
            /// </summary>
            int ID { get; set; }

            /// <summary>
            /// Used for transmitting state
            /// </summary>
            Vector2f Position { get; }
            /// <summary>
            /// Used for transmitting state
            /// </summary>
            float Health { get; }

            /// <summary>
            /// was pretty useful in the past,
            /// just implement it
            /// </summary>
            Vector2f Center{ get; }

            /// <summary>
            /// generic function for updating.
            /// the server will later send the four send-parameters to each client
            /// </summary>
            void Update();
            
        }
    }
}
