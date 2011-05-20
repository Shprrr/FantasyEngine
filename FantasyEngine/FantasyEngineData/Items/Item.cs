using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using FantasyEngineData.Battles;

namespace FantasyEngineData.Items
{
    public class Item : BaseItem
    {
        public const string TYPE_CONSUMABLE = "Consumable";

        public eTargetType DefaultTarget { get; set; }

        #region Constructors
        public Item()
            : base()
        {

        }

        public Item(string name, string type, int price, float weight, string allowableJobs, Effect effect, eTargetType defaultTarget)
            : base(name, type, price, weight, allowableJobs, effect)
        {
            DefaultTarget = defaultTarget;
        }
        #endregion Constructors

        #region Abstract Method Region
        public override object Clone()
        {
            Item item = new Item(
                Name,
                Type,
                Price,
                Weight,
                AllowableJobs,
                Effect,
                DefaultTarget);
            return item;
        }
        #endregion Abstract Method Region
    }
}
