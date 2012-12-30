﻿using System;
using System.Collections.Generic;
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

        public enum eMapNo
        {
            VILLAGE,
            TRANQUILITY_PLAIN
        }

        private eMapNo _MapNo;
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

                    if (obj.Properties["tileSize"] != null)
                    {
                        string[] tileSizeString = obj.Properties["tileSize"].RawValue.Split(' ');
                        tileSize = new Rectangle(int.Parse(tileSizeString[0]), int.Parse(tileSizeString[1]), int.Parse(tileSizeString[2]), int.Parse(tileSizeString[3]));
                    }

                    if (obj.Properties["direction"] != null)
                        if (obj.Properties["regainDirection"] != null)
                            npc = new NPC(Game, obj.Name,
                                @"Overworld\" + obj.Properties["sprite"].RawValue, tileSize,
                                new Vector2(obj.Bounds.X, obj.Bounds.Y),
                                (Sprite.eDirection)Enum.Parse(typeof(Sprite.eDirection), obj.Properties["direction"].RawValue),
                                bool.Parse(obj.Properties["regainDirection"].RawValue));
                        else
                            npc = new NPC(Game, obj.Name,
                                @"Overworld\" + obj.Properties["sprite"].RawValue, tileSize,
                                new Vector2(obj.Bounds.X, obj.Bounds.Y),
                                (Sprite.eDirection)Enum.Parse(typeof(Sprite.eDirection), obj.Properties["direction"].RawValue));
                    else
                        npc = new NPC(Game, obj.Name, @"Overworld\" + obj.Properties["sprite"].RawValue, tileSize, new Vector2(obj.Bounds.X, obj.Bounds.Y));
                    npc.Talking += new NPC.TalkingHandler(npc_Talk);
                    npc.Moving += new NPC.MovingHandler(npc_Moving);
                    NPCs.Add(npc);
                }

            if (hasEvent)
                foreach (MapObject obj in ((MapObjectLayer)_MapData.GetLayer(LAYER_NAME_EVENT)).Objects)
                {
                    if (obj.Type == "teleport")
                    {
                        Event eve = new Event(obj.Bounds, obj.Properties["teleport"].RawValue, new Vector2(float.Parse(obj.Properties["tx"].RawValue), float.Parse(obj.Properties["ty"].RawValue)));
                        Events.Add(eve);
                    }
                }
        }

        void npc_Talk(EventArgs e, NPC npc)
        {
            Thread thr;
            switch (_MapNo)
            {
                case eMapNo.VILLAGE:
                    switch (npc.Name)
                    {
                        case "boy1":
                            thr = new Thread(
                                delegate(object Data)
                                {
                                    npc.Talk(npc.Name + ": Loaded succesfully.");
                                });
                            thr.Start(npc);
                            break;

                        case "Claudia":
                            thr = new Thread(
                                delegate(object Data)
                                {
                                    npc.Talk(npc.Name + ": The fountain is pretty.");
                                });
                            thr.Start(npc);
                            break;

                        case "Woman":
                            thr = new Thread(
                                delegate(object Data)
                                {
                                    npc.Talk(npc.Name + ": I don't have time to talk.  RUN!!");
                                });
                            thr.Start(npc);
                            break;

                        case "Griswold":
                            thr = new Thread(
                                delegate(object Data)
                                {
                                    npc.Talk(npc.Name + ": Hello. What can I do for you ?");
                                    List<FantasyEngineData.Items.BaseItem> shopBuy = new List<FantasyEngineData.Items.BaseItem>();
                                    shopBuy.Add(FantasyEngineData.Items.ItemManager.GetItem("Potion"));
                                    shopBuy.Add(FantasyEngineData.Items.ItemManager.GetWeapon("Dagger"));
                                    shopBuy.Add(FantasyEngineData.Items.ItemManager.GetWeapon("Long Sword"));
                                    Scene.AddSubScene(new ShopScene(Game, shopBuy));
                                });
                            thr.Start(npc);
                            break;

                        case "Inn":
                            Scene.AddSubScene(new InnScene(Game, 10));
                            break;

                        case "JobMaster":
                            Scene.AddSubScene(new JobChangeScene(Game));
                            break;

                        case "Steve":
                            thr = new Thread(
                                delegate(object Data)
                                {
                                    npc.Talk(npc.Name + ": People think I talk too much, but I don't think so. I really don't know why they think that.");
                                    npc.Talk(npc.Name + ": It's not like I'm talking and I'm talking and it seems to have no end to what I'm saying and on top of that, there's no purpose to what I'm saying because I only fill space with unnecessary dialog just to take to another screen to see that I continue to talk again and again...");
                                });
                            thr.Start(npc);
                            break;
                    }
                    break;
            }
        }

        void npc_Moving(EventArgs e, NPC npc, GameTime gameTime)
        {
            switch (_MapNo)
            {
                case eMapNo.VILLAGE:
                    switch (npc.Name)
                    {
                        case "Woman":
                            if (npc.Action == NPC.eAction.Stay || npc.Action == NPC.eAction.Moving || npc.Action == NPC.eAction.Blocked)
                            {
                                if (npc.Step < 256)
                                {
                                    npc.Direction = Sprite.eDirection.UP;
                                    npc.Move(gameTime, Sprite.eDirection.UP, 256 - npc.Step);
                                }
                                else if (npc.Step < 672)
                                {
                                    npc.Direction = Sprite.eDirection.RIGHT;
                                    npc.Move(gameTime, Sprite.eDirection.RIGHT, 672 - npc.Step);
                                }
                                else if (npc.Step < 928)
                                {
                                    npc.Direction = Sprite.eDirection.DOWN;
                                    npc.Move(gameTime, Sprite.eDirection.DOWN, 928 - npc.Step);
                                }
                                else if (npc.Step < 1344)
                                {
                                    npc.Direction = Sprite.eDirection.LEFT;
                                    npc.Move(gameTime, Sprite.eDirection.LEFT, 1344 - npc.Step);
                                }
                                else
                                {
                                    npc.Step = 0;
                                }
                            }
                            break;
                    }
                    break;
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
                case "village":
                    _MapNo = eMapNo.VILLAGE;
                    BackgroundMusic = Game.Content.Load<Song>(@"Audios\Musics\Village");
                    Encounters.Add(new Encounter(Game.Content.Load<Monster>(@"Monsters\Goblin"), 1, 100));

                    NPC boy1 = new NPC(Game, "test", @"Overworld\npc", new Rectangle(0, 16, 128, 16), new Vector2(256, 64));
                    boy1.Talking += new NPC.TalkingHandler(boy1_Talking);
                    NPCs.Add(boy1);
                    break;

                case "tranquility plain":
                    _MapNo = eMapNo.TRANQUILITY_PLAIN;
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
