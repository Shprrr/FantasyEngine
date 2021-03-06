using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData.Entities
{
    public partial class BaseJob
    {
        public const int JOB_NAME_LENGTH = 16;
        public const int MAX_HP = 9999;
        public const int MAX_MP = 9999;
        public const int MAX_STAT = 255;

        private string _JobName;
        private int _MaxHp;
        private int _MaxMp;
        private int _Strength;
        private int _Vitality;
        private int _Accuracy;
        private int _Agility;
        private int _Intelligence;
        private int _Wisdom;

        public BattleSprite BattleSprite;

        #region Properties
        [ContentSerializer(Optional = true)]
        public string JobAbbreviation { get; set; }

        public string JobName
        {
            get { return _JobName; }
            set
            {
                if (value.Length >= JOB_NAME_LENGTH)
                    value = value.Remove(JOB_NAME_LENGTH);
                _JobName = value;
            }
        }

        public int MaxHp
        {
            get { return _MaxHp; }
            set
            {
                if (value > MAX_HP)
                    value = MAX_HP;
                if (value < 0)
                    value = 0;
                _MaxHp = value;
            }
        }

        public int MaxMp
        {
            get { return _MaxMp; }
            set
            {
                if (value > MAX_MP)
                    value = MAX_MP;
                if (value < 0)
                    value = 0;
                _MaxMp = value;
            }
        }

        public int Strength
        {
            get { return _Strength; }
            set
            {
                if (value > MAX_STAT)
                    value = MAX_STAT;
                if (value < 1)
                    value = 1;
                _Strength = value;
            }
        }

        public int Vitality
        {
            get { return _Vitality; }
            set
            {
                if (value > MAX_STAT)
                    value = MAX_STAT;
                if (value < 1)
                    value = 1;
                _Vitality = value;
            }
        }

        public int Accuracy
        {
            get { return _Accuracy; }
            set
            {
                if (value > MAX_STAT)
                    value = MAX_STAT;
                if (value < 1)
                    value = 1;
                _Accuracy = value;
            }
        }

        public int Agility
        {
            get { return _Agility; }
            set
            {
                if (value > MAX_STAT)
                    value = MAX_STAT;
                if (value < 1)
                    value = 1;
                _Agility = value;
            }
        }

        public int Intelligence
        {
            get { return _Intelligence; }
            set
            {
                if (value > MAX_STAT)
                    value = MAX_STAT;
                if (value < 1)
                    value = 1;
                _Intelligence = value;
            }
        }

        public int Wisdom
        {
            get { return _Wisdom; }
            set
            {
                if (value > MAX_STAT)
                    value = MAX_STAT;
                if (value < 1)
                    value = 1;
                _Wisdom = value;
            }
        }

        private List<JobPrerequisite> _PrerequisiteJobs = new List<JobPrerequisite>();
        [ContentSerializer(Optional = true)]
        public List<JobPrerequisite> PrerequisiteJobs { get { return _PrerequisiteJobs; } }

        [ContentSerializer(Optional = true)]
        public Rectangle OverworldSpriteSize { get; set; }
        #endregion Properties

        public BaseJob()
        {
            OverworldSpriteSize = Rectangle.Empty;
        }

        public override string ToString()
        {
            return JobName;
        }

        /// <summary>
        /// Determine if the character is allowed to use this job.
        /// </summary>
        /// <param name="character">Character who wants to use this job</param>
        /// <returns></returns>
        public bool IsAllowed(Character character)
        {
            foreach (var prerequisiteJob in PrerequisiteJobs)
            {
                foreach (var job in character.Jobs)
                {
                    if (job != null && job.BaseJob == prerequisiteJob.Job && job.Level < prerequisiteJob.Level)
                        return false;
                }
            }

            return true;
        }
    }
}
