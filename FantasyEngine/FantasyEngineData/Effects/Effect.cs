using Microsoft.Xna.Framework.Content;
using FantasyEngineData.Entities;

namespace FantasyEngineData.Effects
{
	public class Effect
	{
		public enum eType
		{
			//Modifier
			DrainHP,
			DrainMP,
			ImbueFire,
			ImbueIce,
			ImbueThunder,
			ImbueWater,
			ImbueAero,
			ImbueEarth,
			ImbueLight,
			ImbueDark,
			//Recovery
			RecoveryHP,
			RecoveryMP,
			Life,
			Esuna,
			Poisona,
			Blindna,
			Silencena,
			WakeUp,
			Confusena,
			HolyWater,
			Soft,
			Dispel,
			//Offense
			Fire,
			Ice,
			Thunder,
			Water,
			Aero,
			Earth,
			Holy,
			Dark,
			Flare,
			//Boost
			Protect,
			Shell,
			Reflect,
			Haste,
			Regen,
			NulAll,
			NulFire,
			NulIce,
			NulThunder,
			NulWater,
			NulAero,
			NulEarth,
			NulLight,
			NulDark,
			//Status
			Death,
			Poison,
			Blind,
			Silence,
			Sleep,
			Confuse,
			Berserk,
			Zombie,
			Slow,
			Stone
		}

		public eType Type { get; set; }
		[ContentSerializer(Optional = true)]
		public int Value { get; set; }
		[ContentSerializer(Optional = true)]
		public float Multiplier { get; set; }
		public int HitPourc { get; set; }

		public Effect()
		{
			Multiplier = 1;
		}

		public Effect(eType type, int value = 0, int multiplier = 1)
		{
			Type = type;
			Value = value;
			Multiplier = multiplier;
		}

		/// <summary>
		/// Use or apply the effect on the target as the user casts it.
		/// </summary>
		/// <param name="user">User who casts the effect</param>
		/// <param name="target">Target who receive the effect</param>
		/// <returns>If the effect is succesful</returns>
		public bool Use(Character user, Character target = null)
		{
			Damage damage;
			bool ret = CalculateDamage(user, target, out damage);
			damage.ApplyDamage(target ?? user);
			return ret;
		}

