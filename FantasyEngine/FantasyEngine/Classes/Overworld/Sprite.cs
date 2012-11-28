using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FantasyEngine.Classes.Overworld
{
    public class Sprite : DrawableGameComponent
    {
        public enum eDirection
        {
            DOWN,
            LEFT,
            UP,
            RIGHT
        }

        public static readonly Point OVERWORLD_SIZE = new Point(32, 32);

        private Tileset _SpriteImage;
        private uint _Frame = 0;
        private TimeSpan _MovingTime = TimeSpan.Zero;
        private eDirection _Direction = eDirection.DOWN;

        /// <summary>
        /// Direction of the sprite.
        /// </summary>
        public eDirection Direction
        {
            get { return _Direction; }
            set
            {
                // Si nouvelle direction, on remet le frame d'animation à la bonne place.
                if (_Direction != value)
                    _Frame = (uint)(nbFrameAnimation * (int)value);
                _Direction = value;
            }
        }

        /// <summary>
        /// Position in pixel on the screen.
        /// </summary>
        public Vector2 Position { get; set; }
        public Point Size { get; set; }

        /// <summary>
        /// Image of the sprite.
        /// </summary>
        public Tileset SpriteImage { get { return _SpriteImage; } }

        /// <summary>
        /// Number of frame per direction.
        /// </summary>
        private const int nbFrameAnimation = 2;

        public Sprite(Game game, string charsetName, Rectangle tileSize, Vector2 position, Point size)
            : base(game)
        {
            ChangeSprite(charsetName, tileSize, size);
            Position = position;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (_MovingTime != TimeSpan.Zero && ((gameTime.TotalGameTime - _MovingTime).TotalMilliseconds / (10 * GameMain.MILLISECOND_PER_FRAME)) >= 1)
            {
                AnimateWalking();
                _MovingTime += TimeSpan.FromMilliseconds(10 * GameMain.MILLISECOND_PER_FRAME);
            }

            GameMain.spriteBatch.Draw(_SpriteImage.texture, getRectangle(), _SpriteImage.GetSourceRectangle(_Frame), Color.White);
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
                _MovingTime = gameTime.TotalGameTime;
                return;
            }

            if (Input.keyStateHeld.IsKeyDown(Keys.Up)
                || Input.keyStateHeld.IsKeyDown(Keys.Down)
                || Input.keyStateHeld.IsKeyDown(Keys.Left)
                || Input.keyStateHeld.IsKeyDown(Keys.Right))
            {
                ChangeDirection(Input.keyStateHeld);
                return;
            }
            else
                _MovingTime = TimeSpan.Zero;
        }

        public void AnimateWalking()
        {
            _Frame = (uint)(((_Frame + 1) % nbFrameAnimation) + ((int)Direction * nbFrameAnimation));
        }

        /// <summary>
        /// Change the direction of the sprite.
        /// </summary>
        /// <param name="keyState">Input that tells the new direction</param>
        void ChangeDirection(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Up))
            {
                Direction = eDirection.UP;
                return;
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                Direction = eDirection.DOWN;
                return;
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                Direction = eDirection.LEFT;
                return;
            }

            if (keyState.IsKeyDown(Keys.Right))
            {
                Direction = eDirection.RIGHT;
                return;
            }
        }

        /// <summary>
        /// Change the direction to the opposite of the given direction.  It should be facing this new direction.
        /// </summary>
        /// <param name="direction"></param>
        public void OppositeDirection(eDirection direction)
        {
            switch (direction)
            {
                case eDirection.DOWN:
                    Direction = eDirection.UP;
                    break;
                case eDirection.LEFT:
                    Direction = eDirection.RIGHT;
                    break;
                case eDirection.UP:
                    Direction = eDirection.DOWN;
                    break;
                case eDirection.RIGHT:
                    Direction = eDirection.LEFT;
                    break;
            }
        }

        /// <summary>
        /// Get the drawing zone of the sprite.
        /// </summary>
        /// <returns></returns>
        public Rectangle getRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Size.X, Size.Y);
        }

        /// <summary>
        /// Get the collision zone of the sprite.
        /// </summary>
        /// <returns></returns>
        public Rectangle getCollisionRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Size.X - 1, Size.Y - 1);
        }

        public void ChangeSprite(string charsetName, Rectangle tileSize, Point spriteSize)
        {
            Texture2D texture = Game.Content.Load<Texture2D>(@"Images\" + charsetName);
            if (tileSize == Rectangle.Empty)
                _SpriteImage = new Tileset(texture, texture.Width / (nbFrameAnimation * 4), texture.Height);
            else
                _SpriteImage = new Tileset(texture, tileSize, tileSize.Width / (nbFrameAnimation * 4), tileSize.Height);

            if (spriteSize == Point.Zero)
                Size = new Point(_SpriteImage.TileWidth, _SpriteImage.TileHeight);
            else
                Size = spriteSize;
        }
    }
}
