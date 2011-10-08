using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace FantasyEngineData.Items
{
    public class Shield : BaseItem
    {
        #region Field Region
        int defenseValue;
        //int defenseModifier;
        int evadePourc;
        int magicDefenseValue;
        int magicEvadePourc;
        #endregion

        #region Property Region
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
        public Shield()
            : base()
        {
        }

        public Shield(
                string shieldName,
                string shieldType,
                Texture2D icon,
                int price,
                float weight,
                string description,
                int defenseValue,
            //int defenseModifier,
                int evadePourc,
                int magicDefenseValue,
                int magicEvadePourc,
                string allowableJobs,
                Effect effect)
            : base(shieldName, shieldType, icon, price, weight, description, allowableJobs, effect)
        {
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
            Shield shield = new Shield(
                Name,
                Type,
                Icon,
                Price,
                Weight,
                Description,
                DefenseValue,
                //DefenseModifier,
                EvadePourc,
                MagicDefenseValue,
                MagicEvadePourc,
                AllowableJobs,
                Effect);
            return shield;
        }

        public override string ToString()
        {
            string shieldString = base.ToString() + ", ";
            shieldString += DefenseValue.ToString() + ", ";
            //shieldString += DefenseModifier.ToString();
            shieldString += EvadePourc.ToString() + ", ";
            shieldString += MagicDefenseValue.ToString() + ", ";
            shieldString += MagicEvadePourc.ToString() + ", ";
            //foreach (BaseJob t in allowableJobs)
            //    shieldString += ", " + t.JobName;
            shieldString += ", " + AllowableJobs;
            return shieldString;
        }
        #endregion
    }
}
