using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData;
using FantasyEngineData.Effects;
using FantasyEngineData.Entities;

namespace FantasyEngine.Classes.Menus
{
	public class CharacterScene : Scene
	{
		private Cursor CursorWindow;
		private Window _CharacterWindow;
		/// <summary>
		/// Character who is shown.
		/// </summary>
		public Character ActiveCharacter;
		private Character _RealActiveCharacter;

		public CharacterScene(Game game, Character activeCharacter)
			: base(game)
		{
			CursorWindow = new Cursor(Game, 14, 2);
			_CharacterWindow = new Window(Game, 76, 55, 488, 370);
			_RealActiveCharacter = activeCharacter;
			ActiveCharacter = (Character)activeCharacter.CloneExt();
			_RealActiveCharacter.EquipmentChanged += new Character.EquipmentChangedHandler(_RealActiveCharacter_EquipmentChanged);
		}

		private void _RealActiveCharacter_EquipmentChanged(EventArgs e)
		{
			ActiveCharacter.RightHand = _RealActiveCharacter.RightHand;
			ActiveCharacter.LeftHand = _RealActiveCharacter.LeftHand;
			ActiveCharacter.Head = _RealActiveCharacter.Head;
			ActiveCharacter.Body = _RealActiveCharacter.Body;
			ActiveCharacter.Arms = _RealActiveCharacter.Arms;
			ActiveCharacter.Feet = _RealActiveCharacter.Feet;
		}

