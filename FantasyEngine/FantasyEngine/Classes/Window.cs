using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FantasyEngine.Classes
{
    public class Window : DrawableGameComponent
    {
        public static Tileset Tileset;

        public enum eHAlign
        {
            HA_LEFT,
            HA_CENTER,
            HA_RIGHT
        };

        private SpriteBatch _SpriteBatch;
        public Rectangle Rectangle;
        public Vector2 Offset;

        /// <summary>
        /// Bound of the inside of the window.  It's where is legal to draw.
        /// </summary>
        public Rectangle InsideBound
        {
            get
            {
                return new Rectangle(Rectangle.X + Tileset.TileWidth, Rectangle.Y + Tileset.TileHeight,
                    Rectangle.Width - (Tileset.TileWidth * 2), Rectangle.Height - (Tileset.TileHeight * 2));
            }
        }

        public Window(Game game, int x, int y, int width, int height)
            : base(game)
        {
            Rectangle = new Rectangle(x, y, width, height);
            _SpriteBatch = GameMain.spriteBatch;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!Visible)
                return;

            Rectangle pos;

            pos = Rectangle;
            pos.Offset((int)Offset.X, (int)Offset.Y);
            _SpriteBatch.Draw(Tileset.texture, pos, Tileset.GetSourceRectangle(4),
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); //Fond


            pos = new Rectangle(Rectangle.X + Tileset.TileWidth, Rectangle.Y,
                Rectangle.Width - (Tileset.TileWidth * 2), Tileset.TileHeight);
            pos.Offset((int)Offset.X, (int)Offset.Y);
            _SpriteBatch.Draw(Tileset.texture, pos, Tileset.GetSourceRectangle(1),
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); //Bord Haut

            pos = new Rectangle(Rectangle.X, Rectangle.Y + Tileset.TileHeight,
                Tileset.TileWidth, Rectangle.Height - (Tileset.TileHeight * 2));
            pos.Offset((int)Offset.X, (int)Offset.Y);
            _SpriteBatch.Draw(Tileset.texture, pos, Tileset.GetSourceRectangle(3),
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); //Bord Gauche

            pos = new Rectangle(Rectangle.Right - Tileset.TileWidth, Rectangle.Y + Tileset.TileHeight,
                Tileset.TileWidth, Rectangle.Height - (Tileset.TileHeight * 2));
            pos.Offset((int)Offset.X, (int)Offset.Y);
            _SpriteBatch.Draw(Tileset.texture, pos, Tileset.GetSourceRectangle(5),
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); //Bord Droite

            pos = new Rectangle(Rectangle.X + Tileset.TileWidth, Rectangle.Bottom - Tileset.TileHeight,
                Rectangle.Width - (Tileset.TileWidth * 2), Tileset.TileHeight);
            pos.Offset((int)Offset.X, (int)Offset.Y);
            _SpriteBatch.Draw(Tileset.texture, pos, Tileset.GetSourceRectangle(7),
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); //Bord Bas

            pos = new Rectangle(Rectangle.X, Rectangle.Y,
                Tileset.TileWidth, Tileset.TileHeight);
            pos.Offset((int)Offset.X, (int)Offset.Y);
            _SpriteBatch.Draw(Tileset.texture, pos, Tileset.GetSourceRectangle(0),
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); //Coin HG

            pos = new Rectangle(Rectangle.Right - Tileset.TileWidth, Rectangle.Y,
                Tileset.TileWidth, Tileset.TileHeight);
            pos.Offset((int)Offset.X, (int)Offset.Y);
            _SpriteBatch.Draw(Tileset.texture, pos, Tileset.GetSourceRectangle(2),
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); //Coin HD

            pos = new Rectangle(Rectangle.X, Rectangle.Bottom - Tileset.TileHeight,
                Tileset.TileWidth, Tileset.TileHeight);
            pos.Offset((int)Offset.X, (int)Offset.Y);
            _SpriteBatch.Draw(Tileset.texture, pos, Tileset.GetSourceRectangle(6),
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); //Coin BG

            pos = new Rectangle(Rectangle.Right - Tileset.TileWidth, Rectangle.Bottom - Tileset.TileHeight,
                Tileset.TileWidth, Tileset.TileHeight);
            pos.Offset((int)Offset.X, (int)Offset.Y);
            _SpriteBatch.Draw(Tileset.texture, pos, Tileset.GetSourceRectangle(8),
                Color.White, 0, Vector2.Zero, SpriteEffects.None, 0); //Coin BD
        }

        public void ChangeOffset(int x, int y)
        {
            Rectangle = new Rectangle(x, y, Rectangle.Width, Rectangle.Height);
        }

        public static void ShowDialog(Game game, int x, int y, string text, eHAlign halign = eHAlign.HA_LEFT)
        {
            Vector2 textSize = GameMain.font.MeasureString(text);
            Window window;

            switch (halign)
            {
                case eHAlign.HA_LEFT:
                    window = new Window(game, x, y,
                        (int)(x + textSize.X + (Tileset.TileWidth * 2)),
                        (int)(y + (textSize.Y * 2) + (Tileset.TileHeight * 2)));

                    //window.Draw();

                    SpriteBatch spriteBatch = new SpriteBatch(game.GraphicsDevice);

                    spriteBatch.Begin();

                    spriteBatch.DrawString(GameMain.font, text, new Vector2(x + Tileset.TileWidth,
                        y + Tileset.TileHeight), Color.White);
                    spriteBatch.DrawString(GameMain.font, "OK", new Vector2(x + (textSize.X / 2) + Tileset.TileWidth - 1,
                        y + textSize.Y + Tileset.TileHeight), Color.White);

                    spriteBatch.End();
                    break;
                case eHAlign.HA_CENTER:
                    throw new NotImplementedException();
                //break;
                case eHAlign.HA_RIGHT:
                    throw new NotImplementedException();
                //break;
            }
            /*
            int textWidth = text.length() * pFontNormal->tilew;
            Window* pWin;

            switch(halign)
            {
            case HA_LEFT:
                pWin = new Window(x, y, 
                    x + textWidth + (pWindowTileset->tilew * 2), 
                    y + (pFontNormal->tileh * 2) + (pWindowTileset->tileh * 2));

                pWin->Draw();

                GRRLIB_Printf(x + pWindowTileset->tilew, 
                    y + pWindowTileset->tileh, pFontNormal, clrNormal, 1, text.c_str());
                GRRLIB_Printf(x + (textWidth / 2) + pWindowTileset->tilew - 1, 
                    y + pFontNormal->tileh + pWindowTileset->tileh, pFontNormal, clrNormal, 1, "OK");

                delete(pWin);
                break;

            case HA_CENTER:
                pWin = new Window(x - pWindowTileset->tilew - (textWidth/2-1), 
                    y - 16, x + (pWindowTileset->tilew * 2) + textWidth/2, y + 16);

                pWin->Draw();

                GRRLIB_Printf(x - (textWidth / 2 - 1), y - 12, pFontNormal, clrNormal, 1, text.c_str());
                GRRLIB_Printf(x - pFontNormal->tilew, y, pFontNormal, clrNormal, 1, "OK");

                delete(pWin);
                break;

            case HA_RIGHT:
                pWin = new Window(x - textWidth - (pWindowTileset->tilew * 2), y, 
                    x, y + (pFontNormal->tileh * 2) + (pWindowTileset->tileh * 2));

                pWin->Draw();

                GRRLIB_Printf(x - textWidth - pWindowTileset->tilew, 
                    y + pWindowTileset->tileh, pFontNormal, clrNormal, 1, text.c_str());
                GRRLIB_Printf(x - (textWidth / 2) - pWindowTileset->tilew + 1, 
                    y + pFontNormal->tileh + pWindowTileset->tileh, pFontNormal, clrNormal, 1, "OK");

                delete(pWin);
                break;
            }
            */
        }
    }
}
