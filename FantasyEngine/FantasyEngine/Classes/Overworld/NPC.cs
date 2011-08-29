﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TiledLib;

namespace FantasyEngine.Classes.Overworld
{
    public class NPC : Sprite
    {
        public enum eAction
        {
            Stay,
            Moving,
            Blocked,
            Talking
        }

        private Window _MessageWindow;
        private int _MessageWindowBottomY;
        private string _Message = "";
        private eDirection _InitialDirection;

        public string Name { get; set; }
        public eAction Action { get; private set; }
        public bool RegainDirectionAfterTalk { get; set; }
        public int Step { get; set; }

        public NPC(Game game, string name, string charsetName, Vector2 position)
            : base(game, charsetName, position)
        {
            Name = name;
            RegainDirectionAfterTalk = false;

            int height = (4 * GameMain.font.LineSpacing) + (Window.Tileset.TileHeight * 2);
            _MessageWindowBottomY = Game.GraphicsDevice.Viewport.Height - height;
            _MessageWindow = new Window(Game, 0, _MessageWindowBottomY,
                Game.GraphicsDevice.Viewport.Width, height);
            _MessageWindow.Enabled = false;
            _MessageWindow.Visible = false;
        }

        public NPC(Game game, string name, string charsetName, Vector2 position, eDirection direction)
            : this(game, name, charsetName, position)
        {
            _InitialDirection = direction;
            Direction = direction;
        }

        public NPC(Game game, string name, string charsetName, Vector2 position, eDirection direction, bool regainDirection)
            : this(game, name, charsetName, position, direction)
        {
            RegainDirectionAfterTalk = regainDirection;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            DrawTalk(gameTime);
        }

