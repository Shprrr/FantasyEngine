using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData.Entities;

namespace FantasyEngine.Classes.Menus
{
    public class InnScene : Scene
    {
        private readonly string[] MAIN_COMMANDS = { "Yes", "No" };

        private int _Price;
        private Command _MainCommand;
        private Window _DialogWindow;
        private Window _GoldWindow;

        public InnScene(Game game, int price)
            : base(game)
        {
            _Price = price;

            _DialogWindow = new Window(game, 0, 0, 640, 48);

            _MainCommand = new Command(game, 120, MAIN_COMMANDS);
            _MainCommand.ChangeOffset(640 - _MainCommand.Rectangle.Width, _DialogWindow.Rectangle.Bottom);

            _GoldWindow = new Window(game, 368, 436, 640 - 368, 44);
        }

        public override void DrawGUI(GameTime gameTime)
        {
            base.DrawGUI(gameTime);

            _DialogWindow.Offset = spriteBatchGUI.CameraOffset;
            _DialogWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_DialogWindow.InsideBound);

            spriteBatchGUI.DrawString(GameMain.font, "It will cost " + _Price + " gold to rest. Will you stay ?",
                new Vector2(16, 16) + spriteBatchGUI.CameraOffset, Color.White);

            spriteBatchGUI.ScissorReset();

            _MainCommand.Offset = spriteBatchGUI.CameraOffset;
            _MainCommand.Draw(gameTime);

            DrawGold(gameTime);
        }

        private void DrawGold(GameTime gameTime)
        {
            _GoldWindow.Offset = spriteBatchGUI.CameraOffset;
            _GoldWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_GoldWindow.InsideBound);

            spriteBatchGUI.DrawString(GameMain.font, "Gold:" + Player.GamePlayer.Inventory.Gold.ToString("### ##0").Trim(),
                new Vector2(382, 450) + spriteBatchGUI.CameraOffset, Color.White);

            spriteBatchGUI.ScissorReset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _MainCommand.Update(gameTime);

            if (!Input.UpdateInput(gameTime))
                return;

            if (Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                switch (_MainCommand.CursorPosition)
                {
                    case 0:
                        if (Player.GamePlayer.Inventory.Gold >= _Price)
                        {
                            Player.GamePlayer.Inventory.Gold -= _Price;
                            foreach (Character actor in Player.GamePlayer.Actors)
                            {
                                if (actor == null)
                                    continue;

                                actor.Hp = actor.MaxHp;
                                actor.Mp = actor.MaxMp;
                                actor.Statut = Status.Normal;
                            }
                            Scene.RemoveSubScene();
                        }
                        break;

                    case 1:
                        Scene.RemoveSubScene();
                        break;
                }
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Escape)
                || Input.keyStateDown.IsKeyDown(Keys.Back))
                Scene.RemoveSubScene();
        }
    }
}
