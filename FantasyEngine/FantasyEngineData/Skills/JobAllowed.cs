using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using FantasyEngineData.Entities;

namespace FantasyEngineData.Skills
{
    public partial class BaseSkill
    {
        public class JobAllowed
        {
            public struct SkillNameLevel
            {
                private string _Name;

                [ContentSerializer(Optional = false, FlattenContent = true)]
                public string Name
                {
                    get { return _Name; }
                    set
                    {
                        if (value != null)
                            value = value.Trim();
                        _Name = value;
                    }
                }
                [ContentSerializer(Optional = true)]
                public int Level { get; set; }

                public override string ToString()
                {
                    return Name + " " + Level;
                }
            }

            private string _JobAbbreviation;

            [ContentSerializerIgnore()]
            public BaseJob Job { get; set; }
            [ContentSerializer(ElementName = "Job")]
            public string JobAbbreviation
            {
                get
                {
                    if (Job == null)
                        return _JobAbbreviation;
                    return Job.JobAbbreviation;
                }
                set
                {
                    _JobAbbreviation = value;
                    Job = JobManager.GetBaseJob(value);
                }
            }

            [ContentSerializer(Optional = true)]
            public int Level { get; set; }

            [ContentSerializer(Optional = true, ElementName = "Skill")]
            public SkillNameLevel Skill { get; set; }

            [ContentSerializer(Optional = true)]
            public int MaxLevel { get; set; }

            public override string ToString()
            {
                return JobAbbreviation + " " + Level + " " + Skill + " " + MaxLevel;
            }
        }
    }
}
