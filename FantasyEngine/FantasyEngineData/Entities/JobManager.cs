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
            if (!baseJobs.ContainsKey(baseJob.JobName))
            {
                baseJobs.Add(baseJob.JobName, baseJob);
            }
        }

        public static BaseJob GetBaseJob(string jobName)
        {
            if (baseJobs.ContainsKey(jobName))
            {
                return (BaseJob)baseJobs[jobName];
            }
            return null;
        }
    }
}
