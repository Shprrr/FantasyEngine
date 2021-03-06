using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData.Entities;
using FantasyEngineData.Skills;

namespace FantasyEngine.Classes.Menus
{
	public class SkillScene : Scene
	{
		private struct SkillDraw
		{
			public Color color;
			public string name;
			public string level;
			public float expRatio;
			public string exp;
			public string mpCost;
		}

		private readonly string[] USE_COMMANDS = { "Use", "Sort" };

		private readonly Color COLOR_NORMAL = Color.White;
		private readonly Color COLOR_OVERLEVEL = Color.DarkGray;
		private readonly Color COLOR_UNUSUABLE = Color.Black;
		private readonly Color COLOR_EXP_BAR = Color.DimGray;

		private Character _CurrentActor;
		private Cursor _CursorWindow;
		private int _CursorSortBeginIndex = -1;
		private Window _DescriptionWindow;
		private Window _SkillsWindow;
		private Window _EffectWindow;
		private Window _NextLevelWindow;
		private Window _JobAvailableWindow;
		private Window[] _ActorsWindow;
		private Command _UseCommand;

		private List<SkillDraw> skillDraws = new List<SkillDraw>();

		public Character CurrentActor
		{
			get { return _CurrentActor; }
			set
			{
				_CurrentActor = value;
				UpdateSkills();
				_CursorWindow = new Cursor(Game, _CurrentActor.Skills.Count);
			}
		}

		public SkillScene(Game game)
			: base(game)
		{
			//TODO: Take in parameters the actor for multi-actor.
			CurrentActor = Player.GamePlayer.Actors[0];

			_DescriptionWindow = new Window(game, 0, 0, 640, 48);
			_SkillsWindow = new Window(Game, 0, _DescriptionWindow.Rectangle.Bottom, 640, 348);
			_EffectWindow = new Window(game, 454, _DescriptionWindow.Rectangle.Bottom, 186, 84);
			_NextLevelWindow = new Window(game, _EffectWindow.Rectangle.X, _EffectWindow.Rectangle.Bottom, _EffectWindow.Rectangle.Width, 84);
			_JobAvailableWindow = new Window(game, _EffectWindow.Rectangle.X, _NextLevelWindow.Rectangle.Bottom,
				_EffectWindow.Rectangle.Width, _SkillsWindow.Rectangle.Bottom - _NextLevelWindow.Rectangle.Bottom);

			_ActorsWindow = new Window[Player.MAX_ACTOR];
			for (int i = 0; i < Player.MAX_ACTOR; i++)
			{
				if (Player.GamePlayer.Actors[i] != null)
					_ActorsWindow[i] = new Window(game, 0, _SkillsWindow.Rectangle.Bottom, 160, 480 - _SkillsWindow.Rectangle.Bottom);
			}

			_UseCommand = new Command(Game, 472, USE_COMMANDS, 2);
			_UseCommand.ChangeOffset(84, _SkillsWindow.Rectangle.Bottom - _UseCommand.Rectangle.Height - Window.Tileset.TileHeight);
			_UseCommand.Enabled = false;
			_UseCommand.Visible = false;
		}

		public void UpdateSkills()
		{
			skillDraws.Clear();

			foreach (var skill in CurrentActor.Skills)
			{
				SkillDraw skillDraw = new SkillDraw();

				int skillLevel;
				if (skill.IsUsable(CurrentActor, out skillLevel))
					if (skillLevel != skill.Level) skillDraw.color = COLOR_OVERLEVEL;
					else skillDraw.color = COLOR_NORMAL;
				else
					skillDraw.color = COLOR_UNUSUABLE;

				skillDraw.name = skill.Name;
				skillDraw.level = "L" + skill.Level.ToString().PadLeft(2, '');

				int skillExp = skill.Exp;
				int skillExpForLevel = skill.ExpForLevel(skill.Level);
				skillDraw.expRatio = (float)skillExp / skillExpForLevel;

				skillDraw.exp = skillExp.ToString().PadLeft(5, '') + "/" + skillExpForLevel.ToString().PadLeft(5, '');

				if (skill.Level != 0)
					skillDraw.mpCost = skill.MPCostForLevel(skillLevel).ToString().PadLeft(3, '');
				else
					skillDraw.mpCost = string.Empty;

				skillDraws.Add(skillDraw);
			}
		}

