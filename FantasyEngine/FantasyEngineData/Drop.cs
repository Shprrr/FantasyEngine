using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData
{
    public class Drop
    {
        public class DropRule
        {
            private int _LevelMinimum = 0;
            private int _LevelMaximum = Job.MAX_LEVEL;

            [ContentSerializer(Optional = true)]
            public int LevelMinimum
            {
                get { return _LevelMinimum; }
                set { _LevelMinimum = value; }
            }
            [ContentSerializer(Optional = true)]
            public int LevelMaximum
            {
                get { return _LevelMaximum; }
                set { _LevelMaximum = value; }
            }

            public int Gold { get; set; }
            public List<Item> Treasure { get; set; }

            public DropRule()
            {
                Gold = 0;
                Treasure = new List<Item>();
            }

            public DropRule(int gold, List<Item> treasure)
            {
                Gold = gold;
                Treasure = treasure;
            }

            public override string ToString()
            {
                string condition = string.Empty;
                string oper = string.Empty;

                if (LevelMinimum != 0)
                {
                    condition += oper + "Level>" + LevelMinimum;
                    oper = " AND ";
                }

                if (LevelMaximum != Job.MAX_LEVEL)
                {
                    condition += oper + "Level<" + LevelMaximum;
                    oper = " AND ";
                }

                condition = condition != string.Empty ? "(" + condition + ") " : string.Empty;


                string treasure = "{";
                oper = string.Empty;

                foreach (Item item in Treasure)
                {
                    treasure += oper + item;
                    oper = ",";
                }

                treasure += "}";

                return condition + "G:" + Gold + " Items:" + treasure;
            }
        }

        private List<DropRule> _Drops = new List<DropRule>();

        /// <summary>
        /// 
        /// </summary>
        [ContentSerializer(CollectionItemName = "DropRule")]
        public List<DropRule> Drops
        {
            get { return _Drops; }
        }

        public Drop()
        {

        }

        public void GetDrop(Job job, ref int gold, ref List<Item> treasure)
        {
            foreach (DropRule rule in Drops)
            {
                if (rule.LevelMinimum > job.Level)
                    continue;

                if (rule.LevelMaximum < job.Level)
                    continue;

                // Rule passe toute les validations.
                gold = rule.Gold;
                treasure = rule.Treasure;
                break;
            }
        }
    }
}
