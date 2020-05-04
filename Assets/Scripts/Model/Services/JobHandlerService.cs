using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Data.Jobs;
using Assets.Scripts.Model.Data.Jobs.Parameters;
using Assets.Scripts.Model.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Model.Data.Job;

namespace Assets.Scripts.Model.Services
{
    class JobHandlerService : IJobHandlerService
    {
        private readonly IBuildService _buildService;

        private readonly Dictionary<JobCategory, Queue<Job>> Jobs;
        public JobHandlerService(IBuildService buildService)
        {
            _buildService = buildService;

            //Init all queues
            var categories = Enum.GetValues(typeof(Job.JobCategory)).Cast<Job.JobCategory>();
            Jobs = new Dictionary<JobCategory, Queue<Job>>();
            foreach (var jobCategory in categories)
            {
                Jobs.Add(jobCategory, new Queue<Job>());
            }
        }

        public void QueueJob(JobCategory jobCategory, JobParameter parameter)
        {
            switch (jobCategory)
            {
                case JobCategory.Build:
                    Jobs[jobCategory].Enqueue(new BuildJob(parameter as BuildJobParameter, _buildService));
                    break;
                default:
                    break;
            }
        }

        public IEnumerator ExecuteQueues()
        {
            while (true)
            {
                var categories = Enum.GetValues(typeof(Job.JobCategory)).Cast<Job.JobCategory>();
                foreach (var jobCategory in categories)
                {
                    if (Jobs[jobCategory].Count > 0)
                    {
                        var job = Jobs[jobCategory].Dequeue();
                        job.Execute();
                        yield return null;
                    }
                }
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
}
