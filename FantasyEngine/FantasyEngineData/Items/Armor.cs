using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace FantasyEngineData.Items
{
    public class Armor : BaseItem
    {
        #region Field Region
        ArmorLocation location;
        int defenseValue;
        //int defenseModifier;
        int evadePourc;
        int magicDefenseValue;
        int magicEvadePourc;
        #endregion

        #region Property Region
        public ArmorLocation Location
        {
            get { return location; }
            set { location = value; }
        }
        public int DefenseValue
        {
            get { return defenseValue; }
            set { defenseValue = value; }
        }
        //public int DefenseModifier
        //{
        //    get { return defenseModifier; }
        //    protected set { defenseModifier = value; }
        //}
        public int EvadePourc
        {
            get { return evadePourc; }
            set { evadePourc = value; }
        }
        public int MagicDefenseValue
        {
            get { return magicDefenseValue; }
            set { magicDefenseValue = value; }
        }
        public int MagicEvadePourc
        {
            get { return magicEvadePourc; }
            set { magicEvadePourc = value; }
        }
        #endregion

        #region Constructor Region
        public Armor()
            : base()
        {
        }

        public Armor(
                string armorName,
                string armorType,
                Texture2D icon,
                int price,
                float weight,
                string description,
                ArmorLocation location,
                int defenseValue,
            //int defenseModifier,
                int evadePourc,
                int magicDefenseValue,
                int magicEvadePourc,
                string allowableJobs,
                Effect effect)
            : base(armorName, armorType, icon, price, weight, description, allowableJobs, effect)
        {
            Location = location;
            DefenseValue = defenseValue;
            //DefenseModifier = defenseModifier;
            EvadePourc = evadePourc;
            MagicDefenseValue = magicDefenseValue;
            MagicEvadePourc = magicEvadePourc;
        }
        #endregion

        #region Abstract Method Region
        public override object Clone()
        {
            Armor armor = new Armor(
                Name,
                Type,
                Icon,
                Price,
                Weight,
                Description,
                Location,
                DefenseValue,
                //DefenseModifier,
                EvadePourc,
                MagicDefenseValue,
                MagicEvadePourc,
                AllowableJobs,
                Effect);
            return armor;
        }

        public override string ToString()
        {
            string armorString = base.ToString() + ", ";
            armorString += Location.ToString() + ", ";
            armorString += DefenseValue.ToString() + ", ";
            //armorString += DefenseModifier.ToString();
            armorString += EvadePourc.ToString() + ", ";
            armorString += MagicDefenseValue.ToString() + ", ";
            armorString += MagicEvadePourc.ToString() + ", ";
            //foreach (BaseJob t in allowableJobs)
            //    armorString += ", " + t.JobName;
            armorString += ", " + AllowableJobs;
            return armorString;
        }
        #endregion
    }
}
