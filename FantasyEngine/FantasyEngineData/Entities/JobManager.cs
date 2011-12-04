using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasyEngineData.Entities
{
    public static class JobManager
    {
        static Dictionary<string, BaseJob> baseJobs = new Dictionary<string, BaseJob>();

        public static void Load(params BaseJob[] baseJobs)
        {
            foreach (BaseJob baseJob in baseJobs)
            {
                AddBaseJob(baseJob);
            }
        }

        public static void AddBaseJob(BaseJob baseJob)
        {
            if (!baseJobs.ContainsKey(baseJob.JobAbbreviation))
            {
                baseJobs.Add(baseJob.JobAbbreviation, baseJob);
            }
        }

        public static BaseJob[] GetAllBaseJob()
        {
            return baseJobs.Values.ToArray();
        }

        public static BaseJob GetBaseJob(string jobAbbreviation)
        {
            if (baseJobs.ContainsKey(jobAbbreviation))
            {
                return (BaseJob)baseJobs[jobAbbreviation];
            }
            return null;
        }
    }
}
