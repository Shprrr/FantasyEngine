using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace FantasyEngineData.Items
{
    public class Weapon : BaseItem
    {
        #region Field Region
        Hands hands;
        int damage;
        //int damageModifier;
        int hitPourc;
        //int attackModifier;
        #endregion

        #region Property Region
        public Hands NumberHands
        {
            get { return hands; }
            set { hands = value; }
        }
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        //public int DamageModifier
        //{
        //    get { return damageModifier; }
        //    protected set { damageModifier = value; }
        //}
        public int HitPourc
        {
            get { return hitPourc; }
            set { hitPourc = value; }
        }
        //public int AttackModifier
        //{
        //    get { return attackModifier; }
        //    protected set { attackModifier = value; }
        //}
        #endregion

        #region Constructor Region
        public Weapon()
            : base()
        {
        }

        public Weapon(
                string weaponName,
                string weaponType,
                Texture2D icon,
                int price,
                float weight,
                string description,
                Hands hands,
                int damage,
            //int damageModifier,
                int hitPourc,
            //int attackModifier,
                string allowableJobs,
                Effect effect)
            : base(weaponName, weaponType, icon, price, weight, description, allowableJobs, effect)
        {
            NumberHands = hands;
            Damage = damage;
            //DamageModifier = damageModifier;
            HitPourc = hitPourc;
            //AttackModifier = attackModifier;
        }
        #endregion

        #region Abstract Method Region
        public override object Clone()
        {
            Weapon weapon = new Weapon(
                Name,
                Type,
                Icon,
                Price,
                Weight,
                Description,
                NumberHands,
                Damage,
                //DamageModifier,
                HitPourc,
                //AttackModifier,
                AllowableJobs,
                Effect);
            return weapon;
        }

        public override string ToString()
        {
            string weaponString = base.ToString() + ", ";
            weaponString += NumberHands.ToString() + ", ";
            weaponString += Damage.ToString() + ", ";
            //weaponString += DamageModifier.ToString();
            weaponString += HitPourc.ToString() + "%, ";
            //weaponString += AttackModifier.ToString() + ", ";
            //foreach (BaseJob t in allowableJobs)
            //    weaponString += ", " + t.JobName;
            weaponString += ", " + AllowableJobs;
            return base.ToString();
        }
        #endregion
    }
}
