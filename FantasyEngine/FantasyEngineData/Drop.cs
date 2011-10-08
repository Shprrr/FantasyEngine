using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using FantasyEngineData.Entities;
using FantasyEngineData.Items;

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

            [ContentSerializerIgnore()]
            public List<BaseItem> Treasure { get; set; }
            [ContentSerializer(ElementName = "Treasure")]
            public List<string> TreasureRef { get; set; }

            public DropRule()
            {
                Gold = 0;
                Treasure = new List<BaseItem>();
                TreasureRef = new List<string>();
            }

            public DropRule(int gold, List<BaseItem> treasure)
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

                foreach (BaseItem item in Treasure)
                {
                    treasure += oper + item.Name;
                    oper = "; ";
                }

                treasure += "}";

                return condition + "G:" + Gold + " Items:" + treasure;
            }
        }

        public class DropRuleContentReader : ContentTypeReader<DropRule>
        {
            protected override DropRule Read(ContentReader input, DropRule existingInstance)
            {
                DropRule dropRule = existingInstance;

                if (dropRule == null)
                {
                    dropRule = new DropRule();
                }

                dropRule.LevelMinimum = input.ReadInt32();
                dropRule.LevelMaximum = input.ReadInt32();
                dropRule.Gold = input.ReadInt32();
                dropRule.TreasureRef = input.ReadObject<List<string>>();

                // Add all treasure in the treasure list.
                foreach (string treasure in dropRule.TreasureRef)
                {
                    string[] trea = treasure.Trim().Split('\\');
                    dropRule.Treasure.Add(ItemManager.GetBaseItem(trea[0], trea[1]));
                }

                return dropRule;
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

        public void GetDrop(Job job, ref int gold, ref List<BaseItem> treasure)
        {
            List<DropRule> dropValidated = new List<DropRule>();
            foreach (DropRule rule in Drops)
            {
                if (rule.LevelMinimum > job.Level)
                    continue;

                if (rule.LevelMaximum < job.Level)
                    continue;

                // Rule passe toute les validations.
                dropValidated.Add(rule);
            }

            int index = Extensions.rand.Next(dropValidated.Count);
            gold = dropValidated[index].Gold;
            treasure = dropValidated[index].Treasure;
        }
    }
}
