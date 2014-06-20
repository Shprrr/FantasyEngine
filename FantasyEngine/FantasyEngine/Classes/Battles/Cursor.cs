using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData.Battles;
using CursorData = FantasyEngineData.Battles.Cursor;

namespace FantasyEngine.Classes.Battles
{
	public class Cursor : DrawableGameComponent
	{
		private CursorData _Data;
		private Battler[] _Actors = new Battler[Battle.MAX_ACTOR];
		private Battler[] _Enemies = new Battler[Battle.MAX_ENEMY];

		public Cursor(Game game, Battler[] Actors, Battler[] Enemies, eTargetType defaultTarget, int indexSelf)
			: base(game)
		{
			_Data = new CursorData(defaultTarget, indexSelf);

			for (int i = 0; i < Battle.MAX_ACTOR; i++)
				_Actors[i] = Actors[i];

			for (int i = 0; i < Battle.MAX_ENEMY; i++)
				_Enemies[i] = Enemies[i];
		}

		private byte frame = 0;
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			frame = (byte)((frame + 1) % 8);

#if ENGINE
			if (Player.GamePlayer.ShowDebug)
			{
				GameMain.spriteBatchGUI.DrawString(GameMain.font, "Cursor Index: " + _Data.Index, new Vector2(8, 28), Color.White);
			}
#endif
			Color alpha = Color.White * (frame < 4 ? 0.5f : 1);
			switch (_Data.Target)
			{
				case eTargetType.SINGLE_ENEMY:
					//Dessiner un cursor sur l'enemy sélectionné
					GameMain.spriteBatchGUI.Draw(GameMain.cursor,
						new Vector2(_Enemies[_Data.Index].BattlerPosition.X + _Enemies[_Data.Index].BattlerSprite.TileWidth + 8,
							_Enemies[_Data.Index].BattlerPosition.Y + (_Enemies[_Data.Index].BattlerSprite.TileHeight / 2)),
						null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					break;

				case eTargetType.MULTI_ENEMY:
					//Dessiner un cursor en alpha sur chacun des ennemis
					for (int i = 0; i < Battle.MAX_ENEMY; i++)
						if (_Enemies[i] != null)
							GameMain.spriteBatchGUI.Draw(GameMain.cursor,
								new Vector2(_Enemies[i].BattlerPosition.X + _Enemies[i].BattlerSprite.TileWidth + 8,
									_Enemies[i].BattlerPosition.Y + (_Enemies[i].BattlerSprite.TileHeight / 2)),
								null, alpha, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					break;

				case eTargetType.SINGLE_PARTY:
					//Dessiner un cursor sur l'actor sélectionné
					GameMain.spriteBatchGUI.Draw(GameMain.cursor, new Vector2(_Actors[_Data.Index].BattlerPosition.X - 8,
						_Actors[_Data.Index].BattlerPosition.Y + (Battler.BATTLER_SIZE.Y / 2)), Color.White);
					break;

				case eTargetType.MULTI_PARTY:
					//Dessiner un cursor en alpha sur chacun des actors
					for (int i = 0; i < Battle.MAX_ACTOR; i++)
						if (_Actors[i] != null)
							GameMain.spriteBatchGUI.Draw(GameMain.cursor, new Vector2(_Actors[i].BattlerPosition.X - 8,
								_Actors[i].BattlerPosition.Y + (Battler.BATTLER_SIZE.Y / 2)), alpha);
					break;

				case eTargetType.SELF:
					//Dessiner un cursor sur l'actor courant
					if (_Data.IndexSelf < Battle.MAX_ACTOR)
						GameMain.spriteBatchGUI.Draw(GameMain.cursor, new Vector2(_Actors[_Data.IndexSelf].BattlerPosition.X - 8,
							_Actors[_Data.IndexSelf].BattlerPosition.Y + (Battler.BATTLER_SIZE.Y / 2)), Color.White);
					else
						GameMain.spriteBatchGUI.Draw(GameMain.cursor,
							new Vector2(_Enemies[_Data.IndexSelf - Battle.MAX_ACTOR].BattlerPosition.X + _Enemies[_Data.IndexSelf - Battle.MAX_ACTOR].BattlerSprite.TileWidth + 8,
								_Enemies[_Data.IndexSelf - Battle.MAX_ACTOR].BattlerPosition.Y + (_Enemies[_Data.IndexSelf - Battle.MAX_ACTOR].BattlerSprite.TileHeight / 2)),
							null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					break;

				case eTargetType.ALL:
					//Dessiner un cursor en alpha sur chacun des actors et des ennemis
					for (int i = 0; i < Battle.MAX_ACTOR; i++)
						if (_Actors[i] != null)
							GameMain.spriteBatchGUI.Draw(GameMain.cursor, new Vector2(_Actors[i].BattlerPosition.X - 8,
								_Actors[i].BattlerPosition.Y + (Battler.BATTLER_SIZE.Y / 2)), alpha);

					for (int i = 0; i < Battle.MAX_ENEMY; i++)
						if (_Enemies[i] != null)
							GameMain.spriteBatchGUI.Draw(GameMain.cursor,
								new Vector2(_Enemies[i].BattlerPosition.X + _Enemies[i].BattlerSprite.TileWidth + 8,
									_Enemies[i].BattlerPosition.Y + (_Enemies[i].BattlerSprite.TileHeight / 2)),
								null, alpha, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
					break;

				//case eTargetType.NONE:
				default:
					//Rien dessiner
					break;
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (!Input.UpdateInput(gameTime))
				return;

			if (Input.keyStateHeld.IsKeyDown(Keys.Down))
			{
				//Si on peut descendre
				if (_Data.Target == eTargetType.SINGLE_PARTY)
				{
					if (_Actors[_Data.Index + 1] != null)
						_Data.Index++;
					else
						_Data.Index = 0;
					Input.PutDelay(Keys.Down);
				}
				else if (_Data.Target == eTargetType.SINGLE_ENEMY)
				{
					if (_Enemies[_Data.Index + 1] != null)
						_Data.Index++;
					else
						_Data.Index = 0;
					Input.PutDelay(Keys.Down);
				}

				return;
			}

			if (Input.keyStateHeld.IsKeyDown(Keys.Up))
			{
				//Si on peut monter
				if (_Data.Target == eTargetType.SINGLE_PARTY)
				{
					//Valider l'index le plus haut
					do
					{
						if (_Data.Index != 0)
							_Data.Index--;
						else
							_Data.Index = Battle.MAX_ACTOR - 1;
					}
					while (_Actors[_Data.Index] == null);
					Input.PutDelay(Keys.Up);
				}
				else if (_Data.Target == eTargetType.SINGLE_ENEMY)
				{
					//Valider l'index le plus haut
					do
					{
						if (_Data.Index != 0)
							_Data.Index--;
						else
							_Data.Index = Battle.MAX_ENEMY - 1;
					}
					while (_Enemies[_Data.Index] == null);
					Input.PutDelay(Keys.Up);
				}

				return;
			}

			if (Input.keyStateDown.IsKeyDown(Keys.Left))
			{
				//Changer de target type
				switch (_Data.Target)
				{
					case eTargetType.MULTI_ENEMY:
						_Data.Target = eTargetType.ALL;
						break;

					case eTargetType.SINGLE_ENEMY:
						_Data.Target = eTargetType.MULTI_ENEMY;
						break;

					case eTargetType.SINGLE_PARTY:
						_Data.Target = eTargetType.SINGLE_ENEMY;
						while (_Enemies[_Data.Index] == null)
							if (_Enemies[_Data.Index + 1] != null)
								_Data.Index++;
							else
								_Data.Index = 0;
						break;

					case eTargetType.MULTI_PARTY:
						_Data.Target = eTargetType.SINGLE_PARTY;
						while (_Actors[_Data.Index] == null)
							if (_Actors[_Data.Index + 1] != null)
								_Data.Index++;
							else
								_Data.Index = 0;
						break;

					case eTargetType.ALL:
						_Data.Target = eTargetType.MULTI_PARTY;
						break;
				}

				return;
			}

			if (Input.keyStateDown.IsKeyDown(Keys.Right))
			{
				//Changer de target type
				switch (_Data.Target)
				{
					case eTargetType.ALL:
						_Data.Target = eTargetType.MULTI_ENEMY;
						break;

					case eTargetType.MULTI_ENEMY:
						_Data.Target = eTargetType.SINGLE_ENEMY;
						while (_Enemies[_Data.Index] == null)
							if (_Enemies[_Data.Index + 1] != null)
								_Data.Index++;
							else
								_Data.Index = 0;
						break;

					case eTargetType.SINGLE_ENEMY:
						_Data.Target = eTargetType.SINGLE_PARTY;
						while (_Actors[_Data.Index] == null)
							if (_Actors[_Data.Index + 1] != null)
								_Data.Index++;
							else
								_Data.Index = 0;
						break;

					case eTargetType.SINGLE_PARTY:
						_Data.Target = eTargetType.MULTI_PARTY;
						break;

					case eTargetType.MULTI_PARTY:
						_Data.Target = eTargetType.ALL;
						break;
				}

				return;
			}
		}

		public void getTargetBattler(Battler[] mapTargetBattler)
		{
			//Clear old values
			for (int i = 0; i < mapTargetBattler.Length; i++)
			{
				mapTargetBattler[i] = null;
			}

			switch (_Data.Target)
			{
				case eTargetType.SINGLE_ENEMY:
					mapTargetBattler[Battle.MAX_ACTOR + _Data.Index] = _Enemies[_Data.Index];
					break;

				case eTargetType.MULTI_ENEMY:
					for (int i = 0; i < Battle.MAX_ENEMY; i++)
						mapTargetBattler[Battle.MAX_ACTOR + i] = _Enemies[i];
					break;

				case eTargetType.SINGLE_PARTY:
					mapTargetBattler[_Data.Index] = _Actors[_Data.Index];
					break;

				case eTargetType.MULTI_PARTY:
					for (int i = 0; i < Battle.MAX_ACTOR; i++)
						mapTargetBattler[i] = _Actors[i];
					break;

				case eTargetType.SELF:
					if (_Data.IndexSelf < Battle.MAX_ACTOR)
						mapTargetBattler[_Data.IndexSelf] = _Actors[_Data.IndexSelf];
					else
						mapTargetBattler[_Data.IndexSelf] = _Enemies[_Data.IndexSelf - Battle.MAX_ACTOR];
					break;

				case eTargetType.ALL:
					for (int i = 0; i < Battle.MAX_ACTOR; i++)
						mapTargetBattler[i] = _Actors[i];

					for (int i = 0; i < Battle.MAX_ENEMY; i++)
						mapTargetBattler[Battle.MAX_ACTOR + i] = _Enemies[i];
					break;
			}
		}
	}
}
