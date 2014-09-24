using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FantasyEngine.Classes.Overworld;
using FantasyEngineData.Entities;

namespace FantasyEngine.Classes.Menus
{
	public class JobChangeScene : Scene
	{
		private readonly string[] USE_COMMANDS = { "Change", "Cancel" };

		private readonly Color COLOR_NORMAL = Color.White;
		private readonly Color COLOR_UNUSUABLE = Color.Gray;
		private readonly Texture2D COMPARE_ARROW;

		private Character _CurrentActor;
		private Job[] _Jobs = new Job[Character.MAX_JOB];
		private Cursor _CursorWindow;
		private Window _DescriptionWindow;
		private Window _JobsWindow;
		private Window _CompareWindow;
		private Window _PrerequisiteWindow;
		private Command _UseCommand;
		private Window _MessageWindow;
		private TimeSpan _MessageTime;

		public Character CurrentActor
		{
			get { return _CurrentActor; }
			set { _CurrentActor = value; }
		}

		public JobChangeScene(Game game)
			: base(game)
		{
			COMPARE_ARROW = Game.Content.Load<Texture2D>(@"Images\Menus\compare_arrow");

			//TODO: Take in parameters the actor for multi-actor.
			CurrentActor = Player.GamePlayer.Actors[0];
			int i = 0;
			foreach (var job in CurrentActor.Jobs)
			{
				if (job == null || job == CurrentActor.CurrentJob)
					continue;

				_Jobs[i] = job;
				++i;
			}

			_CursorWindow = new Cursor(Game, _Jobs.Count(j => j != null));
			_DescriptionWindow = new Window(game, 0, 0, 640, 48); _DescriptionWindow.Visible = false;
			_JobsWindow = new Window(Game, 0, _DescriptionWindow.Rectangle.Bottom, 368, 432);
			_CompareWindow = new Window(game, _JobsWindow.Rectangle.Right, _DescriptionWindow.Rectangle.Bottom, 640 - _JobsWindow.Rectangle.Right, 184);
			_PrerequisiteWindow = new Window(game, _CompareWindow.Rectangle.X, _CompareWindow.Rectangle.Bottom, _CompareWindow.Rectangle.Width, 480 - _CompareWindow.Rectangle.Bottom);
			_MessageWindow = new Window(game, 0, 0, 640, 48); _MessageWindow.Enabled = false; _MessageWindow.Visible = false;

			_UseCommand = new Command(Game, 472, USE_COMMANDS, 2);
			_UseCommand.ChangeOffset(84, _JobsWindow.Rectangle.Bottom - _UseCommand.Rectangle.Height - Window.Tileset.TileHeight);
			_UseCommand.Enabled = false;
			_UseCommand.Visible = false;
		}

		public override void DrawGUI(GameTime gameTime)
		{
			base.DrawGUI(gameTime);

			Job job = _Jobs[_CursorWindow.CursorIndex];

			DrawDescription(gameTime, job);

			DrawListJobs(gameTime);

			DrawCompare(gameTime, job);

			DrawPrerequisite(gameTime, job);

			DrawMessage(gameTime);

			_UseCommand.Offset = spriteBatchGUI.CameraOffset;
			_UseCommand.Draw(gameTime);
		}

		private void DrawDescription(GameTime gameTime, Job job)
		{
			if (!_DescriptionWindow.Visible)
				return;

			_DescriptionWindow.Offset = spriteBatchGUI.CameraOffset;
			_DescriptionWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_DescriptionWindow.InsideBound);

			if (job != null)
				spriteBatchGUI.DrawString(GameMain.font, ""/*job.Description*/,
					new Vector2(16, 16) + spriteBatchGUI.CameraOffset, Color.White);

			spriteBatchGUI.ScissorReset();
		}

		private void DrawListJobs(GameTime gameTime)
		{
			_JobsWindow.Offset = spriteBatchGUI.CameraOffset;
			_JobsWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_JobsWindow.InsideBound);

