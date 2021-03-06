using System;
using FantasyEngine.Classes.Overworld;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;

namespace FantasyEngine.Classes
{
	public class Player
	{
		/// <summary>
		/// Player who plays the game.
		/// </summary>
		public static Player GamePlayer = null;

		public const int MAX_ACTOR = 4;

#if ENGINE
		public bool ShowDebug = false;
#endif
		public Character[] Actors = new Character[MAX_ACTOR];
		public Sprite Hero;
		public Inventory Inventory = new Inventory();
		//Quest
		//Bank
		public Map Map;
		public int StepToBattle;
		public float StepMultiplier = 1;
	}
}
