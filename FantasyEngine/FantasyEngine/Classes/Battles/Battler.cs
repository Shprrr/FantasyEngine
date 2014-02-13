using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FantasyEngineData;
using FantasyEngineData.Battles;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;
using FantasyEngineData.Skills;

namespace FantasyEngine.Classes.Battles
{
	public class Battler : Character
	{
		public static readonly Point BATTLER_SIZE = new Point(52, 52);

		/// <summary>
		/// Counter for CTB.  Tell the number of tick to wait for the next action.
		/// </summary>
		public int Counter;

		public float HasteStatus = 1;

		public Tileset BattlerSprite;
		public Vector2 BattlerPosition;

		public int GoldToGive = 0;
		public List<BaseItem> Treasure = new List<BaseItem>();

		public int multiplierRH, multiplierLH;
		public int damageRH, damageLH;

		public BattleSprite? BattleSprite
		{
			get { return CurrentJob != null ? (BattleSprite?)CurrentJob.BattleSprite : null; }
			set { if (CurrentJob != null) CurrentJob.BattleSprite = value.Value; }
		}

		public Rectangle GetRectangle()
		{
			return new Rectangle((int)BattlerPosition.X, (int)BattlerPosition.Y, BATTLER_SIZE.X, BATTLER_SIZE.Y);
		}

		public bool IsActor { get; set; }

		public Battler(Game game, Character character)
			: base(character.Name)
		{
			IsActor = Player.GamePlayer.Actors.Contains(character);

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

			BattleSprite? battleSprite = character.CurrentJob.BattleSprite;
			if (battleSprite.HasValue)
			{
				Texture2D texture = game.Content.Load<Texture2D>(@"Images\Battle\" + battleSprite.Value.SpriteName);
				if (battleSprite.Value.IsTiled)
					BattlerSprite = new Tileset(texture, BattleSprite.Value.SpriteSize, (int)BattleSprite.Value.TileWidth, (int)BattleSprite.Value.TileHeight);
				else
					BattlerSprite = new Tileset(texture, texture.Width, texture.Height);
			}
		}

		public Battler(Game game, Monster monster, int level)
			: base(monster.JobName)
		{
			Jobs[0] = new Job((BaseJob)monster.Clone(), level);

			monster.Drop.GetDrop(CurrentJob, ref GoldToGive, ref Treasure);

			BattleSprite? battleSprite = monster.BattleSprite;
			if (battleSprite.HasValue)
			{
				Texture2D texture = game.Content.Load<Texture2D>(@"Images\Battle\" + battleSprite.Value.SpriteName);
				if (battleSprite.Value.IsTiled)
					BattlerSprite = new Tileset(texture, BattleSprite.Value.SpriteSize, (int)BattleSprite.Value.TileWidth, (int)BattleSprite.Value.TileHeight);
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

		/// <summary>
		/// Decide what action the AI will take.
		/// </summary>
		/// <param name="game"></param>
		/// <param name="actors">AI party</param>
		/// <param name="enemies">Enemies to the AI</param>
		/// <returns></returns>
		public BattleAction AIChooseAction(Game game, Battler[] actors, Battler[] enemies)
		{
			BattleAction action = new BattleAction();
			List<int> indexTargetPotential = new List<int>();

			//TODO: Si aucun skill appris, attack obligatoirement physique.
			action.Kind = BattleAction.eKind.ATTACK;
			for (int i = 0; i < enemies.Length; i++)
			{
				if (enemies[i] != null)
					indexTargetPotential.Add(i);
			}
			action.Target = new Cursor(game, enemies, actors, eTargetType.SINGLE_PARTY,
				indexTargetPotential[Extensions.rand.Next(indexTargetPotential.Count)]);
			return action;

			/*
			 * Si l'attack physique est meilleur que l'attack magic et
			 *  que le target n'est pas résistant à l'attack,
			 * Kind = Attack
			 * Target = targetRandomParmisCeuxPossible
			*/
			for (int i = 0; i < enemies.Length; i++)
			{
				if (enemies[i] == null)
					continue;

				//// Calculer le potentiel de dommage.
				//int baseDamage = getBaseDamage(eDamageOption.RIGHT) - enemy.getDefenseDamage();
				//int hitPourc = getHitPourc(eDamageOption.RIGHT) - enemy.getEvadePourc();
				//int multi = getAttackMultiplier() - enemy.getDefenseMultiplier();

				//// Réquilibrer les valeurs out of range.
				//if (baseDamage < 1)
				//    baseDamage = 1;

				//if (hitPourc < 0)
				//    hitPourc = 0;

				//if (multi < 0)
				//    multi = 0;

				//// ???

				//TODO: Si Weapon équipé main gauche.
				int attPhysic = getBaseDamage(ePhysicalDamageOption.RIGHT) - enemies[i].getDefenseDamage();
				int attMagic = 0;

				if (attPhysic > attMagic)
				{
					action.Kind = BattleAction.eKind.ATTACK;
					indexTargetPotential.Add(i);
				}
			}

			if (action.Kind == BattleAction.eKind.ATTACK)
			{
				action.Target = new Cursor(game, enemies, actors, eTargetType.SINGLE_PARTY,
					indexTargetPotential[Extensions.rand.Next(indexTargetPotential.Count)]);
				return action;
			}

			/*
			 * Si on n'attaque pas physique, regarder la possibilité d'utiliser les skills appris.
			 * Kind = Magic
			 * Target = targetRandomParmisCeuxPossible
			 * skillId = SkillIdChoisi
			*/
			//TODO: Parcourir les skills appris.

			/*
			 * S'il n'y pas de skill utile,
			 * Kind = Guard
			*/
			action.Kind = BattleAction.eKind.GUARD;

			return action;
		}
	}
}
