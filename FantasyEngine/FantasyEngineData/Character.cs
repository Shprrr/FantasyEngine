using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData.Items;

namespace FantasyEngineData
{
    public class Character : ICloneable
    {
        public const int NAME_LENGTH = 32;
        public const int MAX_JOB = 4;

        //TODO: Transferer dans les Weapons
        public enum eDamageOption
        {
            RIGHT,
            LEFT,
            BOTH
        }

        #region Fields
        private string _Name;

        public Job[] Jobs = new Job[MAX_JOB];
        public int IndexCurrentJob = -1;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Job currently active for this character.
        /// </summary>
        public Job CurrentJob
        {
            get { return IndexCurrentJob > -1 && IndexCurrentJob < MAX_JOB ? Jobs[IndexCurrentJob] : null; }
        }

        /// <summary>
        /// Get the name of the character.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value.Length >= NAME_LENGTH)
                    value = value.Remove(NAME_LENGTH);
                _Name = value;
            }
        }

        #region Stats
        public int Level
        {
            get { return CurrentJob != null ? CurrentJob.Level : 0; }
            set { if (CurrentJob != null) CurrentJob.Level = value; }
        }

        public int TotalExp
        {
            get { return CurrentJob != null ? CurrentJob.TotalExp : 0; }
            set { if (CurrentJob != null) CurrentJob.TotalExp = value; }
        }

        /// <summary>
        /// Amount of experience for the current level of the current job.
        /// </summary>
        public int Exp
        {
            get { return CurrentJob != null ? CurrentJob.Exp : 0; }
            set { if (CurrentJob != null) CurrentJob.Exp = value; }
        }

        public int Hp
        {
            get { return CurrentJob != null ? CurrentJob.Hp : 0; }
            set { if (CurrentJob != null) CurrentJob.Hp = value; }
        }

        public int MaxHp
        {
            get { return CurrentJob != null ? CurrentJob.MaxHp : 0; }
        }

        public int Mp
        {
            get { return CurrentJob != null ? CurrentJob.Mp : 0; }
            set { if (CurrentJob != null) CurrentJob.Mp = value; }
        }

        public int MaxMp
        {
            get { return CurrentJob != null ? CurrentJob.MaxMp : 0; }
        }

        public int Strength
        {
            get { return CurrentJob != null ? CurrentJob.Strength : 0; }
            set { if (CurrentJob != null) CurrentJob.Strength = value; }
        }

        public int Vitality
        {
            get { return CurrentJob != null ? CurrentJob.Vitality : 0; }
            set { if (CurrentJob != null) CurrentJob.Vitality = value; }
        }

        public int Accuracy
        {
            get { return CurrentJob != null ? CurrentJob.Accuracy : 0; }
            set { if (CurrentJob != null) CurrentJob.Accuracy = value; }
        }

        public int Agility
        {
            get { return CurrentJob != null ? CurrentJob.Agility : 0; }
            set { if (CurrentJob != null) CurrentJob.Agility = value; }
        }

        public int Intelligence
        {
            get { return CurrentJob != null ? CurrentJob.Intelligence : 0; }
            set { if (CurrentJob != null) CurrentJob.Intelligence = value; }
        }

        public int Wisdom
        {
            get { return CurrentJob != null ? CurrentJob.Wisdom : 0; }
            set { if (CurrentJob != null) CurrentJob.Wisdom = value; }
        }

        /// <summary>
        /// Stat points that still need to be allocated.
        /// </summary>
        public int StatRemaining
        {
            get { return CurrentJob != null ? CurrentJob.StatRemaining : 0; }
            set { if (CurrentJob != null) CurrentJob.StatRemaining = value; }
        }
        #endregion Stats

        public Status Statut
        {
            get { return CurrentJob != null ? CurrentJob.Statut : Status.Normal; }
            set { if (CurrentJob != null) CurrentJob.Statut = value; }
        }

        public bool IsDead { get { return Hp == 0; } }

        #region Equipments
        private BaseItem _RightHand = null;
        private BaseItem _LeftHand = null;
        private Armor _Head = null;
        private Armor _Body = null;
        private Armor _Arms = null;
        private Armor _Feet = null;

        /// <summary>
        /// 
        /// </summary>
        public BaseItem RightHand
        {
            get { return _RightHand; }
            set
            {
                if (value == null && _RightHand != null)
                {
                    _RightHand.IsEquiped = false;
                    _RightHand = value;
                    RaiseOnEquipmentChanged();
                }

                if ((value is Weapon || value is Shield)
                    && value.IsAllowed(CurrentJob.BaseJob))
                {
                    value.IsEquiped = true;
                    _RightHand = value;
                    RaiseOnEquipmentChanged();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BaseItem LeftHand
        {
            get { return _LeftHand; }
            set
            {
                if (value == null && _LeftHand != null)
                {
                    _LeftHand.IsEquiped = false;
                    _LeftHand = value;
                    RaiseOnEquipmentChanged();
                }

                if ((value is Weapon || value is Shield)
                    && value.IsAllowed(CurrentJob.BaseJob))
                {
                    value.IsEquiped = true;
                    _LeftHand = value;
                    RaiseOnEquipmentChanged();
                }
            }
        }

        public Armor Head
        {
            get { return _Head; }
            set
            {
                if (value == null && _Head != null)
                {
                    _Head.IsEquiped = false;
                    _Head = value;
                    RaiseOnEquipmentChanged();
                }

                if (value is Armor
                   && value.IsAllowed(CurrentJob.BaseJob)
                   && value.Location == ArmorLocation.Head)
                {
                    value.IsEquiped = true;
                    _Head = value;
                    RaiseOnEquipmentChanged();
                }
            }
        }

        public Armor Body
        {
            get { return _Body; }
            set
            {
                if (value == null && _Body != null)
                {
                    _Body.IsEquiped = false;
                    _Body = value;
                    RaiseOnEquipmentChanged();
                }

                if (value is Armor
                   && value.IsAllowed(CurrentJob.BaseJob)
                   && value.Location == ArmorLocation.Body)
                {
                    value.IsEquiped = true;
                    _Body = value;
                    RaiseOnEquipmentChanged();
                }
            }
        }

        public Armor Arms
        {
            get { return _Arms; }
            set
            {
                if (value == null && _Arms != null)
                {
                    _Arms.IsEquiped = false;
                    _Arms = value;
                    RaiseOnEquipmentChanged();
                }

                if (value is Armor
                   && value.IsAllowed(CurrentJob.BaseJob)
                   && value.Location == ArmorLocation.Arms)
                {
                    value.IsEquiped = true;
                    _Arms = value;
                    RaiseOnEquipmentChanged();
                }
            }
        }

        public Armor Feet
        {
            get { return _Feet; }
            set
            {
                if (value == null && _Feet != null)
                {
                    _Feet.IsEquiped = false;
                    _Feet = value;
                    RaiseOnEquipmentChanged();
                }

                if (value is Armor
                    && value.IsAllowed(CurrentJob.BaseJob)
                    && value.Location == ArmorLocation.Feet)
                {
                    value.IsEquiped = true;
                    _Feet = value;
                    RaiseOnEquipmentChanged();
                }
            }
        }
        #endregion Equipments
        #endregion Properties

        public Character()
        {
        }

        #region Battle Stats
        /// <summary>
        /// Get the base damage of the current job with the current equipment.
        /// </summary>
        /// <param name="damageOption"></param>
        /// <returns></returns>
        public int getBaseDamage(eDamageOption damageOption)
        {
            int weaponDamage = 0;

            if (damageOption != eDamageOption.LEFT && RightHand is Weapon)
                weaponDamage += ((Weapon)RightHand).Damage;
            if (damageOption != eDamageOption.RIGHT && LeftHand is Weapon)
                weaponDamage += ((Weapon)LeftHand).Damage;
            if (RightHand == null && LeftHand == null) // Est-ce que Shield est barehand ?
                weaponDamage = 1; // Barehand

            return (Strength / 4) + (Level / 4) + weaponDamage;
        }

        /// <summary>
        /// Get the hit pourcentage of the current job with the current equipment.
        /// </summary>
        /// <param name="damageOption"></param>
        /// <returns></returns>
        public int getHitPourc(eDamageOption damageOption)
        {
            int weaponHitPourc = 0;

            if (damageOption != eDamageOption.LEFT && RightHand is Weapon)
                weaponHitPourc += ((Weapon)RightHand).HitPourc;
            if (damageOption != eDamageOption.RIGHT && LeftHand is Weapon)
                weaponHitPourc += ((Weapon)LeftHand).HitPourc;
            if (damageOption == eDamageOption.BOTH && RightHand is Weapon && LeftHand is Weapon)
                weaponHitPourc /= 2; // On a additionné 2 fois un 100%, donc on remet sur 100%
            if (RightHand == null && LeftHand == null) // Est-ce que Shield est barehand ?
                weaponHitPourc = 80; // Barehand

            return (Accuracy / 4) + (Level / 4) + weaponHitPourc;
        }

        /// <summary>
        /// Get the maximum hit number times of the current job with the current equipment.
        /// </summary>
        /// <returns></returns>
        public int getAttackMultiplier()
        {
            int attMul = (Agility / 16) + (Level / 16) + 1;
            return attMul < 16 ? attMul : 16;
        }

        /// <summary>
        /// Get the defense of the current job with the current equipment.
        /// </summary>
        /// <returns></returns>
        public int getDefenseDamage()
        {
            int armorsDefense = 0;

            if (Head != null)
                armorsDefense += Head.DefenseValue;
            if (Body != null)
                armorsDefense += Body.DefenseValue;
            if (Arms != null)
                armorsDefense += Arms.DefenseValue;
            if (Feet != null)
                armorsDefense += Feet.DefenseValue;
            if (RightHand is Shield)
                armorsDefense += ((Shield)RightHand).DefenseValue;
            if (LeftHand is Shield)
                armorsDefense += ((Shield)LeftHand).DefenseValue;

            return (Vitality / 2) + armorsDefense;
        }

        /// <summary>
        /// Get the evade pourcentage of the current job with the current equipment.
        /// </summary>
        /// <returns></returns>
        public int getEvadePourc()
        {
            int armorsEvadePourc = 0;

            if (Head != null)
                armorsEvadePourc += Head.EvadePourc;
            if (Body != null)
                armorsEvadePourc += Body.EvadePourc;
            if (Arms != null)
                armorsEvadePourc += Arms.EvadePourc;
            if (Feet != null)
                armorsEvadePourc += Feet.EvadePourc;
            if (RightHand is Shield)
                armorsEvadePourc += ((Shield)RightHand).EvadePourc;
            if (LeftHand is Shield)
                armorsEvadePourc += ((Shield)LeftHand).EvadePourc;

            return (Agility / 4) + armorsEvadePourc;
        }

        /// <summary>
        /// Get the number of shield currently equiped.
        /// </summary>
        /// <returns></returns>
        private int getNbShield()
        {
            int nbShield = 0;

            if (RightHand is Shield)
                nbShield++;
            if (LeftHand is Shield)
                nbShield++;

            return nbShield;
        }

        /// <summary>
        /// Get the maximum block number times of the current job with the current equipment.
        /// </summary>
        /// <returns></returns>
        public int getDefenseMultiplier()
        {
            return getNbShield() > 0 ? ((Agility / 16) + (Level / 16) + 1) * getNbShield() :
                (Agility / 32) + (Level / 32);
        }

        /// <summary>
        /// Get the magic base damage of the current job with the current equipment.
        /// </summary>
        /// <returns></returns>
        public int getMagicBaseDamage()
        {
            //return (Intelligence / 2) + 1; //FF3
            return (Intelligence / 4) + (Level / 4) + 1;
        }

        /// <summary>
        /// Get the magic hit pourcentage of the current job with the current equipment.
        /// </summary>
        /// <returns></returns>
        public int getMagicHitPourc()
        {
            // 80% barehanded
            //return (Intelligence / 2) + 80; //FF3
            return (Intelligence / 8) + (Accuracy / 8) + (Level / 4) + 80/*magic.HitPourc*/;
        }

        /// <summary>
        /// Get the maximum magic hit number times of the current job with the current equipment.
        /// </summary>
        /// <returns></returns>
        public int getMagicAttackMultiplier()
        {
            int attMul = (Intelligence / 16) + (Level / 16) + 1;
            return attMul < 16 ? attMul : 16;
        }

        /// <summary>
        /// Get the magic defense of the current job with the current equipement.
        /// </summary>
        /// <returns></returns>
        public int getMagicDefenseDamage()
        {
            int armorsDefense = 0;

            if (Head != null)
                armorsDefense += Head.MagicDefenseValue;
            if (Body != null)
                armorsDefense += Body.MagicDefenseValue;
            if (Arms != null)
                armorsDefense += Arms.MagicDefenseValue;
            if (Feet != null)
                armorsDefense += Feet.MagicDefenseValue;
            if (RightHand is Shield)
                armorsDefense += ((Shield)RightHand).MagicDefenseValue;
            if (LeftHand is Shield)
                armorsDefense += ((Shield)LeftHand).MagicDefenseValue;

            return (Wisdom / 2) + armorsDefense;
        }

        /// <summary>
        /// Get the magic evade pourcentage of the current job with the current equipment.
        /// </summary>
        /// <returns></returns>
        public int getMagicEvadePourc()
        {
            int armorsEvadePourc = 0;

            if (Head != null)
                armorsEvadePourc += Head.MagicEvadePourc;
            if (Body != null)
                armorsEvadePourc += Body.MagicEvadePourc;
            if (Arms != null)
                armorsEvadePourc += Arms.MagicEvadePourc;
            if (Feet != null)
                armorsEvadePourc += Feet.MagicEvadePourc;
            if (RightHand is Shield)
                armorsEvadePourc += ((Shield)RightHand).MagicEvadePourc;
            if (LeftHand is Shield)
                armorsEvadePourc += ((Shield)LeftHand).MagicEvadePourc;

            return (Agility / 8) + (Wisdom / 8) + armorsEvadePourc;
        }

        /// <summary>
        /// Get the maximum magic block number times of the current job with the current equipment.
        /// </summary>
        /// <returns></returns>
        public int getMagicDefenseMultiplier()
        {
            return getNbShield() > 0 ? ((Agility / 32) + (Wisdom / 32) + (Level / 16) + 1) * getNbShield() :
                (Agility / 64) + (Wisdom / 64) + (Level / 32);
            //return (Agility / 32) + (Wisdom / 16); //FF3
        }
        #endregion Battle Stats

        public override string ToString()
        {
            return Name + " [Lv:" + Level + "]";
        }

        #region Events
        public delegate void EquipmentChangedHandler(EventArgs e);
        public event EquipmentChangedHandler EquipmentChanged;
        protected virtual void RaiseOnEquipmentChanged()
        {
            // Raise the event by using the () operator.
            if (EquipmentChanged != null)
                EquipmentChanged(new EventArgs());
        }
        #endregion Events

        #region ICloneable Membres

        public object Clone()
        {
            return this.CloneExt();
        }

        #endregion
    }
}
