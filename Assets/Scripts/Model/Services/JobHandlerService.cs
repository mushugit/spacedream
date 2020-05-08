﻿using Assets.Scripts.Model.Data;
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
                        job.Execute();
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

        public bool ExecuteJob(JobCategory jobCategory, IAssignableJob job)
        {
            if (job != null && Jobs[jobCategory].Count > 0)
            {
                if (Jobs[jobCategory].Peek() == job)
                {
                    Jobs[jobCategory].Dequeue();
                    job.Execute();
                    return true;
                }
            }
            return false;
        }
    }
}
