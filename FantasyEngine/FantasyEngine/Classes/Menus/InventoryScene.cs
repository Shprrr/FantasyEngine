using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData.Items;

namespace FantasyEngine.Classes.Menus
{
    public class InventoryScene : Scene
    {
        private readonly string[] USE_COMMANDS = { "Use", "Sort", "Discard" };
        private readonly string[] EQUIP_COMMANDS = { "Equip", "Sort", "Discard" };
        private readonly string[] ITEM_EQUIP_COMMANDS = { "Right Hand", "Head", "Left Hand", "Body", "Arms", "Feet" };

        private Cursor CursorWindow;
        private int _CursorSortBeginIndex = -1;
        private Window _InventoryWindow;
        private Command _UseCommand;
        private Command _EquipCommand;

        public InventoryScene(Game game)
            : base(game)
        {
            CursorWindow = new Cursor(Game, Player.GamePlayer.Inventory.Items.Count, 2);
            _InventoryWindow = new Window(Game, 76, 55, 488, 370);

            _UseCommand = new Command(Game, 472, USE_COMMANDS, 3);
            _UseCommand.ChangeOffset(84, _InventoryWindow.Rectangle.Bottom - _UseCommand.Rectangle.Height - Window.Tileset.TileHeight);
            _UseCommand.Enabled = false;
            _UseCommand.Visible = false;

            _EquipCommand = new Command(Game, 472, ITEM_EQUIP_COMMANDS, 2);
            _EquipCommand.ChangeOffset(84, _InventoryWindow.Rectangle.Bottom - _UseCommand.Rectangle.Height - Window.Tileset.TileHeight);
            _EquipCommand.Enabled = false;
            _EquipCommand.Visible = false;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 offset = new Vector2(-GameMain.OldCameraMatrix.Translation.X, -GameMain.OldCameraMatrix.Translation.Y);

            _InventoryWindow.Offset = offset;
            _InventoryWindow.Draw(gameTime);
            GameMain.Scissor(_InventoryWindow.InsideBound);

            spriteBatch.DrawString(GameMain.font, "Inventory", new Vector2(260, 68) + offset, Color.White);

            // Draw list of items
            int i = 0;
            foreach (var item in Player.GamePlayer.Inventory.Items)
            {
                spriteBatch.DrawString(GameMain.font, item.Item.Name,
                    new Vector2((i % 2 == 0 ? 110 : 347), 96 + i / 2 * 16) + offset, Color.White);
                spriteBatch.DrawString(GameMain.font, item.Number.ToString(),
                    new Vector2((i % 2 == 0 ? 277 : 514), 96 + i / 2 * 16) + offset, Color.White);
                i++;
            }

            // Draw Gold
            spriteBatch.DrawString(GameMain.font, "Gold:" + Player.GamePlayer.Inventory.Gold, new Vector2(110, 390) + offset, Color.White);

            // Draw cursor
            CursorWindow.Position = new Vector2((CursorWindow.CursorIndex % 2 == 0 ? 90 : 327), 96 + CursorWindow.CursorIndex / 2 * 16);
            CursorWindow.Draw(gameTime);

            if (_CursorSortBeginIndex >= 0)
                Cursor.DrawShadow(gameTime, new Vector2((_CursorSortBeginIndex % 2 == 0 ? 90 : 327), 96 + _CursorSortBeginIndex / 2 * 16));

            GameMain.ScissorReset();

            _UseCommand.Offset = offset;
            _UseCommand.Draw(gameTime);

            _EquipCommand.Offset = offset;
            _EquipCommand.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _UseCommand.Update(gameTime);

            _EquipCommand.Update(gameTime);

            if (!Input.UpdateInput(gameTime))
                return;

            CursorWindow.ItemMax = Player.GamePlayer.Inventory.Items.Count;

            // Command actif ou on est en train de faire un tri
            if (!_UseCommand.Enabled && !_EquipCommand.Enabled)
            {
                CursorWindow.Update(gameTime);
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                BaseItem item = Player.GamePlayer.Inventory.Items[CursorWindow.CursorIndex].Item;
                if (_UseCommand.Enabled)
                    switch (_UseCommand.CursorPosition)
                    {
                        case 0: // Use/Equip
                            if (item is Item)
                            {
                                // Use
                            }
                            else
                            {
                                // Equip
                                _EquipCommand.Enabled = true;
                                _EquipCommand.Visible = true;
                            }
                            _UseCommand.Enabled = false;
                            break;

                        case 1: // Sort
                            _CursorSortBeginIndex = CursorWindow.CursorIndex;
                            _UseCommand.Enabled = false;
                            break;

                        case 2: // Discard
                            Player.GamePlayer.Inventory.Drop(item);
                            _UseCommand.CursorPosition = 0;
                            _UseCommand.Enabled = false;
                            _UseCommand.Visible = false;
                            break;
                    }
                else if (_CursorSortBeginIndex >= 0)
                {
                    Player.GamePlayer.Inventory.Sort(Player.GamePlayer.Inventory.Items[_CursorSortBeginIndex].Item, item);
                    _CursorSortBeginIndex = -1;
                    _UseCommand.Enabled = false;
                    _UseCommand.Visible = false;
                }
                else if (_EquipCommand.Enabled)
                {
                    BaseItem lastItemEquiped = null;
                    switch (_EquipCommand.CursorPosition)
                    {
                        case 0: // Right Hand
                            lastItemEquiped = Player.GamePlayer.Actors[0].RightHand;
                            Player.GamePlayer.Actors[0].RightHand = item;
                            break;

                        case 1: // Head
                            if (item is Armor)
                            {
                                lastItemEquiped = Player.GamePlayer.Actors[0].Head;
                                Player.GamePlayer.Actors[0].Head = (Armor)item;
                            }
                            break;

                        case 2: // Left Hand
                            lastItemEquiped = Player.GamePlayer.Actors[0].LeftHand;
                            Player.GamePlayer.Actors[0].LeftHand = item;
                            break;

                        case 3: // Body
                            if (item is Armor)
                            {
                                lastItemEquiped = Player.GamePlayer.Actors[0].Body;
                                Player.GamePlayer.Actors[0].Body = (Armor)item;
                            }
                            break;

                        case 4: // Arms
                            if (item is Armor)
                            {
                                lastItemEquiped = Player.GamePlayer.Actors[0].Arms;
                                Player.GamePlayer.Actors[0].Arms = (Armor)item;
                            }
                            break;

                        case 5: // Feet
                            if (item is Armor)
                            {
                                lastItemEquiped = Player.GamePlayer.Actors[0].Feet;
                                Player.GamePlayer.Actors[0].Feet = (Armor)item;
                            }
                            break;
                    }

                    if (item.IsEquiped)
                    {
                        Player.GamePlayer.Inventory.Drop(item);
                        if (lastItemEquiped != null)
                            Player.GamePlayer.Inventory.Add(lastItemEquiped);

                        _EquipCommand.Enabled = false;
                        _EquipCommand.Visible = false;
                        _UseCommand.Visible = false;
                    }
                }
                else
                {
                    _UseCommand.Choices = item is Item ? USE_COMMANDS : EQUIP_COMMANDS;
                    _UseCommand.Enabled = true;
                    _UseCommand.Visible = true;
                }
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Escape)
                || Input.keyStateDown.IsKeyDown(Keys.Back))
            {
                if (_EquipCommand.Enabled)
                {
                    _EquipCommand.Enabled = false;
                    _EquipCommand.Visible = false;
                    _UseCommand.Enabled = true;
                    _UseCommand.Visible = true;
                }
                else if (_UseCommand.Enabled)
                {
                    _UseCommand.Enabled = false;
                    _UseCommand.Visible = false;
                }
                else if (_CursorSortBeginIndex >= 0)
                {
                    // Annule un tri
                    CursorWindow.CursorIndex = _CursorSortBeginIndex;
                    _CursorSortBeginIndex = -1;
                    _UseCommand.Enabled = true;
                }
                else
                    Scene.RemoveSubScene();
            }

            if (Input.keyStateDown.IsKeyDown(Keys.I))
            {
                Scene.RemoveSubScene();
            }
        }
    }
}