			// Draw list of items
			int i = 0;
			foreach (var job in _Jobs)
			{
				if (job == null)
					continue;

				bool isAllowed = job.BaseJob.IsAllowed(CurrentActor);

				spriteBatchGUI.DrawString(GameMain.font, job.JobName,
					new Vector2(34, 68 + i * 22) + spriteBatchGUI.CameraOffset, isAllowed ? COLOR_NORMAL : COLOR_UNUSUABLE);

				spriteBatchGUI.DrawString(GameMain.font, job.BaseJob.JobAbbreviation,
					new Vector2(230, 68 + i * 22) + spriteBatchGUI.CameraOffset, isAllowed ? COLOR_NORMAL : COLOR_UNUSUABLE);

				spriteBatchGUI.DrawString(GameMain.font, "L:" + job.Level.ToString(),
					new Vector2(294, 68 + i * 22) + spriteBatchGUI.CameraOffset, isAllowed ? COLOR_NORMAL : COLOR_UNUSUABLE);

				++i;
			}

			if (i == 0)
				spriteBatchGUI.DrawString(GameMain.font, "No other job available", new Vector2(34, 68) + spriteBatchGUI.CameraOffset, COLOR_UNUSUABLE);

			// Draw cursor
			_CursorWindow.Position = new Vector2(14, 68 + _CursorWindow.CursorIndex * 22);
			_CursorWindow.Draw(gameTime);

