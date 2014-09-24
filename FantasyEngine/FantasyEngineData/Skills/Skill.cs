using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData;
using FantasyEngineData.Effects;
using FantasyEngineData.Entities;

namespace FantasyEngineData.Skills
{
	public class Skill : BaseSkill, ICloneable
	{
		public const int MAX_LEVEL = 20;

		private int _Level;
		private int _TotalExp;

		#region Properties
		public int Level
		{
			get { return _Level; }
			set
			{
				if (value > MAX_LEVEL)
					value = MAX_LEVEL;
				if (value < 0)
					value = 0;
				_Level = value;
				if (Effect != null)
					Effect.Level = _Level;
			}
		}

		/// <summary>
		/// Total amount of experience from the begining.
		/// </summary>
		public int TotalExp
		{
			get { return _TotalExp; }
			set
			{
				_TotalExp = value;
				LevelUp();
			}
		}

		/// <summary>
		/// Amount of experience for the current level.
		/// </summary>
		public int Exp
		{
			get { return _TotalExp - TotalExpForLevel(Level - 1); }
			set
			{
				_TotalExp = value + TotalExpForLevel(Level - 1);
				LevelUp();
			}
		}

		/// <summary>
		/// How much MP the skill cost to cast at its level.
		/// </summary>
		public int MPCost { get { return MPCostForLevel(Level); } }
		#endregion Properties

		public Skill()
		{

		}

		public Skill(BaseSkill baseSkill)
		{
			Name = baseSkill.Name;
			Description = baseSkill.Description;
			Rank = baseSkill.Rank;
			BaseCostMP = baseSkill.BaseCostMP;
			AddByLevelCostMP = baseSkill.AddByLevelCostMP;
			QuadraticCoefficientLevelCostMP = baseSkill.QuadraticCoefficientLevelCostMP;
			AllowableJobs.AddRange(baseSkill.AllowableJobs);
			Effect = baseSkill.Effect;
			DefaultTarget = baseSkill.DefaultTarget;
			MenuUsable = baseSkill.MenuUsable;
			ExpToLearn = baseSkill.ExpToLearn;
			Level = 0;
		}

		public override string ToString()
		{
			return Name + " [Lv:" + Level + "]";
		}

		/// <summary>
		/// Give the amount of experience to have to get from level to the next.
		/// </summary>
		/// <param name="level">Current level to calculate the exp for the next level.</param>
		/// <returns>Amount of experience to have to get from level to the next.</returns>
		public int ExpForLevel(int level)
		{
			if (level < 0)
				return 0;

			if (level == 0)
				return ExpToLearn;

			return (int)(39 * Math.Pow(level, 2));
		}

		/// <summary>
		/// Give the amount of total experience to have to get from level to the next.
		/// </summary>
		/// <param name="level">Current level to calculate the total exp for the next level.</param>
		/// <returns>Amount of total experience to have to get from level to the next.</returns>
		public int TotalExpForLevel(int level)
		{
			if (level < 0)
				return 0;

			if (level == 0)
				return ExpToLearn;

			return TotalExpForLevel(level - 1) + (int)(39 * Math.Pow(level, 2));
		}

		public void LevelUp()
		{
			// If the total exp is smaller than TotalExpForNextLevel
			// or it is already at the max level, it don't need to level up.
			while (TotalExp >= TotalExpForLevel(Level) && Level < MAX_LEVEL)
				Level++;
		}

		/// <summary>
		/// How much MP the skill cost to cast at a level.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public int MPCostForLevel(int level)
		{
			return (int)((QuadraticCoefficientLevelCostMP * Math.Pow(level, 2)) + (AddByLevelCostMP * level) + BaseCostMP);
		}

		public bool IsUsable(Character user, out int skillLevel)
		{
			skillLevel = Level;
			var jobAllowed = IsAllowed(user.CurrentJob.BaseJob);
			if (jobAllowed != null && jobAllowed.MaxLevel != 0 && Level >= jobAllowed.MaxLevel)
				skillLevel = jobAllowed.MaxLevel;
			return !(jobAllowed == null || (jobAllowed.Level != 0 && user.Level < jobAllowed.Level)
				|| (jobAllowed.Skill.Name != null && user.Skills.Find(s => s.Name == jobAllowed.Skill.Name).Level < jobAllowed.Skill.Level));
		}

		public bool Casting(Character attacker, out int skillLevel)
		{
			skillLevel = 0;
			if (attacker.Mp < MPCost || !IsUsable(attacker, out skillLevel))
				return false;

			attacker.Mp -= MPCost;
			return true;
		}

		public bool CalculateDamage(Character attacker, Character defender, int skillLevel, out Damage damage, int nbTarget = 1)
		{
			damage = Damage.Empty;
			return Effect.EffectForLevel(skillLevel).CalculateDamage(attacker, defender, out damage, nbTarget);
		}

		public bool Use(Character attacker, Character defender, int skillLevel)
		{
			return Effect.EffectForLevel(skillLevel).Use(attacker, defender);
		}

		#region ICloneable Membres

		public object Clone()
		{
			Skill skill = (Skill)this.CloneExt();
			skill.AllowableJobs.AddRange(AllowableJobs);
			return skill;
		}

		#endregion
	}
}
