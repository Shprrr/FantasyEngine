using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FantasyEngine.Classes.Battles
{
    public class Battler : Character
    {
        public const string MISS = "MISS";

        private void CalculateDamage(Battler pAttacker, eDamageOption damageOption, ref int multiplier, ref int damage)
        {
            Random rand = new Random();
            //Calculate min and max base damage
            int baseMinDmg = pAttacker.getBaseDamage(damageOption);

            //Bonus on base damage for Attacker
            //baseMinDmg += HasCheer ? 10 * CheerLevel : 0;
            //ou
            //baseMinDmg += HasCheer ? baseMinDmg * CheerLevel / 15 : 0;
            //baseMinDmg *= IsAlly ? 2 : 1;
            //baseMinDmg *= ElementalEffect(pAttacker);
            //baseMinDmg *= IsMini || IsToad ? 2 : 1;
            //baseMinDmg *= pAttacker->IsMini || pAttacker->IsToad ? 0 : 1;

            int baseMaxDmg = (int)(baseMinDmg * 1.5);

            //Calculate hit%
            int hitPourc = pAttacker.getHitPourc(damageOption);
            hitPourc = (hitPourc < 99 ? hitPourc : 99);
            //hitPourc /= (pAttacker.IsFrontRow || weapon.IsLongRange ? 1 : 2);
            //hitPourc /= (blindStatus ? 2 : 1);
            //hitPourc /= (IsFrontRow || weapon.IsLongRange ? 1 : 2);

            //Calculate attack multiplier
            multiplier = 0;
            for (int i = 0; i < pAttacker.getAttackMultiplier(); i++)
                if (rand.Next(0, 100) < hitPourc)
                    multiplier++;

            //Bonus on defense for Target
            int defense = getDefenseDamage();
            //defense *= (IsDefending ? 4 : 1);
            //defense *= (IsAlly ? 0 : 1);
            //defense *= (IsRunning ? 0 : 1);
            //defense *= (IsMini || IsToad ? 0 : 1);

            //Calculate defense multiplier
            int defenseMul = getDefenseMultiplier();
            //defenseMul *= (IsAlly ? 0 : 1);
            //defenseMul *= (IsRunning ? 0 : 1);
            //defenseMul *= (IsMini || IsToad ? 0 : 1);

            //Calculate multiplier and final damage
            for (int i = 0; i < defenseMul; i++)
                if (rand.Next(0, 100) < getEvadePourc())
                    multiplier--;

            damage = (rand.Next(baseMinDmg, baseMaxDmg + 1) - defense) * multiplier;
            //damage *= AttackIsJump ? 3 : 1;

            //Validate final damage and multiplier
            if (damage < 1) //Min 1 s'il tape au moins une fois
                damage = 1;

            if (multiplier < 1) //Check s'il tape au moins une fois
                damage = 0;
        }

        /// <summary>
        /// Counter for CTB.  Tell the number of tick to wait for the next action.
        /// </summary>
        public int Counter;

        public float HasteStatus = 1;

        public Tileset BattlerSprite;
        public Vector2 BattlerPosition;

        public int GoldToGive = 0;
        public List<Item> Treasure = new List<Item>();

        public int multiplierRH, multiplierLH;
        public int damageRH, damageLH;

        public BattleSprite? BattleSprite
        {
            get { return CurrentJob != null ? (BattleSprite?)CurrentJob.BattleSprite : null; }
            set { if (CurrentJob != null) CurrentJob.BattleSprite = value.Value; }
        }

        public Battler(Game game, Character character)
        {
            Name = character.Name;

            for (int i = 0; i < MAX_JOB; i++)
            {
                Jobs[i] = character.Jobs[i];
            }

            IndexCurrentJob = character.IndexCurrentJob;

            BattleSprite? battleSprite = character.CurrentJob.BattleSprite;
            if (battleSprite.HasValue)
            {
                Texture2D texture = game.Content.Load<Texture2D>(@"Images\Characters\" + battleSprite.Value.SpriteName);
                if (battleSprite.Value.IsTiled)
                    BattlerSprite = new Tileset(texture, (int)BattleSprite.Value.TileWidth, (int)BattleSprite.Value.TileHeight);
                else
                    BattlerSprite = new Tileset(texture, texture.Width, texture.Height);
            }
        }

        public Battler(Game game, Monster monster, int level)
        {
            Jobs[0] = new Job((BaseJob)monster.Clone(), level);
            IndexCurrentJob = 0;

            Name = CurrentJob.JobName;

            monster.Drop.GetDrop(CurrentJob, ref GoldToGive, ref Treasure);

            BattleSprite? battleSprite = monster.BattleSprite;
            if (battleSprite.HasValue)
            {
                Texture2D texture = game.Content.Load<Texture2D>(@"Images\Monsters\" + battleSprite.Value.SpriteName);
                if (battleSprite.Value.IsTiled)
                    BattlerSprite = new Tileset(texture, (int)BattleSprite.Value.TileWidth, (int)BattleSprite.Value.TileHeight);
                else
                    BattlerSprite = new Tileset(texture, texture.Width, texture.Height);
            }
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

            Counter = new Random().Next(minICV, maxICV + 1);
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

            //Si un Weapon est équipée dans la main droite
            {
                CalculateDamage(attacker, eDamageOption.RIGHT, ref multiplierRH, ref damageRH);
            }

            //Si un Weapon est équipée dans la main gauche
            {
            }
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
                + Strenght + Vitality + Accuracy + Agility + Intelligence + Wisdom + Math.Pow(Level, 2)) / 6);
        }
    }
}
