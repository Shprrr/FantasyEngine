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

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _MainCommand.Offset = GameMain.CameraOffset;
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

            _DescriptionWindow.Offset = GameMain.CameraOffset;
            _DescriptionWindow.Draw(gameTime);
            GameMain.Scissor(_DescriptionWindow.InsideBound);

            spriteBatch.DrawString(GameMain.font, item.Description,
                new Vector2(16, 16) + GameMain.CameraOffset, Color.White);

            GameMain.ScissorReset();
        }

        private void DrawListItem(GameTime gameTime)
        {
            if (!_ListItemWindow.Visible)
                return;

            _ListItemWindow.Offset = GameMain.CameraOffset;
            _ListItemWindow.Draw(gameTime);
            GameMain.Scissor(_ListItemWindow.InsideBound);

            // Draw list of items
            int i = 0;
            if (_MainCommand.CursorPosition == 1)
                foreach (var item in Player.GamePlayer.Inventory.Items)
                {
                    spriteBatch.DrawString(GameMain.font, item.Number.ToString().PadRight(3, '') + "" + item.Item.Name,
                        new Vector2(34, 68 + i * 16) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, item.Item.Price.ToString().PadLeft(6, '').Insert(3, " "),
                        new Vector2(252, 68 + i * 16) + GameMain.CameraOffset, Color.White);
                    i++;
                }
            else
                foreach (var item in _ShopBuy)
                {
                    spriteBatch.DrawString(GameMain.font, item.Name,
                        new Vector2(34, 68 + i * 16) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, item.Price.ToString().PadLeft(6, '').Insert(3, " "),
                        new Vector2(252, 68 + i * 16) + GameMain.CameraOffset, Color.White);
                    i++;
                }

            // Draw cursor
            _CursorWindow.Position = new Vector2(14, 68 + _CursorWindow.CursorIndex * 16);
            _CursorWindow.Draw(gameTime);

            GameMain.ScissorReset();
        }

        private void DrawStats(GameTime gameTime, BaseItem item)
        {
            _StatsWindow.Offset = GameMain.CameraOffset;
            _StatsWindow.Draw(gameTime);
            GameMain.Scissor(_StatsWindow.InsideBound);

            Inventory.InvItem invItem = Player.GamePlayer.Inventory.Items.Find(i => i.Item == item);

            spriteBatch.Draw(item.Icon, new Vector2(580, 74) + GameMain.CameraOffset, Color.White);

            spriteBatch.DrawString(GameMain.font, "Stocked:",
                new Vector2(382, 68) + GameMain.CameraOffset, Color.White);
            spriteBatch.DrawString(GameMain.font, (invItem != null ? invItem.Number : 0).ToString().PadLeft(2, ''),
                new Vector2(520, 68) + GameMain.CameraOffset, Color.White);

            if (!(item is Item))
            {
                spriteBatch.DrawString(GameMain.font, "Equipped:",
                    new Vector2(382, 84) + GameMain.CameraOffset, Color.White);
                //TODO: Redo the count when there's more than one character. (In InvItem ?)
                spriteBatch.DrawString(GameMain.font, invItem != null ? (invItem.Item.IsEquiped ? "1" : "0") : "0",
                    new Vector2(520, 84) + GameMain.CameraOffset, Color.White);
            }

            if (item is Weapon)
            {
                Weapon weapon = (Weapon)item;
                spriteBatch.DrawString(GameMain.font, "Damage:",
                    new Vector2(382, 104) + GameMain.CameraOffset, Color.White);
                //spriteBatch.DrawString(GameMain.font, (weapon + "x").PadLeft(3, ''),
                //    new Vector2(520, 104) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, weapon.Damage.ToString().PadLeft(3, ''),
                    new Vector2(580, 104) + GameMain.CameraOffset, Color.White);

                spriteBatch.DrawString(GameMain.font, "Hit %:",
                    new Vector2(382, 120) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, (weapon.HitPourc + "%").PadLeft(3, ''),
                    new Vector2(580, 120) + GameMain.CameraOffset, Color.White);
            }

            if (item is Armor)
            {
                Armor armor = (Armor)item;
                spriteBatch.DrawString(GameMain.font, "Defense:",
                    new Vector2(382, 104) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, armor.DefenseValue.ToString().PadLeft(3, ''),
                    new Vector2(580, 104) + GameMain.CameraOffset, Color.White);

                spriteBatch.DrawString(GameMain.font, "Evade:",
                    new Vector2(382, 120) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, (armor.EvadePourc + "%").PadLeft(3, ''),
                    new Vector2(580, 120) + GameMain.CameraOffset, Color.White);

                spriteBatch.DrawString(GameMain.font, "M.Defense:",
                    new Vector2(382, 136) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, armor.MagicDefenseValue.ToString().PadLeft(3, ''),
                    new Vector2(580, 136) + GameMain.CameraOffset, Color.White);

                spriteBatch.DrawString(GameMain.font, "M.Evade:",
                    new Vector2(382, 152) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, (armor.MagicEvadePourc + "%").PadLeft(3, ''),
                    new Vector2(580, 152) + GameMain.CameraOffset, Color.White);
            }

            if (item is Shield)
            {
                Shield shield = (Shield)item;
                spriteBatch.DrawString(GameMain.font, "Defense:",
                    new Vector2(382, 104) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, shield.DefenseValue.ToString().PadLeft(3, ''),
                    new Vector2(580, 104) + GameMain.CameraOffset, Color.White);

                spriteBatch.DrawString(GameMain.font, "Evade:",
                    new Vector2(382, 120) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, (shield.EvadePourc + "%").PadLeft(3, ''),
                    new Vector2(580, 120) + GameMain.CameraOffset, Color.White);

                spriteBatch.DrawString(GameMain.font, "M.Defense:",
                    new Vector2(382, 136) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, shield.MagicDefenseValue.ToString().PadLeft(3, ''),
                    new Vector2(580, 136) + GameMain.CameraOffset, Color.White);

                spriteBatch.DrawString(GameMain.font, "M.Evade:",
                    new Vector2(382, 152) + GameMain.CameraOffset, Color.White);
                spriteBatch.DrawString(GameMain.font, (shield.MagicEvadePourc + "%").PadLeft(3, ''),
                    new Vector2(580, 152) + GameMain.CameraOffset, Color.White);
            }

            if (item.Effect != null)
                spriteBatch.DrawString(GameMain.font, item.Effect.ToString(),
                    new Vector2(382, 172) + GameMain.CameraOffset, Color.White);

            #region AllowedJobs
            BaseJob baseJob = JobManager.GetBaseJob("OnionKid");
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(384, 188) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = JobManager.GetBaseJob("Soldier");
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(420, 188) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = JobManager.GetBaseJob("Warrior");
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(456, 188) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = JobManager.GetBaseJob("Archer");
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(492, 188) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = JobManager.GetBaseJob("Thief");
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(528, 188) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = JobManager.GetBaseJob("Black Mage");
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(564, 188) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = JobManager.GetBaseJob("White Mage");
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(600, 188) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);

            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Bs";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(420, 204) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Kn";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(456, 204) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Gu";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(492, 204) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Ni";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(528, 204) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "BW";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(564, 204) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "WW";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(600, 204) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);

            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Sc";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(384, 220) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "DK";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(420, 220) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Pa";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(456, 220) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "BD";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(492, 220) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Al";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(528, 220) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Ga";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(564, 220) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Bl";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(600, 220) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);

            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Bk";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(420, 236) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Dr";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(456, 236) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "GM";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(492, 236) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Mk";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(528, 236) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            baseJob = new BaseJob(); baseJob.JobAbbreviation = "Su";
            spriteBatch.DrawString(GameMain.font8, baseJob.JobAbbreviation,
                new Vector2(600, 236) + GameMain.CameraOffset, item.IsAllowed(baseJob) ? Color.White : Color.Gray);
            #endregion AllowedJobs

            GameMain.ScissorReset();
        }

        private void DrawCompare(GameTime gameTime, BaseItem item)
        {
            _CompareWindow.Offset = GameMain.CameraOffset;
            _CompareWindow.Draw(gameTime);
            GameMain.Scissor(_CompareWindow.InsideBound);

            if (!(item is Item))
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

                spriteBatch.DrawString(GameMain.font, actor.Name,
                    new Vector2(382, 276) + GameMain.CameraOffset, Color.White);

                if (item.IsAllowed(actor.CurrentJob.BaseJob))
                {
                    spriteBatch.DrawString(GameMain.font, "Attack:", new Vector2(382, 298) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, (actor.getAttackMultiplier() + "x").PadLeft(3, ''),
                       new Vector2(534, 298) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, actor.getBaseDamage(Character.ePhysicalDamageOption.BOTH).ToString().PadLeft(3, ''),
                        new Vector2(580, 298) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "Hit %:", new Vector2(382, 314) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, (actor.getHitPourc(Character.ePhysicalDamageOption.BOTH) + "%").PadLeft(3, ''),
                        new Vector2(580, 314) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "Defense:", new Vector2(382, 330) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, (actor.getDefenseMultiplier() + "x").PadLeft(3, ''),
                        new Vector2(534, 330) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, actor.getDefenseDamage().ToString().PadLeft(3, ''),
                        new Vector2(580, 330) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "Evade:", new Vector2(382, 346) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, (actor.getEvadePourc() + "%").PadLeft(3, ''),
                        new Vector2(580, 346) + GameMain.CameraOffset, Color.White);

                    spriteBatch.DrawString(GameMain.font, "M.Attack:", new Vector2(382, 362) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, (actor.getMagicAttackMultiplier(Character.eMagicalDamageOption.NONE) + "x").PadLeft(3, ''),
                        new Vector2(534, 362) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, actor.getMagicBaseDamage(Character.eMagicalDamageOption.NONE, 1).ToString().PadLeft(3, ''),
                        new Vector2(580, 362) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "M.Hit %:", new Vector2(382, 378) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, (actor.getMagicHitPourc(Character.eMagicalDamageOption.NONE, 80) + "%").PadLeft(3, ''),
                        new Vector2(580, 378) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "M.Defense:", new Vector2(382, 394) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, (actor.getMagicDefenseMultiplier() + "x").PadLeft(3, ''),
                        new Vector2(534, 394) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, actor.getMagicDefenseDamage().ToString().PadLeft(3, ''),
                        new Vector2(580, 394) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, "M.Evade:", new Vector2(382, 410) + GameMain.CameraOffset, Color.White);
                    spriteBatch.DrawString(GameMain.font, (actor.getMagicEvadePourc() + "%").PadLeft(3, ''),
                        new Vector2(580, 410) + GameMain.CameraOffset, Color.White);
                }
                else
                    spriteBatch.DrawString(GameMain.font, "Cannot equip", new Vector2(382, 298) + GameMain.CameraOffset, Color.Gray);
            }

            GameMain.ScissorReset();
        }

        private void DrawHowMany(GameTime gameTime, BaseItem item)
        {
            if (!_HowManyWindow.Visible)
                return;

            _HowManyWindow.Offset = GameMain.CameraOffset;
            _HowManyWindow.Draw(gameTime);
            GameMain.Scissor(_HowManyWindow.InsideBound);

            spriteBatch.DrawString(GameMain.font, item.Name,
                new Vector2(382, 276) + GameMain.CameraOffset, Color.White);
            spriteBatch.DrawString(GameMain.font, _HowMany.ToString(),
                new Vector2(580, 292) + GameMain.CameraOffset, Color.White);
            spriteBatch.DrawString(GameMain.font, "Total:" + (item.Price * _HowMany).ToString("### ##0").Trim(),
                new Vector2(382, 406) + GameMain.CameraOffset, Color.White);

            GameMain.ScissorReset();
        }

        private void DrawGold(GameTime gameTime)
        {
            _GoldWindow.Offset = GameMain.CameraOffset;
            _GoldWindow.Draw(gameTime);
            GameMain.Scissor(_GoldWindow.InsideBound);

            spriteBatch.DrawString(GameMain.font, "Gold:" + Player.GamePlayer.Inventory.Gold.ToString("### ##0").Trim(),
                new Vector2(382, 450) + GameMain.CameraOffset, Color.White);

            GameMain.ScissorReset();
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
