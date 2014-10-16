using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData.Entities;

namespace FantasyEngineData.Effects
{
	public enum ePhysicalDamageOption
	{
		RIGHT,
		LEFT,
		BOTH
	}

	public enum eMagicalDamageOption
	{
		NONE,
		BLACK,
		WHITE
	}

	public struct Damage
	{
		public enum eDamageType
		{
			HP,
			MP,
			DrainHP,
			DrainMP
		}

		public eDamageType Type { get; set; }
		public int Value { get; set; }
		/// <summary>
		/// Number of hits hitted.  Already calculated in Damage.
		/// </summary>
		public int Multiplier { get; set; }
		public Character User { get; set; }

		public void ApplyDamage(Character target)
		{
			switch (Type)
			{
				case eDamageType.HP:
				case eDamageType.DrainHP:
					target.Hp -= Value;
					if (Type == eDamageType.DrainHP)
						User.Hp += Value;
					break;
				case eDamageType.MP:
				case eDamageType.DrainMP:
					target.Mp -= Value;
					if (Type == eDamageType.DrainMP)
						User.Mp += Value;
					break;
			}
		}

		public override string ToString()
		{
			return Multiplier + "x" + Value + " " + Type;
		}

		public override bool Equals(object obj)
		{
			if (obj is Damage)
			{
				Damage d2 = (Damage)obj;
				return Type == d2.Type && Value == d2.Value && Multiplier == d2.Multiplier && User == d2.User;
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(Damage d1, Damage d2)
		{
			return d1.Equals(d2);
		}

		public static bool operator !=(Damage d1, Damage d2)
		{
			return !d1.Equals(d2);
		}

		/// <summary>
		/// Merges two Damage. Need to be the same Type and by the same User.
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static Damage operator +(Damage d1, Damage d2)
		{
			if (d1 == Empty)
				return d2;
			if (d2 == Empty)
				return d1;
			if (d1.Type != d2.Type || d1.User != d2.User)
				throw new InvalidOperationException("Damages are not compatible for the operation.");

			return new Damage()
			{
				Type = d1.Type,
				Value = d1.Value + d2.Value,
				Multiplier = d1.Multiplier + d2.Multiplier,
				User = d1.User
			};
		}

		public static readonly Damage Empty = new Damage() { Type = eDamageType.HP, Value = 0, Multiplier = 0, User = null };
	}
}
