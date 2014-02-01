using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TiledLib;
using FantasyEngine.Classes.Menus;
using FantasyEngineData.Entities;

namespace FantasyEngine.Classes.Overworld
{
    public class Map : DrawableGameComponent
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

        public const string LAYER_NAME_COLLISION = "Collision";
        public const string LAYER_NAME_NPC = "NPC";
        public const string LAYER_NAME_EVENT = "Event";

        public const string PROP_NAME_NPC_SPRITE = "sprite";
        public const string PROP_NAME_NPC_TILESIZE = "tileSize";
        public const string PROP_NAME_NPC_DIRECTION = "direction";
        public const string PROP_NAME_NPC_REGAINDIRECTION = "regainDirection";
        public const string PROP_NAME_NPC_TALK = "talk";
        public const string PROP_NAME_NPC_MOVE = "move";

        public const string TYPE_NAME_EVENT_TELEPORT = "teleport";
        public const string PROP_NAME_EVENT_TELEPORT = "teleport";
        public const string PROP_NAME_EVENT_TX = "tx";
        public const string PROP_NAME_EVENT_TY = "ty";

        public const string TYPE_NAME_EVENT_ONENTER = "onEnter";
        public const string PROP_NAME_EVENT_ONENTER = "onEnter";

        private TiledLib.Map _MapData;
        private List<NPC> _NPCs = new List<NPC>();
        private List<Event> _Events = new List<Event>();
        private List<Encounter> _Encounters = new List<Encounter>();

        /// <summary>
        /// Offset in pixel of the camera on the map.
        /// </summary>
        public Vector2 Offset { get; set; }

        /// <summary>
        /// Data of the map.
        /// </summary>
        public TiledLib.Map MapData { get { return _MapData; } }

        /// <summary>
        /// Background song of the map.
        /// </summary>
        public Song BackgroundMusic;

        /// <summary>
        /// List all non-playable characters in the map.
        /// </summary>
        public List<NPC> NPCs { get { return _NPCs; } }

        /// <summary>
        /// List all events in the map.
        /// </summary>
        public List<Event> Events { get { return _Events; } }

        /// <summary>
        /// List of all monsters that can be encountered.
        /// </summary>
        public List<Encounter> Encounters { get { return _Encounters; } }

