using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngine.Classes.Battles;
using FantasyEngineData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FantasyEngine.Classes
{
    public class Overworld : Scene
    {
        private bool showPosition = false;

        public Overworld(Game game)
            : base(game)
        {
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Player.GamePlayer.Map.BackgroundMusic);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Player.GamePlayer.Map.Draw(gameTime);
            Player.GamePlayer.Hero.Draw(gameTime);

            if (showPosition)
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

            Player.GamePlayer.Hero.Update(gameTime);

            if (Input.keyStateDown.IsKeyDown(Keys.P))
                showPosition = !showPosition;

            int step = 2;

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

            if (Input.keyStateDown.IsKeyDown(Keys.B))
            {
                MapObject.Encounter mob = Player.GamePlayer.Map.Encounters[0];
                Battle battle = new Battle(Game, "battleback_grass");
                //battle._Enemies[0] = new Battler(Game, @"Monsters\goblin");
                //battle._Enemies[0].mapJobs[0] = Game.Content.Load<Job>(@"Monsters\Goblin");//new Job("Goblin", 1, 10, 1, 2, 2, 2, 1, 2, 2, new BattleSprite("goblin"));
                battle._Enemies[0] = new Battler(Game, mob.Monster, mob.Level);
                //battle._Enemies[0].mCurrentJob = 0;
                battle._Enemies[0].Name = battle._Enemies[0].CurrentJob.JobName + "1";
                battle.StartPhase1();
                Scene.ChangeMainScene(battle);
            }

            if (Input.keyStateDown.IsKeyDown(Keys.N))
            {
                int i = 0;
            }
        }
    }
}
