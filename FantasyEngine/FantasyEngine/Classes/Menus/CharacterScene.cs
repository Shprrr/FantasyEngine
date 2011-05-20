using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData;

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

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 offset = new Vector2(-GameMain.OldCameraMatrix.Translation.X, -GameMain.OldCameraMatrix.Translation.Y);

            _CharacterWindow.Offset = offset;
            _CharacterWindow.Draw(gameTime);
            GameMain.Scissor(_CharacterWindow.InsideBound);

            spriteBatch.DrawString(GameMain.font, "Character", new Vector2(260, 68) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Name, new Vector2(90, 96) + offset, Color.White);

            spriteBatch.DrawString(GameMain.font, "Job:" + ActiveCharacter.CurrentJob.JobName,
                new Vector2(90, 118) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Level:" + ActiveCharacter.Level,
                new Vector2(90, 140) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Exp:" + ActiveCharacter.TotalExp,
                new Vector2(256, 140) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Next Lvl:" + (Job.ExpForLevel(ActiveCharacter.Level) - ActiveCharacter.Exp),
                new Vector2(202, 156) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "HP:" + ActiveCharacter.Hp + "/" + ActiveCharacter.MaxHp,
                new Vector2(90, 184) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "MP:" + ActiveCharacter.Mp + "/" + ActiveCharacter.MaxMp,
                new Vector2(90, 206) + offset, Color.White);

            spriteBatch.DrawString(GameMain.font, "Strength:", new Vector2(90, 236) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Strength.ToString(), new Vector2(274, 236) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Vitality:", new Vector2(90, 258) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Vitality.ToString(), new Vector2(274, 258) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Accuracy:", new Vector2(90, 280) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Accuracy.ToString(), new Vector2(274, 280) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Agility:", new Vector2(90, 302) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Agility.ToString(), new Vector2(274, 302) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Intelligence:", new Vector2(90, 324) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Intelligence.ToString(), new Vector2(274, 324) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Wisdom:", new Vector2(90, 346) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Wisdom.ToString(), new Vector2(274, 346) + offset, Color.White);

            spriteBatch.DrawString(GameMain.font, "Stat Points Remaining", new Vector2(90, 368) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.StatRemaining.ToString(),
                new Vector2(338 - GameMain.font.MeasureString(ActiveCharacter.StatRemaining.ToString()).X, 398) + offset, Color.White);

            spriteBatch.DrawString(GameMain.font, "Attack:", new Vector2(348, 236) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getAttackMultiplier() + "x", new Vector2(474, 236) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getBaseDamage(Character.ePhysicalDamageOption.BOTH).ToString(),
                new Vector2(514, 236) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Hit %:", new Vector2(348, 258) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getHitPourc(Character.ePhysicalDamageOption.BOTH) + "%", new Vector2(514, 258) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Defense:", new Vector2(348, 280) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getDefenseMultiplier() + "x", new Vector2(474, 280) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getDefenseDamage().ToString(),
                new Vector2(514, 280) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Evade:", new Vector2(348, 302) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getEvadePourc() + "%", new Vector2(514, 302) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "M.Attack:", new Vector2(348, 324) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getMagicAttackMultiplier(Character.eMagicalDamageOption.NONE) + "x", new Vector2(474, 324) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getMagicBaseDamage(Character.eMagicalDamageOption.NONE, 1).ToString(),
                new Vector2(514, 324) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "M.Hit %:", new Vector2(348, 346) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getMagicHitPourc(Character.eMagicalDamageOption.NONE, 80) + "%", new Vector2(514, 346) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "M.Defense:", new Vector2(348, 368) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getMagicDefenseMultiplier() + "x", new Vector2(474, 368) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getMagicDefenseDamage().ToString(),
                new Vector2(514, 368) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, "M.Evade:", new Vector2(348, 390) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.getMagicEvadePourc() + "%", new Vector2(514, 390) + offset, Color.White);

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
                    spriteBatch.DrawString(GameMain.font, "-", new Vector2(262, 236) + offset, Color.White);
                if (_RealActiveCharacter.Vitality < ActiveCharacter.Vitality)
                    spriteBatch.DrawString(GameMain.font, "-", new Vector2(262, 258) + offset, Color.White);
                if (_RealActiveCharacter.Accuracy < ActiveCharacter.Accuracy)
                    spriteBatch.DrawString(GameMain.font, "-", new Vector2(262, 280) + offset, Color.White);
                if (_RealActiveCharacter.Agility < ActiveCharacter.Agility)
                    spriteBatch.DrawString(GameMain.font, "-", new Vector2(262, 302) + offset, Color.White);
                if (_RealActiveCharacter.Intelligence < ActiveCharacter.Intelligence)
                    spriteBatch.DrawString(GameMain.font, "-", new Vector2(262, 324) + offset, Color.White);
                if (_RealActiveCharacter.Wisdom < ActiveCharacter.Wisdom)
                    spriteBatch.DrawString(GameMain.font, "-", new Vector2(262, 346) + offset, Color.White);

                // Hide the plus if we can't add anymore stats.
                if (ActiveCharacter.StatRemaining > 0)
                {
                    spriteBatch.DrawString(GameMain.font, "+", new Vector2(312, 236) + offset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "+", new Vector2(312, 258) + offset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "+", new Vector2(312, 280) + offset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "+", new Vector2(312, 302) + offset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "+", new Vector2(312, 324) + offset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "+", new Vector2(312, 346) + offset, Color.White);
                }

                spriteBatch.DrawString(GameMain.font, "Accept", new Vector2(110, 398) + offset, Color.White);
                spriteBatch.DrawString(GameMain.font, "Reset", new Vector2(208, 398) + offset, Color.White);
            }

            GameMain.ScissorReset();
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
