using System;
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

        public enum eMapNo
        {
            VILLAGE
        }

        private eMapNo _MapNo;
        private Map _MapData;
        private List<NPC> _NPCs = new List<NPC>();
        private List<Encounter> _Encounters = new List<Encounter>();

        /// <summary>
        /// Offset in pixel of the camera on the map.
        /// </summary>
        public Vector2 Offset { get; set; }

        /// <summary>
        /// Data of the map.
        /// </summary>
        public Map MapData { get { return _MapData; } }

        /// <summary>
        /// Background song of the map.
        /// </summary>
        public Song BackgroundMusic;

        /// <summary>
        /// List all non-playable character in the map.
        /// </summary>
        public List<NPC> NPCs { get { return _NPCs; } }

        /// <summary>
        /// List of all monsters that can be encountered.
        /// </summary>
        public List<Encounter> Encounters { get { return _Encounters; } }

        private void Init(string mapName)
        {
            _MapData = Game.Content.Load<Map>(@"Maps\" + mapName);
            _MapData.GetLayer("Collision").Visible = false; // Potential bug if there's no layer Collision.
            foreach (TiledLib.MapObject obj in _MapData.GetAllObjects())
            {
                NPC npc;
                if (obj.Properties["direction"] != null)
                    if (obj.Properties["regainDirection"] != null)
                        npc = new NPC(Game, obj.Name,
                            @"NPC\" + obj.Properties["sprite"].RawValue,
                            new Vector2(obj.Bounds.X, obj.Bounds.Y),
                            (Sprite.eDirection)Enum.Parse(typeof(Sprite.eDirection), obj.Properties["direction"].RawValue),
                            bool.Parse(obj.Properties["regainDirection"].RawValue));
                    else
                        npc = new NPC(Game, obj.Name,
                            @"NPC\" + obj.Properties["sprite"].RawValue,
                            new Vector2(obj.Bounds.X, obj.Bounds.Y),
                            (Sprite.eDirection)Enum.Parse(typeof(Sprite.eDirection), obj.Properties["direction"].RawValue));
                else
                    npc = new NPC(Game, obj.Name, @"NPC\" + obj.Properties["sprite"].RawValue, new Vector2(obj.Bounds.X, obj.Bounds.Y));
                npc.Talking += new NPC.TalkingHandler(npc_Talk);
                npc.Moving += new NPC.MovingHandler(npc_Moving);
                NPCs.Add(npc);
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

                        case "Steve":
                            thr = new Thread(
                                delegate(object Data)
                                {
                                    npc.Talk(npc.Name + ": People think I talk too much, but I don't think so. I really don't know why they think that.");
                                    npc.Talk(npc.Name + ": It's not like I'm talking and I'm talking and it seems to have no end to what I'm saying and on top of that, there's no purpose to what I'm saying because I only fill space with inutile dialog...");
                                });
                            thr.Start(npc);
                            break;
                    }
                    break;
            }
        }

        void npc_Moving(EventArgs e, NPC npc)
        {
            switch (_MapNo)
            {
                case eMapNo.VILLAGE:
                    switch (npc.Name)
                    {
                        case "Woman":
                            if (npc.Action == NPC.eAction.Stay || npc.Action == NPC.eAction.Moving || npc.Action == NPC.eAction.Blocked)
                            {
                                if (npc.Step % 10 == 0 && npc.Action != NPC.eAction.Blocked)
                                {
                                    npc.AnimateWalking();
                                }

                                if (npc.Step % 2 == 0)
                                {
                                    if (npc.Step < 256)
                                    {
                                        npc.Direction = Sprite.eDirection.UP;
                                        npc.Move(Sprite.eDirection.UP);
                                    }
                                    else if (npc.Step < 672)
                                    {
                                        npc.Direction = Sprite.eDirection.RIGHT;
                                        npc.Move(Sprite.eDirection.RIGHT);
                                    }
                                    else if (npc.Step < 928)
                                    {
                                        npc.Direction = Sprite.eDirection.DOWN;
                                        npc.Move(Sprite.eDirection.DOWN);
                                    }
                                    else if (npc.Step < 1344)
                                    {
                                        npc.Direction = Sprite.eDirection.LEFT;
                                        npc.Move(Sprite.eDirection.LEFT);
                                    }
                                    else
                                    {
                                        npc.Step = -1;
                                    }
                                }

                                if (npc.Action != NPC.eAction.Blocked)
                                    npc.Step++;
                            }
                            break;
                    }
                    break;
            }
        }

        public MapObject(Game game, eMapNo mapNo, Vector2 offset)
            : base(game)
        {
            Initialize();
            _MapNo = mapNo;
            switch (_MapNo)
            {
                case eMapNo.VILLAGE:
                    Init("village");
                    Offset = offset;
                    BackgroundMusic = Game.Content.Load<Song>(@"Audios\Musics\Village");
                    Encounters.Add(new Encounter(Game.Content.Load<Monster>(@"Monsters\Goblin"), 1, 100));

                    NPC boy1 = new NPC(Game, "test", @"NPC\man2", new Vector2(256, 64));
                    boy1.Talking += new NPC.TalkingHandler(boy1_Talking);
                    NPCs.Add(boy1);
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
            GameMain.cameraMatrix = Matrix.CreateTranslation((int)-Offset.X, (int)-Offset.Y, 0);

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
