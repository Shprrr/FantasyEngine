using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasyEngineData.Battles
{
	public class Cursor
	{
		public static readonly eTargetType[] POSSIBLE_TARGETS_ANYONE = new eTargetType[] {
			eTargetType.SINGLE_ENEMY,
			eTargetType.MULTI_ENEMY,
			eTargetType.SINGLE_PARTY,
			eTargetType.MULTI_PARTY,
			eTargetType.ALL };

		public static readonly eTargetType[] POSSIBLE_TARGETS_ONE = new eTargetType[] {
			eTargetType.SINGLE_ENEMY,
			eTargetType.SINGLE_PARTY };

		public Battler[] Actors { get; private set; }
		public Battler[] Enemies { get; private set; }
		public int Index { get; set; }
		public int IndexSelf { get; set; }
		public eTargetType Target { get; set; }
		public eTargetType[] PossibleTargets { get; set; }

		public Cursor(eTargetType defaultTarget, int indexSelf, eTargetType[] possibleTargets)
		{
			Actors = new Battler[Battle.MAX_ACTOR];
			Enemies = new Battler[Battle.MAX_ENEMY];
			Index = 0;
			IndexSelf = indexSelf;
			Target = defaultTarget;
			PossibleTargets = possibleTargets;
		}

		private static readonly eTargetType[] TARGET_ORDER = new eTargetType[] {
			eTargetType.ALL,
			eTargetType.MULTI_ENEMY,
			eTargetType.SINGLE_ENEMY,
			eTargetType.SINGLE_PARTY,
			eTargetType.MULTI_PARTY };

		public void ChangeTargetTypeToLeft()
		{
			if (!TARGET_ORDER.Contains(Target))
				return;

			int i;
			for (i = 0; i < TARGET_ORDER.Length; i++)
			{
				if (Target == TARGET_ORDER[i])
					break;
			}

			// Go to the previous Possible Target and loop if needed.
			do
			{
				i--;
				if (i < 0) i = TARGET_ORDER.Length - 1;
				Target = TARGET_ORDER[i];
			} while (!PossibleTargets.Contains(Target));

			if (Target == eTargetType.SINGLE_PARTY)
				while (Actors[Index] == null)
					if (Actors[Index + 1] != null)
						Index++;
					else
						Index = 0;

			if (Target == eTargetType.SINGLE_ENEMY)
				while (Enemies[Index] == null)
					if (Enemies[Index + 1] != null)
						Index++;
					else
						Index = 0;
		}

		public void ChangeTargetTypeToRight()
		{
			if (!TARGET_ORDER.Contains(Target))
				return;

			int i;
			for (i = 0; i < TARGET_ORDER.Length; i++)
			{
				if (Target == TARGET_ORDER[i])
					break;
			}

			// Go to the next Possible Target and loop if needed.
			do
			{
				i++;
				if (i >= TARGET_ORDER.Length) i = 0;
				Target = TARGET_ORDER[i];
			} while (!PossibleTargets.Contains(Target));

			if (Target == eTargetType.SINGLE_PARTY)
				while (Actors[Index] == null)
					if (Actors[Index + 1] != null)
						Index++;
					else
						Index = 0;

			if (Target == eTargetType.SINGLE_ENEMY)
				while (Enemies[Index] == null)
					if (Enemies[Index + 1] != null)
						Index++;
					else
						Index = 0;
		}
	}
}
