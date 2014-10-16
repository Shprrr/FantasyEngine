using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData.Entities
{
	public class Job : ICloneable
	{
		public const int MAX_LEVEL = 50;
		public const int MAX_HP = 9999;
		public const int MAX_MP = 9999;
		public const int STAT_PER_LEVEL_UP = 6;

		public BaseJob BaseJob;
		private int _Level;
		private int _TotalExp;

		private int _Hp;
		private int _Mp;
		private int _Strength;
		private int _Vitality;
		private int _Accuracy;
		private int _Agility;
		private int _Intelligence;
		private int _Wisdom;
		private int _StatRemaining;

		#region Properties
		public string JobName
		{
			get { return BaseJob.JobName; }
			set { BaseJob.JobName = value; }
		}

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

		public int MaxHp
		{
			get
			{
				int maxHp = BaseJob.MaxHp + ((int)(Vitality / 4) * (Level - 1)) + ((Level - 1) * 10);
				return maxHp > MAX_HP ? MAX_HP : maxHp;
			}
		}

		public int Hp
		{
			get { return _Hp; }
			set
			{
				if (value > MaxHp)
					value = MaxHp;
				if (value < 0)
					value = 0;
				_Hp = value;
			}
		}

		public int MaxMp
		{
			get
			{
				int maxMp = BaseJob.MaxMp + ((int)(Wisdom / 4) * (Level - 1)) + ((Level - 1) * 5);
				return maxMp > MAX_MP ? MAX_MP : maxMp;
			}
		}

		public int Mp
		{
			get { return _Mp; }
			set
			{
				if (value > MaxMp)
					value = MaxMp;
				if (value < 0)
					value = 0;
				_Mp = value;
			}
		}

		public int Strength
		{
			get { return BaseJob.Strength + _Strength; }
			set
			{
				if (value > 255)
					value = 255;
				if (value < 0)
					value = 0;
				_Strength = value - BaseJob.Strength;
			}
		}

		public int Vitality
		{
			get { return BaseJob.Vitality + _Vitality; }
			set
			{
				if (value > 255)
					value = 255;
				if (value < 0)
					value = 0;
				_Vitality = value - BaseJob.Vitality;
			}
		}

		public int Accuracy
		{
			get { return BaseJob.Accuracy + _Accuracy; }
			set
			{
				if (value > 255)
					value = 255;
				if (value < 0)
					value = 0;
				_Accuracy = value - BaseJob.Accuracy;
			}
		}

		public int Agility
		{
			get { return BaseJob.Agility + _Agility; }
			set
			{
				if (value > 255)
					value = 255;
				if (value < 0)
					value = 0;
				_Agility = value - BaseJob.Agility;
			}
		}

		public int Intelligence
		{
			get { return BaseJob.Intelligence + _Intelligence; }
			set
			{
				if (value > 255)
					value = 255;
				if (value < 0)
					value = 0;
				_Intelligence = value - BaseJob.Intelligence;
			}
		}

		public int Wisdom
		{
			get { return BaseJob.Wisdom + _Wisdom; }
			set
			{
				if (value > 255)
					value = 255;
				if (value < 0)
					value = 0;
				_Wisdom = value - BaseJob.Wisdom;
			}
		}

		/// <summary>
		/// Stat points that still need to be allocated.
		/// </summary>
		public int StatRemaining
		{
			get { return _StatRemaining; }
			set { _StatRemaining = value; }
		}

		public BattleSprite BattleSprite
		{
			get { return BaseJob.BattleSprite; }
			set { BaseJob.BattleSprite = value; }
		}
		#endregion Properties

		public Job()
		{
		}

		public Job(BaseJob baseJob, int level = 1)
		{
			BaseJob = baseJob;
			Level = level;
			Hp = MaxHp;
			Mp = MaxMp;
		}

		public override string ToString()
		{
			return JobName + " [Lv:" + Level + "]";
		}

		/// <summary>
		/// Give the amount of experience to have to get from level to the next.
		/// </summary>
		/// <param name="level">Current level to calculate the exp for the next level.</param>
		/// <returns>Amount of experience to have to get from level to the next.</returns>
		public static int ExpForLevel(int level)
		{
			if (level == 0)
				return 0;

			return (int)(39 * Math.Pow(level, 2));
		}

		/// <summary>
		/// Give the amount of experience to have to get from level to the next.
		/// </summary>
		/// <returns>Amount of experience to have to get from level to the next.</returns>
		public int ExpForLevel()
		{
			return ExpForLevel(Level);
		}

		/// <summary>
		/// Give the amount of total experience to have to get from level to the next.
		/// </summary>
		/// <param name="level">Current level to calculate the total exp for the next level.</param>
		/// <returns>Amount of total experience to have to get from level to the next.</returns>
		public static int TotalExpForLevel(int level)
		{
			if (level == 0)
				return 0;

			return TotalExpForLevel(level - 1) + (int)(39 * Math.Pow(level, 2));
		}

		/// <summary>
		/// Give the amount of total experience to have to get from level to the next.
		/// </summary>
		/// <returns>Amount of total experience to have to get from level to the next.</returns>
		public int TotalExpForLevel()
		{
			return TotalExpForLevel(Level);
		}

		public void LevelUp()
		{
			// If the total exp is smaller than ExpForNextLevel
			// or it is already at the max level, it don't need to level up.
			if (TotalExp < ExpForLevel(Level) || Level >= MAX_LEVEL)
				return;

			int oldMaxHp = MaxHp;
			int oldMaxMp = MaxMp;

			Level++;
			StatRemaining += STAT_PER_LEVEL_UP;
			Strength++;
			Vitality++;
			Accuracy++;
			Agility++;
			Intelligence++;
			Wisdom++;

			// Heal the amount gained with the level up.
			Hp += MaxHp - oldMaxHp;
			Mp += MaxMp - oldMaxMp;
		}

		#region ICloneable Membres

		public object Clone()
		{
			return this.CloneExt();
		}

		#endregion
	}
}
