using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace FantasyEngineData.Battles
{
	public enum eTargetType
	{
		NONE,
		SINGLE_ENEMY,
		MULTI_ENEMY,
		SINGLE_PARTY,
		MULTI_PARTY,
		SELF,
		ALL
	}

	public struct CTBTurn : IComparable<CTBTurn>
	{
		public int counter;
		public int rank;
		public int tickSpeed;
		public Battler battler;

		public override int GetHashCode() { return base.GetHashCode(); }

		public override bool Equals(object obj)
		{
			if (obj is CTBTurn)
			{
				CTBTurn other = (CTBTurn)obj;
				if (this.battler == other.battler)
					return this.counter == other.counter;

				return false;
			}

			return base.Equals(obj);
		}

		public override string ToString()
		{
			return battler + " [C:" + counter + "]";
		}

		#region IComparable<CTBTurn> Membres

		public int CompareTo(CTBTurn other)
		{
			if (this.battler == other.battler)
				return 0;

			if (other.battler == null)
				return -1;

			if (this.battler == null)
				return 1;

			return this.counter.CompareTo(other.counter);
		}

		#endregion
	}

	public class Battle
	{
		public enum ePhases
		{
			NONE,
			PRE_BATTLE,
			ACTOR_COMMAND,
			ACTION,
			POST_BATTLE,
			END_BATTLE
		}

		public static int MAX_ACTOR;
		public static int MAX_ENEMY;
		public const int MAX_CTB = 16;

		protected Battler[] _Actors;
		protected Battler[] _Enemies;

		public int BattleTurn { get; private set; }

		public ePhases Phase { get; private set; }

		private List<CTBTurn> _OrderBattle = new List<CTBTurn>(MAX_CTB);
		public List<CTBTurn> OrderBattle { get { return _OrderBattle; } }

		public int ActiveBattlerIndex { get; private set; }

		/// <summary>
		/// Get the Battler who is taking an action.
		/// </summary>
		/// <returns></returns>
		public Battler getActiveBattler()
		{
			return ActiveBattlerIndex < MAX_ACTOR ? _Actors[ActiveBattlerIndex] : _Enemies[ActiveBattlerIndex - MAX_ACTOR];
		}
		private void setActiveBattler()
		{
			for (int i = 0; i < MAX_ACTOR; i++)
			{
				if (_Actors[i] == OrderBattle[0].battler)
					ActiveBattlerIndex = i;
			}

			for (int i = 0; i < MAX_ENEMY; i++)
			{
				if (_Enemies[i] == OrderBattle[0].battler)
					ActiveBattlerIndex = MAX_ACTOR + i;
			}
		}

		public bool CanEscape { get; set; }
		public bool CanLose { get; set; }

		public Battle()
		{
			CanEscape = true;
			CanLose = true;
			BattleTurn = 0;
			Phase = ePhases.NONE;
		}

		/// <summary>
		/// Start Pre-battle phase.
		/// </summary>
		public void StartBattle()
		{
			if (Phase != ePhases.NONE)
				return;

			Phase = ePhases.PRE_BATTLE;

			InitCTBCounters();

			if (OnStartBattle != null) OnStartBattle(this, EventArgs.Empty);

			// Start party command phase
			BeginTurn();
		}

		public event EventHandler OnStartBattle;

		/// <summary>
		/// Initialise CTB counters for the first time.
		/// </summary>
		private void InitCTBCounters()
		{
			//1-Calcul les ICV, ajoute le plus petit et garde les restes
			List<CTBTurn> tempCTB = new List<CTBTurn>(MAX_ACTOR + MAX_ENEMY);

			//Get ICVs
			for (int i = 0; i < MAX_ACTOR; i++)
			{
				if (_Actors[i] == null)
					continue;

				_Actors[i].CalculateICV();
				CTBTurn turn = new CTBTurn();
				turn.battler = _Actors[i];
				turn.rank = 3;
				turn.counter = turn.battler.Counter;
				turn.tickSpeed = turn.battler.getTickSpeed();
				tempCTB.Add(turn);
			}

			for (int i = 0; i < MAX_ENEMY; i++)
			{
				if (_Enemies[i] == null)
					continue;

				_Enemies[i].CalculateICV();
				CTBTurn turn = new CTBTurn();
				turn.battler = _Enemies[i];
				turn.rank = 3;
				turn.counter = turn.battler.Counter;
				turn.tickSpeed = turn.battler.getTickSpeed();
				tempCTB.Add(turn);
			}

			//Sort ICVs
			tempCTB.Sort();

			//Keep lowest
			if (tempCTB[0].battler == null)
				return;
			_OrderBattle.Add(tempCTB[0]);

			//2-Calcul le NCV de celui ajouté, ajoute le plus petit et garde les restes
			for (int i = 1; i < MAX_CTB; i++)
			{
				//Get Next CV
				CTBTurn turn = tempCTB[0];
				turn.counter += turn.battler.getCounterValue(turn.rank);
				tempCTB[0] = turn;

				//Sort CVs
				tempCTB.Sort();

				//Keep lowest
				_OrderBattle.Add(tempCTB[0]);
				tempCTB[0].battler.Counter = tempCTB[0].counter;
			}

			setActiveBattler();
		}

		private void CalculateCTB()
		{
			// Remove active counter on all other counters.
			int counterLastTurn = _OrderBattle[0].counter;
			for (int i = 0; i < _OrderBattle.Count; i++)
			{
				CTBTurn turn = _OrderBattle[i];
				turn.counter -= counterLastTurn;
				_OrderBattle[i] = turn;
			}

			// Calculate the last CTBTurn to replace the one that will go out.

			//1-Calcul les NCV et ajoute le plus petit
			List<CTBTurn> tempCTB = new List<CTBTurn>(MAX_ACTOR + MAX_ENEMY);

			//Get NCVs
			for (int i = 0; i < MAX_ACTOR; i++)
			{
				if (_Actors[i] == null)
					continue;

				_Actors[i].Counter -= counterLastTurn;
				CTBTurn turn = new CTBTurn();
				turn.battler = _Actors[i];
				turn.rank = 3;
				turn.counter = turn.battler.Counter + turn.battler.getCounterValue(turn.rank);
				turn.tickSpeed = turn.battler.getTickSpeed();
				tempCTB.Add(turn);
			}

			for (int i = 0; i < MAX_ENEMY; i++)
			{
				if (_Enemies[i] == null)
					continue;

				_Enemies[i].Counter -= counterLastTurn;
				CTBTurn turn = new CTBTurn();
				turn.battler = _Enemies[i];
				turn.rank = 3;
				turn.counter = turn.battler.Counter + turn.battler.getCounterValue(turn.rank);
				turn.tickSpeed = turn.battler.getTickSpeed();
				tempCTB.Add(turn);
			}

			//Sort NCVs
			tempCTB.Sort();

			//Keep lowest
			_OrderBattle.Add(tempCTB[0]);
			tempCTB[0].battler.Counter = tempCTB[0].counter;

			_OrderBattle.RemoveAt(0);
			setActiveBattler();
		}

		/// <summary>
		/// Start Actor command phase.
		/// </summary>
		private void BeginTurn()
		{
			if (Phase != ePhases.PRE_BATTLE && Phase != ePhases.ACTION)
				return;

			Phase = ePhases.ACTOR_COMMAND;

			// Determine win/loss situation
			if (Judge())
			{
				// If won or lost: end method
				return;
			}

			if (OnBeginTurn != null) OnBeginTurn(this, EventArgs.Empty);

			BattleTurn++;

			if (getActiveBattler().IsActor)
			{
				if (OnSetupCommandWindow != null) OnSetupCommandWindow(this, EventArgs.Empty);
				return;
			}
			else
			{
				if (OnAIChooseAction != null) OnAIChooseAction(this, EventArgs.Empty);
				StartActionPhase();
				return;
			}
		}

		public event EventHandler OnBeginTurn;
		public event EventHandler OnSetupCommandWindow;
		public event EventHandler OnAIChooseAction;

		/// <summary>
		/// Determine battle Win/Loss results.
		/// </summary>
		/// <returns></returns>
		private bool Judge()
		{
			//TODO: Repenser le BattleEnd/Phase5.
			foreach (Battler actor in _Actors)
			{
				//TODO: Pas sur, mais je ne pense pas que ca marche pour plus qu'un actor.
				if (actor != null && actor.IsDead)
				{
					BattleEnd(CanLose ? 1 : 2);
					return true;
				}
			}

			foreach (Battler enemy in _Enemies)
			{
				if (enemy != null && !enemy.IsDead)
				{
					return false;
				}
			}

			// Start after battle phase (win)
			StartPostBattlePhase();
			return true;
		}

		/// <summary>
		/// Start Main phase.
		/// </summary>
		public void StartActionPhase()
		{
			Phase = ePhases.ACTION;

			if (OnActionPhase != null) OnActionPhase(this, EventArgs.Empty);

			ActionPhasePreparation();
		}

		/// <summary>
		/// Action preparation.
		/// </summary>
		private void ActionPhasePreparation()
		{
			//Hide Help window

			//Init animations

			//Turn damage

			//Natural removal of states

			if (OnActionPhaseStep2 != null) OnActionPhaseStep2(this, EventArgs.Empty);
		}

		public event EventHandler OnActionPhase;
		public event EventHandler OnActionPhaseStep2;

		public void NextTurn()
		{
			// Change the active battler for the next one.
			CalculateCTB();

			BeginTurn();
		}

		/// <summary>
		/// Start After Battle phase.
		/// </summary>
		private void StartPostBattlePhase()
		{
			Phase = ePhases.POST_BATTLE;

			if (OnPostBattlePhase != null) OnPostBattlePhase(this, EventArgs.Empty);
		}

		public event EventHandler OnPostBattlePhase;

		public void EndBattle()
		{
			//TODO: Trouver meilleure façon.
			BattleEnd(0);
		}

		/// <summary>
		/// Battle Ends.
		/// </summary>
		/// <param name="result">Results (0:Win 1:Lose 2:Escape)</param>
		private void BattleEnd(int result)
		{
			switch (result)
			{
				case 0: // Win
					if (OnWinning != null) OnWinning(this, EventArgs.Empty);
					break;

				case 1: // Lose
					if (OnLosing != null) OnLosing(this, EventArgs.Empty);
					// Gameover screen
					// Wakeup in the inn.
					break;
			}

			//Remove battle states.
			foreach (Battler actor in _Actors)
			{
			}

			Phase = ePhases.END_BATTLE;
		}

		public event EventHandler OnWinning;
		public event EventHandler OnLosing;

		/// <summary>
		/// Try to escape.
		/// </summary>
		public void Escape()
		{
			if (!CanEscape)
				return;

			//From FF3
			//(Run Away) : DD=0 DM=0 %Chance to Run = 100 - Hit%
			//(Hit%) : (Weapon Hit%) + (Agility/4) + (JLevel/4)
			//(Run Away for monster ) : IF((Lowest character level) - (Enemy level) > 15) THEN %Chance to Run = 100 - Monster Hit%

			//Calculate enemy agility average
			int enemies_agi = 0;
			int enemies_number = 0;
			for (int i = 0; i < MAX_ENEMY; i++)
			{
				Battler enemy = _Enemies[i];
				if (enemy == null)
					continue;

				if (!enemy.IsDead)
				{
					enemies_agi += enemy.Agility;
					enemies_number += 1;
				}
			}
			if (enemies_number > 0)
			{
				enemies_agi /= enemies_number;
			}

			//Calculate actor agility average
			int actors_agi = 0;
			int actors_number = 0;
			for (int i = 0; i < MAX_ACTOR; i++)
			{
				Battler actor = _Actors[i];
				if (actor == null)
					continue;

				if (!actor.IsDead)
				{
					actors_agi += actor.Agility;
					actors_number += 1;
				}
			}
			if (actors_number > 0)
			{
				actors_agi /= actors_number;
			}

			//Determine if escape is successful
			bool success = new Random().Next(0, 100) < 50 * actors_agi / enemies_agi;

			if (success)
			{
				BattleEnd(2);
			}
			else
			{
				//Next turn
				StartActionPhase();
			}
		}
	}
}
