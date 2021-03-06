﻿using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodenameProjectServer {
    abstract class AbstractServerItem {
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
        public virtual bool IsDead { get; set; }
        public virtual SGlobal.Direction CollisionDirection { get; set; }
        internal virtual int CurrentEffectID { get; set; }

        private SGlobal.Direction lastCollisionDirection = SGlobal.Direction.Uhhhh;

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
            IsDead = false;
            Target = Position;
            CollisionDirection = SGlobal.Direction.Uhhhh;
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
        /// read in abstractserveritem.cs...
        /// This function checks eight points on the bounding rectangle.
        /// The points are top left, top middle, top right etc...
        /// If three points on one side are inside of the item with the parameter id,
        /// we know that the item collides on that side. The idea is that if we collide
        /// on one side, we can cancel the movement in that direction, so it should look
        /// like the person glides along the building. 
        /// we may, however, want not to collide other people.
        /// </summary>
        public SGlobal.Direction checkCollisionDirection(Int32 itemID) {
            if (this.Size == null || Server.Sendlist[itemID]==null)
                return SGlobal.Direction.Uhhhh;
            //create the first three points to save ressources
            Vector2f topLeft = new Vector2f(this.Position.X, this.Position.Y);
            Vector2f topMiddle = new Vector2f(this.Position.X + (float)this.Size.X / 2f, this.Position.Y);
            Vector2f topRight = new Vector2f(this.Position.X + (float)this.Size.X, this.Position.Y);
            //write collision of middle here
            //if (Server.Sendlist[itemID].effectiveRectangle.Contains(topMiddle.X, topMiddle.Y))
            //    lastCollisionDirection = SGlobal.Direction.Top;
            //check top collision
            if (Server.Sendlist[itemID].effectiveRectangle.Contains(topMiddle.X, topMiddle.Y)
                    )
                if (Server.Sendlist[itemID].effectiveRectangle.Contains(topLeft.X, topLeft.Y)
                    || Server.Sendlist[itemID].effectiveRectangle.Contains(topRight.X, topRight.Y)) {
                    lastCollisionDirection = SGlobal.Direction.Top;
                    return SGlobal.Direction.Top;
                }

            //okay its not top, check on right.
            Vector2f middleRight = new Vector2f(this.Position.X + (float)this.Size.X, this.Position.Y + (float)this.Size.Y / 2f);
            Vector2f bottomRight = new Vector2f(this.Position.X + (float)this.Size.X, this.Position.Y + (float)this.Size.Y);
            //write collision of middle here
            if (Server.Sendlist[itemID].effectiveRectangle.Contains(middleRight.X, middleRight.Y))
                lastCollisionDirection = SGlobal.Direction.Right;
            if (Server.Sendlist[itemID].effectiveRectangle.Contains(middleRight.X, middleRight.Y)
                    || lastCollisionDirection == SGlobal.Direction.Right)
                if (Server.Sendlist[itemID].effectiveRectangle.Contains(topRight.X, topRight.Y)
                    || Server.Sendlist[itemID].effectiveRectangle.Contains(bottomRight.X, bottomRight.Y)) {
                    lastCollisionDirection = SGlobal.Direction.Right;
                    return SGlobal.Direction.Right;
                }
            //fuuuuck check bottom
            Vector2f bottomMiddle = new Vector2f(this.Position.X + (float)this.Size.X / 2f, this.Position.Y + (float)this.Size.Y);
            Vector2f bottomLeft = new Vector2f(this.Position.X, this.Position.Y + (float)this.Size.Y);
            //write collision of middle here
            if (Server.Sendlist[itemID].effectiveRectangle.Contains(bottomMiddle.X, bottomMiddle.Y))
                lastCollisionDirection = SGlobal.Direction.Bottom;
            if (Server.Sendlist[itemID].effectiveRectangle.Contains(bottomMiddle.X, bottomMiddle.Y)
                    || lastCollisionDirection == SGlobal.Direction.Bottom)
                if (Server.Sendlist[itemID].effectiveRectangle.Contains(bottomLeft.X, bottomLeft.Y)
                    || Server.Sendlist[itemID].effectiveRectangle.Contains(bottomRight.X, bottomRight.Y)) {
                    lastCollisionDirection = SGlobal.Direction.Bottom;
                    return SGlobal.Direction.Bottom;
                }
            //omfg check left
            Vector2f middleLeft = new Vector2f(this.Position.X, this.Position.Y + (float)this.Size.Y / 2f);
            //write collision of middle here
            if (Server.Sendlist[itemID].effectiveRectangle.Contains(middleLeft.X, middleLeft.Y))
                lastCollisionDirection = SGlobal.Direction.Left;
            //check whether we collided once or are currently colliding with middle; if yes, check whether left or right edges are touching
            //also, fuck collision logic
            if (Server.Sendlist[itemID].effectiveRectangle.Contains(middleLeft.X, middleLeft.Y)
                    || lastCollisionDirection == SGlobal.Direction.Left)
                if (Server.Sendlist[itemID].effectiveRectangle.Contains(topLeft.X, topLeft.Y)
                    || Server.Sendlist[itemID].effectiveRectangle.Contains(bottomLeft.X, bottomLeft.Y)) {
                    lastCollisionDirection = SGlobal.Direction.Left;
                    return SGlobal.Direction.Left;
                }
            //fuck dis
            return SGlobal.Direction.Uhhhh;
        }

        internal Vector2f CancelMovement(Vector2f _nextmove) {
            //abort if unknown direction
            if (CollisionDirection == SGlobal.Direction.Uhhhh)
                return _nextmove;
            //if we have a collision moving upwards, cancel out y, if rightwards, cancel out x etc
            Vector2f cleanedVector = new Vector2f(_nextmove.X, _nextmove.Y);
            if (CollisionDirection == SGlobal.Direction.Top && _nextmove.Y < 0) {
                //cancel movement into object
                cleanedVector.Y = 0;
                //speed up the movement along the side, so that we arent
                //glued to the building
                if (0 < cleanedVector.X && cleanedVector.X < 2f)
                    cleanedVector.X = 1.5f;
                if (-2f < cleanedVector.X && cleanedVector.X < 0)
                    cleanedVector.X = -1.5f;
            }
            else if (CollisionDirection == SGlobal.Direction.Bottom && _nextmove.Y > 0) {
                cleanedVector.Y = 0;
                if (0 < cleanedVector.X && cleanedVector.X < 2f)
                    cleanedVector.X = 1.5f;
                if (-2f < cleanedVector.X && cleanedVector.X < 0)
                    cleanedVector.X = -1.5f;
            }
            else if (CollisionDirection == SGlobal.Direction.Left && _nextmove.X < 0) {
                cleanedVector.X = 0;
                if (0 < cleanedVector.Y && cleanedVector.Y < 2f)
                    cleanedVector.Y = 1.5f;
                if (-2f < cleanedVector.Y && cleanedVector.Y < 0)
                    cleanedVector.Y = -1.5f;
            }
            else if (CollisionDirection == SGlobal.Direction.Right && _nextmove.X > 0) {
                cleanedVector.X = 0;
                if (0 < cleanedVector.Y && cleanedVector.Y < 2f)
                    cleanedVector.Y = 1.5f;
                if (-2f < cleanedVector.Y && cleanedVector.Y < 0)
                    cleanedVector.Y = -1.5f;
            }
            return cleanedVector;
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

        public SizeKeeper Size {
            get {
                return SGlobal.SizeList.Find(i => i.Type == this.Type);
            }
        }

        /// <summary>
        /// Update your building...
        /// </summary>
        public void Update() {
            CollisionDirection = checkCollisionDirection(CurrentEffectID);
            if (Health <= 0)
                IsDead = true;
            internalUpdate();
            //clear collisiondirection to avoid bugging
            CollisionDirection = SGlobal.Direction.Uhhhh;
        }

        public virtual void internalUpdate() { }

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