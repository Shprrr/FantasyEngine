using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData.Battles;

namespace FantasyEngineData.Effects
{
	public class Status
	{
		public enum eStatus
		{
			KO,
			Stone,
			Poison,
			Blind,
			Silence,
			Sleep,
			Confuse,
			Berserk,
			Zombie,
			Protect,
			Shell,
			Reflect,
			Haste,
			Slow,
			Regen,
			NulFire,
			NulIce,
			NulThunder,
			NulWater,
			NulAero, // Underground?
			NulEarth, // Float?
			NulLight,
			NulDark
		}

		public eStatus Type { get; set; }
		public int TurnToLive { get; private set; }

		public Status(eStatus type)
		{
			Type = type;
		}

		public bool OnApply(Battler target, int nbTurn = 1)
		{
			if (target.Statuses.ContainsKey(Type) || target.Statuses.ContainsKey(eStatus.KO) || target.Statuses.ContainsKey(eStatus.Stone))
				return false;

			if (Type == eStatus.KO)
			{
				// Apply damage directly.
				target.Hp = 0;
				return true;
			}

			if (Type == eStatus.Stone)
			{
				// This status is alone.
				for (int i = 0; i < target.Statuses.Count; i++)
				{
					target.Statuses.RemoveAt(i);
				}

				TurnToLive = target.Hp;
				target.Statuses.Add(Type, this);
				return true;
			}

			TurnToLive = nbTurn;
			target.Statuses.Add(Type, this);
			return true;
		}

		public void OnCure(Battler target)
		{
			target.Statuses.Remove(Type);
		}

		public void OnBeginTurn(Battler target, int counterElapsed)
		{
			if (Type == eStatus.Regen)
			{
				Damage damage = new Damage();
				damage.Type = Damage.eDamageType.HP;
				damage.Value = -(int)(counterElapsed * target.MaxHp / 256f + 10);
				damage.Multiplier = 1;
				damage.User = target;
				RaiseOnDamage(target, damage);
			}
		}

		public void OnAppliedDamage(Battler target, int damageValue)
		{
			if (Type == eStatus.Stone)
			{
				TurnToLive -= damageValue;
			}
		}

		public void OnEndTurn(Battler target)
		{
			// Affects target with end turn statuses. ex:Poison
			if (Type == eStatus.Poison)
			{
				Damage damage = new Damage();
				damage.Type = Damage.eDamageType.HP;
				damage.Value = (int)(target.MaxHp * 0.2);
				damage.Multiplier = 1;
				damage.User = target;
				RaiseOnDamage(target, damage);
			}

			// Theses statuses are infinite.
			if (Type != eStatus.KO && Type != eStatus.Stone)
			{
				TurnToLive--;
				if (TurnToLive <= 0)
					OnCure(target);
			}
		}

		public override string ToString()
		{
			if (Type == eStatus.KO || Type == eStatus.Stone)
				return Type.ToString();

			//return Type.ToString() + " for " + TurnToLive + " turn" + (TurnToLive > 1 ? "s" : "");
			return Type.ToString() + " (" + TurnToLive + ")";
		}

		public delegate void DamageHandler(object sender, Battler target, Damage damage);
		public event DamageHandler OnDamage;
		protected virtual void RaiseOnDamage(Battler target, Damage damage)
		{
			// Raise the event by using the () operator.
			if (OnDamage != null)
				OnDamage(this, target, damage);
		}
	}
}
