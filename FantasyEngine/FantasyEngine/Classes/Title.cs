using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FantasyEngine.Classes.Overworld;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;
using FantasyEngineData.Skills;

namespace FantasyEngine.Classes
{
	public class Title : Scene
	{
		private enum eCursor
		{
			CUR_NEW_GAME,
			CUR_CONTINUE,
			CUR_QUIT,
			CUR_MAX_DATA
		}

		private enum eState
		{
			STATE_MAIN,
			STATE_SELECTION
		}

		private const int MAX_SLOT = 3; //TODO: Déplacer vers la classe Character ou File

		private Texture2D _BackTitle;
		private Texture2D _TitleText;
		private Song _BackgroundMusic;

		//TODO: Changer ce menu pour un Command
		private Window _Menu;
		private Window[] _SaveSlots = new Window[MAX_SLOT];

		//Savoir quelles fenetres à afficher et à traiter
		private eState state;
		private eCursor cursorIndex;
		private int cursorIndexSelection;

		/// <summary>
		/// Create a new game.
		/// </summary>
		private void NewGame()
		{
			Player.GamePlayer = new Player();

#if ENGINE
			for (int i = 0; i < 1; i++)
#else
			for (int i = 0; i < Player.MAX_ACTOR; i++)
#endif
			{
				Player.GamePlayer.Actors[i] = new Character("H" + (i + 1));
#if ENGINE
				BaseJob[] baseJobs = JobManager.GetAllBaseJob();
				for (int j = 0; j < baseJobs.Length; j++)
				{
					if (baseJobs[j].JobAbbreviation != "Ar")
						Player.GamePlayer.Actors[i].Jobs[j] = new Job(baseJobs[j]);
				}
				Player.GamePlayer.Actors[i].JobSort();
#endif
			}

			Vector2 startingPoint = new Vector2(64, 64);
			Player.GamePlayer.Hero = new Sprite(Game, @"Overworld\characters", Player.GamePlayer.Actors[0].CurrentJob.BaseJob.OverworldSpriteSize, startingPoint, Sprite.OVERWORLD_SIZE);
			Player.GamePlayer.Map = Map.GetFactory("Village").CreateMap(Game);
			Player.GamePlayer.Inventory.Items.Add(new Inventory.InvItem(ItemManager.GetItem("Potion"), 2));
			Player.GamePlayer.Inventory.Items.Add(new Inventory.InvItem(ItemManager.GetWeapon("Knife"), 1));
#if ENGINE
			Player.GamePlayer.Inventory.Items.Add(new Inventory.InvItem(ItemManager.GetShield("Leather Shield"), 2));
			Player.GamePlayer.Inventory.Items.Add(new Inventory.InvItem(ItemManager.GetArmor("Leather Helmet"), 1));
			Player.GamePlayer.Inventory.Items.Add(new Inventory.InvItem(ItemManager.GetArmor("Leather Armor"), 1));
			Player.GamePlayer.Inventory.Items.Add(new Inventory.InvItem(ItemManager.GetArmor("Leather Gloves"), 1));
			Player.GamePlayer.Inventory.Items.Add(new Inventory.InvItem(ItemManager.GetArmor("Leather Boots"), 1));
			Player.GamePlayer.Inventory.Items.Add(new Inventory.InvItem(ItemManager.GetItem("Phoenix Down"), 3));
			Player.GamePlayer.Inventory.Items.Add(new Inventory.InvItem(ItemManager.GetItem("Ultima Scroll"), 1));
			Player.GamePlayer.Inventory.Gold = 20;

			Player.GamePlayer.Actors[0].Skills.Add(new Skill(SkillManager.GetBaseSkill("Fire")));
			Player.GamePlayer.Actors[0].Skills.Add(new Skill(SkillManager.GetBaseSkill("Ice")));
			Player.GamePlayer.Actors[0].Skills[1].Exp += 70;
			Player.GamePlayer.Actors[0].Skills.Add(new Skill(SkillManager.GetBaseSkill("Fira")));
			Player.GamePlayer.Actors[0].Skills[2].Exp += 60;
			Player.GamePlayer.Actors[0].Skills.Add(new Skill(SkillManager.GetBaseSkill("Firaga")));
			Player.GamePlayer.Actors[0].Skills.Add(new Skill(SkillManager.GetBaseSkill("Thunder")));
			Player.GamePlayer.Actors[0].Skills[4].Exp += 17075;
			Player.GamePlayer.Actors[0].Skills.Add(new Skill(SkillManager.GetBaseSkill("Cure")));
			Player.GamePlayer.Actors[0].Skills[5].Exp += 70;
			Player.GamePlayer.Actors[0].Skills.Add(new Skill(SkillManager.GetBaseSkill("Osmose")));
			Player.GamePlayer.Actors[0].Skills[6].Exp += 156;
#endif
			Player.GamePlayer.StepToBattle = FantasyEngineData.Extensions.rand.Next(Overworld.Overworld.STEP_TO_BATTLE_MIN, Overworld.Overworld.STEP_TO_BATTLE_MAX);

			Scene.ChangeMainScene(new Overworld.Overworld(Game));
		}

