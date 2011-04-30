using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData;
using FantasyEngineData.Items;

namespace FantasyEngine.Classes.Menus
{
    public class EquipScene : Scene
    {
        private Cursor CursorWindow;
        private Window _EquipWindow;
        /// <summary>
        /// Character who is shown.
        /// </summary>
        public Character ActiveCharacter;

        public EquipScene(Game game, Character activeCharacter)
            : base(game)
        {
            CursorWindow = new Cursor(Game, 6, 2);
            _EquipWindow = new Window(Game, 76, 55, 488, 145);
            ActiveCharacter = activeCharacter;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 offset = new Vector2(-GameMain.OldCameraMatrix.Translation.X, -GameMain.OldCameraMatrix.Translation.Y);

            _EquipWindow.Offset = offset;
            _EquipWindow.Draw(gameTime);
            GameMain.Scissor(_EquipWindow.InsideBound);

            spriteBatch.DrawString(GameMain.font, "Equipment", new Vector2(260, 68) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Name, new Vector2(90, 96) + offset, Color.White);

            spriteBatch.DrawString(GameMain.font, ActiveCharacter.RightHand != null ? ActiveCharacter.RightHand.Name : "Right Hand",
                new Vector2(110, 126) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.LeftHand != null ? ActiveCharacter.LeftHand.Name : "Left Hand",
                new Vector2(110, 148) + offset, Color.White);

            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Head != null ? ActiveCharacter.Head.Name : "Head",
                new Vector2(347, 126) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Body != null ? ActiveCharacter.Body.Name : "Body",
                new Vector2(347, 148) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Arms != null ? ActiveCharacter.Arms.Name : "Arms",
                new Vector2(110, 170) + offset, Color.White);
            spriteBatch.DrawString(GameMain.font, ActiveCharacter.Feet != null ? ActiveCharacter.Feet.Name : "Feet",
                new Vector2(347, 170) + offset, Color.White);

            // Draw cursor
            CursorWindow.Position = new Vector2((CursorWindow.CursorIndex % 2 == 0 ? 90 : 327), 126 + CursorWindow.CursorIndex / 2 * 22);
            CursorWindow.Draw(gameTime);

            GameMain.ScissorReset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Input.UpdateInput(gameTime);

            if (!Input.UpdateInput(gameTime))
                return;

            CursorWindow.Update(gameTime);

            if (Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                BaseItem lastItemEquiped = null;
                switch (CursorWindow.CursorIndex)
                {
                    case 0: // Right Hand
                        lastItemEquiped = Player.GamePlayer.Actors[0].RightHand;
                        Player.GamePlayer.Actors[0].RightHand = null;
                        break;

                    case 1: // Head
                        lastItemEquiped = Player.GamePlayer.Actors[0].Head;
                        Player.GamePlayer.Actors[0].Head = null;
                        break;

                    case 2: // Left Hand
                        lastItemEquiped = Player.GamePlayer.Actors[0].LeftHand;
                        Player.GamePlayer.Actors[0].LeftHand = null;
                        break;

                    case 3: // Body
                        lastItemEquiped = Player.GamePlayer.Actors[0].Body;
                        Player.GamePlayer.Actors[0].Body = null;
                        break;

                    case 4: // Arms
                        lastItemEquiped = Player.GamePlayer.Actors[0].Arms;
                        Player.GamePlayer.Actors[0].Arms = null;
                        break;

                    case 5: // Feet
                        lastItemEquiped = Player.GamePlayer.Actors[0].Feet;
                        Player.GamePlayer.Actors[0].Feet = null;
                        break;
                }

                if (lastItemEquiped != null)
                    Player.GamePlayer.Inventory.Add(lastItemEquiped);
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Escape)
                || Input.keyStateDown.IsKeyDown(Keys.Back))
            {
                //if (_CursorSortBeginIndex >= 0)
                // {
                //     _CursorIndex = _CursorSortBeginIndex;
                //     _CursorSortBeginIndex = -1;
                //     _UseCommand.Enabled = true;
                // }
                // else
                Scene.RemoveSubScene();
            }

            if (Input.keyStateDown.IsKeyDown(Keys.E))
            {
                Scene.RemoveSubScene();
            }
        }
    }
}
