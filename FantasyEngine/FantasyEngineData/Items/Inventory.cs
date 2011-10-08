using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using FantasyEngineData.Entities;

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

            public override string ToString()
            {
                return Item.Name + " * " + Number;
            }

            public override bool Equals(object obj)
            {
                if (obj is InvItem)
                    return Equals(obj as InvItem);
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            #region IEquatable<InvItem> Membres

            public bool Equals(InvItem other)
            {
                return Item == other.Item;
            }

            #endregion

            public static implicit operator InvItem(BaseItem item)
            {
                return new InvItem(item);
            }

            public static bool operator ==(InvItem invItem1, InvItem invItem2)
            {
                if (((object)invItem1) == null)
                    return ((object)invItem2) == null;
                if (((object)invItem2) == null)
                    return ((object)invItem1) == null;
                return invItem1.Equals(invItem2);
            }

            public static bool operator !=(InvItem invItem1, InvItem invItem2) { return !(invItem1 == invItem2); }
        }

        private List<InvItem> _Items = new List<InvItem>();

        [ContentSerializerIgnore]
        public List<InvItem> Items { get { return _Items; } }

        public int Gold { get; set; }

#if false
        [ContentSerializerIgnore]
        public List<Weapon> Weapons { get; set; }

        public List<string> WeaponList { get; set; }
#endif

        public Inventory()
        {
            Gold = 0;

#if false
            Weapons = new List<Weapon>();
#endif
        }

        public void Add(InvItem invItem)
        {
            if (Items.Contains(invItem))
                Items.Find(i => i == invItem).Number += invItem.Number;
            else
                Items.Add(invItem);
        }

        public void AddRange(List<BaseItem> items)
        {
            foreach (BaseItem item in items)
            {
                Add(item);
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

        public void Use(BaseItem item, Character user, Character target = null)
        {
            if (item.Effect != null
                && item.Effect.Use(user, target))
                Drop(item);
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

#if false
            //inventory.Name = input.ReadString();
            //armory.ArmorList = input.ReadObject<List<string>>();
            inventory.WeaponList = input.ReadObject<List<string>>();

            // Add all weapons in weapon list
            foreach (string weapon in inventory.WeaponList)
            {
                Weapon newWeapon = input.ContentManager.Load<Weapon>(weapon).Clone() as Weapon;
                inventory.Weapons.Add(newWeapon);
            }
#endif

            return inventory;
        }
    }
}
