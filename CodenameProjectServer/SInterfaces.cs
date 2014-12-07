using SFML.Graphics;
using SFML.Window;
using System;


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
            byte Faction { get; }

            /// <summary>
            /// Basically just dont change this value
            /// </summary>
            int ID { get; set; }

            /// <summary>
            /// Absolute Position of Item
            /// </summary>
            Vector2f Position { get; }

            /// <summary>
            /// Health, pretty self-explanatory
            /// </summary>
            float Health { get; }

            /// <summary>
            /// was pretty useful in the past,
            /// just implement it
            /// </summary>
            Vector2f Center{ get; }

            /// <summary>
            /// Target id instead of position
            /// </summary>
            int TargetID { get; set; }

            bool implementAggroOrEffectEffects { get; }

            /// <summary>
            /// if your effRect intersects with the targets effRect,
            /// TakeEffect() will be called, and you need to handle that.
            /// </summary>
            FloatRect effectiveRectangle { get; }

            /// <summary>
            /// see effectiveRectangle
            /// </summary>
            void TakeEffect(int itemID);


            /// <summary>
            /// if something of the enemy is within this rectangle, and no target
            /// is set, TargetAggro() will be called, so that you can move towards
            /// the aggro
            /// </summary>
            FloatRect aggroRectangle { get; }

            /// <summary>
            /// see aggroRectangle
            /// </summary>
            void TargetAggro(int itemId);

            /// <summary>
            /// If the server gets notified about the unit being supposed to move,
            /// it uses the setter of target, so do what needs to be done to start
            /// the movement there
            /// </summary>
            Vector2f Target { get; set; }

            /// <summary>
            /// This value is asked by the server to determine whether it should send
            /// or not send an update for this item. Just save your item state in a
            /// few variables, and check after each Update() whether we need to update
            /// it.
            /// </summary>
            bool UpdateNeeded { get; set; }

            /// <summary>
            /// generic function for updating.
            /// the server will later send the four send-parameters to each client
            /// </summary>
            void Update();
            
        }
    }
}
