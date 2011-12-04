using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using FantasyEngineData.Battles;
using FantasyEngineData.Entities;

namespace FantasyEngineData.Skills
{
    public partial class BaseSkill
    {
        public const int NAME_LENGTH = 14;

        #region Field Region
        private string name;
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

        public string Description { get; set; }

        /// <summary>
        /// Minimum MP cost to cast the skill.
        /// </summary>
        public int BaseCostMP { get; set; }
        /// <summary>
        /// Add this number to the MP cost to each level learned.
        /// </summary>
        public float AddByLevelCostMP { get; set; }
        /// <summary>
        /// Coefficient to the squared level for the MP cost.
        /// </summary>
        public float QuadraticCoefficientLevelCostMP { get; set; }

        //public string AllowableJobs { get; set; }
        private List<JobAllowed> _AllowableJobs = new List<JobAllowed>();
        public List<JobAllowed> AllowableJobs { get { return _AllowableJobs; } }

        [ContentSerializer(Optional = true)]
        public EffectLevel Effect { get; set; }

        public eTargetType DefaultTarget { get; set; }

        public bool MenuUsable { get; set; }

        /// <summary>
        /// Experience to learn this skill level 1.
        /// </summary>
        public int ExpToLearn { get; set; }
        #endregion

        public BaseSkill()
        {
            BaseCostMP = 0;
            AddByLevelCostMP = 0;
            QuadraticCoefficientLevelCostMP = 0;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Determine if the job is allowed to use this item.
        /// </summary>
        /// <param name="job">Job who wants to use this item</param>
        /// <returns></returns>
        public JobAllowed IsAllowed(BaseJob job)
        {
            foreach (JobAllowed allowableJob in AllowableJobs)
            {
                if (job == allowableJob.Job)
                    return allowableJob;
            }

            return null;
        }
    }
}