			spriteBatchGUI.ScissorReset();
		}

		private void DrawCompare(GameTime gameTime, Job job)
		{
			if (!_CompareWindow.Visible)
				return;

			_CompareWindow.Offset = spriteBatchGUI.CameraOffset;
			_CompareWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_CompareWindow.InsideBound);

			if (job != null && job.Level != 0)
			{
				spriteBatchGUI.DrawString(GameMain.font, CurrentActor.Name,
					new Vector2(382, 68) + spriteBatchGUI.CameraOffset, Color.White);

				spriteBatchGUI.DrawString(GameMain.font8, "HP:",
					new Vector2(382, 90) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, CurrentActor.MaxHp.ToString().PadLeft(4, ''),
					new Vector2(510, 90) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.Draw(COMPARE_ARROW, new Vector2(562, 90) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, job.MaxHp.ToString().PadLeft(4, ''),
					new Vector2(578, 90) + spriteBatchGUI.CameraOffset, Color.White);

				spriteBatchGUI.DrawString(GameMain.font8, "MP:",
					new Vector2(382, 106) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, CurrentActor.MaxMp.ToString().PadLeft(4, ''),
					new Vector2(510, 106) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.Draw(COMPARE_ARROW, new Vector2(562, 106) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, job.MaxMp.ToString().PadLeft(4, ''),
					new Vector2(578, 106) + spriteBatchGUI.CameraOffset, Color.White);

				spriteBatchGUI.DrawString(GameMain.font8, "Strength:",
					new Vector2(382, 122) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, CurrentActor.Strength.ToString().PadLeft(3, ''),
					new Vector2(538, 122) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.Draw(COMPARE_ARROW, new Vector2(574, 122) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, job.Strength.ToString().PadLeft(3, ''),
					new Vector2(590, 122) + spriteBatchGUI.CameraOffset, Color.White);

				spriteBatchGUI.DrawString(GameMain.font8, "Vitality:",
					new Vector2(382, 138) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, CurrentActor.Vitality.ToString().PadLeft(3, ''),
					new Vector2(538, 138) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.Draw(COMPARE_ARROW, new Vector2(574, 138) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, job.Vitality.ToString().PadLeft(3, ''),
					new Vector2(590, 138) + spriteBatchGUI.CameraOffset, Color.White);

				spriteBatchGUI.DrawString(GameMain.font8, "Accuracy:",
					new Vector2(382, 154) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, CurrentActor.Accuracy.ToString().PadLeft(3, ''),
					new Vector2(538, 154) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.Draw(COMPARE_ARROW, new Vector2(574, 154) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, job.Accuracy.ToString().PadLeft(3, ''),
					new Vector2(590, 154) + spriteBatchGUI.CameraOffset, Color.White);

				spriteBatchGUI.DrawString(GameMain.font8, "Agility:",
					new Vector2(382, 170) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, CurrentActor.Agility.ToString().PadLeft(3, ''),
					new Vector2(538, 170) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.Draw(COMPARE_ARROW, new Vector2(574, 170) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, job.Agility.ToString().PadLeft(3, ''),
					new Vector2(590, 170) + spriteBatchGUI.CameraOffset, Color.White);

				spriteBatchGUI.DrawString(GameMain.font8, "Intelligence:",
					new Vector2(382, 186) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, CurrentActor.Intelligence.ToString().PadLeft(3, ''),
					new Vector2(538, 186) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.Draw(COMPARE_ARROW, new Vector2(574, 186) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, job.Intelligence.ToString().PadLeft(3, ''),
					new Vector2(590, 186) + spriteBatchGUI.CameraOffset, Color.White);

				spriteBatchGUI.DrawString(GameMain.font8, "Wisdom:",
					new Vector2(382, 202) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, CurrentActor.Wisdom.ToString().PadLeft(3, ''),
					new Vector2(538, 202) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.Draw(COMPARE_ARROW, new Vector2(574, 202) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font8, job.Wisdom.ToString().PadLeft(3, ''),
					new Vector2(590, 202) + spriteBatchGUI.CameraOffset, Color.White);
			}

			spriteBatchGUI.ScissorReset();
		}

		private void DrawPrerequisite(GameTime gameTime, Job job)
		{
			if (!_PrerequisiteWindow.Visible)
				return;

			_PrerequisiteWindow.Offset = spriteBatchGUI.CameraOffset;
			_PrerequisiteWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_PrerequisiteWindow.InsideBound);

			if (job != null)
			{
				int i = 0;
				foreach (var jobPrerequisite in job.BaseJob.PrerequisiteJobs)
				{
					int indexJob = Array.IndexOf<BaseJob>(CurrentActor.BaseJobs, jobPrerequisite.Job);
					if (CurrentActor.Jobs[indexJob].Level < jobPrerequisite.Level)
					{
						spriteBatchGUI.DrawString(GameMain.font, jobPrerequisite.Job.JobAbbreviation + " Level " + jobPrerequisite.Level,
							new Vector2(382, 248 + i * 22) + spriteBatchGUI.CameraOffset, Color.White);
						++i;
					}
				}

				if (i == 0)
					spriteBatchGUI.DrawString(GameMain.font, "Available", new Vector2(382, 248 + i * 22) + spriteBatchGUI.CameraOffset, Color.White);
			}

			spriteBatchGUI.ScissorReset();
		}

		private void DrawMessage(GameTime gameTime)
		{
			if (!_MessageWindow.Visible)
				return;

			_MessageWindow.Offset = spriteBatchGUI.CameraOffset;
			_MessageWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_MessageWindow.InsideBound);

			spriteBatchGUI.DrawString(GameMain.font, "Job changed and item unequipped.",
				new Vector2(16, 16) + spriteBatchGUI.CameraOffset, Color.White);

			spriteBatchGUI.ScissorReset();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_UseCommand.Update(gameTime);

			if (!Input.UpdateInput(gameTime))
				return;

			if (_MessageWindow.Enabled)
			{
				if (gameTime.TotalGameTime - _MessageTime >= TimeSpan.FromSeconds(2) || Input.keyStateDown.IsKeyDown(Keys.Enter))
					Scene.RemoveSubScene();

				return;
			}

			if (!_UseCommand.Enabled)
				_CursorWindow.Update(gameTime);

			if (Input.keyStateDown.IsKeyDown(Keys.Enter))
			{
				Job job = _Jobs[_CursorWindow.CursorIndex];
				if (_UseCommand.Enabled)
					switch (_UseCommand.CursorPosition)
					{
						case 0: // Change
							CurrentActor.Hp = CurrentActor.MaxHp;
							CurrentActor.Mp = CurrentActor.MaxMp;
							CurrentActor.Statut = Status.Normal;

							for (int i = 0; i < Character.MAX_JOB; i++)
							{
								if (job == CurrentActor.Jobs[i])
								{
									CurrentActor.IndexCurrentJob = i;
									break;
								}
							}

							CurrentActor.UnequipAll(Player.GamePlayer.Inventory);
							Player.GamePlayer.Hero.ChangeSprite(@"Overworld\characters", CurrentActor.CurrentJob.BaseJob.OverworldSpriteSize, Sprite.OVERWORLD_SIZE);

							_MessageWindow.Enabled = true;
							_MessageWindow.Visible = true;
							_MessageTime = gameTime.TotalGameTime;
							_UseCommand.Enabled = false;
							_UseCommand.Visible = false;
							break;

						case 1: // Cancel
							_UseCommand.Enabled = false;
							_UseCommand.Visible = false;
							break;
					}
				else
					if (job != null && job.BaseJob.IsAllowed(CurrentActor))
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
				else
					Scene.RemoveSubScene();
			}
		}
	}
}
