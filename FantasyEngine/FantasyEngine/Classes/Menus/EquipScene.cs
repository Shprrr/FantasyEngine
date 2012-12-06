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

        private Cursor _CursorWindow;
        private Window _EquipWindow;
        /// <summary>
        /// Character who is shown.
        /// </summary>
        public Character CurrentActor;

        public EquipScene(Game game, Character activeCharacter)
            : base(game)
        {
            _CursorWindow = new Cursor(Game, 6, 2);
            _EquipWindow = new Window(Game, 76, 55, 488, 145);
            CurrentActor = activeCharacter;
        }

        public override void DrawGUI(GameTime gameTime)
        {
            base.DrawGUI(gameTime);

            _EquipWindow.Offset = spriteBatchGUI.CameraOffset;
            _EquipWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_EquipWindow.InsideBound);

            spriteBatchGUI.DrawString(GameMain.font, "Equipment", new Vector2(260, 68) + spriteBatchGUI.CameraOffset, Color.White);
            spriteBatchGUI.DrawString(GameMain.font, CurrentActor.Name, new Vector2(90, 96) + spriteBatchGUI.CameraOffset, Color.White);

            if (CurrentActor.RightHand != null)
                spriteBatchGUI.DrawString(GameMain.font, CurrentActor.RightHand.Name,
                    new Vector2(110, 126) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Right Hand",
                    new Vector2(110, 126) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (CurrentActor.LeftHand != null)
                spriteBatchGUI.DrawString(GameMain.font, CurrentActor.LeftHand.Name,
                    new Vector2(110, 148) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Left Hand",
                    new Vector2(110, 148) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (CurrentActor.Head != null)
                spriteBatchGUI.DrawString(GameMain.font, CurrentActor.Head.Name,
                    new Vector2(347, 126) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Head",
                    new Vector2(347, 126) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (CurrentActor.Body != null)
                spriteBatchGUI.DrawString(GameMain.font, CurrentActor.Body.Name,
                    new Vector2(347, 148) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Body",
                    new Vector2(347, 148) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (CurrentActor.Arms != null)
                spriteBatchGUI.DrawString(GameMain.font, CurrentActor.Arms.Name,
                    new Vector2(110, 170) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Arms",
                    new Vector2(110, 170) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            if (CurrentActor.Feet != null)
                spriteBatchGUI.DrawString(GameMain.font, CurrentActor.Feet.Name,
                    new Vector2(347, 170) + spriteBatchGUI.CameraOffset, COLOR_NORMAL);
            else
                spriteBatchGUI.DrawString(GameMain.font, "Feet",
                    new Vector2(347, 170) + spriteBatchGUI.CameraOffset, COLOR_UNEQUIP);

            // Draw cursor
            _CursorWindow.Position = new Vector2((_CursorWindow.CursorIndex % 2 == 0 ? 90 : 327), 126 + _CursorWindow.CursorIndex / 2 * 22);
            _CursorWindow.Draw(gameTime);

            spriteBatchGUI.ScissorReset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Input.UpdateInput(gameTime))
                return;

            _CursorWindow.Update(gameTime);

            if (Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                switch (_CursorWindow.CursorIndex)
                {
                    case 0: // Right Hand
                        CurrentActor.Unequip(Character.eEquipSlot.RightHand, Player.GamePlayer.Inventory);
                        break;

                    case 1: // Head
                        CurrentActor.Unequip(Character.eEquipSlot.Head, Player.GamePlayer.Inventory);
                        break;

                    case 2: // Left Hand
                        CurrentActor.Unequip(Character.eEquipSlot.LeftHand, Player.GamePlayer.Inventory);
                        break;

                    case 3: // Body
                        CurrentActor.Unequip(Character.eEquipSlot.Body, Player.GamePlayer.Inventory);
                        break;

                    case 4: // Arms
                        CurrentActor.Unequip(Character.eEquipSlot.Arms, Player.GamePlayer.Inventory);
                        break;

                    case 5: // Feet
                        CurrentActor.Unequip(Character.eEquipSlot.Feet, Player.GamePlayer.Inventory);
                        break;
                }
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Escape)
                || Input.keyStateDown.IsKeyDown(Keys.Back))
            {
                Scene.RemoveSubScene();
            }

            if (Input.keyStateDown.IsKeyDown(Keys.E))
            {
                Scene.RemoveSubScene();
            }
        }
    }
}
