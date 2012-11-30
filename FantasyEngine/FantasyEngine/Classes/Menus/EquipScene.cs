using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;

namespace FantasyEngine.Classes.Menus
{
    public class EquipScene : Scene
    {
        private readonly Color COLOR_NORMAL = Color.White;
        private readonly Color COLOR_UNEQUIP = Color.Gray;

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

        public override void DrawGUI(GameTime gameTime)
        {
            base.DrawGUI(gameTime);

            _EquipWindow.Offset = spriteBatchGUI.CameraOffset;
            _EquipWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_EquipWindow.InsideBound);

            spriteBatchGUI.DrawString(GameMain.font, "Equipment", new Vector2(260, 68) + spriteBatchGUI.CameraOffset, Color.White);
            spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Name, new Vector2(90, 96) + spriteBatchGUI.CameraOffset, Color.White);

            if (ActiveCharacter.RightHand != null)
                spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.RightHand.Name,
                    new Vector2(110, 126) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Right Hand",
                    new Vector2(110, 126) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (ActiveCharacter.LeftHand != null)
                spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.LeftHand.Name,
                    new Vector2(110, 148) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Left Hand",
                    new Vector2(110, 148) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (ActiveCharacter.Head != null)
                spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Head.Name,
                    new Vector2(347, 126) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Head",
                    new Vector2(347, 126) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (ActiveCharacter.Body != null)
                spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Body.Name,
                    new Vector2(347, 148) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Body",
                    new Vector2(347, 148) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (ActiveCharacter.Arms != null)
                spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Arms.Name,
                    new Vector2(110, 170) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Arms",
                    new Vector2(110, 170) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (ActiveCharacter.Feet != null)
                spriteBatchGUI.DrawString(GameMain.font, ActiveCharacter.Feet.Name,
                    new Vector2(347, 170) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Feet",
                    new Vector2(347, 170) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            // Draw cursor
            CursorWindow.Position = new Vector2((CursorWindow.CursorIndex % 2 == 0 ? 90 : 327), 126 + CursorWindow.CursorIndex / 2 * 22);
            CursorWindow.Draw(gameTime);

            spriteBatchGUI.ScissorReset();
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
