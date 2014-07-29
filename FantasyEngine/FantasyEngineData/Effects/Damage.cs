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

		public static readonly Damage Empty = new Damage() { Type = eDamageType.HP, Value = 0, Multiplier = 0, User = null };
	}
}
