using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FantasyEngineData.Battles;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;
using FantasyEngineData.Skills;
using BattleData = FantasyEngineData.Battles.Battle;

namespace FantasyEngine.Classes.Battles
{
	public struct BattleAction
	{
		public enum eKind
		{
			WAIT,
			ATTACK,
			MAGIC,
			ITEM,
			GUARD
		}

		public eKind Kind;
		public Cursor Target;
		public Skill Skill;
		public BaseItem Item;

		public BattleAction(eKind kind = eKind.WAIT, Skill skill = null, BaseItem item = null)
		{
			this.Kind = kind;
			this.Target = null;
			this.Skill = skill;
			this.Item = item;
		}
	}

	//TODO: Enemy mort attaque encore et encore targetable.
	public class BattleScene : Scene
	{
		private readonly string[] playerCommands = { "Attack", "Magic", "Item", "Guard", "Run" };
		private enum ePlayerCommand
		{
			Attack,
			Magic,
			Item,
			Guard,
			Run
		}

		private const string MISS = "MISS";

		private Battle _Battle;
		private Texture2D _BattleBack;
		private Song _BattleMusic;

		private Command _PlayerCommand;
		private Window _HelpWindow;
		private Window _StatusWindow;
		private Window _MessageWindow;
		private Window _CTBWindow;
		private Window _ResultWindow;
		private ItemSelection _ItemSelection;
		private SkillSelection _SkillSelection;

		private int _CTBWindowScrollY;
		private BattleAction _CurrentAction;
		private Cursor _Target = null;
		private Battler[] _TargetBattler;

		private int _PhaseStep;
		private int _AnimationWait = 0;

		private int _Exp = 0;
		private int _Gold = 0;
		private List<BaseItem> _Treasure = new List<BaseItem>();

