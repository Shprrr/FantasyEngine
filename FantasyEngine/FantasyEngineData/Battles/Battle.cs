using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FantasyEngineData.Entities;

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
		public const int RANK_DEFAULT = 3;

		public int counter;
		public int rank;
		public int tickSpeed;
		public Battler battler;

		public void SetCounter()
		{
			counter += (int)(tickSpeed * rank * battler.HasteStatus);
			rank = RANK_DEFAULT;
		}

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

		public enum eBattleResult
		{
			NONE,
			WIN,
			LOSE,
			ESCAPE
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

		private int _CounterActiveTurn;
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
			_CounterActiveTurn = _OrderBattle[0].counter;
			for (int i = 0; i < MAX_ACTOR; i++)
			{
				if (Character.IsNullOrDead(_Actors[i]))
					continue;

				_Actors[i].Counter -= _CounterActiveTurn;
				if (_Actors[i] == OrderBattle[0].battler)
					ActiveBattlerIndex = i;
			}

			for (int i = 0; i < MAX_ENEMY; i++)
			{
				if (Character.IsNullOrDead(_Enemies[i]))
					continue;

				_Enemies[i].Counter -= _CounterActiveTurn;
				if (_Enemies[i] == OrderBattle[0].battler)
					ActiveBattlerIndex = MAX_ACTOR + i;
			}

			// Remove active counter on all other counters.
			for (int i = 0; i < _OrderBattle.Count; i++)
			{
				CTBTurn turn = _OrderBattle[i];
				turn.counter -= _CounterActiveTurn;
				_OrderBattle[i] = turn;
			}
		}

		public bool CanEscape { get; set; }
		public bool CanLose { get; set; }

		private eBattleResult _Result;
		public eBattleResult Result
		{
			get { return _Result; }
			private set
			{
				_Result = value;
				if (_Result != eBattleResult.NONE)
					StartPostBattlePhase();
			}
		}

		public Battle()
		{
			CanEscape = true;
			CanLose = true;
			BattleTurn = 0;
			Phase = ePhases.NONE;
			Result = eBattleResult.NONE;
		}

		/// <summary>
		/// Start Pre-battle phase.
		/// </summary>
		public void StartBattle()
		{
			if (Phase != ePhases.NONE)
				return;

			Phase = ePhases.PRE_BATTLE;

			CalculateCTB();

			if (OnStartBattle != null) OnStartBattle(this, EventArgs.Empty);

			// Start party command phase
			BeginTurn();
		}

		public event EventHandler OnStartBattle;

		private void CalculateCTB()
		{
			CTBTurn? firstTurn = null;
			if (BattleTurn > 0)
			{
				firstTurn = _OrderBattle[0];
				// Empty the OrderBattle to recalculate TickSpeed changes and Dead/Alive changes.
				_OrderBattle.Clear();
			}

			//1-Get next CV as ICV, ajoute le plus petit et garde les restes
			List<CTBTurn> tempCTB = new List<CTBTurn>(MAX_ACTOR + MAX_ENEMY);

			//Get ICVs
			for (int i = 0; i < MAX_ACTOR; i++)
			{
				if (Character.IsNullOrDead(_Actors[i]))
					continue;

				if (BattleTurn == 0)
					_Actors[i].CalculateICV();
				if (firstTurn.HasValue && _Actors[i] == firstTurn.Value.battler)
					tempCTB.Add(firstTurn.Value);
				else
				{
					CTBTurn turn = new CTBTurn();
					turn.battler = _Actors[i];
					turn.rank = CTBTurn.RANK_DEFAULT;
					turn.counter = turn.battler.Counter;
					turn.tickSpeed = turn.battler.getTickSpeed();
					tempCTB.Add(turn);
				}
			}

			for (int i = 0; i < MAX_ENEMY; i++)
			{
				if (Character.IsNullOrDead(_Enemies[i]))
					continue;

				if (BattleTurn == 0)
					_Enemies[i].CalculateICV();
				if (firstTurn.HasValue && _Enemies[i] == firstTurn.Value.battler)
					tempCTB.Add(firstTurn.Value);
				else
				{
					CTBTurn turn = new CTBTurn();
					turn.battler = _Enemies[i];
					turn.rank = CTBTurn.RANK_DEFAULT;
					turn.counter = turn.battler.Counter;
					turn.tickSpeed = turn.battler.getTickSpeed();
					tempCTB.Add(turn);
				}
			}

			//Sort ICVs
			tempCTB.Sort();

			//Keep lowest
			_OrderBattle.Add(tempCTB[0]);

			//2-Calcul le NCV de celui ajouté, ajoute le plus petit et garde les restes
			for (int i = 1; i < MAX_CTB; i++)
			{
				//Get Next CV
				CTBTurn turn = tempCTB[0];
				turn.SetCounter();
				tempCTB[0] = turn;

				//Sort CVs
				tempCTB.Sort();

				//Keep lowest
				_OrderBattle.Add(tempCTB[0]);
			}

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
				// If won or lost: end method
				return;

			if (OnBeginTurn != null) OnBeginTurn(this, EventArgs.Empty);

			BattleTurn++;

			foreach (var actor in _Actors)
			{
				if (!Character.IsNullOrDead(actor))
					foreach (var status in actor.Statuses)
					{
						status.Value.OnBeginTurn(actor, _CounterActiveTurn);
					}
			}

			foreach (var enemy in _Enemies)
			{
				if (!Character.IsNullOrDead(enemy))
					foreach (var status in enemy.Statuses)
					{
						status.Value.OnBeginTurn(enemy, _CounterActiveTurn);
					}
			}

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
			int nbBattlerAlive = _Actors.Count(b => !Character.IsNullOrDead(b));
			if (nbBattlerAlive == 0)
			{
				Result = CanLose ? eBattleResult.LOSE : eBattleResult.ESCAPE;
				return true;
			}

			nbBattlerAlive = _Enemies.Count(b => !Character.IsNullOrDead(b));
			if (nbBattlerAlive == 0)
			{
				Result = eBattleResult.WIN;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Change the rank of the turn for the active battler.
		/// </summary>
		/// <param name="rank">rank of this turn</param>
		public void ChangeActiveRank(int rank)
		{
			var firstTurn = _OrderBattle[0];
			firstTurn.rank = rank;
			_OrderBattle[0] = firstTurn;
			CalculateCTB();
		}

		/// <summary>
		/// Start Main phase.
		/// </summary>
		public void StartActionPhase()
		{
			if (Phase != ePhases.ACTOR_COMMAND)
				return;

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

		public Action ShowingDamage;

		private void WaitShowDamage()
		{
			var threadDamage = new Thread(new ThreadStart(ShowingDamage));
			threadDamage.Start();
			threadDamage.Join();
		}

		public void NextTurn()
		{
			if (Phase != ePhases.ACTOR_COMMAND && Phase != ePhases.ACTION)
				return;

			// Threading this to not blocking for the wait.
			new Thread(new ThreadStart(delegate
			{
				for (int i = getActiveBattler().Statuses.Count - 1; i >= 0; i--)
				{
					getActiveBattler().Statuses.ElementAt(i).Value.OnEndTurn(getActiveBattler());
				}

				WaitShowDamage();

				// Keep the CV of the next turn of the current battler
				CTBTurn turn = _OrderBattle[0];
				turn.SetCounter();
				_OrderBattle[0] = turn;
				_OrderBattle[0].battler.Counter = _OrderBattle[0].counter;

				// Update CTB and change the active battler for the next one.
				CalculateCTB();

				BeginTurn();
			})).Start();
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

		/// <summary>
		/// Battle Ends.
		/// </summary>
		/// <param name="result">Results</param>
		public void BattleEnd()
		{
			if (Phase != ePhases.POST_BATTLE || Result == eBattleResult.NONE)
				return;

			switch (Result)
			{
				case eBattleResult.WIN:
				case eBattleResult.ESCAPE:
					if (OnWinning != null) OnWinning(this, EventArgs.Empty);
					break;

				case eBattleResult.LOSE:
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
				Result = eBattleResult.ESCAPE;
			}
			else
			{
				//Next turn
				StartActionPhase();
			}
		}
	}
}