        private byte frame = 0;
        private void DrawTalk(GameTime gameTime)
        {
            if (!_MessageWindow.Visible)
                return;

            frame = (byte)((frame + 1) % 16);

            _MessageWindow.Offset = GameMain.CameraOffset;
            _MessageWindow.Draw(gameTime);

            GameMain.Scissor(_MessageWindow.InsideBound);

            GameMain.spriteBatch.DrawString(GameMain.font, _Message, new Vector2(_MessageWindow.InsideBound.X, _MessageWindow.InsideBound.Y) + GameMain.CameraOffset, Color.White);

            GameMain.ScissorReset();

            if (frame < 12)
                GameMain.spriteBatch.Draw(Window.NextDialog,
                    new Vector2(_MessageWindow.InsideBound.Right - Window.NextDialog.Width,
                        _MessageWindow.Rectangle.Bottom - Window.NextDialog.Height * 2) + GameMain.CameraOffset,
                    Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            RaiseOnMoving();

            if (_MessageWindow.Enabled && Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                _MessageWindow.Enabled = false;
                _MessageWindow.Visible = false;
                if (RegainDirectionAfterTalk)
                    Direction = _InitialDirection;
                Player.GamePlayer.Hero.Enabled = true;
                Action = eAction.Stay;
                Input.CatchKeys(Keys.Enter);
            }
        }

        public void Stay()
        {
            Action = eAction.Stay;
        }

        public void Talk(string message)
        {
            _Message = message;
            _MessageWindow.Enabled = true;
            _MessageWindow.Visible = true;
            Player.GamePlayer.Hero.Enabled = false;
            Action = eAction.Talking;
        }

        public void Move(eDirection direction)
        {
            Vector2 newOffset;

            if (CheckCollision(direction, out newOffset))
            {
                //Player.GamePlayer.Map.Offset += newOffset;
                Position += newOffset;
                //AnimateWalking(); //TODO: Revoir l'animation quand on fera la partie sur l'animation.
                Action = eAction.Moving;
            }
            else
            {
                Action = eAction.Blocked;
            }
        }

        /// <summary>
        /// Return if it's free of moving in the direction.
        /// </summary>
        /// <param name="direction">Direction of moving</param>
        /// <param name="newOffset">Where it will land if it moves in the direction</param>
        /// <returns>Return if it's free of moving in the direction.</returns>
        public bool CheckCollision(eDirection direction, out Vector2 newOffset)
        {
            int step = 2; //TODO: Constante
            Rectangle npcRect = getRectangle();

            Vector2 npc1 = Vector2.Zero;
            Vector2 npc2 = Vector2.Zero;

            newOffset = Vector2.Zero;

            if (direction == eDirection.RIGHT)
            {
                newOffset = new Vector2(step, 0);

                npc1 = new Vector2(npcRect.Right, npcRect.Top);
                npc2 = new Vector2(npcRect.Right, npcRect.Bottom);
            }

            if (direction == eDirection.LEFT)
            {
                newOffset = new Vector2(-step, 0);

                npc1 = new Vector2(npcRect.Left, npcRect.Top);
                npc2 = new Vector2(npcRect.Left, npcRect.Bottom);
            }

            if (direction == eDirection.DOWN)
            {
                newOffset = new Vector2(0, step);

                npc1 = new Vector2(npcRect.Left, npcRect.Bottom);
                npc2 = new Vector2(npcRect.Right, npcRect.Bottom);
            }

            if (direction == eDirection.UP)
            {
                newOffset = new Vector2(0, -step);

                npc1 = new Vector2(npcRect.Left, npcRect.Top);
                npc2 = new Vector2(npcRect.Right, npcRect.Top);
            }

            // Clamp the camera so it never leaves the area of the map.
            Vector2 cameraMax = new Vector2(
                Player.GamePlayer.Map.MapData.Width * Player.GamePlayer.Map.MapData.TileWidth - 1,
                Player.GamePlayer.Map.MapData.Height * Player.GamePlayer.Map.MapData.TileHeight - 1);
            newOffset = Vector2.Clamp(npc1 + newOffset, Vector2.Zero, cameraMax) - npc1;
            newOffset = Vector2.Clamp(npc2 + newOffset, Vector2.Zero, cameraMax) - npc2;

            TileLayer layer = (TileLayer)Player.GamePlayer.Map.MapData.GetLayer("Collision");
            Point tile1 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(npc1 + newOffset);
            Point tile2 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(npc2 + newOffset);

            bool npcCollision = false;
            // Check collision with the player.
            if (this != Player.GamePlayer.Hero)
            {
                Vector2 vect = npc1 + newOffset;
                npcCollision = npcCollision | Player.GamePlayer.Hero.getRectangle().Contains((int)vect.X, (int)vect.Y);
                vect = npc2 + newOffset;
                npcCollision = npcCollision | Player.GamePlayer.Hero.getRectangle().Contains((int)vect.X, (int)vect.Y);
            }

            // Check collision with other npcs.
            foreach (NPC npc in Player.GamePlayer.Map.NPCs)
            {
                Vector2 vect = npc1 + newOffset;
                npcCollision = npcCollision | npc.getRectangle().Contains((int)vect.X, (int)vect.Y);
                vect = npc2 + newOffset;
                npcCollision = npcCollision | npc.getRectangle().Contains((int)vect.X, (int)vect.Y);
            }

            return layer.Tiles[tile1.X, tile1.Y] == null
                && layer.Tiles[tile2.X, tile2.Y] == null
                && !npcCollision;
        }

        /// <summary>
        /// Return if it's free of moving in the direction.
        /// </summary>
        /// <param name="direction">Direction of moving</param>
        /// <returns>Return if it's free of moving in the direction.</returns>
        public bool CheckCollision(eDirection direction)
        {
            Vector2 newOffset;
            return CheckCollision(direction, out newOffset);
        }

        #region Events
        public delegate void TalkingHandler(EventArgs e, NPC npc);
        public event TalkingHandler Talking;
        public virtual void RaiseOnTalking()
        {
            // Raise the event by using the () operator.
            if (Talking != null)
                Talking(new EventArgs(), this);
        }

        public delegate void MovingHandler(EventArgs e, NPC npc);
        public event MovingHandler Moving;
        public virtual void RaiseOnMoving()
        {
            // Raise the event by using the () operator.
            if (Moving != null)
                Moving(new EventArgs(), this);
        }
        #endregion Events
    }
}