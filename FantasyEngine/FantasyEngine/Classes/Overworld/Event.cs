using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace FantasyEngine.Classes.Overworld
{
    public class Event
    {
        public enum eType
        {
            Teleport
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
        public Event(Rectangle eventSize, string teleportMap, Vector2 teleportPosition)
        {
            Type = eType.Teleport;
            EventSize = eventSize;
            TeleportMap = teleportMap;
            TeleportPosition = teleportPosition;
        }

        public override string ToString()
        {
            if (Type == eType.Teleport)
                return "Teleport to " + TeleportMap + " at " + TeleportPosition;

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
    }
}
