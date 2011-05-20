using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData.Items
{
    public enum Hands { One, Two }
    public enum ArmorLocation { Body, Head, Arms, Feet }

    public abstract class BaseItem
    {
        public const int NAME_LENGTH = 30;

        #region Field Region
        string name;
        string type;
        int price;
        float weight;
        bool equipped = false;
        #endregion

        #region Property Region
        public string Name
        {
            get { return name; }
            set
            {
                if (value.Length >= NAME_LENGTH)
                    value = value.Remove(NAME_LENGTH);
                name = value;
            }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public int Price
        {
            get { return price; }
            set { price = value; }
        }
        public float Weight
        {
            get { return weight; }
            protected set { weight = value; }
        }

        public string AllowableJobs { get; set; }

        [ContentSerializer(Optional = true)]
        public Effect Effect { get; set; }

        [ContentSerializerIgnore()]
        public bool IsEquiped
        {
            get { return equipped; }
            set { if (!(this is Item)) equipped = value; }
        }
        #endregion

        #region Constructor Region
        public BaseItem()
        {

        }

        public BaseItem(string name, string type, int price, float weight, string allowableJobs, Effect effect)
        {
            Name = name;
            Type = type;
            Price = price;
            Weight = weight;
            AllowableJobs = allowableJobs;
            Effect = effect;
        }
        #endregion

        #region Methods
        public abstract object Clone();

        /// <summary>
        /// Determine if the job is allowed to use this item.
        /// </summary>
        /// <param name="job">Job who wants to use this item</param>
        /// <returns></returns>
        public bool IsAllowed(BaseJob job)
        {
            if (AllowableJobs == "*")
                return true;

            string[] allowableJobs = AllowableJobs.Trim().Split(' ');

            foreach (string allowableJob in allowableJobs)
            {
                if (job.JobAbbreviation == allowableJob)
                    return true;
            }

            return false;
        }

        public override string ToString()
        {
            string itemString = "";
            itemString += Name + ", ";
            itemString += Type + ", ";
            itemString += Price.ToString() + ", ";
            itemString += Weight.ToString();
            return itemString;
        }

        public override bool Equals(object obj)
        {
            if (obj is BaseItem)
                return Name == ((BaseItem)obj).Name;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Methods

        public static bool operator ==(BaseItem item1, BaseItem item2)
        {
            if (((object)item1) == null)
                return ((object)item2) == null;
            return item1.Equals(item2);
        }

        public static bool operator !=(BaseItem item1, BaseItem item2) { return !(item1 == item2); }
    }
}