		public override void DrawGUI(GameTime gameTime)
		{
			base.DrawGUI(gameTime);

			Skill skill = null;
			if (CurrentActor.Skills.Count != 0)
				skill = CurrentActor.Skills[_CursorWindow.CursorIndex];

			DrawDescription(gameTime, skill);

			DrawListSkill(gameTime);

			DrawEffect(gameTime, skill);

			DrawNextLevel(gameTime, skill);

			DrawJobAvailable(gameTime, skill);

			for (int i = 0; i < Player.MAX_ACTOR; i++)
			{
				if (_ActorsWindow[i] != null)
					DrawActor(gameTime, _ActorsWindow[i], Player.GamePlayer.Actors[i]);
			}

			_UseCommand.Offset = spriteBatchGUI.CameraOffset;
			_UseCommand.Draw(gameTime);
		}

		private void DrawDescription(GameTime gameTime, Skill skill)
		{
			if (!_DescriptionWindow.Visible)
				return;

			_DescriptionWindow.Offset = spriteBatchGUI.CameraOffset;
			_DescriptionWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_DescriptionWindow.InsideBound);

			if (skill != null)
				spriteBatchGUI.DrawString(GameMain.font, skill.Description,
					new Vector2(16, 16) + spriteBatchGUI.CameraOffset, Color.White);

			spriteBatchGUI.ScissorReset();
		}

		private void DrawListSkill(GameTime gameTime)
		{
			_SkillsWindow.Offset = spriteBatchGUI.CameraOffset;
			_SkillsWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_SkillsWindow.InsideBound);

			spriteBatchGUI.DrawString(GameMain.font, "Skills", new Vector2(275, 61) + spriteBatchGUI.CameraOffset, Color.White);

			// Draw list of items
			for (int i = 0; i < skillDraws.Count; i++)
			{
				spriteBatchGUI.DrawString(GameMain.font8, skillDraws[i].name,
					new Vector2(34, 89 + i * 16) + spriteBatchGUI.CameraOffset, skillDraws[i].color);
				spriteBatchGUI.DrawString(GameMain.font8, skillDraws[i].level,
					new Vector2(212, 89 + i * 16) + spriteBatchGUI.CameraOffset, skillDraws[i].color);

				spriteBatchGUI.DrawOutlineRectangle(spriteBatchGUI.pixel, new Rectangle(254, 86 + i * 16, 140, 16), Color.White);
				spriteBatchGUI.Draw(spriteBatchGUI.pixel, new Rectangle(255, 87 + i * 16, (int)(138 * skillDraws[i].expRatio), 15), COLOR_EXP_BAR);

				spriteBatchGUI.DrawString(GameMain.font8, skillDraws[i].exp,
					new Vector2(258, 89 + i * 16) + spriteBatchGUI.CameraOffset, skillDraws[i].color);

				spriteBatchGUI.DrawString(GameMain.font8, skillDraws[i].mpCost,
					new Vector2(404, 89 + i * 16) + spriteBatchGUI.CameraOffset, skillDraws[i].color);
			}

			// Draw cursor
			_CursorWindow.Position = new Vector2(14, 89 + _CursorWindow.CursorIndex * 16);
			_CursorWindow.Draw(gameTime);

			if (_CursorSortBeginIndex >= 0)
				Cursor.DrawShadow(gameTime, new Vector2(14, 89 + _CursorSortBeginIndex * 16));

