using System;
using System.Collections.Generic;
using System.Linq;

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
            LEFT
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

        public Status Statut
        {
            get { return CurrentJob != null ? CurrentJob.Statut : Status.Normal; }
            set { if (CurrentJob != null) CurrentJob.Statut = value; }
        }

        public bool IsDead { get { return Hp == 0; } }
        #endregion Properties

        public Character()
        {
        }

        #region Battle Stats
        /// <summary>
        /// Get the base damage of the current job with the current equipement.
        /// </summary>
        /// <param name="damageOption"></param>
        /// <returns></returns>
        public int getBaseDamage(eDamageOption damageOption)
        {
            // 1 barehanded
            return (Strength / 4) + (Level / 4) + 1/*weapon.Damage*/;
        }

        /// <summary>
        /// Get the hit pourcentage of the current job with the current equipement.
        /// </summary>
        /// <param name="damageOption"></param>
        /// <returns></returns>
        public int getHitPourc(eDamageOption damageOption)
        {
            // 80% barehanded
            return (Accuracy / 4) + (Level / 4) + 80/*weapon.HitPourc*/;
        }

        /// <summary>
        /// Get the maximum hit number times of the current job with the current equipement.
        /// </summary>
        /// <returns></returns>
        public int getAttackMultiplier()
        {
            int attMul = (Agility / 16) + (Level / 16) + 1;
            return attMul < 16 ? attMul : 16;
        }

        /// <summary>
        /// Get the defense of the current job with the current equipement.
        /// </summary>
        /// <returns></returns>
        public int getDefenseDamage()
        {
            return (Vitality / 2) + 0/*armors.Defense*/;
        }

        /// <summary>
        /// Get the evade pourcentage of the current job with the current equipement.
        /// </summary>
        /// <returns></returns>
        public int getEvadePourc()
        {
            return (Agility / 4) + 0/*armors.EvadePourc*/;
        }

        /// <summary>
        /// Get the maximum block number times of the current job with the current equipement.
        /// </summary>
        /// <returns></returns>
        public int getDefenseMultiplier()
        {
            return /*getNbShield() > 0 ? ((Agility / 16) + (Level / 16) + 1) * getNbShield() :*/
                (Agility / 32) + (Level / 32);
        }

        /// <summary>
        /// Get the magic base damage of the current job with the current equipement.
        /// </summary>
        /// <returns></returns>
        public int getMagicBaseDamage()
        {
            //return (Intelligence / 2) + 1; //FF3
            return (Intelligence / 4) + (Level / 4) + 1;
        }

        /// <summary>
        /// Get the magic hit pourcentage of the current job with the current equipement.
        /// </summary>
        /// <returns></returns>
        public int getMagicHitPourc()
        {
            // 80% barehanded
            //return (Intelligence / 2) + 80; //FF3
            return (Intelligence / 8) + (Accuracy / 8) + (Level / 4) + 80/*magic.HitPourc*/;
        }

        /// <summary>
        /// Get the maximum magic hit number times of the current job with the current equipement.
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
            return (Wisdom / 2) + 0/*armors.Defense*/;
        }

        /// <summary>
        /// Get the magic evade pourcentage of the current job with the current equipement.
        /// </summary>
        /// <returns></returns>
        public int getMagicEvadePourc()
        {
            return (Agility / 8) + (Wisdom / 8) + 0/*armors.EvadePourc*/;
        }

        /// <summary>
        /// Get the maximum magic block number times of the current job with the current equipement.
        /// </summary>
        /// <returns></returns>
        public int getMagicDefenseMultiplier()
        {
            return /*getNbShield() > 0 ? ((Agility / 32) + (Wisdom / 32) + (Level / 16) + 1) * getNbShield() :*/
                (Agility / 64) + (Wisdom / 64) + (Level / 32);
            //return (Agility / 32) + (Wisdom / 16); //FF3
        }
        #endregion Battle Stats

        public override string ToString()
        {
            return Name + " [Lv:" + Level + "]";
        }

        #region ICloneable Membres

        public object Clone()
        {
            return this.CloneExt();
        }

        #endregion
    }
}
