using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo
{
    class CInterfaces
    {
        /// <summary>
        /// interface everything you want to send
        /// has to implement
        /// </summary>
        public interface IDrawable
        {
            //idea: WHAT, ID (NOT WHAT), WHERE, HEALTH (possibly reassigned correlating to WHAT parameter)
            /// <summary>
            /// Used for transmitting state
            /// </summary>
            int Type { get; set; }
            /// <summary>
            /// Used for transmitting state
            /// </summary>
            bool Faction { get; set; }
            /// <summary>
            /// Used for transmitting state
            /// </summary>
            int ID { get; set; }
            /// <summary>
            /// Used for transmitting state
            /// </summary>
            Vector2f Position { get; set; }
            /// <summary>
            /// Used for transmitting state
            /// </summary>
            float Health { get; set; }

            /// <summary>
            /// was pretty useful in the past,
            /// just implement it
            /// </summary>
            Vector2f Center { get; }

            /// <summary>
            /// only the client interface has this
            /// </summary>
            void Draw();

        }
    }
}
