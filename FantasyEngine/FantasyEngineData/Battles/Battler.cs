using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData.Effects;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;
using FantasyEngineData.Skills;

namespace FantasyEngineData.Battles
{
	public class Battler : Character
	{
		private SortedList<Status.eStatus, Status> _Statuses = new SortedList<Status.eStatus, Status>();

		/// <summary>
		/// Counter for CTB.  Tell the number of tick to wait for the next action.
		/// </summary>
		public int Counter;

		public float HasteStatus = 1;

		public int GoldToGive = 0;
		public List<BaseItem> Treasure = new List<BaseItem>();

		public Damage damageR, damageL;

		public bool IsActor { get; set; }

		public SortedList<Status.eStatus, Status> Statuses
		{
			get { return _Statuses; }
		}

		public string GetPrimaryStatus()
		{
			if (Statuses.Count > 0)
				return Statuses.First().Value.ToString();
			return "Normal";
		}

		public Battler(Character character, bool isActor)
			: base(character.Name)
		{
			IsActor = isActor;
			//TODO: Clone ?

			for (int i = 0; i < MAX_JOB; i++)
			{
				Jobs[i] = character.Jobs[i];
			}

			IndexCurrentJob = character.IndexCurrentJob;

			RightHand = character.RightHand;
			LeftHand = character.LeftHand;
			Head = character.Head;
			Body = character.Body;
			Arms = character.Arms;
			Feet = character.Feet;

			Skills.AddRange(character.Skills);

			OnDead += Battler_OnDead;
			OnRevive += Battler_OnRevive;
		}

		public Battler(Monster monster, int level)
			: base(monster.JobName)
		{
			Jobs[0] = new Job((BaseJob)monster.Clone(), level);

			monster.Drop.GetDrop(CurrentJob, ref GoldToGive, ref Treasure);

			OnDead += Battler_OnDead;
			OnRevive += Battler_OnRevive;
		}

		public int getTickSpeed()
		{
			int agility = Agility;

			if (agility == 0)
				return 28;

			if (agility == 1)
				return 26;

			if (agility == 2)
				return 24;

			if (agility == 3)
				return 22;

			if (agility == 4)
				return 20;

			if (agility >= 5 && agility <= 6)
				return 16;

			if (agility >= 7 && agility <= 9)
				return 15;

			if (agility >= 10 && agility <= 11)
				return 14;

			if (agility >= 12 && agility <= 14)
				return 13;

			if (agility >= 15 && agility <= 16)
				return 12;

			if (agility >= 17 && agility <= 18)
				return 11;

			if (agility >= 19 && agility <= 22)
				return 10;

			if (agility >= 23 && agility <= 28)
				return 9;

			if (agility >= 29 && agility <= 34)
				return 8;

			if (agility >= 35 && agility <= 43)
				return 7;

			if (agility >= 44 && agility <= 61)
				return 6;

			if (agility >= 62 && agility <= 97)
				return 5;

			if (agility >= 98 && agility <= 169)
				return 4;

			if (agility >= 170 && agility <= 255)
				return 3;

			return 0;
		}

		/// <summary>
		/// Calculate the Initial Counter Value for the CTB system.
		/// </summary>
		public void CalculateICV()
		{
			int TS = getTickSpeed();
			int minICV = 3 * TS;
			int maxICV = 30 * TS / 9;

			Counter = Extensions.rand.Next(minICV, maxICV + 1);
		}

		public int getCounterValue(int rank)
		{
			return (int)(getTickSpeed() * rank * HasteStatus);
		}

		public void Attacked(Battler attacker)
		{
			damageR = Damage.Empty;
			damageL = Damage.Empty;

			if (attacker.RightHand is Weapon)
			{
				CalculatePhysicalDamage(attacker, ePhysicalDamageOption.RIGHT, out damageR);
			}

			if (attacker.LeftHand is Weapon)
			{
				CalculatePhysicalDamage(attacker, ePhysicalDamageOption.LEFT, out damageL);
			}

			if (!(attacker.RightHand is Weapon && attacker.LeftHand is Weapon))
			{
				CalculatePhysicalDamage(attacker, ePhysicalDamageOption.RIGHT, out damageR);
			}
		}

		public void Used(Battler attacker, BaseItem item, int nbTarget)
		{
			damageR = Damage.Empty;
			damageL = Damage.Empty;

			if (item.Effect != null)
				item.Effect.CalculateDamage(attacker, this, out damageR, nbTarget);
			//TODO: Faire le drop ici ?
		}

		public void Used(Battler attacker, Skill skill, int skillLevel, int nbTarget)
		{
			damageR = Damage.Empty;
			damageL = Damage.Empty;

			if (skill.Effect != null)
				skill.CalculateDamage(attacker, this, skillLevel, out damageR, nbTarget);
		}

		public void GiveDamage()
		{
			damageR.ApplyDamage(this);
			damageL.ApplyDamage(this);
		}

		public int ExpToGive()
		{
			return (int)(((CurrentJob.BaseJob.MaxHp / 4) + (CurrentJob.BaseJob.MaxMp / 2)
				+ Strength + Vitality + Accuracy + Agility + Intelligence + Wisdom + Math.Pow(Level, 2)) / 6);
		}

		private void Battler_OnDead(object sender, EventArgs e)
		{
			Counter = 0; // CTB system in Battle will clean itself.

			for (int i = 0; i < Statuses.Count; i++)
			{
				Statuses.RemoveAt(i);
			}
			Statuses.Add(Status.eStatus.KO, new Status(Status.eStatus.KO));
		}

		private void Battler_OnRevive(object sender, EventArgs e)
		{
			Counter = getCounterValue(CTBTurn.RANK_DEFAULT); // Supposed to be : Reviving is rank=3*tickSpeed without Haste/Slow even with Auto-Haste.
			Statuses[Status.eStatus.KO].OnCure(this);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool IsNullDeadOrStone(Battler character)
		{
			return Character.IsNullOrDead(character) || character.Statuses.ContainsKey(Status.eStatus.Stone);
		}

		public static bool operator ==(Battler b1, Battler b2)
		{
			// Change to have breaked stone battler considered as null.
			if ((object)b2 == null)
				return (object)b1 == null || b1.Statuses.Any(s => s.Key == Status.eStatus.Stone && s.Value.TurnToLive <= 0);

			if ((object)b1 == null)
				return (object)b2 == null || b2.Statuses.Any(s => s.Key == Status.eStatus.Stone && s.Value.TurnToLive <= 0);

			return b1.Equals(b2);
		}

		public static bool operator !=(Battler b1, Battler b2)
		{
			return !(b1 == b2);
		}
	}
}
