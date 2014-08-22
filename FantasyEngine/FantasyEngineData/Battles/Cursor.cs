using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData.Entities;

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

			while (Target == eTargetType.SINGLE_PARTY && Character.IsNullOrDead(Actors[Index]))
				GoToNextActor();

			while (Target == eTargetType.SINGLE_ENEMY && Character.IsNullOrDead(Enemies[Index]))
				GoToNextEnemy();
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

			while (Target == eTargetType.SINGLE_PARTY && Character.IsNullOrDead(Actors[Index]))
				GoToNextActor();

			while (Target == eTargetType.SINGLE_ENEMY && Character.IsNullOrDead(Enemies[Index]))
				GoToNextEnemy();
		}

		public bool ChangeCursorDown()
		{
			// If we can go down.
			if (Target == eTargetType.SINGLE_PARTY)
			{
				GoToNextActor();
				return true;
			}
			else if (Target == eTargetType.SINGLE_ENEMY)
			{
				GoToNextEnemy();
				return true;
			}
			return false;
		}

		public bool ChangeCursorUp()
		{
			// If we can go up.
			if (Target == eTargetType.SINGLE_PARTY)
			{
				GoToPreviousActor();
				return true;
			}
			else if (Target == eTargetType.SINGLE_ENEMY)
			{
				GoToPreviousEnemy();
				return true;
			}
			return false;
		}

		private void GoToPreviousActor()
		{
			do
			{
				if (Index > 0)
					Index--;
				else
					Index = Actors.Length - 1;
			}
			while (Actors[Index] == null);
		}

		private void GoToPreviousEnemy()
		{
			do
			{
				if (Index > 0)
					Index--;
				else
					Index = Enemies.Length - 1;
			}
			while (Enemies[Index] == null);
		}

		private void GoToNextActor()
		{
			do
			{
				if (Index < Actors.Length - 1)
					Index++;
				else
					Index = 0;
			}
			while (Actors[Index] == null);
		}

		private void GoToNextEnemy()
		{
			do
			{
				if (Index < Enemies.Length - 1)
					Index++;
				else
					Index = 0;
			}
			while (Enemies[Index] == null);
		}
	}
}
