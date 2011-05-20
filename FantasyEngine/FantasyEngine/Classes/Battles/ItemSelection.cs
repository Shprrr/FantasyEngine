using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData.Items;

namespace FantasyEngine.Classes.Battles
{
    public class ItemSelection : Command
    {
        private List<Inventory.InvItem> list;

        public BaseItem ItemSelected { get { return list[CursorPosition].Item; } }

        public ItemSelection(Game game, int width, int height)
            : base(game, width, new string[] { "" }, 2)
        {
            list = Player.GamePlayer.Inventory.Items.FindAll(i => i.Item is Item);
            Choices = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                Choices[i] = list[i].Item.Name + ": " + list[i].Number;
            }
            Rectangle.Height = height;
        }

        /// <summary>
        /// Refresh the list of items.
        /// </summary>
        public void RefreshChoices()
        {
            int height = Rectangle.Height;
            list = Player.GamePlayer.Inventory.Items.FindAll(i => i.Item is Item);
            Choices = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                Choices[i] = list[i].Item.Name + ": " + list[i].Number;
            }
            Rectangle.Height = height;
        }
    }
}
