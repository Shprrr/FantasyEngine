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

        private int _CursorIndex = 0;
        private int _CursorSortBeginIndex = -1;
        private Window _InventoryWindow;
        private Command _UseCommand;

        public InventoryScene(Game game)
            : base(game)
        {
            _InventoryWindow = new Window(Game, 76, 55, 488, 370);

            _UseCommand = new Command(Game, 472, USE_COMMANDS, 3);
            _UseCommand.ChangeOffset(84, _InventoryWindow.Rectangle.Bottom - _UseCommand.Rectangle.Height - Window.Tileset.TileHeight);
            _UseCommand.Enabled = false;
            _UseCommand.Visible = false;
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
            spriteBatch.Draw(GameMain.cursor,
                new Vector2((_CursorIndex % 2 == 0 ? 90 : 327), 96 + _CursorIndex / 2 * 16) + offset, Color.White);

            if (_CursorSortBeginIndex >= 0)
                spriteBatch.Draw(GameMain.cursor,
                    new Vector2((_CursorSortBeginIndex % 2 == 0 ? 90 : 327), 96 + _CursorSortBeginIndex / 2 * 16) + offset, Color.White * 0.5f);

            GameMain.ScissorReset();

            _UseCommand.Offset = offset;
            _UseCommand.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _UseCommand.Update(gameTime);

            if (!Input.UpdateInput(gameTime))
                return;

            int item_max = Player.GamePlayer.Inventory.Items.Count;
            int column_max = 2;

            // Command actif ou on est en train de faire un tri
            if (!_UseCommand.Enabled /*|| _CursorSortBeginIndex >= 0*/)
            {
                //TODO: Encapsuler dans une classe Cursor ?
                if (Input.keyStateHeld.IsKeyDown(Keys.Up))
                {
                    if ((column_max == 1 && Input.keyStateDown.IsKeyDown(Keys.Up)) ||
                        _CursorIndex >= column_max)
                    {
                        // Move cursor up
                        _CursorIndex = (_CursorIndex - column_max + item_max) % item_max;
                    }

                    Input.PutDelay(Keys.Up);
                    return;
                }

                if (Input.keyStateHeld.IsKeyDown(Keys.Down))
                {
                    if ((column_max == 1 && Input.keyStateDown.IsKeyDown(Keys.Down)) ||
                        _CursorIndex < item_max - column_max)
                    {
                        // Move cursor down
                        _CursorIndex = (_CursorIndex + column_max) % item_max;
                    }

                    Input.PutDelay(Keys.Down);
                    return;
                }

                if (Input.keyStateHeld.IsKeyDown(Keys.Left))
                {
                    // If column count is 2 or more, and cursor position is more back than 0
                    if (column_max >= 2 && _CursorIndex > 0)
                    {
                        // Move cursor left
                        _CursorIndex -= 1;
                    }

                    Input.PutDelay(Keys.Left);
                    return;
                }

                if (Input.keyStateHeld.IsKeyDown(Keys.Right))
                {
                    // If column count is 2 or more, and cursor position is closer to front
                    // than (item count -1)
                    if (column_max >= 2 && _CursorIndex < item_max - 1)
                    {
                        // Move cursor right
                        _CursorIndex += 1;
                    }

                    Input.PutDelay(Keys.Right);
                    return;
                }
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                BaseItem item = Player.GamePlayer.Inventory.Items[_CursorIndex].Item;
                if (_UseCommand.Enabled)
                    switch (_UseCommand.CursorPosition)
                    {
                        case 0: // Use/Equip
                            _UseCommand.Enabled = false;
                            _UseCommand.Visible = false;
                            break;

                        case 1: // Sort
                            _CursorSortBeginIndex = _CursorIndex;
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
                // Annule un tri
                if (_UseCommand.Enabled)
                {
                    _UseCommand.Enabled = false;
                    _UseCommand.Visible = false;
                }
                else if (_CursorSortBeginIndex >= 0)
                {
                    _CursorIndex = _CursorSortBeginIndex;
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