        private void Init(string mapName)
        {
            _MapData = Game.Content.Load<TiledLib.Map>(@"Maps\" + mapName);

            bool hasCollision = false, hasNPC = false, hasEvent = false;
            foreach (var layer in _MapData.Layers)
            {
                hasCollision |= layer.Name == LAYER_NAME_COLLISION;
                hasNPC |= layer.Name == LAYER_NAME_NPC;
                hasEvent |= layer.Name == LAYER_NAME_EVENT;
            }

            if (hasCollision)
                _MapData.GetLayer(LAYER_NAME_COLLISION).Visible = false;

            if (hasNPC)
                foreach (MapObject obj in ((MapObjectLayer)_MapData.GetLayer(LAYER_NAME_NPC)).Objects)
                {
                    NPC npc;
                    Rectangle tileSize = Rectangle.Empty;

                    if (obj.Properties[PROP_NAME_NPC_TILESIZE] != null)
                    {
                        string[] tileSizeString = obj.Properties[PROP_NAME_NPC_TILESIZE].RawValue.Split(' ');
                        tileSize = new Rectangle(int.Parse(tileSizeString[0]), int.Parse(tileSizeString[1]), int.Parse(tileSizeString[2]), int.Parse(tileSizeString[3]));
                    }

                    if (obj.Properties[PROP_NAME_NPC_DIRECTION] != null)
                        if (obj.Properties[PROP_NAME_NPC_REGAINDIRECTION] != null)
                            npc = new NPC(Game, obj.Name,
                                @"Overworld\" + obj.Properties[PROP_NAME_NPC_SPRITE].RawValue, tileSize,
                                new Vector2(obj.Bounds.X, obj.Bounds.Y),
                                (Sprite.eDirection)Enum.Parse(typeof(Sprite.eDirection), obj.Properties[PROP_NAME_NPC_DIRECTION].RawValue),
                                bool.Parse(obj.Properties[PROP_NAME_NPC_REGAINDIRECTION].RawValue));
                        else
                            npc = new NPC(Game, obj.Name,
                                @"Overworld\" + obj.Properties[PROP_NAME_NPC_SPRITE].RawValue, tileSize,
                                new Vector2(obj.Bounds.X, obj.Bounds.Y),
                                (Sprite.eDirection)Enum.Parse(typeof(Sprite.eDirection), obj.Properties[PROP_NAME_NPC_DIRECTION].RawValue));
                    else
                        npc = new NPC(Game, obj.Name, @"Overworld\" + obj.Properties[PROP_NAME_NPC_SPRITE].RawValue, tileSize, new Vector2(obj.Bounds.X, obj.Bounds.Y));

                    if (obj.Properties[PROP_NAME_NPC_TALK] != null)
                        npc.Talking += (NPC.TalkingHandler)Delegate.CreateDelegate(typeof(NPC.TalkingHandler),
                            Type.GetType("FantasyEngine.Classes.Overworld.Maps." + mapName.Replace(" ", "")).GetMethod(obj.Properties[PROP_NAME_NPC_TALK].RawValue, BindingFlags.Public | BindingFlags.Static));

                    if (obj.Properties[PROP_NAME_NPC_MOVE] != null)
                        npc.Moving += (NPC.MovingHandler)Delegate.CreateDelegate(typeof(NPC.MovingHandler),
                            Type.GetType("FantasyEngine.Classes.Overworld.Maps." + mapName.Replace(" ", "")).GetMethod(obj.Properties[PROP_NAME_NPC_MOVE].RawValue, BindingFlags.Public | BindingFlags.Static));

                    NPCs.Add(npc);
                }

            if (hasEvent)
                foreach (MapObject obj in ((MapObjectLayer)_MapData.GetLayer(LAYER_NAME_EVENT)).Objects)
                {
                    Event eve = null;
                    switch (obj.Type)
                    {
                        case TYPE_NAME_EVENT_TELEPORT:
                            eve = new Event(Game, obj.Bounds, obj.Properties[PROP_NAME_EVENT_TELEPORT].RawValue, new Vector2(float.Parse(obj.Properties[PROP_NAME_EVENT_TX].RawValue), float.Parse(obj.Properties[PROP_NAME_EVENT_TY].RawValue)));
                            break;

                        case TYPE_NAME_EVENT_ONENTER:
                            eve = new Event(Game, obj.Bounds);
                            eve.OnEnter += (Event.OnEnterHandler)Delegate.CreateDelegate(typeof(Event.OnEnterHandler),
                                Type.GetType("FantasyEngine.Classes.Overworld.Maps." + mapName.Replace(" ", "")).GetMethod(obj.Properties[PROP_NAME_EVENT_ONENTER].RawValue, BindingFlags.Public | BindingFlags.Static));
                            break;
                    }

                    if (eve != null)
                        Events.Add(eve);
                }
        }

        public Map(Game game, string mapName, Vector2 offset)
            : base(game)
        {
            Initialize();
            Init(mapName);
            Offset = offset;

            switch (mapName)
            {
                case "Village":
                    BackgroundMusic = Game.Content.Load<Song>(@"Audios\Musics\Village");
                    Encounters.Add(new Encounter(Game.Content.Load<Monster>(@"Monsters\Goblin"), 1, 100));

                    NPC boy1 = new NPC(Game, "test", @"Overworld\npc", new Rectangle(0, 16, 128, 16), new Vector2(256, 64));
                    boy1.Talking += new NPC.TalkingHandler(boy1_Talking);
                    NPCs.Add(boy1);
                    break;

                case "Tranquility Plain":
                    BackgroundMusic = Game.Content.Load<Song>(@"Audios\Musics\Village");
                    Encounters.Add(new Encounter(Game.Content.Load<Monster>(@"Monsters\Goblin"), 2, 100));
                    break;
            }
        }

        void boy1_Talking(EventArgs e, NPC npc)
        {
            Thread thr = new Thread(
                delegate(object Data)
                {
                    npc.Talk(npc.Name + ": Test to position " + npc.Position);
                });
            thr.Start(npc);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // create a matrix for the camera to offset everything we draw, the map and our objects. since the
            // camera coordinates are where the camera is, we offset everything by the negative of that to simulate
            // a camera moving. we also cast to integers to avoid filtering artifacts
            GameMain.spriteBatch.cameraMatrix = Matrix.CreateTranslation((int)-Offset.X, (int)-Offset.Y, 0);

            Rectangle visibleArea = new Rectangle(
                (int)Offset.X,
                (int)Offset.Y,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

            _MapData.Draw(GameMain.spriteBatch, visibleArea);

            foreach (NPC npc in NPCs)
            {
                npc.Draw(gameTime);
            }
        }

        public void DrawGUI(GameTime gameTime)
        {
            foreach (NPC npc in NPCs)
            {
                npc.DrawGUI(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Events for each NPC.
            foreach (NPC npc in NPCs)
            {
                npc.Update(gameTime);
            }
        }
    }
}
