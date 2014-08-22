using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData.Battles;
using FantasyEngineData.Entities;
using CursorData = FantasyEngineData.Battles.Cursor;

namespace FantasyEngine.Classes.Battles
{
	public class Cursor : DrawableGameComponent
	{
		public static readonly eTargetType[] POSSIBLE_TARGETS_ANYONE = CursorData.POSSIBLE_TARGETS_ANYONE;
		public static readonly eTargetType[] POSSIBLE_TARGETS_ONE = CursorData.POSSIBLE_TARGETS_ONE;

		private CursorData _Data;

		public Cursor(Game game, Battler[] actors, Battler[] enemies, eTargetType defaultTarget, int indexSelf)
			: this(game, actors, enemies, defaultTarget, indexSelf, CursorData.POSSIBLE_TARGETS_ANYONE)
		{
		}

		public Cursor(Game game, Battler[] actors, Battler[] enemies, eTargetType defaultTarget, int indexSelf, eTargetType[] possibleTargets)
			: base(game)
		{
			_Data = new CursorData(defaultTarget, indexSelf, possibleTargets);

			for (int i = 0; i < Battle.MAX_ACTOR; i++)
				_Data.Actors[i] = actors[i];

			for (int i = 0; i < Battle.MAX_ENEMY; i++)
				_Data.Enemies[i] = enemies[i];
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
					DrawCursorOnEnemy(_Data.Enemies[_Data.Index] as Battler, Color.White);
					break;

				case eTargetType.MULTI_ENEMY:
					//Dessiner un cursor en alpha sur chacun des ennemis
					for (int i = 0; i < Battle.MAX_ENEMY; i++)
						DrawCursorOnEnemy(_Data.Enemies[i] as Battler, alpha);
					break;

				case eTargetType.SINGLE_PARTY:
					//Dessiner un cursor sur l'actor sélectionné
					DrawCursorOnActor(_Data.Actors[_Data.Index] as Battler, Color.White);
					break;

				case eTargetType.MULTI_PARTY:
					//Dessiner un cursor en alpha sur chacun des actors
					for (int i = 0; i < Battle.MAX_ACTOR; i++)
						DrawCursorOnActor(_Data.Actors[i] as Battler, alpha);
					break;

				case eTargetType.SELF:
					//Dessiner un cursor sur l'actor courant
					if (_Data.IndexSelf < Battle.MAX_ACTOR)
						DrawCursorOnActor(_Data.Actors[_Data.IndexSelf] as Battler, Color.White);
					else
						DrawCursorOnEnemy(_Data.Enemies[_Data.IndexSelf - Battle.MAX_ACTOR] as Battler, Color.White);
					break;

				case eTargetType.ALL:
					//Dessiner un cursor en alpha sur chacun des actors et des ennemis
					for (int i = 0; i < Battle.MAX_ACTOR; i++)
						DrawCursorOnActor(_Data.Actors[i] as Battler, alpha);

					for (int i = 0; i < Battle.MAX_ENEMY; i++)
						DrawCursorOnEnemy(_Data.Enemies[i] as Battler, alpha);
					break;

				//case eTargetType.NONE:
				default:
					//Rien dessiner
					break;
			}
		}

		private static void DrawCursorOnActor(Battler actor, Color alpha)
		{
			if (actor != null)
				GameMain.spriteBatchGUI.Draw(GameMain.cursor, new Vector2(actor.BattlerPosition.X - 8,
					actor.BattlerPosition.Y + (Battler.BATTLER_SIZE.Y / 2)), alpha);
		}

		private static Color DrawCursorOnEnemy(Battler enemy, Color alpha)
		{
			if (enemy != null)
				GameMain.spriteBatchGUI.Draw(GameMain.cursor,
					new Vector2(enemy.BattlerPosition.X + enemy.BattlerSprite.TileWidth + 8,
						enemy.BattlerPosition.Y + (enemy.BattlerSprite.TileHeight / 2)),
					null, alpha, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
			return alpha;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (!Input.UpdateInput(gameTime))
				return;

			if (Input.keyStateHeld.IsKeyDown(Keys.Down))
			{
				if (_Data.ChangeCursorDown())
					Input.PutDelay(Keys.Down);

				return;
			}

			if (Input.keyStateHeld.IsKeyDown(Keys.Up))
			{
				if (_Data.ChangeCursorUp())
					Input.PutDelay(Keys.Up);

				return;
			}

			if (Input.keyStateDown.IsKeyDown(Keys.Left))
			{
				_Data.ChangeTargetTypeToLeft();

				return;
			}

			if (Input.keyStateDown.IsKeyDown(Keys.Right))
			{
				_Data.ChangeTargetTypeToRight();

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
					mapTargetBattler[Battle.MAX_ACTOR + _Data.Index] = (Battler)_Data.Enemies[_Data.Index];
					break;

				case eTargetType.MULTI_ENEMY:
					for (int i = 0; i < Battle.MAX_ENEMY; i++)
						mapTargetBattler[Battle.MAX_ACTOR + i] = (Battler)_Data.Enemies[i];
					break;

				case eTargetType.SINGLE_PARTY:
					mapTargetBattler[_Data.Index] = (Battler)_Data.Actors[_Data.Index];
					break;

				case eTargetType.MULTI_PARTY:
					for (int i = 0; i < Battle.MAX_ACTOR; i++)
						mapTargetBattler[i] = (Battler)_Data.Actors[i];
					break;

				case eTargetType.SELF:
					if (_Data.IndexSelf < Battle.MAX_ACTOR)
						mapTargetBattler[_Data.IndexSelf] = (Battler)_Data.Actors[_Data.IndexSelf];
					else
						mapTargetBattler[_Data.IndexSelf] = (Battler)_Data.Enemies[_Data.IndexSelf - Battle.MAX_ACTOR];
					break;

				case eTargetType.ALL:
					for (int i = 0; i < Battle.MAX_ACTOR; i++)
						mapTargetBattler[i] = (Battler)_Data.Actors[i];

					for (int i = 0; i < Battle.MAX_ENEMY; i++)
						mapTargetBattler[Battle.MAX_ACTOR + i] = (Battler)_Data.Enemies[i];
					break;
			}
		}
	}
}
