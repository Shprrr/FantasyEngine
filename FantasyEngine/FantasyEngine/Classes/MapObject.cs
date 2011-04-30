using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TiledLib;
using FantasyEngineData;

namespace FantasyEngine.Classes
{
    public class MapObject : DrawableGameComponent
    {
        public struct Encounter
        {
            public Monster Monster;
            public int Level;
            public float Chances;

            public Encounter(Monster monster, int level, float chances)
            {
                Monster = monster;
                Level = level;
                Chances = chances;
            }
        }

        /// <summary>
        /// Offset in pixel of the camera on the map.
        /// </summary>
        public Vector2 Offset { get; set; }

        private Map _MapData;
        /// <summary>
        /// Data of the map.
        /// </summary>
        public Map MapData { get { return _MapData; } }

        /// <summary>
        /// Background song of the map.
        /// </summary>
        public Song BackgroundMusic;

        private List<Encounter> _Encounters = new List<Encounter>();
        /// <summary>
        /// List of all monsters that can be encountered.
        /// </summary>
        public List<Encounter> Encounters { get { return _Encounters; } }

        private void Init(string mapName)
        {
            _MapData = Game.Content.Load<Map>(@"Maps\" + mapName);
            _MapData.GetLayer("Collision").Visible = false; // Potential bug if there's no layer Collision.
        }

        public enum eMapNo
        {
            VILLAGE
        }

        public MapObject(Game game, eMapNo mapNo, Vector2 offset)
            : base(game)
        {
            Initialize();
            switch (mapNo)
            {
                case eMapNo.VILLAGE:
                    Init("village");
                    Offset = offset;
                    BackgroundMusic = Game.Content.Load<Song>(@"Audios\Musics\Village");
                    Encounters.Add(new Encounter(Game.Content.Load<Monster>(@"Monsters\Goblin"), 1, 100));
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // create a matrix for the camera to offset everything we draw, the map and our objects. since the
            // camera coordinates are where the camera is, we offset everything by the negative of that to simulate
            // a camera moving. we also cast to integers to avoid filtering artifacts
            GameMain.cameraMatrix = Matrix.CreateTranslation((int)-Offset.X, (int)-Offset.Y, 0);

            Rectangle visibleArea = new Rectangle(
                (int)Offset.X,
                (int)Offset.Y,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

            _MapData.Draw(GameMain.spriteBatch, visibleArea);
        }
    }
}
