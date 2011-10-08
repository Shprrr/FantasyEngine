using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FantasyEngine.Classes
{
    /// <summary>
    /// Unused and unfinished.
    /// </summary>
    public class NumericDropDown : Window
    {
        private int _CursorIndex = 0;

        public string Text { get; set; }
        public int Value { get; set; }

        public NumericDropDown(Game game, int x, int y, int width, string text, int value = 0)
            : base(game, x, y, width, (4 * GameMain.font.LineSpacing) + (Tileset.TileHeight * 2))
        {
            Text = text;
            Value = value;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!Visible)
                return;

            GameMain.Scissor(InsideBound);
            GameMain.spriteBatch.DrawString(GameMain.font, Text, new Vector2(InsideBound.X, InsideBound.Y) + Offset, Color.White);

            SpriteFont font = _CursorIndex == 0 ? GameMain.font : GameMain.font8;
            GameMain.spriteBatch.DrawString(font, "+",
                new Vector2(InsideBound.Center.X - font.MeasureString("+").X, InsideBound.Y + GameMain.font.LineSpacing) + Offset, Color.White);

            Vector2 valueSize = GameMain.font.MeasureString(Value.ToString());
            GameMain.spriteBatch.DrawString(GameMain.font, Value.ToString(),
                new Vector2(InsideBound.Center.X - valueSize.X, InsideBound.Y + GameMain.font.LineSpacing * 2) + Offset, Color.White);

            font = _CursorIndex == 1 ? GameMain.font : GameMain.font8;
            GameMain.spriteBatch.DrawString(font, "-",
                new Vector2(InsideBound.Center.X - font.MeasureString("-").X, InsideBound.Y + GameMain.font.LineSpacing * 3) + Offset, Color.White);

            GameMain.ScissorReset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Input.UpdateInput(gameTime))
                return;

            if (!Enabled)
                return;

            //If cursor is movable
            if (Input.keyStateHeld.IsKeyDown(Keys.Down))
            {
                if (_CursorIndex > 0)
                {
                    _CursorIndex -= 1;
                }

                Input.PutDelay(Keys.Down);
                return;
            }

            if (Input.keyStateHeld.IsKeyDown(Keys.Up))
            {
                if (_CursorIndex < 1)
                {
                    _CursorIndex += 1;
                }

                Input.PutDelay(Keys.Up);
                return;
            }
        }
    }
}
