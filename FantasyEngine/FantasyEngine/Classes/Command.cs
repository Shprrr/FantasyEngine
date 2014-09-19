using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FantasyEngine.Classes
{
	public class Selectable : Window
	{
		protected int _Item_max = 1;
		protected int _Column_max = 1;
		private int _Index = -1;
		protected int oy = 0;

		public Selectable(Game game, int x, int y, int width, int height)
			: base(game, x, y, width, height)
		{
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (!Visible)
				return;

			//Draw the cursor
			if (CursorPosition < 0)
				return;

			////Get current row
			//int row = _Index / _Column_max;

			//// If current row is before top row
			//if (row < getTop_Row())
			//{
			//    // Scroll so that current row becomes top row
			//    setTop_Row(row);
			//}

			//// If current row is more to back than back row
			//if (row > getTop_Row() + (getPage_Row_Max() - 1))
			//{
			//    // Scroll so that current row becomes back row
			//    setTop_Row(row - (getPage_Row_Max() - 1));
			//}

			// Calculate cursor width
			int cursor_width = Rectangle.Width / _Column_max - 32;

			// Calculate cursor coordinates
			int cursorx = CursorPosition % _Column_max * (cursor_width + 32);
			int cursory = CursorPosition / _Column_max * GameMain.font.LineSpacing - oy;

			// Update cursor rectangle
			_SpriteBatch.Draw(GameMain.cursor,
				new Vector2(Rectangle.Left + Tileset.TileWidth + cursorx,
					Rectangle.Top + Tileset.TileHeight + cursory) + Offset,
				Color.White);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (!Input.UpdateInput(gameTime))
				return;

			//If cursor is movable
			if (Enabled && _Item_max > 0 && CursorPosition >= 0)
			{
				if (Input.keyStateHeld.IsKeyDown(Keys.Down))
				{
					// If column count is 1 and directional button was pressed down with no
					// repeat, or if cursor position is more to the front than
					// (item count - column count)
					if ((_Column_max == 1 && Input.keyStateDown.IsKeyDown(Keys.Down)) ||
						CursorPosition < _Item_max - _Column_max)
					{
						// Move cursor down
						CursorPosition = (CursorPosition + _Column_max) % _Item_max;
					}

					Input.PutDelay(Keys.Down);
					return;
				}

				if (Input.keyStateHeld.IsKeyDown(Keys.Up))
				{
					// If column count is 1 and directional button was pressed up with no
					// repeat, or if cursor position is more to the back than column count
					if ((_Column_max == 1 && Input.keyStateDown.IsKeyDown(Keys.Up)) ||
						CursorPosition >= _Column_max)
					{
						// Move cursor up
						CursorPosition = (CursorPosition - _Column_max + _Item_max) % _Item_max;
					}

					Input.PutDelay(Keys.Up);
					return;
				}

				if (Input.keyStateHeld.IsKeyDown(Keys.Right))
				{
					// If column count is 2 or more, and cursor position is closer to front
					// than (item count -1)
					if (_Column_max >= 2 && CursorPosition < _Item_max - 1)
					{
						// Move cursor right
						CursorPosition += 1;
					}

					Input.PutDelay(Keys.Right);
					return;
				}

				if (Input.keyStateHeld.IsKeyDown(Keys.Left))
				{
					// If column count is 2 or more, and cursor position is more back than 0
					if (_Column_max >= 2 && CursorPosition > 0)
					{
						// Move cursor left
						CursorPosition -= 1;
					}

					Input.PutDelay(Keys.Left);
					return;
				}

				//TODO: If L and R pressed
			}
		}

		/// <summary>
		/// Index of the cursor.
		/// </summary>
		public int CursorPosition
		{
			get { return _Index; }
			set
			{
				int oldValue = _Index;
				_Index = value;

				if (_Index != oldValue)
					RaiseOnIndexChanged();
			}
		}

		/// <summary>
		/// Get the first row shown.
		/// </summary>
		/// <returns></returns>
		public int getTop_Row() { return oy / 32; }

		/// <summary>
		/// Set the first row shown.
		/// </summary>
		/// <param name="row"></param>
		public void setTop_Row(int row)
		{
			// If row is less than 0, change it to 0
			if (row < 0)
				row = 0;

			// If row exceeds row_max - 1, change it to row_max - 1
			if (row > getRow_max() - 1)
				row = getRow_max() - 1;

			// Multiply 1 row height by 32 for y-coordinate of window contents
			// transfer origin
			oy = row * 32;
		}

		public int getPage_Row_Max() { return (Rectangle.Height - Tileset.TileHeight * 2) / 32; }

		/// <summary>
		/// Compute rows from number of items and columns.
		/// </summary>
		/// <returns></returns>
		public int getRow_max() { return (_Item_max + _Column_max - 1) / _Column_max; }

		public event EventHandler OnIndexChanged;
		protected virtual void RaiseOnIndexChanged()
		{
			// Raise the event by using the () operator.
			if (OnIndexChanged != null)
				OnIndexChanged(this, new EventArgs());
		}
	}

	public class Command : Selectable
	{
		private string[] _Choices;

		public string[] Choices
		{
			get { return _Choices; }
			set
			{
				_Choices = value;
				_Item_max = value.Length;
				Rectangle.Height = (value.Length / _Column_max * GameMain.font.LineSpacing) + (Tileset.TileHeight * 2);
			}
		}

		public Command(Game game, int width, string[] choices)
			: base(game, 0, 0, width, (choices.Length * GameMain.font.LineSpacing) + (Tileset.TileHeight * 2))
		{
			Choices = choices;
			CursorPosition = 0;
		}

		public Command(Game game, int width, string[] choices, int nbColumn)
			: this(game, width, choices)
		{
			_Column_max = nbColumn;
			Rectangle.Height = (choices.Length / _Column_max * GameMain.font.LineSpacing) + (Tileset.TileHeight * 2);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (!Visible)
				return;

			// Calculate cursor width
			int cursor_width = Rectangle.Width / _Column_max - 32;

			for (int i = 0; i < _Choices.Length; i++)
				_SpriteBatch.DrawString(GameMain.font, _Choices[i],
					new Vector2(Rectangle.Left + Tileset.TileWidth + GameMain.cursor.Width * 1.5f + i % _Column_max * (cursor_width + 32),
						Rectangle.Top + Tileset.TileHeight + i / _Column_max * GameMain.font.LineSpacing - oy) + Offset,
					Color.White);
		}
	}
}
