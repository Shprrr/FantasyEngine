using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData.Items
{
    public class Item : BaseItem
    {
        //#region Properties
        //new public string Name
        //{
        //    get { return base.Name; }
        //    set { base.Name = value; }
        //}
        //new public string Type
        //{
        //    get { return base.Type; }
        //    protected set { base.Type = value; }
        //}
        //new public int Price
        //{
        //    get { return base.Price; }
        //    protected set { base.Price = value; }
        //}
        //#endregion Properties

        #region Constructors
        public Item()
            : base()
        {

        }

        public Item(string name, string type, int price, float weight, params BaseJob[]
                allowableClasses)
            : base(name, type, price, weight, allowableClasses)
        {

        }
        #endregion Constructors

        #region Abstract Method Region
        public override object Clone()
        {
            BaseJob[] allowedClasses = new BaseJob[allowableJobs.Count];
            for (int i = 0; i < allowableJobs.Count; i++)
                allowedClasses[i] = allowableJobs[i];
            Item item = new Item(
            Name,
            Type,
            Price,
            Weight,
            allowedClasses);
            return item;
        }
        #endregion Abstract Method Region
    }
}
