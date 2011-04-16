using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData.Items
{
    public enum Hands { One, Two }
    public enum ArmorLocation { Body, Head, Hands, Feet }

    public abstract class BaseItem
    {
        public const int NAME_LENGTH = 30;

        #region Field Region
        string name;
        string type;
        int price;
        float weight;
        protected List<BaseJob> allowableJobs = new List<BaseJob>();
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

        [ContentSerializerIgnore()]
        public List<BaseJob> AllowableJobs
        {
            get { return allowableJobs; }
        }
        [ContentSerializer(ElementName = "AllowableJobs")]
        public string AllowableJobsRef { get; set; }

        public bool IsEquiped
        {
            get { return equipped; }
            protected set { equipped = value; }
        }
        #endregion

        #region Constructor Region
        public BaseItem()
        {

        }

        public BaseItem(string name, string type, int price, float weight, params BaseJob[]
            allowableClasses)
        {
            foreach (BaseJob job in allowableClasses)
                AllowableJobs.Add(job);
            Name = name;
            Type = type;
            Price = price;
            Weight = weight;
        }
        #endregion

        #region Abstract Method Region
        public abstract object Clone();

        public virtual bool CanEquip(BaseJob characterType)
        {
            return allowableJobs.Contains(characterType);
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
        #endregion

        /// <summary>
        /// Determine if the job is allowed to use this item.
        /// </summary>
        /// <param name="job">Job who wants to use this item</param>
        /// <returns></returns>
        public bool IsAllowed(BaseJob job)
        {
            if (AllowableJobsRef == "*")
                return true;

            string[] allowableJobs = AllowableJobsRef.Trim().Split(' ');

            foreach (string allowableJob in allowableJobs)
            {
                if (job.JobAbbreviation == allowableJob)
                    return true;
            }

            return false;
        }
    }
}
