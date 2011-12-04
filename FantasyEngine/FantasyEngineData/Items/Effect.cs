using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using FantasyEngineData.Entities;

namespace FantasyEngineData.Items
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
            ImbueQuake,
            ImbueLight,
            ImbueDark,
            //Recovery
            RecoveryHP,
            RecoveryMP,
            Life,
            Esuna,
            Poisona,
            Confusena,
            Blindna,
            Silencena,
            WakeUp,
            HolyWater,
            Soft,
            //Offense
            Fire,
            Ice,
            Thunder,
            Water,
            Aero,
            Quake,
            Holy,
            Dark,
            Flare,
            //Boost
            Protect,
            Shell,
            Reflect,
            Haste,
            Regen,
            NullAll,
            NullFire,
            NullIce,
            NullThunder,
            NullWater,
            NullAero,
            NullQuake,
            NullLight,
            NullDark,
            //Status
            Poison,
            Confuse,
            Blind,
            Silence,
            Sleep,
            Berserk,
            Zombie,
            Slow,
            Dispel,
            Stone,
            KO
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
            int damage;
            return Use(user, target, out damage);
        }

        /// <summary>
        /// Use or apply the effect on the target as the user casts it.
        /// </summary>
        /// <param name="user">User who casts the effect</param>
        /// <param name="target">Target who receive the effect</param>
        /// <param name="damage">Amount of damage (or heal if negative) done</param>
        /// <param name="nbTarget">Number of target the effect is applied in the same time</param>
        /// <returns>If the effect is succesful</returns>
        public bool Use(Character user, Character target, out int damage, int nbTarget = 1)
        {
            if (target == null)
                target = user;

            switch (Type)
            {
                case eType.DrainHP:
                    damage = target.Hp;
                    target.Hp -= Value;
                    target.Hp -= (int)(target.Hp * Multiplier);
                    damage -= target.Hp;

                    user.Hp += damage;
                    return true;
                case eType.DrainMP:
                    damage = target.Mp;
                    target.Mp -= Value;
                    target.Mp -= (int)(target.Mp * Multiplier);
                    damage -= target.Mp;

                    user.Mp += damage;
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
                case eType.ImbueQuake:
                    break;
                case eType.ImbueLight:
                    break;
                case eType.ImbueDark:
                    break;
                case eType.RecoveryHP:
                    if (target.IsDead)
                    {
                        damage = 0;
                        return false;
                    }

                    damage = target.Hp;
                    target.Hp += Value;
                    target.Hp += (int)(target.MaxHp * Multiplier);
                    damage -= target.Hp;
                    return true;
                case eType.RecoveryMP:
                    if (target.IsDead)
                    {
                        damage = 0;
                        return false;
                    }

                    damage = target.Mp;
                    target.Mp += Value;
                    target.Mp += (int)(target.MaxMp * Multiplier);
                    damage -= target.Mp;
                    return true;
                case eType.Life:
                    if (!target.IsDead)
                    {
                        damage = 0;
                        return false;
                    }

                    damage = target.Hp;
                    target.Hp += Value;
                    target.Hp += (int)(target.MaxHp * Multiplier);
                    damage -= target.Hp;
                    return true;
                case eType.Esuna:
                    break;
                case eType.Poisona:
                    break;
                case eType.Confusena:
                    break;
                case eType.Blindna:
                    break;
                case eType.Silencena:
                    break;
                case eType.WakeUp:
                    break;
                case eType.HolyWater:
                    break;
                case eType.Soft:
                    break;
                case eType.Fire:
                    {
                        int multiplier = (int)Multiplier;
                        target.CalculateMagicalDamage(user, Character.eMagicalDamageOption.BLACK, Value, 100, nbTarget, ref multiplier, out damage);
                        target.Hp -= damage;
                        return true;
                    }
                case eType.Ice:
                    break;
                case eType.Thunder:
                    break;
                case eType.Water:
                    break;
                case eType.Aero:
                    break;
                case eType.Quake:
                    break;
                case eType.Holy:
                    break;
                case eType.Dark:
                    break;
                case eType.Flare:
                    {
                        int multiplier = (int)Multiplier;
                        target.CalculateMagicalDamage(user, Character.eMagicalDamageOption.BLACK, Value, 100, nbTarget, ref multiplier, out damage);
                        target.Hp -= damage;
                        return true;
                    }
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
                case eType.NullAll:
                    break;
                case eType.NullFire:
                    break;
                case eType.NullIce:
                    break;
                case eType.NullThunder:
                    break;
                case eType.NullWater:
                    break;
                case eType.NullAero:
                    break;
                case eType.NullQuake:
                    break;
                case eType.NullLight:
                    break;
                case eType.NullDark:
                    break;
                case eType.Poison:
                    break;
                case eType.Confuse:
                    break;
                case eType.Blind:
                    break;
                case eType.Silence:
                    break;
                case eType.Sleep:
                    break;
                case eType.Berserk:
                    break;
                case eType.Zombie:
                    break;
                case eType.Slow:
                    break;
                case eType.Dispel:
                    break;
                case eType.Stone:
                    break;
                case eType.KO:
                    break;
            }

            damage = 0;
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
                case eType.ImbueQuake:
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
                case eType.Confusena:
                    return "";
                case eType.Blindna:
                    return "";
                case eType.Silencena:
                    return "";
                case eType.WakeUp:
                    return "";
                case eType.HolyWater:
                    return "";
                case eType.Soft:
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
                case eType.Quake:
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
                case eType.NullAll:
                    return "";
                case eType.NullFire:
                    return "";
                case eType.NullIce:
                    return "";
                case eType.NullThunder:
                    return "";
                case eType.NullWater:
                    return "";
                case eType.NullAero:
                    return "";
                case eType.NullQuake:
                    return "";
                case eType.NullLight:
                    return "";
                case eType.NullDark:
                    return "";
                case eType.Poison:
                    return "";
                case eType.Confuse:
                    return "";
                case eType.Blind:
                    return "";
                case eType.Silence:
                    return "";
                case eType.Sleep:
                    return "";
                case eType.Berserk:
                    return "";
                case eType.Zombie:
                    return "";
                case eType.Slow:
                    return "";
                case eType.Dispel:
                    return "";
                case eType.Stone:
                    return "";
                case eType.KO:
                    return "";
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
