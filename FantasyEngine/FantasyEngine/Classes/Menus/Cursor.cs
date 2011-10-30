using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FantasyEngine.Classes.Menus
{
    public class Cursor : DrawableGameComponent
    {
        private int _CursorIndex = 0;
        private int _ItemMax = 0;

        #region Properties
        public int CursorIndex
        {
            get { return _CursorIndex; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value >= ItemMax)
                    value = _ItemMax == 0 ? 0 : ItemMax - 1;
                _CursorIndex = value;
            }
        }
        public int ItemMax
        {
            get { return _ItemMax; }
            set
            {
                if (value < 0)
                    value = 0;
                _ItemMax = value;
                if (CursorIndex >= _ItemMax)
                    _CursorIndex = _ItemMax == 0 ? 0 : _ItemMax - 1;
            }
        }
        public int ColumnMax { get; set; }

        /// <summary>
        /// Position to draw the cursor.  Don't need the camera translation.
        /// </summary>
        public Vector2 Position { get; set; }

        public SpriteEffects Effects { get; set; }
        #endregion Properties

        public Cursor(Game game, int itemMax, int columnMax = 1)
            : base(game)
        {
            ItemMax = itemMax;
            ColumnMax = columnMax;
            Effects = SpriteEffects.None;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GameMain.spriteBatchGUI.Draw(GameMain.cursor, Position + GameMain.spriteBatchGUI.CameraOffset,
                null, Color.White, 0, Vector2.Zero, 1, Effects, 0);
        }

        public static void DrawShadow(GameTime gameTime, Vector2 position, SpriteEffects effects = SpriteEffects.None)
        {
            GameMain.spriteBatchGUI.Draw(GameMain.cursor, position + GameMain.spriteBatchGUI.CameraOffset,
                null, Color.White * 0.5f, 0, Vector2.Zero, 1, effects, 0);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Input.keyStateHeld.IsKeyDown(Keys.Up))
            {
                if ((ColumnMax == 1 && Input.keyStateDown.IsKeyDown(Keys.Up)) ||
                    CursorIndex >= ColumnMax)
                {
                    // Move cursor up
                    CursorIndex = (CursorIndex - ColumnMax + ItemMax) % ItemMax;
                }

                Input.PutDelay(Keys.Up);
                return;
            }

            if (Input.keyStateHeld.IsKeyDown(Keys.Down))
            {
                if ((ColumnMax == 1 && Input.keyStateDown.IsKeyDown(Keys.Down)) ||
                    CursorIndex < ItemMax - ColumnMax)
                {
                    // Move cursor down
                    CursorIndex = (CursorIndex + ColumnMax) % ItemMax;
                }

                Input.PutDelay(Keys.Down);
                return;
            }

            if (Input.keyStateHeld.IsKeyDown(Keys.Left))
            {
                // If column count is 2 or more, and cursor position is more back than 0
                if (ColumnMax >= 2 && CursorIndex > 0)
                {
                    // Move cursor left
                    CursorIndex -= 1;
                }

                Input.PutDelay(Keys.Left);
                return;
            }

            if (Input.keyStateHeld.IsKeyDown(Keys.Right))
            {
                // If column count is 2 or more, and cursor position is closer to front
                // than (item count -1)
                if (ColumnMax >= 2 && CursorIndex < ItemMax - 1)
                {
                    // Move cursor right
                    CursorIndex += 1;
                }

                Input.PutDelay(Keys.Right);
                return;
            }
        }
    }
}