		/// <summary>
		/// Use or apply the effect on the target as the user casts it.
		/// </summary>
		/// <param name="user">User who casts the effect</param>
		/// <param name="target">Target who receive the effect</param>
		/// <param name="damage">Amount of damage (or heal if negative) done</param>
		/// <param name="nbTarget">Number of target the effect is applied in the same time</param>
		/// <returns>If the effect is succesful</returns>
		public bool CalculateDamage(Character user, Character target, out Damage damage, int nbTarget = 1)
		{
			damage = Damage.Empty;
			if (target == null)
				target = user;
			damage.User = user;

			switch (Type)
			{
				case eType.DrainHP:
					if (target.IsDead)
					{
						damage = Damage.Empty;
						return false;
					}

					damage.Type = Damage.eDamageType.DrainHP;
					damage.Multiplier = 1;
					damage.Value = Value;
					damage.Value += (int)(target.MaxHp * Multiplier);
					if (damage.Value > target.Hp) damage.Value = target.Hp;
					return true;
				case eType.DrainMP:
					if (target.IsDead)
					{
						damage = Damage.Empty;
						return false;
					}

					damage.Type = Damage.eDamageType.DrainMP;
					damage.Multiplier = 1;
					damage.Value = Value;
					damage.Value += (int)(target.MaxMp * Multiplier);
					if (damage.Value > target.Mp) damage.Value = target.Mp;
					return true;
				case eType.ImbueFire:
					break;
				case eType.ImbueIce:
					break;
				case eType.ImbueThunder:
					break;
				case eType.ImbueWater:
					break;
				case eType.ImbueAero:
					break;
				case eType.ImbueEarth:
					break;
				case eType.ImbueLight:
					break;
				case eType.ImbueDark:
					break;
				case eType.RecoveryHP:
					if (target.IsDead)
					{
						damage = Damage.Empty;
						return false;
					}

					damage.Type = Damage.eDamageType.HP;
					damage.Multiplier = 1;
					damage.Value = target.Hp;
					damage.Value += Value;
					damage.Value += (int)(target.MaxHp * Multiplier);
					if (damage.Value > target.MaxHp) damage.Value = target.MaxHp;
					damage.Value -= target.Hp;
					damage.Value *= -1;
					return true;
				case eType.RecoveryMP:
					if (target.IsDead)
					{
						damage = Damage.Empty;
						return false;
					}

					damage.Type = Damage.eDamageType.MP;
					damage.Multiplier = 1;
					damage.Value = target.Mp;
					damage.Value += Value;
					damage.Value += (int)(target.MaxMp * Multiplier);
					if (damage.Value > target.MaxMp) damage.Value = target.MaxMp;
					damage.Value -= target.Mp;
					damage.Value *= -1;
					return true;
				case eType.Life:
					if (!target.IsDead)
					{
						damage = Damage.Empty;
						return false;
					}
					damage.Type = Damage.eDamageType.HP;
					damage.Multiplier = 1;
					damage.Value = target.Hp;
					damage.Value += Value;
					damage.Value += (int)(target.MaxHp * Multiplier);
					if (damage.Value > target.MaxHp) damage.Value = target.MaxHp;
					damage.Value -= target.Hp;
					damage.Value *= -1;
					return true;
				case eType.Esuna:
					break;
				case eType.Poisona:
					break;
				case eType.Blindna:
					break;
				case eType.Silencena:
					break;
				case eType.WakeUp:
					break;
				case eType.Confusena:
					break;
				case eType.HolyWater:
					break;
				case eType.Soft:
					break;
				case eType.Dispel:
					break;
				case eType.Fire:
					damage.Multiplier = (int)Multiplier;
					target.CalculateMagicalDamage(user, eMagicalDamageOption.BLACK, Value, HitPourc, nbTarget, ref damage);
					return true;
				case eType.Ice:
					damage.Multiplier = (int)Multiplier;
					target.CalculateMagicalDamage(user, eMagicalDamageOption.BLACK, Value, HitPourc, nbTarget, ref damage);
					return true;
				case eType.Thunder:
					damage.Multiplier = (int)Multiplier;
					target.CalculateMagicalDamage(user, eMagicalDamageOption.BLACK, Value, HitPourc, nbTarget, ref damage);
					return true;
				case eType.Water:
					damage.Multiplier = (int)Multiplier;
					target.CalculateMagicalDamage(user, eMagicalDamageOption.BLACK, Value, HitPourc, nbTarget, ref damage);
					return true;
				case eType.Aero:
					damage.Multiplier = (int)Multiplier;
					target.CalculateMagicalDamage(user, eMagicalDamageOption.BLACK, Value, HitPourc, nbTarget, ref damage);
					return true;
				case eType.Earth:
					damage.Multiplier = (int)Multiplier;
					target.CalculateMagicalDamage(user, eMagicalDamageOption.BLACK, Value, HitPourc, nbTarget, ref damage);
					return true;
				case eType.Holy:
					damage.Multiplier = (int)Multiplier;
					target.CalculateMagicalDamage(user, eMagicalDamageOption.BLACK, Value, HitPourc, nbTarget, ref damage);
					return true;
				case eType.Dark:
					damage.Multiplier = (int)Multiplier;
					target.CalculateMagicalDamage(user, eMagicalDamageOption.BLACK, Value, HitPourc, nbTarget, ref damage);
					return true;
				case eType.Flare:
					damage.Multiplier = (int)Multiplier;
					target.CalculateMagicalDamage(user, eMagicalDamageOption.BLACK, Value, HitPourc, nbTarget, ref damage);
					return true;
				case eType.Protect:
					break;
				case eType.Shell:
					break;
				case eType.Reflect:
					break;
				case eType.Haste:
					break;
				case eType.Regen:
					break;
				case eType.NulAll:
					break;
				case eType.NulFire:
					break;
				case eType.NulIce:
					break;
				case eType.NulThunder:
					break;
				case eType.NulWater:
					break;
				case eType.NulAero:
					break;
				case eType.NulEarth:
					break;
				case eType.NulLight:
					break;
				case eType.NulDark:
					break;
				case eType.Death:
					break;
				case eType.Poison:
					break;
				case eType.Blind:
					break;
				case eType.Silence:
					break;
				case eType.Sleep:
					break;
				case eType.Confuse:
					break;
				case eType.Berserk:
					break;
				case eType.Zombie:
					break;
				case eType.Slow:
					break;
				case eType.Stone:
					break;
			}

			return false;
		}

