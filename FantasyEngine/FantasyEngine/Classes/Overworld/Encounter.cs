using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData;
using FantasyEngineData.Entities;

namespace FantasyEngine.Classes.Overworld
{
	public class Encounter
	{
		public struct MonsterLevel
		{
			public Monster Monster;
			public int Level;

			public MonsterLevel(Monster monster, int level)
			{
				Monster = monster;
				Level = level;
			}

			public override string ToString()
			{
				return Monster + " L:" + Level;
			}
		}

		public MonsterLevel[] Monsters;
		public float Chances;

		public Encounter(MonsterLevel[] monsters, float chances)
		{
			Monsters = monsters;
			Chances = chances;
		}

		public Encounter(Monster monster, int level, float chances)
		{
			Monsters = new MonsterLevel[] { new MonsterLevel(monster, level) };
			Chances = chances;
		}

		public static int GetIndexByChances(List<Encounter> encounters)
		{
			float totalChance = encounters.Sum(e => e.Chances);
			float chance = (float)Extensions.rand.NextDouble() * totalChance;
			float chanceCurrent = 0;

			for (int i = 0; i < encounters.Count; i++)
			{
				if (chanceCurrent + encounters[i].Chances > chance)
					return i;
				chanceCurrent += encounters[i].Chances;
			}

			return 0;
		}
	}
}