		public BattleScene(Game game, Battle battle, string battleBackName)
			: base(game)
		{
			_Battle = battle;

			//Init textures
			_BattleBack = Game.Content.Load<Texture2D>(@"Images\Battle\Battlebacks\" + battleBackName);

			_BattleMusic = Game.Content.Load<Song>(@"Audios\Musics\Battle");
			MediaPlayer.Stop();
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(_BattleMusic);

			//Making windows
			_PlayerCommand = new Command(Game, 160, playerCommands);
			_PlayerCommand.ChangeOffset(640 - 160, 320);
			_PlayerCommand.Enabled = false;
			_PlayerCommand.Visible = false;

			_HelpWindow = new Window(Game, 0, 0, 640, 48);
			_HelpWindow.Visible = false;

			_StatusWindow = new Window(Game, 0, 320, 640, 160);
			_MessageWindow = new Window(Game, 0, 320, 480, 160);
			_MessageWindow.Visible = false;

			_CTBWindow = new Window(Game, 640 - 160, _HelpWindow.Rectangle.Bottom,
				160, 480 - _HelpWindow.Rectangle.Bottom - 160);
			_CTBWindowScrollY = 0;

			_ItemSelection = new ItemSelection(Game, 480, 160);
			_ItemSelection.ChangeOffset(80, 320);
			_ItemSelection.Enabled = false;
			_ItemSelection.Visible = false;

			_SkillSelection = new SkillSelection(Game, 480, 160);
			_SkillSelection.ChangeOffset(80, 320);
			_SkillSelection.Enabled = false;
			_SkillSelection.Visible = false;

			_ResultWindow = new Window(Game, 0, 0, 640, 480);
			_ResultWindow.Visible = false;

			_TargetBattler = new Battler[BattleData.MAX_ACTOR + BattleData.MAX_ENEMY];

			spriteBatch.cameraMatrix = Matrix.Identity;

			_Battle.OnBeginTurn += _Battle_OnBeginTurn;
			_Battle.OnSetupCommandWindow += Battle_OnSetupCommandWindow;
			_Battle.OnAIChooseAction += Battle_OnAIChooseAction;
			_Battle.OnActionPhase += Battle_OnActionPhase;
			_Battle.OnActionPhaseStep2 += Battle_OnActionPhaseStep2;
			_Battle.OnPostBattlePhase += Battle_OnPostBattlePhase;
			_Battle.OnWinning += Battle_OnWinning;
		}

		void _Battle_OnBeginTurn(object sender, EventArgs e)
		{
			for (int i = 0; i < _TargetBattler.Length; i++)
			{
				_TargetBattler[i] = null;
			}
		}

		private void Battle_OnSetupCommandWindow(object sender, EventArgs e)
		{
			// Enable actor command window
			_PlayerCommand.Enabled = true;
			_PlayerCommand.Visible = true;
			// Set index to 0
			// Note: They may want to keep position of the last action.
			_PlayerCommand.CursorPosition = 0;
		}

		/// <summary>
		/// Start the select of the target with a default target.
		/// </summary>
		/// <param name="defaultValue">First position of the target cursor</param>
		/// <param name="possibleTargets">Possible targets that can be selected</param>
		private void StartTargetSelection(eTargetType defaultValue, params eTargetType[] possibleTargets)
		{
			//Make cursor
			_Target = new Cursor(Game, _Battle.Actors, _Battle.Enemies, defaultValue, _Battle.ActiveBattlerIndex, possibleTargets);

			_PlayerCommand.Enabled = false;
		}

		private void EndTargetSelection()
		{
			_Target = null;
		}

		private void StartItemSelection()
		{
			_ItemSelection.Enabled = true;
			_ItemSelection.Visible = true;

			_PlayerCommand.Enabled = false;
		}

		private void EndItemSelection()
		{
			_ItemSelection.Enabled = false;
			_ItemSelection.Visible = false;

			if (_CurrentAction.Item is Item)
				StartTargetSelection(((Item)_CurrentAction.Item).DefaultTarget, Cursor.POSSIBLE_TARGETS_ANYONE);
		}

		private void StartSkillSelection()
		{
			_SkillSelection.Enabled = true;
			_SkillSelection.Visible = true;
			_SkillSelection.Actor = _Battle.getActiveBattler();

			_PlayerCommand.Enabled = false;
		}

		private void EndSkillSelection()
		{
			_SkillSelection.Enabled = false;
			_SkillSelection.Visible = false;

			if (_CurrentAction.Skill != null)
				StartTargetSelection(_CurrentAction.Skill.DefaultTarget, Cursor.POSSIBLE_TARGETS_ANYONE);
		}

		private void Battle_OnAIChooseAction(object sender, EventArgs e)
		{
			_CurrentAction = ((Battler)_Battle.getActiveBattler()).AIChooseAction(Game, _Battle.Enemies, _Battle.Actors);
		}

		private void Battle_OnActionPhase(object sender, EventArgs e)
		{
			_PlayerCommand.Enabled = false;
			_PlayerCommand.Visible = false;

			_PhaseStep = 1;
		}

		/// <summary>
		/// Start action.
		/// </summary>
		void Battle_OnActionPhaseStep2(object sender, EventArgs e)
		{
			if (_Battle.Phase != BattleData.ePhases.ACTION && _PhaseStep != 1)
				return;

			_PhaseStep = 2;
			switch (_CurrentAction.Kind)
			{
				case BattleAction.eKind.ATTACK:
					//Set animation id

					_CurrentAction.Target.getTargetBattler(_TargetBattler);
					for (int i = 0; i < BattleData.MAX_ACTOR + BattleData.MAX_ENEMY; i++)
						if (_TargetBattler[i] != null)
							_TargetBattler[i].Attacked(_Battle.getActiveBattler());
					break;

				case BattleAction.eKind.MAGIC:
					{
						//Set animation id

						int skillLevel;
						if (!_CurrentAction.Skill.Casting(_Battle.getActiveBattler(), out skillLevel))
							break;

						int nbTarget = 0;
						_CurrentAction.Target.getTargetBattler(_TargetBattler);
						for (int i = 0; i < BattleData.MAX_ACTOR + BattleData.MAX_ENEMY; i++)
							if (_TargetBattler[i] != null)
								nbTarget++;

						for (int i = 0; i < BattleData.MAX_ACTOR + BattleData.MAX_ENEMY; i++)
							if (_TargetBattler[i] != null)
								_TargetBattler[i].Used(_Battle.getActiveBattler(), _CurrentAction.Skill, skillLevel, nbTarget);
					}
					break;

				case BattleAction.eKind.ITEM:
					{
						//Set animation id

						int nbTarget = 0;
						_CurrentAction.Target.getTargetBattler(_TargetBattler);
						for (int i = 0; i < BattleData.MAX_ACTOR + BattleData.MAX_ENEMY; i++)
							if (_TargetBattler[i] != null)
								nbTarget++;

						for (int i = 0; i < BattleData.MAX_ACTOR + BattleData.MAX_ENEMY; i++)
							if (_TargetBattler[i] != null)
								_TargetBattler[i].Used(_Battle.getActiveBattler(), _CurrentAction.Item, nbTarget);

						if (_Battle.getActiveBattler().IsActor)
						{
							Player.GamePlayer.Inventory.Drop(_CurrentAction.Item);
							_ItemSelection.RefreshChoices();
						}
					}
					break;

				case BattleAction.eKind.GUARD:
					break;

				case BattleAction.eKind.WAIT:
					break;
			}

			ActionPhaseStep3();
		}

		/// <summary>
		/// Animation for action performer.
		/// </summary>
		private void ActionPhaseStep3()
		{
			if (_Battle.Phase != BattleData.ePhases.ACTION && _PhaseStep != 2)
				return;

			_PhaseStep = 3;
			//Set animation of attacker
			_AnimationWait = 30;
		}

		/// <summary>
		/// Animation for target.
		/// </summary>
		private void ActionPhaseStep4()
		{
			if (_Battle.Phase != BattleData.ePhases.ACTION && _PhaseStep != 3)
				return;

			_PhaseStep = 4;
			//Set animation of target

			//Animation has at least 8 frames, regardless of its length
			_AnimationWait = 30;
		}

		/// <summary>
		/// Damage display.
		/// </summary>
		private void ActionPhaseStep5()
		{
			if (_Battle.Phase != BattleData.ePhases.ACTION && _PhaseStep != 4)
				return;

			_PhaseStep = 5;
			//Display damage

			_AnimationWait = 30;
		}

		private void Battle_OnPostBattlePhase(object sender, EventArgs e)
		{
			_PhaseStep = 1;

			// Play the victory song.
			Song victory = Game.Content.Load<Song>(@"Audios\Musics\Victory");
			MediaPlayer.Stop();
			MediaPlayer.Play(victory);
			MediaPlayer.IsRepeating = false;

			// Get Exp, Gold and Items from dead enemies.
			foreach (Battler enemy in _Battle.Enemies)
			{
				if (enemy == null)
					continue;

				_Exp += enemy.ExpToGive();
				_Gold += enemy.GoldToGive;
				_Treasure.AddRange(enemy.Treasure);
			}

			// Wait 100 frames.
			_AnimationWait = 30;

			// Show the result.
		}

		private void Battle_OnWinning(object sender, EventArgs e)
		{
			int nbActorAlive = 0;
			foreach (Battler actor in _Battle.Actors)
			{
				if (actor != null && !actor.IsDead)
					nbActorAlive++;
			}

			foreach (Battler actor in _Battle.Actors)
			{
				if (actor != null && !actor.IsDead)
				{
					int oldLevel = actor.Level;
					actor.Exp += _Exp / nbActorAlive;
#if DEBUG
					actor.Exp += 40;
#endif
					if (actor.Level != oldLevel)
						Scene.AddSubScene(new FantasyEngine.Classes.Menus.LevelUpScene(Game, actor));
					Player.GamePlayer.Inventory.Gold += _Gold / nbActorAlive;
					Player.GamePlayer.Inventory.AddRange(_Treasure);
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			// Background
			spriteBatch.Draw(_BattleBack, new Vector2(0, _HelpWindow.Rectangle.Bottom), Color.White);
			spriteBatch.Draw(_BattleBack, new Vector2(_BattleBack.Width, _HelpWindow.Rectangle.Bottom), Color.White);
			spriteBatch.Draw(_BattleBack, new Vector2(_BattleBack.Width * 2, _HelpWindow.Rectangle.Bottom), Color.White);

			for (int i = 0; i < BattleData.MAX_ACTOR; i++)
			{
				if (_Battle.Actors[i] == null || _Battle.Actors[i].IsDead)
					continue;

				Color color = _Battle.Actors[i] == _TargetBattler[i] ? new Color(0xFF, 0, 0, 0xFF) : Color.White;
				if (_Battle.Actors[i].BattlerSprite is Tileset)
					spriteBatch.Draw(_Battle.Actors[i].BattlerSprite.texture, _Battle.Actors[i].GetRectangle(), _Battle.Actors[i].BattlerSprite.GetSourceRectangle(0), color);
				else
					spriteBatch.Draw(_Battle.Actors[i].BattlerSprite.texture, _Battle.Actors[i].BattlerPosition, color);
			}

			for (int i = 0; i < BattleData.MAX_ENEMY; i++)
			{
				if (_Battle.Enemies[i] == null || _Battle.Enemies[i].IsDead)
					continue;

				Color color = _Battle.Enemies[i] == _TargetBattler[BattleData.MAX_ACTOR + i] ? new Color(0xFF, 0, 0, 0xFF) : Color.White;
				if (_Battle.Enemies[i].BattlerSprite is Tileset)
					spriteBatch.Draw(_Battle.Enemies[i].BattlerSprite.texture, _Battle.Enemies[i].BattlerPosition, _Battle.Enemies[i].BattlerSprite.GetSourceRectangle(0), color);
				else
					spriteBatch.Draw(_Battle.Enemies[i].BattlerSprite.texture, _Battle.Enemies[i].BattlerPosition, color);
			}

			if (_Battle.Phase == BattleData.ePhases.ACTION && _PhaseStep == 3)
			{
				switch (_CurrentAction.Kind)
				{
					case BattleAction.eKind.ATTACK:
						if (_Battle.getActiveBattler().RightHand is Weapon)
							spriteBatch.DrawString(GameMain.font,
								_Battle.getActiveBattler().RightHand.Name + " swing !", new Vector2(0, 200), Color.White);

						if (_Battle.getActiveBattler().LeftHand is Weapon)
							spriteBatch.DrawString(GameMain.font,
								_Battle.getActiveBattler().LeftHand.Name + " swing !", new Vector2(0, 220), Color.White);

						if (!(_Battle.getActiveBattler().RightHand is Weapon || _Battle.getActiveBattler().LeftHand is Weapon))
							spriteBatch.DrawString(GameMain.font, "Barehand swing !", new Vector2(0, 200), Color.White);
						break;

					case BattleAction.eKind.MAGIC:
						spriteBatch.DrawString(GameMain.font, _CurrentAction.Skill.Name + " is used !", new Vector2(0, 200), Color.White);
						break;

					case BattleAction.eKind.ITEM:
						spriteBatch.DrawString(GameMain.font, _CurrentAction.Item.Name + " is used !", new Vector2(0, 200), Color.White);
						break;

					case BattleAction.eKind.GUARD:
						break;

					case BattleAction.eKind.WAIT:
						break;
				}

				_AnimationWait--;
				if (_AnimationWait < 0)
				{
					ActionPhaseStep4();
				}
			}

			if (_Battle.Phase == BattleData.ePhases.ACTION && _PhaseStep == 4)
			{
				switch (_CurrentAction.Kind)
				{
					case BattleAction.eKind.ATTACK:
						if (_Battle.getActiveBattler().RightHand is Weapon)
							spriteBatch.DrawString(GameMain.font,
								_Battle.getActiveBattler().RightHand.Name + " hitted !", new Vector2(0, 200), Color.White);

						if (_Battle.getActiveBattler().LeftHand is Weapon)
							spriteBatch.DrawString(GameMain.font,
								_Battle.getActiveBattler().LeftHand.Name + " hitted !", new Vector2(0, 220), Color.White);

						if (!(_Battle.getActiveBattler().RightHand is Weapon || _Battle.getActiveBattler().LeftHand is Weapon))
							spriteBatch.DrawString(GameMain.font, "Barehand hitted !", new Vector2(0, 200), Color.White);
						break;

					case BattleAction.eKind.MAGIC:
						spriteBatch.DrawString(GameMain.font, _CurrentAction.Skill.Name + " hitted !", new Vector2(0, 200), Color.White);
						break;

					case BattleAction.eKind.ITEM:
						spriteBatch.DrawString(GameMain.font, _CurrentAction.Item.Name + " hitted !", new Vector2(0, 200), Color.White);
						break;

					case BattleAction.eKind.GUARD:
						break;

					case BattleAction.eKind.WAIT:
						break;
				}

				_AnimationWait--;
				if (_AnimationWait < 0)
				{
					ActionPhaseStep5();
				}
			}

			if (_Battle.Phase == BattleData.ePhases.ACTION && _PhaseStep == 5)
			{
				switch (_CurrentAction.Kind)
				{
					case BattleAction.eKind.ATTACK:
					case BattleAction.eKind.MAGIC:
					case BattleAction.eKind.ITEM:
						for (int i = 0; i < BattleData.MAX_ACTOR + BattleData.MAX_ENEMY; i++)
							if (_TargetBattler[i] != null)
							{
								int totalMultiplier = _TargetBattler[i].multiplierRH + _TargetBattler[i].multiplierLH;
								int totalDamage = _TargetBattler[i].damageRH + _TargetBattler[i].damageLH;
								string damage = totalMultiplier == 0 ? MISS :
									(_CurrentAction.Kind != BattleAction.eKind.ITEM ?
										totalMultiplier + " hit" + (totalMultiplier > 1 ? "s" : "") : "") +
									Environment.NewLine + totalDamage;
								spriteBatch.DrawString(GameMain.font, damage,
									new Vector2(_TargetBattler[i].BattlerPosition.X,
										_TargetBattler[i].BattlerPosition.Y - 12),
									new Color(0xFF, 0x80, 0x80, 0xFF));
							}
						break;

					case BattleAction.eKind.GUARD:
						break;

					case BattleAction.eKind.WAIT:
						break;
				}

				_AnimationWait--;
				if (_AnimationWait < 0)
				{
					for (int i = 0; i < BattleData.MAX_ACTOR + BattleData.MAX_ENEMY; i++)
						if (_TargetBattler[i] != null)
						{
							_TargetBattler[i].GiveDamage();
						}

					_Battle.NextTurn();
				}
			}

			if (_Battle.Phase == BattleData.ePhases.POST_BATTLE)
			{
				if (_PhaseStep == 1)
				{
					_AnimationWait--;
					if (_AnimationWait < 0)
					{
						_ResultWindow.Visible = true;
						_PhaseStep = 2;
					}
				}
			}
		}

		public override void DrawGUI(GameTime gameTime)
		{
			base.DrawGUI(gameTime);

			if (_Target != null)
				_Target.Draw(gameTime);

			_HelpWindow.Draw(gameTime);

			DrawStatusWindow(gameTime);

			_MessageWindow.Draw(gameTime);
			_PlayerCommand.Draw(gameTime);

			DrawCTBWindow(gameTime);

			_ItemSelection.Draw(gameTime);

			_SkillSelection.Draw(gameTime);

			if (_Battle.Phase == BattleData.ePhases.POST_BATTLE && _PhaseStep == 2)
			{
				DrawResultWindow(gameTime);
			}
		}

		private void DrawStatusWindow(GameTime gameTime)
		{
			_StatusWindow.Draw(gameTime);

			spriteBatchGUI.Scissor(_StatusWindow.InsideBound);

			int x, y, right, height;

			Texture2D pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
			pixel.SetData(new Color[1] { new Color(255, 255, 255) });

			//// Test Statut 2
			//x = _StatusWindow.Rectangle.Left + (_StatusWindow.Rectangle.Width / 4);
			//y = _StatusWindow.Rectangle.Top;
			//right = x + (_StatusWindow.Rectangle.Width / 4);
			//height = _StatusWindow.Rectangle.Height;

			//Rectangle pos = new Rectangle(x, y, Window.Tileset.TileWidth, height);
			//spriteBatchGUI.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(3), Color.White);

			//spriteBatchGUI.DrawString(GameMain.font, "ABCDEFGHIJ", new Vector2(x + 12, y + 8), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
			//spriteBatchGUI.DrawString(GameMain.font, "L100", new Vector2(x + 84, y + 20), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
			//spriteBatchGUI.DrawString(GameMain.font, "HP:", new Vector2(x + 12, y + 36), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
			//spriteBatchGUI.Draw(pixel, new Rectangle(x + 60, y + 38, 60, 8), new Color(0, 255, 0));
			//spriteBatchGUI.DrawString(GameMain.font, "9999/9999", new Vector2(x + 24, y + 48), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
			//spriteBatchGUI.DrawString(GameMain.font, "MP:", new Vector2(x + 12, y + 64), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
			//spriteBatchGUI.Draw(pixel, new Rectangle(x + 60, y + 66, 60, 8), new Color(0, 255, 0));
			//spriteBatchGUI.DrawString(GameMain.font, "9999/9999", new Vector2(x + 24, y + 76), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
			////spriteBatchGUI.DrawString(GameMain.font, "Statut:", new Vector2(x +  12, y + 104), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
			//spriteBatchGUI.DrawString(GameMain.font, "NormalXY", new Vector2(x + 12, y + 96), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

			//pos = new Rectangle(right - Window.Tileset.TileWidth, y, Window.Tileset.TileWidth, height);
			//spriteBatchGUI.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(5), Color.White); //Bord Droite


			x = _StatusWindow.Rectangle.Left;
			y = _StatusWindow.Rectangle.Top;
			right = x + (_StatusWindow.Rectangle.Width / 4);
			height = _StatusWindow.Rectangle.Height;
			for (int i = 0; i < BattleData.MAX_ACTOR; i++)
			{
				if (i != 0)
				{
					Rectangle pos = new Rectangle(x, y, Window.Tileset.TileWidth, height);
					spriteBatchGUI.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(3), Color.White); //Bord Gauche
				}

				if (_Battle.Actors[i] != null)
				{
					spriteBatchGUI.DrawString(GameMain.font,
						_Battle.Actors[i].Name,
						new Vector2(x + 12, y + 8), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
					spriteBatchGUI.DrawString(GameMain.font,
						_Battle.Actors[i].Level < 100 ? "Lv" + _Battle.Actors[i].Level : "L" + _Battle.Actors[i].Level,
						new Vector2(x + 84, y + 20), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

					spriteBatchGUI.DrawString(GameMain.font,
						"HP:",
						new Vector2(x + 12, y + 36), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
					//TODO: Changer la grandeur d'apres le ratio
					float ratio = 1;
					spriteBatchGUI.Draw(pixel, new Rectangle(x + 60, y + 38, (int)(60 * ratio), 8),
						new Color(255 - 255 * ratio, 255 * ratio, 0));
					spriteBatchGUI.DrawString(GameMain.font,
						_Battle.Actors[i].Hp + "/" + _Battle.Actors[i].MaxHp,
						new Vector2(x + 24, y + 48), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

					spriteBatchGUI.DrawString(GameMain.font,
						"MP:",
						new Vector2(x + 12, y + 64), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
					//TODO: Changer la grandeur d'apres le ratio
					ratio = 1;
					spriteBatchGUI.Draw(pixel, new Rectangle(x + 60, y + 66, (int)(60 * ratio), 8),
						new Color(255 - 255 * ratio, 255 * ratio, 0));
					spriteBatchGUI.DrawString(GameMain.font,
						_Battle.Actors[i].Mp + "/" + _Battle.Actors[i].MaxMp,
						new Vector2(x + 24, y + 76), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

					spriteBatchGUI.DrawString(GameMain.font,
						_Battle.Actors[i].Statut.ToString(),
						new Vector2(x + 12, y + 96), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
				}

				if (i != BattleData.MAX_ACTOR - 1)
				{
					Rectangle pos = new Rectangle(right - Window.Tileset.TileWidth, y, Window.Tileset.TileWidth, height);
					spriteBatchGUI.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(5), Color.White); //Bord Droite
				}

				x += _StatusWindow.Rectangle.Width / 4;
				right = x + (_StatusWindow.Rectangle.Width / 4);
			}

			spriteBatchGUI.ScissorReset();
		}

		private void DrawCTBWindow(GameTime gameTime)
		{
			//TODO: Ajouter les flèches pour indiquer le scroll.
			_CTBWindow.Draw(gameTime);

			spriteBatchGUI.Scissor(_CTBWindow.InsideBound);

			for (int i = 0; i < BattleData.MAX_CTB; i++)
			{
				/*GRRLIB_Rectangle(640 - 160 + 8, mpHelpWindow.Rectangle.Bottom + 8 + 32 * i, 
					160 - 16, 32, clrNormal, false);*/

				//TODO: Dessiner le rectangle counter
				spriteBatchGUI.DrawString(GameMain.font, "C:" + _Battle.OrderBattle[i].counter,
					new Vector2(_CTBWindow.Rectangle.Left + 8,
						_CTBWindow.Rectangle.Top + 8 + _CTBWindowScrollY + 32 * i),
					Color.White);

				//TODO: Dessiner la face
				spriteBatchGUI.DrawString(GameMain.font8, _Battle.OrderBattle[i].battler.Name,
					new Vector2(_CTBWindow.Rectangle.Left + 8 + 6 * 16,
						_CTBWindow.Rectangle.Top + 8 + _CTBWindowScrollY + 32 * i),
					Color.White);
			}
			spriteBatchGUI.ScissorReset();
		}

		private void DrawResultWindow(GameTime gameTime)
		{
			_ResultWindow.Draw(gameTime);

			spriteBatchGUI.DrawString(GameMain.font, "Exp. gained : " + _Exp, new Vector2(16, 16), Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Gold gained : " + _Gold, new Vector2(16, 16 + GameMain.font.LineSpacing), Color.White);

			Rectangle pos = new Rectangle(_ResultWindow.Rectangle.X + Window.Tileset.TileWidth, _ResultWindow.Rectangle.Y + 24 + (GameMain.font.LineSpacing * 2),
				_ResultWindow.Rectangle.Width - (Window.Tileset.TileWidth * 2), Window.Tileset.TileHeight);
			spriteBatchGUI.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(1), Color.White); //Bord Haut

			int x = 16;
			int y = 32 + (GameMain.font.LineSpacing * 2) + Window.Tileset.TileHeight;
			foreach (BaseItem item in _Treasure)
			{
				spriteBatchGUI.DrawString(GameMain.font, item.Name, new Vector2(x, y), Color.White);
				y += GameMain.font.LineSpacing;
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_PlayerCommand.Update(gameTime);

			if (_Target != null)
				_Target.Update(gameTime);

			_ItemSelection.Update(gameTime);

			_SkillSelection.Update(gameTime);

			if (_Battle.Phase == BattleData.ePhases.END_BATTLE && CurrentScene == this) //WaitEnd
			{
				// Return to map
				MediaPlayer.Stop();
				Scene.ChangeMainScene(new Overworld.Overworld(Game));
				return;
			}

			if (Input.keyStateDown.IsKeyDown(Keys.Enter))
			{
				switch (_Battle.Phase)
				{
					case BattleData.ePhases.ACTOR_COMMAND:
						if (_PlayerCommand.Enabled)
						{
							switch (_PlayerCommand.CursorPosition)
							{
								case (int)ePlayerCommand.Attack:
									//Set current action
									_CurrentAction = new BattleAction(BattleAction.eKind.ATTACK);

									//Select the target
									StartTargetSelection(eTargetType.SINGLE_ENEMY, Cursor.POSSIBLE_TARGETS_ONE);
									return;

								case (int)ePlayerCommand.Magic:
									//Set current action
									_CurrentAction = new BattleAction(BattleAction.eKind.MAGIC);

									//Select the magic
									StartSkillSelection();
									return;

								case (int)ePlayerCommand.Item:
									//Set current action
									_CurrentAction = new BattleAction(BattleAction.eKind.ITEM);

									//Select the item
									StartItemSelection();
									return;

								case (int)ePlayerCommand.Guard:
									_CurrentAction = new BattleAction(BattleAction.eKind.GUARD);
									//TODO: Implémenter le guard.

									//Confirm the target
									StartTargetSelection(eTargetType.SELF, eTargetType.SELF);
									return;

								case (int)ePlayerCommand.Run:
									_CurrentAction = new BattleAction(BattleAction.eKind.WAIT);

									_Battle.Escape();
									return;
							}
						} // if (_PlayerCommand.Enabled)
						else if (_Target != null)
						{
							_CurrentAction.Target = _Target;
							EndTargetSelection();

							//Next
							_Battle.StartActionPhase();
							return;
						}
						else if (_ItemSelection.Enabled)
						{
							_CurrentAction.Item = _ItemSelection.ItemSelected;
							EndItemSelection();
						}
						else if (_SkillSelection.Enabled)
						{
							_CurrentAction.Skill = _SkillSelection.SkillSelected;
							EndSkillSelection();
						}
						break;

					case BattleData.ePhases.POST_BATTLE:
						_Battle.EndBattle();
						break;
				} // switch (_Phase)
			} // if (Input.keyStateDown.IsKeyDown(Keys.Enter))

			if (Input.keyStateDown.IsKeyDown(Keys.Back))
			{
				switch (_Battle.Phase)
				{
					case BattleData.ePhases.ACTOR_COMMAND:
						if (_Target != null)
						{
							EndTargetSelection();

							switch (_PlayerCommand.CursorPosition)
							{
								case (int)ePlayerCommand.Attack:
									_PlayerCommand.Enabled = true;
									break;
								case (int)ePlayerCommand.Magic:
									StartSkillSelection();
									break;
								case (int)ePlayerCommand.Item:
									StartItemSelection();
									break;
							}
							return;
						}
						else if (_ItemSelection.Enabled)
						{
							_CurrentAction.Item = null;
							EndItemSelection();
							_PlayerCommand.Enabled = true;
							return;
						}
						else if (_SkillSelection.Enabled)
						{
							_CurrentAction.Skill = null;
							EndSkillSelection();
							_PlayerCommand.Enabled = true;
							return;
						}
						break;

					case BattleData.ePhases.POST_BATTLE:
						_Battle.EndBattle();
						break;
				}
			}

			if (Input.keyStateHeld.IsKeyDown(Keys.PageUp))
			{
				//Scroll le CTB
				_CTBWindowScrollY += 4;

				if (_CTBWindowScrollY > 0)
					_CTBWindowScrollY = 0;
				return;
			}

			if (Input.keyStateHeld.IsKeyDown(Keys.PageDown))
			{
				//Scroll le CTB
				_CTBWindowScrollY -= 4;

				if (_CTBWindowScrollY < -(32 * BattleData.MAX_CTB - _CTBWindow.Rectangle.Height))
					_CTBWindowScrollY = -(32 * BattleData.MAX_CTB - _CTBWindow.Rectangle.Height);
				return;
			}
		}
	}

	public class Battle : BattleData
	{
		public BattleScene BattleScene { get; private set; }

		public Battler[] Actors { get { return (Battler[])_Actors; } }
		public Battler[] Enemies { get { return (Battler[])_Enemies; } }


		/// <summary>
		/// Take all battler and set their position.
		/// </summary>
		private void SetBattlerPositions()
		{
			for (int i = 0; i < MAX_ACTOR; i++)
				if (_Actors[i] != null)
					Actors[i].BattlerPosition = new Vector2(320, 160 + 40 * i);

			for (int i = 0; i < MAX_ENEMY; i++)
				if (_Enemies[i] != null)
					Enemies[i].BattlerPosition = new Vector2(100, 160 + 40 * i);
		}

		public Battle(Game game, string battleBackName)
			: base()
		{
			MAX_ENEMY = MAX_ACTOR = Player.MAX_ACTOR;
			_Actors = new Battler[MAX_ACTOR];
			_Enemies = new Battler[MAX_ENEMY];

			//Prepare battlers
			for (int i = 0; i < MAX_ACTOR; i++)
			{
				if (Player.GamePlayer.Actors[i] != null)
					_Actors[i] = new Battler(game, Player.GamePlayer.Actors[i]);
			}

			BattleScene = new BattleScene(game, this, battleBackName);

			OnStartBattle += Battle_OnStartBattle;
		}

		private void Battle_OnStartBattle(object sender, EventArgs e)
		{
			SetBattlerPositions();
		}
	}
}
