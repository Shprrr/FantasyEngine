using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FantasyEngineData.Entities
{
    public partial class BaseJob
    {
        public class JobPrerequisite
        {
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

            public int Level { get; set; }

            public override string ToString()
            {
                return JobAbbreviation + " " + Level;
            }
        }
    }
}
