using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData.Effects;
using FantasyEngineData.Items;
using FantasyEngineData.Skills;

namespace FantasyEngineData.Entities
{
	public class Character : ICloneable
	{
		public const int NAME_LENGTH = 32;
		public const int MAX_JOB = 8;

		#region Fields
		private string _Name;
		private Job[] _Jobs = new Job[MAX_JOB];
		private List<Skill> _Skills = new List<Skill>();

		public int IndexCurrentJob = -1;
		#endregion Fields

		#region Properties
		public Job[] Jobs { get { return _Jobs; } }

		/// <summary>
		/// Job currently active for this character.
		/// </summary>
		public Job CurrentJob
		{
			get { return IndexCurrentJob > -1 && IndexCurrentJob < MAX_JOB ? Jobs[IndexCurrentJob] : null; }
		}

		public List<Skill> Skills { get { return _Skills; } }

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
			set
			{
				if (CurrentJob == null) return;
				int hpBefore = CurrentJob.Hp;
				CurrentJob.Hp = value;
				if (hpBefore > 0 && CurrentJob.Hp <= 0) RaiseOnDead();
				if (hpBefore <= 0 && CurrentJob.Hp > 0) RaiseOnRevive();
			}
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

		public enum eEquipSlot
		{
			RightHand,
			LeftHand,
			Head,
			Body,
			Arms,
			Feet
		}

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

		public Character(string name)
		{
			Name = name;
			Jobs[0] = new Job(JobManager.GetBaseJob("OK"));
			IndexCurrentJob = 0;
			JobSort();
		}

		/// <summary>
		/// Permet de trier le tableau des jobs pour correspondre à l'ordre officiel suite à une manipulation (ajout ou supression).
		/// </summary>
		public void JobSort()
		{
			BaseJob[] baseJobs = JobManager.GetAllBaseJob();
			Array.Sort(Jobs, delegate(Job x, Job y)
				{
					if (x == null)
					{
						if (y == null)
							return 0;
						else
							return 1;
					}
					else
					{
						if (y == null)
							return -1;
						else
						{
							if (x.BaseJob == y.BaseJob)
								return 0;

							BaseJob firstBaseJob = baseJobs.First(bj => bj == x.BaseJob || bj == y.BaseJob);
							return firstBaseJob == x.BaseJob ? -1 : (firstBaseJob == y.BaseJob ? 1 : 0);
						}
					}
				});
		}

