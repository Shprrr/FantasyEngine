using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace FantasyEngine.Classes.Overworld
{
    public class Event : GameComponent
    {
        public enum eType
        {
            Teleport,
            OnEnter
        }

        public eType Type { get; protected set; }
        public Rectangle EventSize { get; set; }

        public string TeleportMap { get; set; }
        public Vector2 TeleportPosition { get; set; }

        /// <summary>
        /// Event de type Teleport
        /// </summary>
        /// <param name="eventSize">Zone to teleport</param>
        /// <param name="teleportMap">Where to teleport</param>
        /// <param name="teleportPosition">Where to teleport</param>
        public Event(Game game, Rectangle eventSize, string teleportMap, Vector2 teleportPosition)
            : base(game)
        {
            Type = eType.Teleport;
            EventSize = eventSize;
            TeleportMap = teleportMap;
            TeleportPosition = teleportPosition;
        }

        /// <summary>
        /// Event de type OnEnter
        /// </summary>
        /// <param name="eventSize">Zone to enter to trigger the event</param>
        public Event(Game game, Rectangle eventSize)
            : base(game)
        {
            Type = eType.OnEnter;
            EventSize = eventSize;
        }

        public override string ToString()
        {
            if (Type == eType.Teleport)
                return "Teleport to " + TeleportMap + " at " + TeleportPosition;

            if (Type == eType.OnEnter)
                return "Enter at " + EventSize + (OnEnter != null ? " for " + OnEnter.Method.Name : string.Empty);

            return base.ToString();
        }

        public void Teleport()
        {
            if (Type != eType.Teleport)
                return;

            Player.GamePlayer.Hero.Position = Vector2.Multiply(TeleportPosition, Sprite.OVERWORLD_SIZE);
            Game game = Player.GamePlayer.Map.Game;
            Player.GamePlayer.Map = new Map(game, TeleportMap, Player.GamePlayer.Hero.Position - Overworld.CAMERA_CENTER);
        }

        #region Events
        public delegate void OnEnterHandler(EventArgs e, Event eve, GameTime gameTime);
        public event OnEnterHandler OnEnter;
        public virtual void RaiseOnEnter(GameTime gameTime)
        {
            // Raise the event by using the () operator.
            if (OnEnter != null)
                OnEnter(new EventArgs(), this, gameTime);
        }
        #endregion Events
    }
}
