using Assets.Scripts.Controllers;
using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Data.Jobs;
using Assets.Scripts.Model.Data.Jobs.Parameters;
using Assets.Scripts.Model.Interfaces.Data;
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
        private IJobExecutorController _jobExecutorController;

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

        public void RegisterJobExecutor(IJobExecutorController jobExecutorController)
        {
            _jobExecutorController = jobExecutorController;
        }

        public void QueueJob(JobCategory jobCategory, JobParameter parameter)
        {
            Job job;
            switch (jobCategory)
            {
                case JobCategory.Build:
                    job = new BuildJob(parameter as BuildJobParameter, _buildService);
                    if (job.Doable())
                    {
                        Jobs[jobCategory].Enqueue(job);
                    }
                    else
                    {
                        Debug.Log($"Job <Build> at {parameter.Coord} not doable");
                    }
                    break;
                case JobCategory.Destroy:
                    job = new DestroyJob(parameter as DestroyJobParameter, _buildService);
                    if (job.Doable())
                    {
                        Jobs[jobCategory].Enqueue(job);
                    }
                    else
                    {
                        Debug.Log($"Job <Destroy> at {parameter.Coord} not doable");
                    }
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
                        ExecuteJob(job);
                        yield return null;
                    }
                }
                yield return new WaitForSeconds(0.005f);
            }
        }

        public IAssignableJob PeekJobQueue(JobCategory jobCategory)
        {
            return Jobs[jobCategory].Count > 0 ? Jobs[jobCategory].Peek() : null;
        }

        public bool ExecuteJob(JobCategory jobCategory, IAssignableJob job, Action callback = null)
        {
            if (job != null && Jobs[jobCategory].Count > 0)
            {
                var executableJob = Jobs[jobCategory].Peek();
                if (executableJob == job)
                {
                    Jobs[jobCategory].Dequeue();
                    ExecuteJob(executableJob, callback);
                    return true;
                }
            }
            return false;
        }

        private void ExecuteJob(IExecutableJob job, Action callback = null)
        {
            _jobExecutorController.ExecuteJob(job, callback);
        }
    }
}
