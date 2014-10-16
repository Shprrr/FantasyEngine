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

		public bool OnApply(Battler target, int nbTurn)
		{
			if (target.Statuses.ContainsKey(Type))
				return false;

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
				damage.Value = -(int)(counterElapsed * target.MaxHp / 256f + 100);
				damage.Multiplier = 1;
				damage.User = target;
				RaiseOnDamage(target, damage);
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

			TurnToLive--;
			if (TurnToLive <= 0)
				OnCure(target);
		}

		public override string ToString()
		{
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
