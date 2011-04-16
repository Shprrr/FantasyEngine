using System;
using System.Collections.Generic;
using System.Linq;
using FantasyEngineData;
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

        public Character[] Actors = new Character[MAX_ACTOR];
        public Sprite Hero;
        public Inventory Inventory = new Inventory();
        //Quest
        //Bank
        public MapObject Map;
    }
}
