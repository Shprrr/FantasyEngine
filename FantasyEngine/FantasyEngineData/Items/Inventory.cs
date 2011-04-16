using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData.Items
{
    public class Inventory
    {
        public class InvItem : IEquatable<InvItem>
        {
            public BaseItem Item { get; set; }

            public int Number { get; set; }

            public InvItem()
            {

            }

            public InvItem(BaseItem item, int number = 1)
            {
                Item = item;
                Number = number;
            }

            public static implicit operator InvItem(BaseItem item)
            {
                return new InvItem(item);
            }

            public override string ToString()
            {
                return Item.Name + " * " + Number;
            }

            #region IEquatable<InvItem> Membres

            public bool Equals(InvItem other)
            {
                return Item == other.Item;
            }

            #endregion
        }

        private List<InvItem> _Items = new List<InvItem>();

        [ContentSerializerIgnore]
        public List<InvItem> Items { get { return _Items; } }

        public int Gold { get; set; }

        [ContentSerializerIgnore]
        public List<Weapon> Weapons { get; set; }

        public List<string> WeaponList { get; set; }

        public Inventory()
        {
            Gold = 0;

            Weapons = new List<Weapon>();
        }

        public void AddRange(List<BaseItem> items)
        {
            foreach (BaseItem item in items)
            {
                if (Items.Contains(item))
                    Items.Find(i => i.Item == item).Number++;
                else
                    Items.Add(new InvItem(item, 1));
            }
        }

        public void Drop(BaseItem item, int number = 1)
        {
            Items.Find(i => i.Item == item).Number -= number;
            if (Items.Find(i => i.Item == item).Number <= 0)
                Items.Remove(item);
        }

        /// <summary>
        /// Sort one item before the other.
        /// </summary>
        /// <param name="itemToMove">Item to move.  It's the sorted item.</param>
        /// <param name="itemBeforeNewPosition">Item where it will be moved.  Null to move at the end.
        /// The item moved will be before this item in parameter.</param>
        public void Sort(BaseItem itemToMove, BaseItem itemBeforeNewPosition)
        {
            int index = Items.IndexOf(itemBeforeNewPosition);
            InvItem item = Items.Find(i => i.Item == itemToMove);

            if (index == -1)
                index = Items.Count - 1;

            Items.Remove(item);
            Items.Insert(index, item);
        }

        public override string ToString()
        {
            return "G=" + Gold + "; Unique item=" + Items.Count + "; Item count=" + Items.Sum(i => i.Number);
        }
    }

    public class InventoryContentReader : ContentTypeReader<Inventory>
    {
        protected override Inventory Read(ContentReader input, Inventory existingInstance)
        {
            Inventory inventory = existingInstance;

            if (inventory == null)
            {
                inventory = new Inventory();
            }

            //inventory.Name = input.ReadString();
            //armory.ArmorList = input.ReadObject<List<string>>();
            inventory.WeaponList = input.ReadObject<List<string>>();

            // Add all weapons in weapon list
            foreach (string weapon in inventory.WeaponList)
            {
                Weapon newWeapon = input.ContentManager.Load<Weapon>(weapon).Clone() as Weapon;
                inventory.Weapons.Add(newWeapon);
            }

            return inventory;
        }
    }
}
