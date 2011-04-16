using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngine.Classes.Battles;
using FantasyEngineData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FantasyEngine.Classes.Menus;

namespace FantasyEngine.Classes
{
    public class Overworld : Scene
    {
        private readonly string[] MENU_COMMANDS = { "Character", "Inventory", "Quit game" };

        private bool _ShowPosition = false;
        private Command _Menu;

        public Overworld(Game game)
            : base(game)
        {
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Player.GamePlayer.Map.BackgroundMusic);

            _Menu = new Command(Game, 180, MENU_COMMANDS);
            _Menu.ChangeOffset(Game.GraphicsDevice.Viewport.Width - _Menu.Rectangle.Width, 0);
            _Menu.Enabled = false;
            _Menu.Visible = false;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 offset = new Vector2(-GameMain.OldCameraMatrix.Translation.X, -GameMain.OldCameraMatrix.Translation.Y);

            Player.GamePlayer.Map.Draw(gameTime);
            Player.GamePlayer.Hero.Draw(gameTime);

            _Menu.Offset = offset;
            _Menu.Draw(gameTime);

            if (_ShowPosition)
            {
                Rectangle heroRect = Player.GamePlayer.Hero.getRectangle();
                Vector2 hero1 = new Vector2(heroRect.Left, heroRect.Top);
                Vector2 hero2 = new Vector2(heroRect.Right, heroRect.Bottom);
                Point tile1 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(hero1);
                Point tile2 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(hero2);
                GameMain.spriteBatch.DrawString(GameMain.font,
                    "X:" + tile1.X + ", Y:" + tile1.Y + " (" + hero1 + ")" + Environment.NewLine +
                    "X:" + tile2.X + ", Y:" + tile2.Y + " (" + hero2 + ")",
                   new Vector2(-GameMain.OldCameraMatrix.Translation.X, -GameMain.OldCameraMatrix.Translation.Y) + new Vector2(8, 16), Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (UpdateMenu(gameTime))
                return;

            Player.GamePlayer.Hero.Update(gameTime);

            if (Input.keyStateDown.IsKeyDown(Keys.P))
                _ShowPosition = !_ShowPosition;

            int step = 2;

            #region Update Direction
            if (Input.keyStateHeld.IsKeyDown(Keys.Up)
                        || Input.keyStateHeld.IsKeyDown(Keys.Down)
                        || Input.keyStateHeld.IsKeyDown(Keys.Left)
                        || Input.keyStateHeld.IsKeyDown(Keys.Right))
            {
                Rectangle heroRect = Player.GamePlayer.Hero.getRectangle();

                Vector2 hero1 = Vector2.Zero;
                Vector2 hero2 = Vector2.Zero;

                Vector2 newOffset = Vector2.Zero;

                if (Input.keyStateHeld.IsKeyDown(Keys.Up))
                {
                    newOffset = new Vector2(0, -step);

                    hero1 = new Vector2(heroRect.Left, heroRect.Top);
                    hero2 = new Vector2(heroRect.Right, heroRect.Top);
                }

                if (Input.keyStateHeld.IsKeyDown(Keys.Down))
                {
                    newOffset = new Vector2(0, step);

                    hero1 = new Vector2(heroRect.Left, heroRect.Bottom);
                    hero2 = new Vector2(heroRect.Right, heroRect.Bottom);
                }

                if (Input.keyStateHeld.IsKeyDown(Keys.Left))
                {
                    newOffset = new Vector2(-step, 0);

                    hero1 = new Vector2(heroRect.Left, heroRect.Top);
                    hero2 = new Vector2(heroRect.Left, heroRect.Bottom);
                }

                if (Input.keyStateHeld.IsKeyDown(Keys.Right))
                {
                    newOffset = new Vector2(step, 0);

                    hero1 = new Vector2(heroRect.Right, heroRect.Top);
                    hero2 = new Vector2(heroRect.Right, heroRect.Bottom);
                }

                // Clamp the camera so it never leaves the visible area of the map.
                Vector2 cameraMax = new Vector2(
                    Player.GamePlayer.Map.MapData.Width * Player.GamePlayer.Map.MapData.TileWidth - 1,
                    Player.GamePlayer.Map.MapData.Height * Player.GamePlayer.Map.MapData.TileHeight - 1);
                newOffset = Vector2.Clamp(hero1 + newOffset, Vector2.Zero, cameraMax) - hero1;
                newOffset = Vector2.Clamp(hero2 + newOffset, Vector2.Zero, cameraMax) - hero2;

                TiledLib.TileLayer layer = (TiledLib.TileLayer)Player.GamePlayer.Map.MapData.GetLayer("Collision");
                Point tile1 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(hero1 + newOffset);
                Point tile2 = Player.GamePlayer.Map.MapData.WorldPointToTileIndex(hero2 + newOffset);
                if (layer.Tiles[tile1.X, tile1.Y] == null
                    && layer.Tiles[tile2.X, tile2.Y] == null)
                {
                    Player.GamePlayer.Map.Offset += newOffset;
                    Player.GamePlayer.Hero.Position += newOffset;
                }

                return;
            } // if (Direction held)
            #endregion Update Direction

            if (Input.keyStateDown.IsKeyDown(Keys.C))
            {
                AddSubScene(new CharacterScene(Game, Player.GamePlayer.Actors[0]));
            }

            if (Input.keyStateDown.IsKeyDown(Keys.I))
            {
                AddSubScene(new InventoryScene(Game));
            }

            if (Input.keyStateDown.IsKeyDown(Keys.B))
            {
                MapObject.Encounter mob = Player.GamePlayer.Map.Encounters[0];
                Battle battle = new Battle(Game, "battleback_grass");
                battle._Enemies[0] = new Battler(Game, mob.Monster, mob.Level);
                battle._Enemies[0].Name = battle._Enemies[0].CurrentJob.JobName + "1";
                battle.StartPhase1();
                Scene.ChangeMainScene(battle);
            }

            if (Input.keyStateDown.IsKeyDown(Keys.N))
            {
                int i = 0;
            }
        }

        /// <summary>
        /// Update the Menu window.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns>If it catches the update</returns>
        private bool UpdateMenu(GameTime gameTime)
        {
            _Menu.Update(gameTime);

            if (Input.keyStateDown.IsKeyDown(Keys.Escape))
            {
                _Menu.Enabled = !_Menu.Enabled;
                _Menu.Visible = !_Menu.Visible;
                return true;
            }

            if (_Menu.Enabled)
                if (Input.keyStateDown.IsKeyDown(Keys.Enter))
                {
                    switch (_Menu.CursorPosition)
                    {
                        case 0:
                            AddSubScene(new CharacterScene(Game, Player.GamePlayer.Actors[0]));
                            break;

                        case 1:
                            AddSubScene(new InventoryScene(Game));
                            break;

                        case 2:
                            Game.Exit();
                            break;
                    }
                }

            return _Menu.Enabled;
        }
    }
}
