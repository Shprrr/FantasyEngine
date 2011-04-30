using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData.Items
{
    public class Item : BaseItem
    {
        #region Constructors
        public Item()
            : base()
        {

        }

        public Item(string name, string type, int price, float weight, string allowableJobs)
            : base(name, type, price, weight, allowableJobs)
        {

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
                AllowableJobs);
            return item;
        }
        #endregion Abstract Method Region
    }
}
