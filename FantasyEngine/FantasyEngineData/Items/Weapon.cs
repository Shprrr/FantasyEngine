using System;
using System.Collections.Generic;
using System.Linq;

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
        //TODO: Elements, Effets
        #endregion

        #region Constructor Region
        public Weapon()
            : base()
        {
        }

        public Weapon(
                string weaponName,
                string weaponType,
                int price,
                float weight,
                Hands hands,
                int damage,
            //int damageModifier,
                int hitPourc,
            //int attackModifier,
                params BaseJob[] allowableClasses)
            : base(weaponName, weaponType, price, weight, allowableClasses)
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
            BaseJob[] allowedClasses = new BaseJob[allowableJobs.Count];
            for (int i = 0; i < allowableJobs.Count; i++)
                allowedClasses[i] = allowableJobs[i];
            Weapon weapon = new Weapon(
            Name,
            Type,
            Price,
            Weight,
            NumberHands,
            Damage,
                //DamageModifier,
            HitPourc,
                //AttackModifier,
            allowedClasses);
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
            foreach (BaseJob t in allowableJobs)
                weaponString += ", " + t.JobName;
            return base.ToString();
        }
        #endregion
    }
}
