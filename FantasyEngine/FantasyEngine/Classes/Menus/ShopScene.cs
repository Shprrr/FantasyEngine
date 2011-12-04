using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;

namespace FantasyEngine.Classes.Menus
{
    public class ShopScene : Scene
    {
        private readonly string[] MAIN_COMMANDS = { "Buy", "Sell", "Exit" };

        private List<BaseItem> _ShopBuy;
        private List<BaseItem> _ShopSold = new List<BaseItem>();
        private int _HowMany = 0;

        private Cursor _CursorWindow;
        private Command _MainCommand;
        private Window _DescriptionWindow;
        private Window _ListItemWindow;
        private Window _StatsWindow;
        private Window _CompareWindow;
        private Window _HowManyWindow;
        private Window _GoldWindow;

        public ShopScene(Game game, List<BaseItem> shopBuy)
            : base(game)
        {
            _ShopBuy = shopBuy;

            _CursorWindow = new Cursor(Game, _ShopBuy.Count);

            _MainCommand = new Command(game, 640, MAIN_COMMANDS, 3);

            _DescriptionWindow = new Window(game, 0, 0, 640, 48);
            _DescriptionWindow.Visible = false;

            _ListItemWindow = new Window(game, 0, _DescriptionWindow.Rectangle.Bottom,
                368, 480 - _DescriptionWindow.Rectangle.Bottom);
            _ListItemWindow.Enabled = false;
            _ListItemWindow.Visible = false;

            _StatsWindow = new Window(game, _ListItemWindow.Rectangle.Right, _DescriptionWindow.Rectangle.Bottom,
                640 - _ListItemWindow.Rectangle.Right, 208);
            _StatsWindow.Visible = false;

            _CompareWindow = new Window(game, _ListItemWindow.Rectangle.Right, _StatsWindow.Rectangle.Bottom,
                640 - _ListItemWindow.Rectangle.Right, 180);
            _CompareWindow.Visible = false;

            _HowManyWindow = new Window(game, _CompareWindow.Rectangle.X, _CompareWindow.Rectangle.Y,
                _CompareWindow.Rectangle.Width, _CompareWindow.Rectangle.Height);
            _HowManyWindow.Visible = false;

            _GoldWindow = new Window(game, _ListItemWindow.Rectangle.Right, _CompareWindow.Rectangle.Bottom,
                640 - _ListItemWindow.Rectangle.Right, 44);
        }

        public override void DrawGUI(GameTime gameTime)
        {
            base.DrawGUI(gameTime);

            _MainCommand.Offset = spriteBatchGUI.CameraOffset;
            _MainCommand.Draw(gameTime);

            if (!_MainCommand.Visible)
            {
                // Get the item selected.
                BaseItem item = null;
                if (_MainCommand.CursorPosition == 1)
                {
                    if (Player.GamePlayer.Inventory.Items.Count != 0)
                        item = Player.GamePlayer.Inventory.Items[_CursorWindow.CursorIndex].Item;
                }
                else
                    item = _ShopBuy[_CursorWindow.CursorIndex];

                DrawDescription(gameTime, item);

                DrawListItem(gameTime);

                DrawStats(gameTime, item);

                DrawCompare(gameTime, item);

                DrawHowMany(gameTime, item);
            }

            DrawGold(gameTime);
        }

        private void DrawDescription(GameTime gameTime, BaseItem item)
        {
            if (!_DescriptionWindow.Visible)
                return;

            _DescriptionWindow.Offset = spriteBatchGUI.CameraOffset;
            _DescriptionWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_DescriptionWindow.InsideBound);

            if (item != null)
                spriteBatchGUI.DrawString(GameMain.font, item.Description,
                    new Vector2(16, 16) + spriteBatchGUI.CameraOffset, Color.White);

            spriteBatchGUI.ScissorReset();
        }

        private void DrawListItem(GameTime gameTime)
        {
            if (!_ListItemWindow.Visible)
                return;

            _ListItemWindow.Offset = spriteBatchGUI.CameraOffset;
            _ListItemWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_ListItemWindow.InsideBound);