		public override void DrawGUI(GameTime gameTime)
		{
			base.DrawGUI(gameTime);

			_CharacterWindow.Offset = spriteBatchGUI.CameraOffset;
			_CharacterWindow.Draw(gameTime);
			spriteBatchGUI.Scissor(_CharacterWindow.InsideBound);

			spriteBatchGUI.DrawString(GameMain.font, "Character", new Vector2(260, 68) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Name, new Vector2(90, 96) + spriteBatchGUI.CameraOffset, Color.White);

			spriteBatchGUI.DrawString(GameMain.font, "Job:" + ActiveCharacter.CurrentJob.JobName,
				new Vector2(90, 118) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Level:" + ActiveCharacter.Level,
				new Vector2(90, 140) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Exp:" + ActiveCharacter.TotalExp.ToString("### ##0").Trim(),
				new Vector2(256, 140) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Next Lvl:" + (Job.ExpForLevel(ActiveCharacter.Level) - ActiveCharacter.Exp).ToString("### ##0").Trim(),
				new Vector2(202, 156) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "HP:" + ActiveCharacter.Hp + "/" + ActiveCharacter.MaxHp,
				new Vector2(90, 184) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "MP:" + ActiveCharacter.Mp + "/" + ActiveCharacter.MaxMp,
				new Vector2(90, 206) + spriteBatchGUI.CameraOffset, Color.White);

			spriteBatchGUI.DrawString(GameMain.font, "Strength:", new Vector2(90, 236) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Strength.ToString().PadLeft(3),
				new Vector2(274, 236) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Vitality:", new Vector2(90, 258) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Vitality.ToString().PadLeft(3),
				new Vector2(274, 258) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Accuracy:", new Vector2(90, 280) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Accuracy.ToString().PadLeft(3),
				new Vector2(274, 280) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Agility:", new Vector2(90, 302) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Agility.ToString().PadLeft(3),
				new Vector2(274, 302) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Intelligence:", new Vector2(90, 324) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Intelligence.ToString().PadLeft(3),
				new Vector2(274, 324) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Wisdom:", new Vector2(90, 346) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Wisdom.ToString().PadLeft(3),
				new Vector2(274, 346) + spriteBatchGUI.CameraOffset, Color.White);

			spriteBatchGUI.DrawString(GameMain.font, "Stat Points Remaining", new Vector2(90, 368) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.StatRemaining.ToString(),
				new Vector2(338 - GameMain.font.MeasureString(ActiveCharacter.StatRemaining.ToString()).X, 398) + spriteBatchGUI.CameraOffset, Color.White);

			spriteBatchGUI.DrawString(GameMain.font, "Attack:", new Vector2(348, 236) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, (ActiveCharacter.getAttackMultiplier() + "x").PadLeft(3, ''),
				new Vector2(474, 239) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, ActiveCharacter.getBaseDamage(ePhysicalDamageOption.BOTH).ToString().PadLeft(3, ''),
				new Vector2(514, 239) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Hit %:", new Vector2(348, 258) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, (ActiveCharacter.getHitPourc(ePhysicalDamageOption.BOTH) + "%").PadLeft(3, ''),
				new Vector2(514, 261) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Defense:", new Vector2(348, 280) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, (ActiveCharacter.getDefenseMultiplier() + "x").PadLeft(3, ''),
				new Vector2(474, 283) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, ActiveCharacter.getDefenseDamage().ToString().PadLeft(3, ''),
				new Vector2(514, 283) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "Evade:", new Vector2(348, 302) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, (ActiveCharacter.getEvadePourc() + "%").PadLeft(3, ''),
				new Vector2(514, 305) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "M.Attack:", new Vector2(348, 324) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, (ActiveCharacter.getMagicAttackMultiplier(eMagicalDamageOption.NONE) + "x").PadLeft(3, ''),
				new Vector2(474, 327) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, ActiveCharacter.getMagicBaseDamage(eMagicalDamageOption.NONE, 1).ToString().PadLeft(3, ''),
				new Vector2(514, 327) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "M.Hit %:", new Vector2(348, 346) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, (ActiveCharacter.getMagicHitPourc(eMagicalDamageOption.NONE, 80) + "%").PadLeft(3, ''),
				new Vector2(514, 349) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "M.Defense:", new Vector2(348, 368) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, (ActiveCharacter.getMagicDefenseMultiplier() + "x").PadLeft(3, ''),
				new Vector2(474, 371) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, ActiveCharacter.getMagicDefenseDamage().ToString().PadLeft(3, ''),
				new Vector2(514, 371) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font, "M.Evade:", new Vector2(348, 390) + spriteBatchGUI.CameraOffset, Color.White);
			spriteBatchGUI.DrawString(GameMain.font8, (ActiveCharacter.getMagicEvadePourc() + "%").PadLeft(3, ''),
				new Vector2(514, 393) + spriteBatchGUI.CameraOffset, Color.White);

			if (_RealActiveCharacter.StatRemaining > 0)
			{
				// Place the cursor to his place for his index.
				CursorWindow.Effects = SpriteEffects.None;
				if (CursorWindow.CursorIndex < 12)
					if (CursorWindow.CursorIndex % 2 == 0)
						CursorWindow.Position = new Vector2(244, 236 + (CursorWindow.CursorIndex / 2 * 22));
					else
					{
						CursorWindow.Position = new Vector2(326, 236 + ((CursorWindow.CursorIndex - 1) / 2 * 22));
						CursorWindow.Effects = SpriteEffects.FlipHorizontally;
					}
				else
					if (CursorWindow.CursorIndex == 12)
						CursorWindow.Position = new Vector2(90, 398);
					else
						CursorWindow.Position = new Vector2(188, 398);

				CursorWindow.Draw(gameTime);

				// Hide the minus if we can't do it.
				if (_RealActiveCharacter.Strength < ActiveCharacter.Strength)
					spriteBatchGUI.DrawString(GameMain.font, "-", new Vector2(262, 236) + spriteBatchGUI.CameraOffset, Color.White);
				if (_RealActiveCharacter.Vitality < ActiveCharacter.Vitality)
					spriteBatchGUI.DrawString(GameMain.font, "-", new Vector2(262, 258) + spriteBatchGUI.CameraOffset, Color.White);
				if (_RealActiveCharacter.Accuracy < ActiveCharacter.Accuracy)
					spriteBatchGUI.DrawString(GameMain.font, "-", new Vector2(262, 280) + spriteBatchGUI.CameraOffset, Color.White);
				if (_RealActiveCharacter.Agility < ActiveCharacter.Agility)
					spriteBatchGUI.DrawString(GameMain.font, "-", new Vector2(262, 302) + spriteBatchGUI.CameraOffset, Color.White);
				if (_RealActiveCharacter.Intelligence < ActiveCharacter.Intelligence)
					spriteBatchGUI.DrawString(GameMain.font, "-", new Vector2(262, 324) + spriteBatchGUI.CameraOffset, Color.White);
				if (_RealActiveCharacter.Wisdom < ActiveCharacter.Wisdom)
					spriteBatchGUI.DrawString(GameMain.font, "-", new Vector2(262, 346) + spriteBatchGUI.CameraOffset, Color.White);

				// Hide the plus if we can't add anymore stats.
				if (ActiveCharacter.StatRemaining > 0)
				{
					spriteBatchGUI.DrawString(GameMain.font, "+", new Vector2(312, 236) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font, "+", new Vector2(312, 258) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font, "+", new Vector2(312, 280) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font, "+", new Vector2(312, 302) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font, "+", new Vector2(312, 324) + spriteBatchGUI.CameraOffset, Color.White);
					spriteBatchGUI.DrawString(GameMain.font, "+", new Vector2(312, 346) + spriteBatchGUI.CameraOffset, Color.White);
				}

				spriteBatchGUI.DrawString(GameMain.font, "Accept", new Vector2(110, 398) + spriteBatchGUI.CameraOffset, Color.White);
				spriteBatchGUI.DrawString(GameMain.font, "Reset", new Vector2(208, 398) + spriteBatchGUI.CameraOffset, Color.White);
			}

			spriteBatchGUI.ScissorReset();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (!Input.UpdateInput(gameTime))
				return;

			if (_RealActiveCharacter.StatRemaining > 0)
			{
				CursorWindow.Update(gameTime);

				if (Input.keyStateDown.IsKeyDown(Keys.Enter))
				{
					switch (CursorWindow.CursorIndex)
					{
						case 0: //-Strength
							if (_RealActiveCharacter.Strength < ActiveCharacter.Strength)
							{
								ActiveCharacter.Strength--;
								ActiveCharacter.StatRemaining++;
							}
							break;

						case 1: //+Strength
							if (ActiveCharacter.StatRemaining > 0)
							{
								ActiveCharacter.Strength++;
								ActiveCharacter.StatRemaining--;
							}
							break;

						case 2: //-Vitality
							if (_RealActiveCharacter.Vitality < ActiveCharacter.Vitality)
							{
								int oldMaxHp = ActiveCharacter.MaxHp;
								ActiveCharacter.Vitality--;
								ActiveCharacter.StatRemaining++;
								// Heal the amount gained with the level up.
								ActiveCharacter.Hp += ActiveCharacter.MaxHp - oldMaxHp;
							}
							break;

						case 3: //+Vitality
							if (ActiveCharacter.StatRemaining > 0)
							{
								int oldMaxHp = ActiveCharacter.MaxHp;
								ActiveCharacter.Vitality++;
								ActiveCharacter.StatRemaining--;
								// Heal the amount gained with the level up.
								ActiveCharacter.Hp += ActiveCharacter.MaxHp - oldMaxHp;
							}
							break;

						case 4: //-Accuracy
							if (_RealActiveCharacter.Accuracy < ActiveCharacter.Accuracy)
							{
								ActiveCharacter.Accuracy--;
								ActiveCharacter.StatRemaining++;
							}
							break;

						case 5: //+Accuracy
							if (ActiveCharacter.StatRemaining > 0)
							{
								ActiveCharacter.Accuracy++;
								ActiveCharacter.StatRemaining--;
							}
							break;

						case 6: //-Agility
							if (_RealActiveCharacter.Agility < ActiveCharacter.Agility)
							{
								ActiveCharacter.Agility--;
								ActiveCharacter.StatRemaining++;
							}
							break;

						case 7: //+Agility
							if (ActiveCharacter.StatRemaining > 0)
							{
								ActiveCharacter.Agility++;
								ActiveCharacter.StatRemaining--;
							}
							break;

						case 8: //-Intelligence
							if (_RealActiveCharacter.Intelligence < ActiveCharacter.Intelligence)
							{
								ActiveCharacter.Intelligence--;
								ActiveCharacter.StatRemaining++;
							}
							break;

						case 9: //+Intelligence
							if (ActiveCharacter.StatRemaining > 0)
							{
								ActiveCharacter.Intelligence++;
								ActiveCharacter.StatRemaining--;
							}
							break;

						case 10: //-Wisdom
							if (_RealActiveCharacter.Wisdom < ActiveCharacter.Wisdom)
							{
								int oldMaxMp = ActiveCharacter.MaxMp;
								ActiveCharacter.Wisdom--;
								ActiveCharacter.StatRemaining++;
								// Heal the amount gained with the level up.
								ActiveCharacter.Mp += ActiveCharacter.MaxMp - oldMaxMp;
							}
							break;

						case 11: //+Wisdom
							if (ActiveCharacter.StatRemaining > 0)
							{
								int oldMaxMp = ActiveCharacter.MaxMp;
								ActiveCharacter.Wisdom++;
								ActiveCharacter.StatRemaining--;
								// Heal the amount gained with the level up.
								ActiveCharacter.Mp += ActiveCharacter.MaxMp - oldMaxMp;
							}
							break;

						case 12: //Accept
							_RealActiveCharacter.Strength = ActiveCharacter.Strength;
							_RealActiveCharacter.Vitality = ActiveCharacter.Vitality;
							_RealActiveCharacter.Accuracy = ActiveCharacter.Accuracy;
							_RealActiveCharacter.Agility = ActiveCharacter.Agility;
							_RealActiveCharacter.Intelligence = ActiveCharacter.Intelligence;
							_RealActiveCharacter.Wisdom = ActiveCharacter.Wisdom;
							_RealActiveCharacter.StatRemaining = ActiveCharacter.StatRemaining;
							_RealActiveCharacter.Hp = ActiveCharacter.Hp;
							_RealActiveCharacter.Mp = ActiveCharacter.Mp;
							break;

						case 13: //Reset
							ActiveCharacter = (Character)_RealActiveCharacter.CloneExt();
							break;
					}
				}
			}

			if (Input.keyStateDown.IsKeyDown(Keys.E))
				Scene.AddSubScene(new EquipScene(Game, _RealActiveCharacter));

			if (Input.keyStateDown.IsKeyDown(Keys.Escape)
				|| Input.keyStateDown.IsKeyDown(Keys.Back)
				|| Input.keyStateDown.IsKeyDown(Keys.C))
			{
				Scene.RemoveSubScene();
			}
		}
	}
}
