using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngine.Classes.Battles;
using FantasyEngine.Classes.Menus;
using FantasyEngineData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FantasyEngine.Classes.Overworld
{
	public class Overworld : Scene
	{
		private static readonly string[] MENU_COMMANDS = { "Character", "Equipment", "Inventory", "Skills", "Quit game" };

		public static readonly Vector2 CAMERA_CENTER = new Vector2(304, 224);

		/// <summary>
		/// Value of a tile who've been walked.
		/// </summary>
		public const int STEP_BASE = 2;
		/// <summary>
		/// Minimum of step before having a chance to have a battle. (Based on the number of tile)
		/// </summary>
		public const int STEP_TO_BATTLE_MIN = (int)(16 * STEP_BASE * Sprite.OVERWORLD_TILE_SIZE / Sprite.MOVE_PX_PER_MILLISECOND / GameMain.MILLISECOND_PER_FRAME);
		/// <summary>
		/// Maximum of step to ensure having a chance to have a battle. (Based on the number of tile)
		/// </summary>
		public const int STEP_TO_BATTLE_MAX = (int)(64 * STEP_BASE * Sprite.OVERWORLD_TILE_SIZE / Sprite.MOVE_PX_PER_MILLISECOND / GameMain.MILLISECOND_PER_FRAME);
		public const int STEP_WILD = 4;

		private Command _Menu;

		public Overworld(Game game)
			: base(game)
		{
			MediaPlayer.Stop();
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(Player.GamePlayer.Map.BackgroundMusic);

			_Menu = new Command(Game, 180, MENU_COMMANDS);
			_Menu.ChangeOffset(Game.GraphicsDevice.Viewport.Width - _Menu.Rectangle.Width, 0);
			_Menu.Enabled = false;
			_Menu.Visible = false;
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			Player.GamePlayer.Map.Draw(gameTime);
			Player.GamePlayer.Hero.Draw(gameTime);
		}

		public override void DrawGUI(GameTime gameTime)
		{
			base.DrawGUI(gameTime);

			Player.GamePlayer.Map.DrawGUI(gameTime);

			_Menu.Offset = spriteBatchGUI.CameraOffset;
			_Menu.Draw(gameTime);

#if ENGINE
			if (Player.GamePlayer.ShowDebug)
			{
				Rectangle heroRect = Player.GamePlayer.Hero.getRectangle();
				Vector2 hero1 = new Vector2(heroRect.Left, heroRect.Top);
				Vector2 hero2 = new Vector2(heroRect.Right, heroRect.Bottom);
				Point tile1 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(hero1);
				Point tile2 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(hero2);
				spriteBatchGUI.DrawString(GameMain.font,
					"X:" + tile1.X + ", Y:" + tile1.Y + " (" + hero1 + ")" + Environment.NewLine +
					"X:" + tile2.X + ", Y:" + tile2.Y + " (" + hero2 + ")",
					new Vector2(8, 28) + spriteBatchGUI.CameraOffset, Color.White);

				spriteBatchGUI.DrawString(GameMain.font, "Step: " + Player.GamePlayer.StepToBattle, new Vector2(8, 60) + spriteBatchGUI.CameraOffset, Color.White);
			}
#endif
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Player.GamePlayer.Map.Update(gameTime);

			if (!Player.GamePlayer.Hero.Enabled)
				return;

			if (UpdateMenu(gameTime))
				return;

			Player.GamePlayer.Hero.Update(gameTime);

			#region Update Direction
			if (Input.keyStateHeld.IsKeyDown(Keys.Up)
				|| Input.keyStateHeld.IsKeyDown(Keys.Down)
				|| Input.keyStateHeld.IsKeyDown(Keys.Left)
				|| Input.keyStateHeld.IsKeyDown(Keys.Right))
			{
				Sprite.eDirection direction = Sprite.eDirection.DOWN;
				if (Input.keyStateHeld.IsKeyDown(Keys.Right))
					direction = Sprite.eDirection.RIGHT;

				if (Input.keyStateHeld.IsKeyDown(Keys.Left))
					direction = Sprite.eDirection.LEFT;

				if (Input.keyStateHeld.IsKeyDown(Keys.Down))
					direction = Sprite.eDirection.DOWN;

				if (Input.keyStateHeld.IsKeyDown(Keys.Up))
					direction = Sprite.eDirection.UP;

				Vector2 newOffset;
				if (Player.GamePlayer.Hero.CheckCollision(gameTime, direction, out newOffset))
				{
					Player.GamePlayer.Map.Offset += newOffset;
					Player.GamePlayer.Hero.Position += newOffset;

					Event eve = Player.GamePlayer.Hero.CheckEvent();
					if (eve != null && eve.Type == Event.eType.Teleport)
						eve.Teleport();

					if (eve != null && eve.Type == Event.eType.OnEnter)
						eve.RaiseOnEnter(gameTime); //TODO: Don't raise if already in. (Must get out to reraise.)

					if (Player.GamePlayer.Map.Encounters.Count > 0)
					{
						Player.GamePlayer.StepToBattle -= (int)(STEP_BASE * Player.GamePlayer.StepMultiplier);
						if (Player.GamePlayer.StepToBattle <= 0)
						{
							int encounterIndex = Encounter.GetIndexByChances(Player.GamePlayer.Map.Encounters);
							Encounter encounter = Player.GamePlayer.Map.Encounters[encounterIndex];
							Battle battle = new Battle(Game, Player.GamePlayer.Map.BattleBackName);
							for (int i = 0; i < encounter.Monsters.Length; i++)
							{
								battle.Enemies[i] = new Battler(Game, encounter.Monsters[i].Monster, encounter.Monsters[i].Level);
								battle.Enemies[i].Name = battle.Enemies[i].CurrentJob.JobName[0].ToString() + (i + 1) + " L" + battle.Enemies[i].CurrentJob.Level;
							}
							battle.StartBattle();
							Scene.ChangeMainScene(battle.BattleScene);

							Player.GamePlayer.StepToBattle = Extensions.rand.Next(STEP_TO_BATTLE_MIN, STEP_TO_BATTLE_MAX);
						}
					}
				}

				return;
			} // if (Direction held)
			#endregion Update Direction

			if (Input.keyStateDown.IsKeyDown(Keys.Enter))
			{
				Rectangle heroRect = Player.GamePlayer.Hero.getCollisionRectangle();

				//If facing an NPC, talk to him.
				switch (Player.GamePlayer.Hero.Direction)
				{
					case Sprite.eDirection.DOWN:
						heroRect.Height += 2;
						break;
					case Sprite.eDirection.LEFT:
						heroRect.X -= 2;
						break;
					case Sprite.eDirection.UP:
						heroRect.Y -= 2;
						break;
					case Sprite.eDirection.RIGHT:
						heroRect.Width += 2;
						break;
				}

				foreach (NPC npc in Player.GamePlayer.Map.NPCs)
				{
					// If the hero is next to the npc
					if (heroRect.Intersects(npc.getCollisionRectangle()))
					{
						npc.OppositeDirection(Player.GamePlayer.Hero.Direction);
						npc.RaiseOnTalking();
					}
				}
			}

			if (Input.keyStateDown.IsKeyDown(Keys.C))
			{
				AddSubScene(new CharacterScene(Game, Player.GamePlayer.Actors[0]));
			}

			if (Input.keyStateDown.IsKeyDown(Keys.E))
			{
				AddSubScene(new EquipScene(Game, Player.GamePlayer.Actors[0]));
			}

			if (Input.keyStateDown.IsKeyDown(Keys.I))
			{
				AddSubScene(new InventoryScene(Game));
			}

			if (Input.keyStateDown.IsKeyDown(Keys.K))
			{
				AddSubScene(new SkillScene(Game));
			}

#if ENGINE
			if (Input.keyStateDown.IsKeyDown(Keys.B))
			{
				Encounter encounter;

				if (Player.GamePlayer.Map.Encounters.Count > 0)
					encounter = Player.GamePlayer.Map.Encounters[0];
				else
					encounter = new Encounter(Game.Content.Load<FantasyEngineData.Entities.Monster>(@"Monsters\Goblin"), 1, 100);

				Battle battle = new Battle(Game, Player.GamePlayer.Map.BattleBackName);
				battle.Enemies[0] = new Battler(Game, encounter.Monsters[0].Monster, encounter.Monsters[0].Level);
				battle.Enemies[0].Name = battle.Enemies[0].CurrentJob.JobName + "1";
				battle.StartBattle();
				Scene.ChangeMainScene(battle.BattleScene);
			}

			if (Input.keyStateDown.IsKeyDown(Keys.V))
			{
				Player.GamePlayer.StepMultiplier = Player.GamePlayer.StepMultiplier == 0 ? 1 : 0;
			}

			if (Input.keyStateDown.IsKeyDown(Keys.L))
			{
				Player.GamePlayer.Actors[0].Exp += Player.GamePlayer.Actors[0].CurrentJob.ExpForLevel();
			}

			if (Input.keyStateDown.IsKeyDown(Keys.N))
			{
				return;
			}
#endif
		}

		/// <summary>
		/// Update the Menu window.
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns>If it catches the update</returns>
		private bool UpdateMenu(GameTime gameTime)
		{
			_Menu.Update(gameTime);

			if (Input.keyStateDown.IsKeyDown(Keys.Escape))
			{
				_Menu.Enabled = !_Menu.Enabled;
				_Menu.Visible = !_Menu.Visible;
				return true;
			}

			if (_Menu.Enabled)
				if (Input.keyStateDown.IsKeyDown(Keys.Enter))
				{
					switch (_Menu.CursorPosition)
					{
						case 0:
							AddSubScene(new CharacterScene(Game, Player.GamePlayer.Actors[0]));
							break;

						case 1:
							AddSubScene(new EquipScene(Game, Player.GamePlayer.Actors[0]));
							break;

						case 2:
							AddSubScene(new InventoryScene(Game));
							break;

						case 3:
							AddSubScene(new SkillScene(Game));
							break;

						case 4:
							ChangeMainScene(new Title(Game));
							break;
					}
				}

			return _Menu.Enabled;
		}
	}
}
