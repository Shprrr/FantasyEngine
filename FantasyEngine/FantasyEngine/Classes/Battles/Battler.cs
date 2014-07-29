using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FantasyEngineData;
using FantasyEngineData.Battles;
using FantasyEngineData.Effects;
using FantasyEngineData.Entities;

namespace FantasyEngine.Classes.Battles
{
	public class Battler : FantasyEngineData.Battles.Battler
	{
		public static readonly Point BATTLER_SIZE = new Point(52, 52);

		public Tileset BattlerSprite;
		public Vector2 BattlerPosition;

		public BattleSprite? BattleSprite
		{
			get { return CurrentJob != null ? (BattleSprite?)CurrentJob.BattleSprite : null; }
			set { if (CurrentJob != null) CurrentJob.BattleSprite = value.Value; }
		}

		public Rectangle GetRectangle()
		{
			return new Rectangle((int)BattlerPosition.X, (int)BattlerPosition.Y, BATTLER_SIZE.X, BATTLER_SIZE.Y);
		}

		public Battler(Game game, Character character)
			: base(character, Player.GamePlayer.Actors.Contains(character))
		{
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
			: base(monster, level)
		{
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
