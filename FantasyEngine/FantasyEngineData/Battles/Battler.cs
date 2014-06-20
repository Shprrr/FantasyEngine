﻿using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;
using FantasyEngineData.Skills;

namespace FantasyEngineData.Battles
{
	public class Battler : Character
	{
		/// <summary>
		/// Counter for CTB.  Tell the number of tick to wait for the next action.
		/// </summary>
		public int Counter;

		public float HasteStatus = 1;

		public int GoldToGive = 0;
		public List<BaseItem> Treasure = new List<BaseItem>();

		public int multiplierRH, multiplierLH;
		public int damageRH, damageLH;

		public bool IsActor { get; set; }

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
		}

		public Battler(Monster monster, int level)
			: base(monster.JobName)
		{
			Jobs[0] = new Job((BaseJob)monster.Clone(), level);

			monster.Drop.GetDrop(CurrentJob, ref GoldToGive, ref Treasure);
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
			multiplierRH = 0;
			multiplierLH = 0;
			damageRH = 0;
			damageLH = 0;

			if (attacker.RightHand is Weapon)
			{
				CalculatePhysicalDamage(attacker, ePhysicalDamageOption.RIGHT, out multiplierRH, out damageRH);
			}

			if (attacker.LeftHand is Weapon)
			{
				CalculatePhysicalDamage(attacker, ePhysicalDamageOption.LEFT, out multiplierLH, out damageLH);
			}

			if (!(attacker.RightHand is Weapon && attacker.LeftHand is Weapon))
			{
				CalculatePhysicalDamage(attacker, ePhysicalDamageOption.RIGHT, out multiplierRH, out damageRH);
			}
		}

		public void Used(Battler attacker, BaseItem item, int nbTarget)
		{
			multiplierRH = 0;
			multiplierLH = 0;
			damageRH = 0;
			damageLH = 0;

			if (item.Effect != null)
				multiplierRH = item.Effect.Use(attacker, this, out damageRH, nbTarget) ? 1 : 0;
		}

		public void Used(Battler attacker, Skill skill, int skillLevel, int nbTarget)
		{
			multiplierRH = 0;
			multiplierLH = 0;
			damageRH = 0;
			damageLH = 0;

			if (skill.Effect != null)
				multiplierRH = skill.Use(attacker, this, skillLevel, out damageRH, nbTarget) ? 1 : 0;
		}

		public void GiveDamage()
		{
			if (multiplierRH > 0)
			{
				Hp -= damageRH;
			}

			if (multiplierLH > 0)
			{
				Hp -= damageLH;
			}
		}

		public int ExpToGive()
		{
			return (int)(((CurrentJob.BaseJob.MaxHp / 4) + (CurrentJob.BaseJob.MaxMp / 2)
				+ Strength + Vitality + Accuracy + Agility + Intelligence + Wisdom + Math.Pow(Level, 2)) / 6);
		}
	}
}