			spriteBatchGUI.ScissorReset();
		}

		private void DrawEffect(GameTime gameTime, Skill skill)
		{
			if (!_EffectWindow.Visible)
				return;

			_EffectWindow.Offset = spriteBatchGUI.CameraOffset;
			_EffectWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_EffectWindow.InsideBound);

			if (skill != null)
			{
				int level = skill.Level;

				var jobAllowed = skill.IsAllowed(CurrentActor.CurrentJob.BaseJob);
				if (jobAllowed != null)
				{
					if (jobAllowed.MaxLevel != 0 && jobAllowed.MaxLevel <= skill.Level)
						level = jobAllowed.MaxLevel;
				}

				spriteBatchGUI.DrawString(GameMain.font8, skill.Effect.EffectForLevel(level).TypeToString(),
					new Vector2(468, 62) + spriteBatchGUI.CameraOffset, Color.White);

				if (level > 0 && jobAllowed != null)
				{
					spriteBatchGUI.DrawString(GameMain.font8, "Power:",
						new Vector2(468, 76) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font8, skill.Effect.EffectForLevel(level).Value.ToString().PadLeft(3, ''),
						new Vector2(576, 76) + spriteBatchGUI.CameraOffset, Color.White);

					spriteBatchGUI.DrawString(GameMain.font8, "Hit%:",
						new Vector2(468, 90) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font8, skill.Effect.EffectForLevel(level).HitPourc.ToString().PadLeft(2, '') + "%",
						new Vector2(576, 90) + spriteBatchGUI.CameraOffset, Color.White);

					spriteBatchGUI.DrawString(GameMain.font8, "MP Cost:",
						new Vector2(468, 104) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font8, skill.MPCostForLevel(level).ToString().PadLeft(3, ''),
						new Vector2(576, 104) + spriteBatchGUI.CameraOffset, Color.White);
				}
			}

			spriteBatchGUI.ScissorReset();
		}

		private void DrawNextLevel(GameTime gameTime, Skill skill)
		{
			if (!_NextLevelWindow.Visible)
				return;

			_NextLevelWindow.Offset = spriteBatchGUI.CameraOffset;
			_NextLevelWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_NextLevelWindow.InsideBound);

			if (skill != null)
			{
				var jobAllowed = skill.IsAllowed(CurrentActor.CurrentJob.BaseJob);

				if (skill != null && skill.Level != Skill.MAX_LEVEL && (jobAllowed == null || jobAllowed.MaxLevel == 0 || skill.Level < jobAllowed.MaxLevel))
				{
					spriteBatchGUI.DrawString(GameMain.font8, "Next level",
						new Vector2(468, 146) + spriteBatchGUI.CameraOffset, Color.White);

					spriteBatchGUI.DrawString(GameMain.font8, "Power:",
						new Vector2(468, 160) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font8, skill.Effect.EffectForLevel(skill.Level + 1).Value.ToString().PadLeft(3, ''),
						new Vector2(576, 160) + spriteBatchGUI.CameraOffset, Color.White);

					spriteBatchGUI.DrawString(GameMain.font8, "Hit%:",
						new Vector2(468, 174) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font8, skill.Effect.EffectForLevel(skill.Level + 1).HitPourc.ToString().PadLeft(2, '') + "%",
						new Vector2(576, 174) + spriteBatchGUI.CameraOffset, Color.White);

					spriteBatchGUI.DrawString(GameMain.font8, "MP Cost:",
						new Vector2(468, 188) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font8, skill.MPCostForLevel(skill.Level + 1).ToString().PadLeft(3, ''),
						new Vector2(576, 188) + spriteBatchGUI.CameraOffset, Color.White);
				}
			}

			spriteBatchGUI.ScissorReset();
		}

		private void DrawJobAvailable(GameTime gameTime, Skill skill)
		{
			if (!_JobAvailableWindow.Visible)
				return;

			_JobAvailableWindow.Offset = spriteBatchGUI.CameraOffset;
			_JobAvailableWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_JobAvailableWindow.InsideBound);

			if (skill != null)
			{
				int i = 0;
				foreach (var jobAllowed in skill.AllowableJobs)
				{
					int indexJob = Array.IndexOf<BaseJob>(CurrentActor.BaseJobs, jobAllowed.Job);
					if (jobAllowed.Level > 0 && CurrentActor.Jobs[indexJob].Level < jobAllowed.Level)
						spriteBatchGUI.DrawString(GameMain.font8, jobAllowed.Job.JobAbbreviation + " Level " + jobAllowed.Level,
							new Vector2(468, 230 + i * 14) + spriteBatchGUI.CameraOffset, Color.White);
					else if (jobAllowed.Skill.Name != null && CurrentActor.Skills.Find(s => s.Name == jobAllowed.Skill.Name).Level < jobAllowed.Skill.Level)
						spriteBatchGUI.DrawString(GameMain.font8, jobAllowed.Job.JobAbbreviation + " " + jobAllowed.Skill.Name + " " + jobAllowed.Skill.Level,
							new Vector2(468, 230 + i * 14) + spriteBatchGUI.CameraOffset, Color.White);
					else if (jobAllowed.MaxLevel > 0 && skill.Level > jobAllowed.MaxLevel)
						spriteBatchGUI.DrawString(GameMain.font8, jobAllowed.Job.JobAbbreviation + " MaxLevel " + jobAllowed.MaxLevel,
							new Vector2(468, 230 + i * 14) + spriteBatchGUI.CameraOffset, Color.White);
					else if (skill.Level == 0)
						spriteBatchGUI.DrawString(GameMain.font8, jobAllowed.Job.JobAbbreviation + " Learnable",
							new Vector2(468, 230 + i * 14) + spriteBatchGUI.CameraOffset, Color.White);
					else
						spriteBatchGUI.DrawString(GameMain.font8, jobAllowed.Job.JobAbbreviation + " Usable",
							new Vector2(468, 230 + i * 14) + spriteBatchGUI.CameraOffset, Color.White);

					i++;
				}
			}

			spriteBatchGUI.ScissorReset();
		}

		private void DrawActor(GameTime gameTime, Window actorWindow, Character actor)
		{
			if (!actorWindow.Visible)
				return;

			actorWindow.Offset = spriteBatchGUI.CameraOffset;
			actorWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(actorWindow.InsideBound);

			if (actor != null)
			{
				spriteBatchGUI.DrawString(GameMain.font8, actor.Name, new Vector2(14, 410) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, "HP", new Vector2(14, 424) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, actor.Hp + "/" + actor.MaxHp, new Vector2(42, 424) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, "MP", new Vector2(14, 438) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, actor.Mp + "/" + actor.MaxMp, new Vector2(42, 438) + spriteBatchGUI.CameraOffset, Color.White);
				//spriteBatchGUI.DrawString(GameMain.font8, actor.Statut.ToString(), new Vector2(14, 452) + spriteBatchGUI.CameraOffset, Color.White);
			}

			spriteBatchGUI.ScissorReset();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_UseCommand.Update(gameTime);

			if (!Input.UpdateInput(gameTime))
				return;

			_CursorWindow.ItemMax = CurrentActor.Skills.Count;

			// Command actif ou on est en train de faire un tri
			if (!_UseCommand.Enabled)
				_CursorWindow.Update(gameTime);

			if (Input.keyStateDown.IsKeyDown(Keys.Enter))
			{
				Skill skill = CurrentActor.Skills[_CursorWindow.CursorIndex];
				if (_UseCommand.Enabled)
					switch (_UseCommand.CursorPosition)
					{
						case 0: // Use
							if (skill.MenuUsable && skill.Level > 0)
							{
								//TODO: Choose an Actor.
								int skillLevel;
								if (skill.Casting(CurrentActor, out skillLevel))
									skill.Use(CurrentActor, Player.GamePlayer.Actors[0], skillLevel);
							}
							_UseCommand.Visible = false;
							_UseCommand.Enabled = false;
							break;

						case 1: // Sort
							_CursorSortBeginIndex = _CursorWindow.CursorIndex;
							_UseCommand.Enabled = false;
							break;
					}
				else if (_CursorSortBeginIndex >= 0)
				{
					int index = CurrentActor.Skills.IndexOf(skill);
					Skill skillToMove = CurrentActor.Skills.Find(s => s == CurrentActor.Skills[_CursorSortBeginIndex]);

					if (index == -1)
						index = CurrentActor.Skills.Count - 1;

					CurrentActor.Skills.Remove(skillToMove);
					CurrentActor.Skills.Insert(index, skillToMove);
					UpdateSkills();

					_CursorSortBeginIndex = -1;
					_UseCommand.Enabled = false;
					_UseCommand.Visible = false;
				}
				else
				{
					_UseCommand.Enabled = true;
					_UseCommand.Visible = true;
				}
			}

			if (Input.keyStateDown.IsKeyDown(Keys.Escape)
				|| Input.keyStateDown.IsKeyDown(Keys.Back))
			{
				if (_UseCommand.Enabled)
				{
					_UseCommand.Enabled = false;
					_UseCommand.Visible = false;
				}
				else if (_CursorSortBeginIndex >= 0)
				{
					// Annule un tri
					_CursorWindow.CursorIndex = _CursorSortBeginIndex;
					_CursorSortBeginIndex = -1;
					_UseCommand.Enabled = true;
				}
				else
					Scene.RemoveSubScene();
			}

			if (Input.keyStateDown.IsKeyDown(Keys.K))
			{
				Scene.RemoveSubScene();
			}
		}
	}
}