		public static string TypeToString(eType type)
		{
			switch (type)
			{
				case eType.DrainHP:
					return "Absorbs HP";
				case eType.DrainMP:
					return "Absorbs MP";
				case eType.ImbueFire:
					return "";
				case eType.ImbueIce:
					return "";
				case eType.ImbueThunder:
					return "";
				case eType.ImbueWater:
					return "";
				case eType.ImbueAero:
					return "";
				case eType.ImbueEarth:
					return "";
				case eType.ImbueLight:
					return "";
				case eType.ImbueDark:
					return "";
				case eType.RecoveryHP:
					return "Recovers HP";
				case eType.RecoveryMP:
					return "Recovers MP";
				case eType.Life:
					return "Revives";
				case eType.Esuna:
					return "";
				case eType.Poisona:
					return "";
				case eType.Blindna:
					return "";
				case eType.Silencena:
					return "";
				case eType.WakeUp:
					return "";
				case eType.Confusena:
					return "";
				case eType.HolyWater:
					return "";
				case eType.Soft:
					return "";
				case eType.Dispel:
					return "";
				case eType.Fire:
					return "Casts Fire";
				case eType.Ice:
					return "Casts Ice";
				case eType.Thunder:
					return "Casts Thunder";
				case eType.Water:
					return "Casts Water";
				case eType.Aero:
					return "Casts Aero";
				case eType.Earth:
					return "Casts Quake";
				case eType.Holy:
					return "Casts Holy";
				case eType.Dark:
					return "Casts Dark";
				case eType.Flare:
					return "Casts Flare";
				case eType.Protect:
					return "Casts Protect";
				case eType.Shell:
					return "Casts Shell";
				case eType.Reflect:
					return "Casts Reflect";
				case eType.Haste:
					return "Casts Haste";
				case eType.Regen:
					return "Casts Regen";
				case eType.NulAll:
					return "";
				case eType.NulFire:
					return "";
				case eType.NulIce:
					return "";
				case eType.NulThunder:
					return "";
				case eType.NulWater:
					return "";
				case eType.NulAero:
					return "";
				case eType.NulEarth:
					return "";
				case eType.NulLight:
					return "";
				case eType.NulDark:
					return "";
				case eType.Death:
					return "Inflicts Death";
				case eType.Poison:
					return "Inflicts Poison";
				case eType.Blind:
					return "Inflicts Blind";
				case eType.Silence:
					return "Inflicts Silence";
				case eType.Sleep:
					return "Inflicts Sleep";
				case eType.Confuse:
					return "Inflicts Confuse";
				case eType.Berserk:
					return "Inflicts Berserk";
				case eType.Zombie:
					return "Inflicts Zombie";
				case eType.Slow:
					return "Inflicts Slow";
				case eType.Stone:
					return "Inflicts Stone";
				default:
					return string.Empty;
			}
		}

		public string TypeToString()
		{
			return TypeToString(Type);
		}

		public override string ToString()
		{
			string retour = TypeToString();

			if (Multiplier != 1) retour += " " + Multiplier + "x";

			if (Value != 0) retour += " " + Value;

			if (HitPourc != 100)
				retour += " " + HitPourc + "%";

			return retour;
		}
	}
}
