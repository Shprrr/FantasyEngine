using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;

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

		public const int OVERWORLD_TILE_SIZE = 32;
		public static readonly Vector2 OVERWORLD_SIZE = new Vector2(OVERWORLD_TILE_SIZE);
		public const float MOVE_PX_PER_MILLISECOND = 0.12f;

		private Tileset _SpriteImage;
		private uint _Frame = 0;
		protected TimeSpan _MovingTime = TimeSpan.Zero;
		private eDirection _Direction = eDirection.DOWN;
		protected float _MovePxPerMillisecond = MOVE_PX_PER_MILLISECOND;

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
		public Vector2 Size { get; set; }

		/// <summary>
		/// Image of the sprite.
		/// </summary>
		public Tileset SpriteImage { get { return _SpriteImage; } }

		/// <summary>
		/// Number of frame per direction.
		/// </summary>
		private const int nbFrameAnimation = 2;

		public Sprite(Game game, string charsetName, Rectangle tileSize, Vector2 position, Vector2 size)
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

		public void AnimateWalking()
		{
			_Frame = (uint)(((_Frame + 1) % nbFrameAnimation) + ((int)Direction * nbFrameAnimation));
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
		/// Return if it's free of moving in the direction.
		/// </summary>
		/// <param name="direction">Direction of moving</param>
		/// <param name="newOffset">Where it will land if it moves in the direction</param>
		/// <param name="maxStep">Maximum allowed to go in that direction</param>
		/// <returns>Return if it's free of moving in the direction.</returns>
		public bool CheckCollision(GameTime gameTime, eDirection direction, out Vector2 newOffset, int maxStep = int.MaxValue)
		{
			int step = (int)(gameTime.ElapsedGameTime.TotalMilliseconds * _MovePxPerMillisecond);
			if (step > maxStep) step = maxStep;
			Rectangle spriteRect = getCollisionRectangle();
			newOffset = Vector2.Zero;

			// If the step if greater than the space to clip to something solid, this will clip to the nearest place possible.
			while (step > 0)
			{
				Vector2 sprite1 = Vector2.Zero;
				Vector2 sprite2 = Vector2.Zero;

				newOffset = Vector2.Zero;

				if (direction == eDirection.RIGHT)
				{
					newOffset = new Vector2(step, 0);

					sprite1 = new Vector2(spriteRect.Right, spriteRect.Top);
					sprite2 = new Vector2(spriteRect.Right, spriteRect.Bottom);
				}

				if (direction == eDirection.LEFT)
				{
					newOffset = new Vector2(-step, 0);

					sprite1 = new Vector2(spriteRect.Left, spriteRect.Top);
					sprite2 = new Vector2(spriteRect.Left, spriteRect.Bottom);
				}

				if (direction == eDirection.DOWN)
				{
					newOffset = new Vector2(0, step);

					sprite1 = new Vector2(spriteRect.Left, spriteRect.Bottom);
					sprite2 = new Vector2(spriteRect.Right, spriteRect.Bottom);
				}

				if (direction == eDirection.UP)
				{
					newOffset = new Vector2(0, -step);

					sprite1 = new Vector2(spriteRect.Left, spriteRect.Top);
					sprite2 = new Vector2(spriteRect.Right, spriteRect.Top);
				}

				// Clamp the camera so it never leaves the area of the map.
				Vector2 cameraMax = new Vector2(
					Player.GamePlayer.Map.MapData.Width * Player.GamePlayer.Map.MapData.TileWidth - 1,
					Player.GamePlayer.Map.MapData.Height * Player.GamePlayer.Map.MapData.TileHeight - 1);
				newOffset = Vector2.Clamp(sprite1 + newOffset, Vector2.Zero, cameraMax) - sprite1;
				newOffset = Vector2.Clamp(sprite2 + newOffset, Vector2.Zero, cameraMax) - sprite2;

				TileLayer layer = (TileLayer)Player.GamePlayer.Map.MapData.GetLayer(Map.LAYER_NAME_COLLISION);
				Point tile1 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(sprite1 + newOffset);
				Point tile2 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(sprite2 + newOffset);

				bool npcCollision = false;
				// Check collision with the player.
				if (this != Player.GamePlayer.Hero)
				{
					Vector2 vect = sprite1 + newOffset;
					npcCollision = npcCollision | Player.GamePlayer.Hero.getCollisionRectangle().Contains((int)vect.X, (int)vect.Y);
					vect = sprite2 + newOffset;
					npcCollision = npcCollision | Player.GamePlayer.Hero.getCollisionRectangle().Contains((int)vect.X, (int)vect.Y);
				}

				// Check collision with other npcs.
				foreach (NPC npc in Player.GamePlayer.Map.NPCs)
				{
					if (this == npc)
						continue;

					Vector2 vect = sprite1 + newOffset;
					npcCollision = npcCollision | npc.getCollisionRectangle().Contains((int)vect.X, (int)vect.Y);
					vect = sprite2 + newOffset;
					npcCollision = npcCollision | npc.getCollisionRectangle().Contains((int)vect.X, (int)vect.Y);
				}

				if (layer.Tiles[tile1.X, tile1.Y] == null
					&& layer.Tiles[tile2.X, tile2.Y] == null
					&& !npcCollision)
					return true;
				else
					step--; // Need clipping.
			}

			return false;
		}

		/// <summary>
		/// Return if an event is triggered.
		/// </summary>
		/// <returns>Return the event triggered.</returns>
		public Event CheckEvent()
		{
			foreach (Event eve in Player.GamePlayer.Map.Events)
			{
				if (eve.EventSize.Intersects(getCollisionRectangle()))
					return eve;
			}

			return null;
		}

		/// <summary>
		/// Get the drawing zone of the sprite.
		/// </summary>
		/// <returns></returns>
		public Rectangle getRectangle()
		{
			return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
		}

		/// <summary>
		/// Get the collision zone of the sprite.
		/// </summary>
		/// <returns></returns>
		public Rectangle getCollisionRectangle()
		{
			return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X - 1, (int)Size.Y - 1);
		}

		public void ChangeSprite(string charsetName, Rectangle tileSize, Vector2 spriteSize)
		{
			Texture2D texture = Game.Content.Load<Texture2D>(@"Images\" + charsetName);
			if (tileSize == Rectangle.Empty)
				_SpriteImage = new Tileset(texture, texture.Width / (nbFrameAnimation * 4), texture.Height);
			else
				_SpriteImage = new Tileset(texture, tileSize, tileSize.Width / (nbFrameAnimation * 4), tileSize.Height);

			if (spriteSize == Vector2.Zero)
				Size = new Vector2(_SpriteImage.TileWidth, _SpriteImage.TileHeight);
			else
				Size = spriteSize;
		}
	}
}
