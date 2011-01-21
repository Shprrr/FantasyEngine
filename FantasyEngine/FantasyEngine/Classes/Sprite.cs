using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FantasyEngine.Classes
{
    public class Sprite : DrawableGameComponent
    {
        private enum eDirection
        {
            DOWN,
            LEFT,
            UP,
            RIGHT
        }

        private Tileset _SpriteImage;
        private uint _Frame = 0;
        private eDirection _Direction = eDirection.DOWN;

        /// <summary>
        /// Position in pixel on the screen.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Image of the sprite.
        /// </summary>
        public Tileset SpriteImage { get { return _SpriteImage; } }

        /// <summary>
        /// Number of frame per direction.
        /// </summary>
        private const int nbFrameAnimation = 2;

        public Sprite(Game game, string charsetName, Vector2 position)
            : base(game)
        {
            Texture2D texture = Game.Content.Load<Texture2D>(@"Images\Characters\" + charsetName);
            _SpriteImage = new Tileset(texture, texture.Width / (nbFrameAnimation * 4), texture.Height);
            Position = position;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GameMain.spriteBatch.Draw(_SpriteImage.texture, Position, _SpriteImage.GetSourceRectangle(_Frame), Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Input.UpdateInput(gameTime))
                return;

            if (Input.keyStateDown.IsKeyDown(Keys.Up)
                || Input.keyStateDown.IsKeyDown(Keys.Down)
                || Input.keyStateDown.IsKeyDown(Keys.Left)
                || Input.keyStateDown.IsKeyDown(Keys.Right))
            {
                ChangeDirection(Input.keyStateDown);

                _Frame = (uint)(nbFrameAnimation * (int)_Direction);
                return;
            }

            if (Input.keyStateHeld.IsKeyDown(Keys.Up)
                || Input.keyStateHeld.IsKeyDown(Keys.Down)
                || Input.keyStateHeld.IsKeyDown(Keys.Left)
                || Input.keyStateHeld.IsKeyDown(Keys.Right))
            {
                ChangeDirection(Input.keyStateHeld);

                _Frame = (uint)(((_Frame + 1) % nbFrameAnimation) + ((int)_Direction * nbFrameAnimation));
                Input.PutDelay(10, Input.keyStateHeld.GetPressedKeys());
                return;
            }
        }

        /// <summary>
        /// Change the direction of the sprite.
        /// </summary>
        /// <param name="keyState">Input that tells the new direction</param>
        void ChangeDirection(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Up))
                _Direction = eDirection.UP;

            if (keyState.IsKeyDown(Keys.Down))
                _Direction = eDirection.DOWN;

            if (keyState.IsKeyDown(Keys.Left))
                _Direction = eDirection.LEFT;

            if (keyState.IsKeyDown(Keys.Right))
                _Direction = eDirection.RIGHT;
        }

        /// <summary>
        /// Get the drawing zone of the sprite.
        /// </summary>
        /// <returns></returns>
        public Rectangle getRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, _SpriteImage.TileWidth - 1, _SpriteImage.TileHeight - 1);
        }
    }
}