		#region Battle Stats
		/// <summary>
		/// Get the base damage of the current job with the current equipment.
		/// </summary>
		/// <param name="damageOption"></param>
		/// <returns></returns>
		public int getBaseDamage(ePhysicalDamageOption damageOption)
		{
			int weaponDamage = 0;

			if (damageOption != ePhysicalDamageOption.LEFT && RightHand is Weapon)
				weaponDamage += ((Weapon)RightHand).Damage;
			if (damageOption != ePhysicalDamageOption.RIGHT && LeftHand is Weapon)
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
		public int getHitPourc(ePhysicalDamageOption damageOption)
		{
			int weaponHitPourc = 0;

			if (damageOption != ePhysicalDamageOption.LEFT && RightHand is Weapon)
				weaponHitPourc += ((Weapon)RightHand).HitPourc;
			if (damageOption != ePhysicalDamageOption.RIGHT && LeftHand is Weapon)
				weaponHitPourc += ((Weapon)LeftHand).HitPourc;
			if (damageOption == ePhysicalDamageOption.BOTH && RightHand is Weapon && LeftHand is Weapon)
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
		/// <param name="damageOption"></param>
		/// <param name="spellDamage"></param>
		/// <returns></returns>
		public int getMagicBaseDamage(eMagicalDamageOption damageOption, int spellDamage)
		{
			//return (Intelligence / 2) + spellDamage; //FF3
			switch (damageOption)
			{
				case eMagicalDamageOption.BLACK:
					return (Intelligence / 4) + (Level / 4) + spellDamage;
				case eMagicalDamageOption.WHITE:
					return (Wisdom / 4) + (Level / 4) + spellDamage;
				default:
					return (Intelligence / 8) + (Wisdom / 8) + (Level / 4) + spellDamage;
			}
		}

		/// <summary>
		/// Get the magic hit pourcentage of the current job with the current equipment.
		/// </summary>
		/// <returns></returns>
		public int getMagicHitPourc(eMagicalDamageOption damageOption, int spellHitPourc)
		{
			// 80% barehanded
			//return (Intelligence / 2) + spellHitPourc; //FF3
			switch (damageOption)
			{
				case eMagicalDamageOption.BLACK:
					return (Intelligence / 8) + (Accuracy / 8) + (Level / 4) + spellHitPourc;
				case eMagicalDamageOption.WHITE:
					return (Wisdom / 8) + (Accuracy / 8) + (Level / 4) + spellHitPourc;
				default:
					return (Intelligence / 16) + (Wisdom / 16) + (Accuracy / 8) + (Level / 4) + spellHitPourc;
			}
		}

		/// <summary>
		/// Get the maximum magic hit number times of the current job with the current equipment.
		/// </summary>
		/// <returns></returns>
		public int getMagicAttackMultiplier(eMagicalDamageOption damageOption)
		{
			int attMul = 0;
			switch (damageOption)
			{
				case eMagicalDamageOption.BLACK:
					attMul = (Intelligence / 16) + (Level / 16) + 1;
					break;
				case eMagicalDamageOption.WHITE:
					attMul = (Wisdom / 16) + (Level / 16) + 1;
					break;
				default:
					attMul = (Intelligence / 32) + (Wisdom / 32) + (Level / 16) + 1;
					break;
			}
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

		public void CalculatePhysicalDamage(Character attacker, ePhysicalDamageOption damageOption, out Damage damage)
		{
			damage = new Damage();
			damage.Type = Damage.eDamageType.HP;
			damage.User = this;

			//Calculate min and max base damage
			int baseMinDmg = attacker.getBaseDamage(damageOption);

			//Bonus on base damage for Attacker
			//baseMinDmg += HasCheer ? 10 * CheerLevel : 0;
			//ou
			//baseMinDmg += HasCheer ? baseMinDmg * CheerLevel / 15 : 0;
			//baseMinDmg *= IsAlly ? 2 : 1;
			//baseMinDmg *= ElementalEffect(attacker);
			//baseMinDmg *= IsMini || IsToad ? 2 : 1;
			//baseMinDmg *= attacker->IsMini || attacker->IsToad ? 0 : 1;

			int baseMaxDmg = (int)(baseMinDmg * 1.5);

			//Calculate hit%
			int hitPourc = attacker.getHitPourc(damageOption);
			hitPourc = (hitPourc < 99 ? hitPourc : 99);
			//hitPourc /= (attacker.IsFrontRow || weapon.IsLongRange ? 1 : 2);
			//hitPourc /= (blindStatus ? 2 : 1);
			//hitPourc /= (IsFrontRow || weapon.IsLongRange ? 1 : 2);

			//Calculate attack multiplier
			damage.Multiplier = 0;
			for (int i = 0; i < attacker.getAttackMultiplier(); i++)
				if (Extensions.rand.Next(0, 100) < hitPourc)
					damage.Multiplier++;

			//Bonus on defense for Target
			int defense = getDefenseDamage();
			//defense *= (IsDefending ? 4 : 1);
			//defense *= (IsAlly ? 0 : 1);
			//defense *= (IsRunning ? 0 : 1);
			//defense *= (IsMini || IsToad ? 0 : 1);

			//Calculate defense multiplier
			int defenseMul = getDefenseMultiplier();
			//defenseMul *= (IsAlly ? 0 : 1);
			//defenseMul *= (IsRunning ? 0 : 1);
			//defenseMul *= (IsMini || IsToad ? 0 : 1);

			//Calculate multiplier and final damage
			for (int i = 0; i < defenseMul; i++)
				if (Extensions.rand.Next(0, 100) < getEvadePourc())
					damage.Multiplier--;

			damage.Value = (Extensions.rand.Next(baseMinDmg, baseMaxDmg + 1) - defense) * damage.Multiplier;
			//damage *= AttackIsJump ? 3 : 1;

			//Validate final damage and multiplier
			if (damage.Value < 1) //Min 1 s'il tape au moins une fois
				damage.Value = 1;

			if (damage.Multiplier < 1) //Check s'il tape au moins une fois
				damage.Value = 0;
		}

		public void CalculateMagicalDamage(Character attacker, eMagicalDamageOption damageOption, int spellDamage, int spellHitPourc, int nbTarget, ref Damage damage)
		{
			bool isItem = spellHitPourc == 100;
			damage.Type = Damage.eDamageType.HP;

			//Calculate min and max base damage
			int baseMinDmg = attacker.getMagicBaseDamage(damageOption, spellDamage);

			//Bonus on base damage for Attacker
			//baseMinDmg *= ElementalEffect(attacker);
			//baseMinDmg *= IsMini || IsToad ? 2 : 1;

			int baseMaxDmg = (int)(baseMinDmg * 1.5);

			//Calculate hit%
			int hitPourc = 0;
			if (isItem)
				hitPourc = 100;
			else
			{
				hitPourc = attacker.getMagicHitPourc(damageOption, spellHitPourc);
				hitPourc = (hitPourc < 99 ? hitPourc : 99);
			}

			//Calculate attack multiplier
			if (!isItem)
			{
				damage.Multiplier = 0;
				for (int i = 0; i < attacker.getMagicAttackMultiplier(damageOption); i++)
					if (Extensions.rand.Next(0, 100) < hitPourc)
						damage.Multiplier++;
			}

			//Bonus on defense for Target
			int defense = getMagicDefenseDamage();
			//defense *= (IsDefending ? 4 : 1);
			//defense *= (IsAlly ? 0 : 1);
			//defense *= (IsRunning ? 0 : 1);
			//defense *= (IsMini || IsToad ? 0 : 1);

			//Calculate defense multiplier
			int defenseMul = getMagicDefenseMultiplier();
			//defenseMul *= (IsAlly ? 0 : 1);
			//defenseMul *= (IsRunning ? 0 : 1);
			//defenseMul *= (IsMini || IsToad ? 0 : 1);

			//Calculate multiplier and final damage
			if (!isItem)
				for (int i = 0; i < defenseMul; i++)
					if (Extensions.rand.Next(0, 100) < getMagicEvadePourc())
						damage.Multiplier--;

			damage.Value = (Extensions.rand.Next(baseMinDmg, baseMaxDmg + 1) - defense) * damage.Multiplier;

			damage.Value /= nbTarget;

			//Validate final damage and multiplier
			if (damage.Value < 1) //Min 1 s'il tape au moins une fois
				damage.Value = 1;

			if (damage.Multiplier < 1) //Check s'il tape au moins une fois
				damage.Value = 0;
		}

		public void Unequip(eEquipSlot slot, Inventory inventory)
		{
			BaseItem lastItemEquiped = null;
			switch (slot)
			{
				case eEquipSlot.RightHand:
					lastItemEquiped = RightHand;
					RightHand = null;
					break;
				case eEquipSlot.LeftHand:
					lastItemEquiped = LeftHand;
					LeftHand = null;
					break;
				case eEquipSlot.Head:
					lastItemEquiped = Head;
					Head = null;
					break;
				case eEquipSlot.Body:
					lastItemEquiped = Body;
					Body = null;
					break;
				case eEquipSlot.Arms:
					lastItemEquiped = Arms;
					Arms = null;
					break;
				case eEquipSlot.Feet:
					lastItemEquiped = Feet;
					Feet = null;
					break;
			}

			if (lastItemEquiped != null)
				inventory.Add(lastItemEquiped);
		}

		public void UnequipAll(Inventory inventory)
		{
			Unequip(eEquipSlot.RightHand, inventory);
			Unequip(eEquipSlot.LeftHand, inventory);
			Unequip(eEquipSlot.Head, inventory);
			Unequip(eEquipSlot.Body, inventory);
			Unequip(eEquipSlot.Arms, inventory);
			Unequip(eEquipSlot.Feet, inventory);
		}

		public override string ToString()
		{
			return Name + " [Lv:" + Level + "]";
		}

		public static bool IsNullOrDead(Character character)
		{
			return character == null || character.IsDead;
		}

		#region Events
		public event EventHandler OnEquipmentChanged;
		protected virtual void RaiseOnEquipmentChanged()
		{
			// Raise the event by using the () operator.
			if (OnEquipmentChanged != null)
				OnEquipmentChanged(this, new EventArgs());
		}

		public event EventHandler OnDead;
		protected virtual void RaiseOnDead()
		{
			// Raise the event by using the () operator.
			if (OnDead != null)
				OnDead(this, new EventArgs());
		}

		public event EventHandler OnRevive;
		protected virtual void RaiseOnRevive()
		{
			// Raise the event by using the () operator.
			if (OnRevive != null)
				OnRevive(this, new EventArgs());
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
