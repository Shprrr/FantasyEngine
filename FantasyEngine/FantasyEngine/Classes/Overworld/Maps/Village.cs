using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using FantasyEngine.Classes.Menus;

namespace FantasyEngine.Classes.Overworld.Maps
{
    public static class Village
    {
        public static void boy1_Talk(EventArgs e, NPC npc)
        {
            Thread thr = new Thread(
                delegate(object Data)
                {
                    npc.Talk(npc.Name + ": Loaded succesfully.");
                });
            thr.Start(npc);
        }

        public static void Claudia_Talk(EventArgs e, NPC npc)
        {
            Thread thr = new Thread(
                delegate(object Data)
                {
                    npc.Talk(npc.Name + ": The fountain is pretty.");
                });
            thr.Start(npc);
        }

        public static void Woman_Talk(EventArgs e, NPC npc)
        {
            Thread thr = new Thread(
                delegate(object Data)
                {
                    npc.Talk(npc.Name + ": I don't have time to talk.  RUN!!");
                });
            thr.Start(npc);
        }

        public static void Woman_Move(EventArgs e, NPC npc, GameTime gameTime)
        {
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
        }

        public static void Griswold_Talk(EventArgs e, NPC npc)
        {
            Thread thr = new Thread(
                delegate(object Data)
                {
                    npc.Talk(npc.Name + ": Hello. What can I do for you ?");
                    List<FantasyEngineData.Items.BaseItem> shopBuy = new List<FantasyEngineData.Items.BaseItem>();
                    shopBuy.Add(FantasyEngineData.Items.ItemManager.GetItem("Potion"));
                    shopBuy.Add(FantasyEngineData.Items.ItemManager.GetWeapon("Dagger"));
                    shopBuy.Add(FantasyEngineData.Items.ItemManager.GetWeapon("Long Sword"));
                    Scene.AddSubScene(new ShopScene(npc.Game, shopBuy));
                });
            thr.Start(npc);
        }

        public static void Inn_Talk(EventArgs e, NPC npc)
        {
            Scene.AddSubScene(new InnScene(npc.Game, 10));
        }

        public static void JobMaster_Talk(EventArgs e, NPC npc)
        {
            Scene.AddSubScene(new JobChangeScene(npc.Game));
        }

        public static void Steve_Talk(EventArgs e, NPC npc)
        {
            Thread thr = new Thread(
                delegate(object Data)
                {
                    npc.Talk(npc.Name + ": People think I talk too much, but I don't think so. I really don't know why they think that.");
                    npc.Talk(npc.Name + ": It's not like I'm talking and I'm talking and it seems to have no end to what I'm saying and on top of that, there's no purpose to what I'm saying because I only fill space with unnecessary dialog just to take to another screen to see that I continue to talk again and again...");
                });
            thr.Start(npc);
        }
    }
}
