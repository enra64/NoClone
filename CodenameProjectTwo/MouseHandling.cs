using Lidgren.Network;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectTwo
{
    static class MouseHandling
    {
        //handle right-click-mouse-moving
        private static Vector2f mouseMovementStartingPoint;
        private static bool rightButtonClicked = false;

        public static void mouseRelease(object sender, MouseButtonEventArgs e)
        {
            if (rightButtonClicked == false)
                return;
            //check whether the mouse moved significantly or not
            FloatRect checkRect = new FloatRect(mouseMovementStartingPoint.X - 4f, mouseMovementStartingPoint.Y - 4f, 8f, 8f);
            if (checkRect.Contains(e.X, e.Y))
                Console.WriteLine("should handle this somehow");
            else
                Client.cView.Move(new Vector2f(e.X - mouseMovementStartingPoint.X, e.Y - mouseMovementStartingPoint.Y));
            Console.WriteLine(checkRect);
            Console.WriteLine(e.X + " x;y " + e.Y);
        }

        public static void Scrolling(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                Client.cView.Zoom(1.02f);
            else
                Client.cView.Zoom(0.98f);
        }

        /// <summary>
        /// Handle mouse clicks, decide whether to move map
        /// or send event to server
        /// </summary>
        public static void mouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                rightButtonClicked = false;
                Console.WriteLine(e.X + ": x, y: " + e.Y);
                NetOutgoingMessage mes = Client.netClient.CreateMessage();
                //identify message as mouseclick
                mes.Write(CGlobal.MOUSE_CLICK_MESSAGE);
                //write x
                mes.Write(e.X + CGlobal.CURRENT_WINDOW_ORIGIN.X);
                //write y
                mes.Write(e.Y + CGlobal.CURRENT_WINDOW_ORIGIN.Y);
                //send
                Client.netClient.SendMessage(mes, NetDeliveryMethod.ReliableOrdered);
                Client.netClient.FlushSendQueue();
            }
            else if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                rightButtonClicked = true;
                mouseMovementStartingPoint = new Vector2f(e.X, e.Y);
            }
        }
    }
}