            // Draw list of items
            int i = 0;
            if (_MainCommand.CursorPosition == 1)
                foreach (var item in Player.GamePlayer.Inventory.Items)
                {
                    spriteBatchGUI.DrawString(GameMain.font, item.Number.ToString().PadRight(3, '') + "" + item.Item.Name,
                        new Vector2(34, 68 + i * 16) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, item.Item.Price.ToString().PadLeft(6, '').Insert(3, " "),
                        new Vector2(252, 68 + i * 16) + spriteBatchGUI.CameraOffset, Color.White);
                    i++;
                }
            else
                foreach (var item in _ShopBuy)
                {
                    spriteBatchGUI.DrawString(GameMain.font, item.Name,
                        new Vector2(34, 68 + i * 16) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, item.Price.ToString().PadLeft(6, '').Insert(3, " "),
                        new Vector2(252, 68 + i * 16) + spriteBatchGUI.CameraOffset, Color.White);
                    i++;
                }

            // Draw cursor
            _CursorWindow.Position = new Vector2(14, 68 + _CursorWindow.CursorIndex * 16);
            _CursorWindow.Draw(gameTime);

            spriteBatchGUI.ScissorReset();
        }

        private void DrawStats(GameTime gameTime, BaseItem item)
        {
            _StatsWindow.Offset = spriteBatchGUI.CameraOffset;
            _StatsWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_StatsWindow.InsideBound);

            Inventory.InvItem invItem = Player.GamePlayer.Inventory.Items.Find(i => i.Item == item);

            if (item != null)
                spriteBatchGUI.Draw(item.Icon, new Vector2(580, 74) + spriteBatchGUI.CameraOffset, Color.White);

            spriteBatchGUI.DrawString(GameMain.font, "Stocked:",
                new Vector2(382, 68) + spriteBatchGUI.CameraOffset, Color.White);
            spriteBatchGUI.DrawString(GameMain.font, (invItem != null ? invItem.Number : 0).ToString().PadLeft(2, ''),
                new Vector2(520, 68) + spriteBatchGUI.CameraOffset, Color.White);

            if (!(item is Item))
            {
                spriteBatchGUI.DrawString(GameMain.font, "Equipped:",
                    new Vector2(382, 84) + spriteBatchGUI.CameraOffset, Color.White);
                //TODO: Redo the count when there's more than one character. (In InvItem ?)
                spriteBatchGUI.DrawString(GameMain.font, invItem != null ? (invItem.Item.IsEquiped ? "1" : "0") : "0",
                    new Vector2(520, 84) + spriteBatchGUI.CameraOffset, Color.White);
            }

            if (item is Weapon)
            {
                Weapon weapon = (Weapon)item;
                spriteBatchGUI.DrawString(GameMain.font, "Damage:",
                    new Vector2(382, 104) + spriteBatchGUI.CameraOffset, Color.White);
                //spriteBatchGUI.DrawString(GameMain.font, (weapon + "x").PadLeft(3, ''),
                //    new Vector2(520, 104) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, weapon.Damage.ToString().PadLeft(3, ''),
                    new Vector2(580, 104) + spriteBatchGUI.CameraOffset, Color.White);

                spriteBatchGUI.DrawString(GameMain.font, "Hit %:",
                    new Vector2(382, 120) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, (weapon.HitPourc + "%").PadLeft(3, ''),
                    new Vector2(580, 120) + spriteBatchGUI.CameraOffset, Color.White);
            }

            if (item is Armor)
            {
                Armor armor = (Armor)item;
                spriteBatchGUI.DrawString(GameMain.font, "Defense:",
                    new Vector2(382, 104) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, armor.DefenseValue.ToString().PadLeft(3, ''),
                    new Vector2(580, 104) + spriteBatchGUI.CameraOffset, Color.White);

                spriteBatchGUI.DrawString(GameMain.font, "Evade:",
                    new Vector2(382, 120) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, (armor.EvadePourc + "%").PadLeft(3, ''),
                    new Vector2(580, 120) + spriteBatchGUI.CameraOffset, Color.White);

                spriteBatchGUI.DrawString(GameMain.font, "M.Defense:",
                    new Vector2(382, 136) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, armor.MagicDefenseValue.ToString().PadLeft(3, ''),
                    new Vector2(580, 136) + spriteBatchGUI.CameraOffset, Color.White);

                spriteBatchGUI.DrawString(GameMain.font, "M.Evade:",
                    new Vector2(382, 152) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, (armor.MagicEvadePourc + "%").PadLeft(3, ''),
                    new Vector2(580, 152) + spriteBatchGUI.CameraOffset, Color.White);
            }

            if (item is Shield)
            {
                Shield shield = (Shield)item;
                spriteBatchGUI.DrawString(GameMain.font, "Defense:",
                    new Vector2(382, 104) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, shield.DefenseValue.ToString().PadLeft(3, ''),
                    new Vector2(580, 104) + spriteBatchGUI.CameraOffset, Color.White);

                spriteBatchGUI.DrawString(GameMain.font, "Evade:",
                    new Vector2(382, 120) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, (shield.EvadePourc + "%").PadLeft(3, ''),
                    new Vector2(580, 120) + spriteBatchGUI.CameraOffset, Color.White);

                spriteBatchGUI.DrawString(GameMain.font, "M.Defense:",
                    new Vector2(382, 136) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, shield.MagicDefenseValue.ToString().PadLeft(3, ''),
                    new Vector2(580, 136) + spriteBatchGUI.CameraOffset, Color.White);

                spriteBatchGUI.DrawString(GameMain.font, "M.Evade:",
                    new Vector2(382, 152) + spriteBatchGUI.CameraOffset, Color.White);
                spriteBatchGUI.DrawString(GameMain.font, (shield.MagicEvadePourc + "%").PadLeft(3, ''),
                    new Vector2(580, 152) + spriteBatchGUI.CameraOffset, Color.White);
            }

            if (item != null)
            {
                if (item.Effect != null)
                    spriteBatchGUI.DrawString(GameMain.font8, item.Effect.ToString(),
                        new Vector2(382, 172) + spriteBatchGUI.CameraOffset, Color.White);

                #region AllowedJobs
                BaseJob baseJob = JobManager.GetBaseJob("OK");
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(384, 188) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = JobManager.GetBaseJob("So");
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(420, 188) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = JobManager.GetBaseJob("Wa");
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(456, 188) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = JobManager.GetBaseJob("Ar");
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(492, 188) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = JobManager.GetBaseJob("Th");
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(528, 188) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = JobManager.GetBaseJob("BM");
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(564, 188) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = JobManager.GetBaseJob("WM");
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(600, 188) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);

                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Bs";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(420, 204) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Kn";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(456, 204) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Gu";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(492, 204) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Ni";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(528, 204) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "BW";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(564, 204) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "WW";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(600, 204) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);

                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Sc";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(384, 220) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "DK";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(420, 220) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Pa";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(456, 220) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "BD";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(492, 220) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Al";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(528, 220) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Ga";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(564, 220) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Bl";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(600, 220) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);

                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Bk";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(420, 236) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Dr";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(456, 236) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "GM";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(492, 236) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Mk";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(528, 236) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                baseJob = new BaseJob(); baseJob.JobAbbreviation = "Su";
                spriteBatchGUI.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                    new Vector2(600, 236) + spriteBatchGUI.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
                #endregion AllowedJobs
            }

            spriteBatchGUI.ScissorReset();
        }

        private void DrawCompare(GameTime gameTime, BaseItem item)
        {
            _CompareWindow.Offset = spriteBatchGUI.CameraOffset;
            _CompareWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_CompareWindow.InsideBound);

            if (item != null && !(item is Item))
            {
                // Try to equip a clone to see what it does.
                Character actor = (Character)Player.GamePlayer.Actors[0].Clone();
                if (item.IsAllowed(actor.CurrentJob.BaseJob))
                {
                    BaseItem itemClone = (BaseItem)item.Clone();
                    if (itemClone is Weapon)
                        actor.RightHand = itemClone;
                    if (itemClone is Shield)
                        actor.LeftHand = itemClone;
                    if (itemClone is Armor)
                        switch (((Armor)itemClone).Location)
                        {
                            case ArmorLocation.Body:
                                actor.Body = (Armor)itemClone;
                                break;
                            case ArmorLocation.Head:
                                actor.Head = (Armor)itemClone;
                                break;
                            case ArmorLocation.Arms:
                                actor.Arms = (Armor)itemClone;
                                break;
                            case ArmorLocation.Feet:
                                actor.Feet = (Armor)itemClone;
                                break;
                        }
                }

                spriteBatchGUI.DrawString(GameMain.font, actor.Name,
                    new Vector2(382, 276) + spriteBatchGUI.CameraOffset, Color.White);

                if (item.IsAllowed(actor.CurrentJob.BaseJob))
                {
                    spriteBatchGUI.DrawString(GameMain.font, "Attack:", new Vector2(382, 298) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, (actor.getAttackMultiplier() + "x").PadLeft(3, ''),
                       new Vector2(534, 298) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, actor.getBaseDamage(Character.ePhysicalDamageOption.BOTH).ToString().PadLeft(3, ''),
                        new Vector2(580, 298) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, "Hit %:", new Vector2(382, 314) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, (actor.getHitPourc(Character.ePhysicalDamageOption.BOTH) + "%").PadLeft(3, ''),
                        new Vector2(580, 314) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, "Defense:", new Vector2(382, 330) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, (actor.getDefenseMultiplier() + "x").PadLeft(3, ''),
                        new Vector2(534, 330) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, actor.getDefenseDamage().ToString().PadLeft(3, ''),
                        new Vector2(580, 330) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, "Evade:", new Vector2(382, 346) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, (actor.getEvadePourc() + "%").PadLeft(3, ''),
                        new Vector2(580, 346) + spriteBatchGUI.CameraOffset, Color.White);

                    spriteBatchGUI.DrawString(GameMain.font, "M.Attack:", new Vector2(382, 362) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, (actor.getMagicAttackMultiplier(Character.eMagicalDamageOption.NONE) + "x").PadLeft(3, ''),
                        new Vector2(534, 362) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, actor.getMagicBaseDamage(Character.eMagicalDamageOption.NONE, 1).ToString().PadLeft(3, ''),
                        new Vector2(580, 362) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, "M.Hit %:", new Vector2(382, 378) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, (actor.getMagicHitPourc(Character.eMagicalDamageOption.NONE, 80) + "%").PadLeft(3, ''),
                        new Vector2(580, 378) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, "M.Defense:", new Vector2(382, 394) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, (actor.getMagicDefenseMultiplier() + "x").PadLeft(3, ''),
                        new Vector2(534, 394) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, actor.getMagicDefenseDamage().ToString().PadLeft(3, ''),
                        new Vector2(580, 394) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, "M.Evade:", new Vector2(382, 410) + spriteBatchGUI.CameraOffset, Color.White);
                    spriteBatchGUI.DrawString(GameMain.font, (actor.getMagicEvadePourc() + "%").PadLeft(3, ''),
                        new Vector2(580, 410) + spriteBatchGUI.CameraOffset, Color.White);
                }
                else
                    spriteBatchGUI.DrawString(GameMain.font, "Cannot equip", new Vector2(382, 298) + spriteBatchGUI.CameraOffset, Color.Gray);
            }

            spriteBatchGUI.ScissorReset();
        }

        private void DrawHowMany(GameTime gameTime, BaseItem item)
        {
            if (!_HowManyWindow.Visible)
                return;

            _HowManyWindow.Offset = spriteBatchGUI.CameraOffset;
            _HowManyWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_HowManyWindow.InsideBound);

            spriteBatchGUI.DrawString(GameMain.font, item.Name,
                new Vector2(382, 276) + spriteBatchGUI.CameraOffset, Color.White);
            spriteBatchGUI.DrawString(GameMain.font, _HowMany.ToString(),
                new Vector2(580, 292) + spriteBatchGUI.CameraOffset, Color.White);
            spriteBatchGUI.DrawString(GameMain.font, "Total:" + (item.Price * _HowMany).ToString("### ##0").Trim(),
                new Vector2(382, 406) + spriteBatchGUI.CameraOffset, Color.White);

            spriteBatchGUI.ScissorReset();
        }

        private void DrawGold(GameTime gameTime)
        {
            _GoldWindow.Offset = spriteBatchGUI.CameraOffset;
            _GoldWindow.Draw(gameTime);
            spriteBatchGUI.Scissor(_GoldWindow.InsideBound);

            spriteBatchGUI.DrawString(GameMain.font, "Gold:" + Player.GamePlayer.Inventory.Gold.ToString("### ##0").Trim(),
                new Vector2(382, 450) + spriteBatchGUI.CameraOffset, Color.White);

            spriteBatchGUI.ScissorReset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _MainCommand.Update(gameTime);

            if (!Input.UpdateInput(gameTime))
                return;

            _CursorWindow.ItemMax = _MainCommand.CursorPosition == 1 ?
                Player.GamePlayer.Inventory.Items.Count : _ShopBuy.Count;

            if (!_MainCommand.Enabled && !_HowManyWindow.Visible)
                _CursorWindow.Update(gameTime);

            if (_HowManyWindow.Visible)
            {
                int howManyMax = 0;
                if (_MainCommand.CursorPosition == 1)
                {
                    if (Player.GamePlayer.Inventory.Items.Count != 0)
                        howManyMax = Player.GamePlayer.Inventory.Items[_CursorWindow.CursorIndex].Number;
                }
                else
                    howManyMax = Player.GamePlayer.Inventory.Gold / _ShopBuy[_CursorWindow.CursorIndex].Price;

                if (Input.keyStateHeld.IsKeyDown(Keys.Left))
                {
                    _HowMany -= 1;
                    if (_HowMany < 0)
                        _HowMany = 0;

                    Input.PutDelay(Keys.Left);
                    return;
                }

                if (Input.keyStateHeld.IsKeyDown(Keys.Right))
                {
                    _HowMany += 1;
                    if (_HowMany > howManyMax)
                        _HowMany = howManyMax;

                    Input.PutDelay(Keys.Right);
                    return;
                }

                if (Input.keyStateHeld.IsKeyDown(Keys.Up))
                {
                    _HowMany -= 10;
                    if (_HowMany < 0)
                        _HowMany = 0;

                    Input.PutDelay(Keys.Up);
                    return;
                }

                if (Input.keyStateHeld.IsKeyDown(Keys.Down))
                {
                    _HowMany += 10;
                    if (_HowMany > howManyMax)
                        _HowMany = howManyMax;

                    Input.PutDelay(Keys.Down);
                    return;
                }
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                if (_MainCommand.Enabled)
                    switch (_MainCommand.CursorPosition)
                    {
                        case 0:
                        case 1:
                            _MainCommand.Enabled = false;
                            _MainCommand.Visible = false;
                            _DescriptionWindow.Visible = true;
                            _ListItemWindow.Enabled = true;
                            _ListItemWindow.Visible = true;
                            _StatsWindow.Visible = true;
                            _CompareWindow.Visible = true;

                            _CursorWindow.CursorIndex = 0;
                            break;

                        case 2:
                            Scene.RemoveSubScene();
                            return;
                    }
                else if (_HowManyWindow.Visible)
                {
                    if (_HowMany != 0)
                    {
                        BaseItem item;
                        if (_MainCommand.CursorPosition == 1)
                        {
                            item = Player.GamePlayer.Inventory.Items[_CursorWindow.CursorIndex].Item;
                            Player.GamePlayer.Inventory.Drop(item, _HowMany);
                            Player.GamePlayer.Inventory.Gold += item.Price * _HowMany;
                            _CursorWindow.ItemMax = Player.GamePlayer.Inventory.Items.Count;
                        }
                        else
                        {
                            item = _ShopBuy[_CursorWindow.CursorIndex];
                            if (Player.GamePlayer.Inventory.Gold >= item.Price * _HowMany)
                            {
                                Player.GamePlayer.Inventory.Add(new Inventory.InvItem(item, _HowMany));
                                Player.GamePlayer.Inventory.Gold -= item.Price * _HowMany;
                            }
                        }
                    }
                    _HowManyWindow.Visible = false;
                }
                else
                {
                    _HowMany = 0;
                    _HowManyWindow.Visible = true;
                }
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Escape)
                || Input.keyStateDown.IsKeyDown(Keys.Back))
            {
                if (_HowManyWindow.Visible)
                    _HowManyWindow.Visible = false;
                else if (_ListItemWindow.Enabled)
                {
                    _MainCommand.Enabled = true;
                    _MainCommand.Visible = true;
                    _DescriptionWindow.Visible = false;
                    _ListItemWindow.Enabled = false;
                    _ListItemWindow.Visible = false;
                    _StatsWindow.Visible = false;
                    _CompareWindow.Visible = false;
                }
                else if (_MainCommand.Enabled)
                    Scene.RemoveSubScene();
            }
        }
    }
}
