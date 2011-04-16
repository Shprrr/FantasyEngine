using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasyEngineData.Items
{
    public static class ItemManager
    {
        public const string ITEM_TYPE = "Items";
        public const string WEAPON_TYPE = "Weapons";
        public const string ARMOR_TYPE = "Armors";
        public const string SHIELD_TYPE = "Shields";

        #region Fields Region
        static Dictionary<string, Item> items = new Dictionary<string, Item>();
        static Dictionary<string, Weapon> weapons = new Dictionary<string, Weapon>();
        static Dictionary<string, Armor> armors = new Dictionary<string, Armor>();
        static Dictionary<string, Shield> shields = new Dictionary<string, Shield>();
        #endregion

        #region Keys Property Region
        public static Dictionary<string, Item>.KeyCollection ItemKeys
        {
            get { return items.Keys; }
        }
        public static Dictionary<string, Weapon>.KeyCollection WeaponKeys
        {
            get { return weapons.Keys; }
        }
        public static Dictionary<string, Armor>.KeyCollection ArmorKeys
        {
            get { return armors.Keys; }
        }
        public static Dictionary<string, Shield>.KeyCollection ShieldKeys
        {
            get { return shields.Keys; }
        }
        #endregion

        public static void Load(BaseItem[] baseItems)
        {
            foreach (BaseItem baseItem in baseItems)
            {
                if (baseItem is Item)
                {
                    AddItem((Item)baseItem);
                }

                if (baseItem is Weapon)
                {
                    AddWeapon((Weapon)baseItem);
                }

                if (baseItem is Armor)
                {
                    AddArmor((Armor)baseItem);
                }

                if (baseItem is Shield)
                {
                    AddShield((Shield)baseItem);
                }
            }
        }

        public static BaseItem GetBaseItem(string type, string name)
        {
            if (type == ITEM_TYPE)
                return GetItem(name);

            if (type == WEAPON_TYPE)
                return GetWeapon(name);

            if (type == ARMOR_TYPE)
                return GetArmor(name);

            if (type == SHIELD_TYPE)
                return GetShield(name);

            return null;
        }

        #region Item Methods
        public static void AddItem(Item item)
        {
            if (!items.ContainsKey(item.Name))
            {
                items.Add(item.Name, item);
            }
        }

        public static Item GetItem(string name)
        {
            if (items.ContainsKey(name))
            {
                return (Item)items[name].Clone();
            }
            return null;
        }

        public static bool ContainsItem(string name)
        {
            return items.ContainsKey(name);
        }
        #endregion

        #region Weapon Methods
        public static void AddWeapon(Weapon weapon)
        {
            if (!weapons.ContainsKey(weapon.Name))
            {
                weapons.Add(weapon.Name, weapon);
            }
        }

        public static Weapon GetWeapon(string name)
        {
            if (weapons.ContainsKey(name))
            {
                return (Weapon)weapons[name].Clone();
            }
            return null;
        }

        public static bool ContainsWeapon(string name)
        {
            return weapons.ContainsKey(name);
        }
        #endregion

        #region Armor Methods
        public static void AddArmor(Armor armor)
        {
            if (!armors.ContainsKey(armor.Name))
            {
                armors.Add(armor.Name, armor);
            }
        }

        public static Armor GetArmor(string name)
        {
            if (armors.ContainsKey(name))
            {
                return (Armor)armors[name].Clone();
            }
            return null;
        }

        public static bool ContainsArmor(string name)
        {
            return armors.ContainsKey(name);
        }
        #endregion

        #region Shield Methods
        public static void AddShield(Shield shield)
        {
            if (!shields.ContainsKey(shield.Name))
            {
                shields.Add(shield.Name, shield);
            }
        }

        public static Shield GetShield(string name)
        {
            if (shields.ContainsKey(name))
            {
                return (Shield)shields[name].Clone();
            }
            return null;
        }

        public static bool ContainsShield(string name)
        {
            return shields.ContainsKey(name);
        }
        #endregion
    }
}
