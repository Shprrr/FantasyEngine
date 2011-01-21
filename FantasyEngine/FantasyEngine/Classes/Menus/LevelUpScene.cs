using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FantasyEngine.Classes.Menus
{
    public class LevelUpScene : Scene
    {
        private Window _LevelUpWindow;
        /// <summary>
        /// Character who is level uping.  He's the one showed on this screen.
        /// </summary>
        public Character LevelUpingCharacter;

        public LevelUpScene(Game game, Character levelUpingCharacter)
            : base(game)
        {
            _LevelUpWindow = new Window(Game, 160, 68, 320, 344);
            LevelUpingCharacter = levelUpingCharacter;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _LevelUpWindow.Draw(gameTime);
            GameMain.Scissor(_LevelUpWindow.InsideBound);

            spriteBatch.DrawString(GameMain.font, "LEVEL UP !", new Vector2(260, 82), Color.White);
            spriteBatch.DrawString(GameMain.font, LevelUpingCharacter.Name, new Vector2(174, 110), Color.White);

            spriteBatch.DrawString(GameMain.font, "Job:" + LevelUpingCharacter.CurrentJob.JobName,
                new Vector2(174, 132), Color.White);
            spriteBatch.DrawString(GameMain.font, "Level:" + (LevelUpingCharacter.Level - 1) + " -> " + LevelUpingCharacter.Level,
                new Vector2(174, 154), Color.White);
            spriteBatch.DrawString(GameMain.font, "HP:" + LevelUpingCharacter.Hp + "/" + LevelUpingCharacter.MaxHp,
                new Vector2(174, 176), Color.White);
            spriteBatch.DrawString(GameMain.font, "MP:" + LevelUpingCharacter.Mp + "/" + LevelUpingCharacter.MaxMp,
                new Vector2(174, 198), Color.White);

            spriteBatch.DrawString(GameMain.font, "Strenght:", new Vector2(174, 228), Color.White);
            spriteBatch.DrawString(GameMain.font, LevelUpingCharacter.Strenght.ToString(), new Vector2(340, 228), Color.White);
            spriteBatch.DrawString(GameMain.font, "Vitality:", new Vector2(174, 250), Color.White);
            spriteBatch.DrawString(GameMain.font, LevelUpingCharacter.Vitality.ToString(), new Vector2(340, 250), Color.White);
            spriteBatch.DrawString(GameMain.font, "Accuracy:", new Vector2(174, 272), Color.White);
            spriteBatch.DrawString(GameMain.font, LevelUpingCharacter.Accuracy.ToString(), new Vector2(340, 272), Color.White);
            spriteBatch.DrawString(GameMain.font, "Agility:", new Vector2(174, 294), Color.White);
            spriteBatch.DrawString(GameMain.font, LevelUpingCharacter.Agility.ToString(), new Vector2(340, 294), Color.White);
            spriteBatch.DrawString(GameMain.font, "Intelligence:", new Vector2(174, 316), Color.White);
            spriteBatch.DrawString(GameMain.font, LevelUpingCharacter.Intelligence.ToString(), new Vector2(340, 316), Color.White);
            spriteBatch.DrawString(GameMain.font, "Wisdom:", new Vector2(174, 338), Color.White);
            spriteBatch.DrawString(GameMain.font, LevelUpingCharacter.Wisdom.ToString(), new Vector2(340, 338), Color.White);
            spriteBatch.DrawString(GameMain.font, "Stat Points Remaining", new Vector2(174, 368), Color.White);
            spriteBatch.DrawString(GameMain.font, LevelUpingCharacter.CurrentJob.StatRemaining.ToString(), new Vector2(420, 390), Color.White);

            GameMain.ScissorReset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                Scene.RemoveSubScene();
            }
        }
    }
}