		public Title(Game game)
			: base(game)
		{
			_Menu = new Window(Game, 236, 270, 168, 96);

			for (int i = 0; i < MAX_SLOT; i++)
				_SaveSlots[i] = new Window(Game, 120, 168 + i * 80, 400, 64);

			state = eState.STATE_MAIN;
			cursorIndex = eCursor.CUR_NEW_GAME; //TODO: Détecter si ya au moins un save pour changer le default
			cursorIndexSelection = 0; //TODO: Détecter le dernier save utilisé

			GameMain.spriteBatch.cameraMatrix = Matrix.Identity;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			_BackTitle = Game.Content.Load<Texture2D>(@"Images\back_title");
			_TitleText = Game.Content.Load<Texture2D>(@"Images\title_text");

			_BackgroundMusic = Game.Content.Load<Song>(@"Audios\Musics\Title");
			MediaPlayer.Stop();
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(_BackgroundMusic);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			//Background
			spriteBatch.Draw(_BackTitle, Game.GraphicsDevice.ScissorRectangle, Color.White);

			//Title
			spriteBatch.Draw(_TitleText, new Vector2(264, 62), Color.White);
		}

		public override void DrawGUI(GameTime gameTime)
		{
			base.DrawGUI(gameTime);

			switch (state)
			{
				case eState.STATE_MAIN:
					//Window
					_Menu.Draw(gameTime);

					//Text
					spriteBatchGUI.DrawString(GameMain.font, "New Game", new Vector2(268, 278), Color.White);
					spriteBatchGUI.DrawString(GameMain.font, "Continue", new Vector2(268, 310), Color.White);
					spriteBatchGUI.DrawString(GameMain.font, "Quit", new Vector2(268, 342), Color.White);

					//Cursor
					spriteBatchGUI.Draw(GameMain.cursor, new Vector2(244, 278 + (int)cursorIndex * 32), Color.White);
					break;

				case eState.STATE_SELECTION:
					for (int i = 0; i < MAX_SLOT; i++)
					{
						//Window
						_SaveSlots[i].Draw(gameTime);

						//Text
						spriteBatchGUI.DrawString(GameMain.font, "No Game", new Vector2(152, 176 + i * 80), Color.White);
					}

					//Cursor
					spriteBatchGUI.Draw(GameMain.cursor, new Vector2(128, 176 + cursorIndexSelection * 80), Color.White);
					break;
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (!Input.UpdateInput(gameTime))
				return;

			if (Input.keyStateHeld.IsKeyDown(Keys.Up))
			{
				switch (state)
				{
					case eState.STATE_MAIN:
						if (cursorIndex > 0)
							cursorIndex = (eCursor)(cursorIndex - 1);
						else
							cursorIndex = (eCursor)(eCursor.CUR_MAX_DATA - 1);

						Input.PutDelay(Keys.Up);
						return;

					case eState.STATE_SELECTION:
						if (cursorIndexSelection > 0)
							cursorIndexSelection--;
						else
							cursorIndexSelection = MAX_SLOT - 1;

						Input.PutDelay(Keys.Up);
						return;
				}
			} // if (Input.keyStateHeld.IsKeyDown(Keys.Up))

			if (Input.keyStateHeld.IsKeyDown(Keys.Down))
			{
				switch (state)
				{
					case eState.STATE_MAIN:
						if (cursorIndex < eCursor.CUR_MAX_DATA - 1)
							cursorIndex = (eCursor)(cursorIndex + 1);
						else
							cursorIndex = (eCursor)0;

						Input.PutDelay(Keys.Down);
						return;

					case eState.STATE_SELECTION:
						if (cursorIndexSelection < MAX_SLOT - 1)
							cursorIndexSelection++;
						else
							cursorIndexSelection = 0;

						Input.PutDelay(Keys.Down);
						return;
				}
			} // if (Input.keyStateHeld.IsKeyDown(Keys.Down))

			if (Input.keyStateDown.IsKeyDown(Keys.Enter))
			{
				switch (state)
				{
					case eState.STATE_MAIN:
						switch (cursorIndex)
						{
							case eCursor.CUR_NEW_GAME:
								//Goto to Selection screen
								state = eState.STATE_SELECTION;
								Input.PutDelay(Keys.Enter);
								//Start this new game
								return;

							case eCursor.CUR_CONTINUE:
								//Goto to Selection screen
								state = eState.STATE_SELECTION;
								Input.PutDelay(Keys.Enter);
								//Continue where it was
								return;

							case eCursor.CUR_QUIT:
								Game.Exit();
								return;

							default:
								break;
						}
						break;

					case eState.STATE_SELECTION:
						if (cursorIndex == eCursor.CUR_NEW_GAME)
						{
							NewGame();
						}
						else if (cursorIndex == eCursor.CUR_CONTINUE)
						{
							//Continue
							throw new NotImplementedException();
						}
						return;
				}
			} // if (Input.keyStateDown.IsKeyDown(Keys.Enter))

			if (Input.keyStateDown.IsKeyDown(Keys.Back))
			{
				switch (state)
				{
					case eState.STATE_MAIN:
						break;

					case eState.STATE_SELECTION:
						state = eState.STATE_MAIN;
						Input.PutDelay(Keys.Back);
						return;
				}
			} // if (Input.keyStateDown.IsKeyDown(Keys.Back))
		} // public override void Update(GameTime gameTime)
	}
}
