using Assets.Scripts.Controllers;
using Assets.Scripts.Model.Data.Jobs.Parameters;
using Assets.Scripts.Model.Interfaces.Data;
using System;
using static Assets.Scripts.Model.Data.Job;

namespace Assets.Scripts.Model.Interfaces.Services
{
    public interface IJobHandlerService
    {
        void QueueJob(JobCategory jobCategory, JobParameter parameter);

        IAssignableJob PeekJobQueue(JobCategory jobCategory);
        bool ExecuteJob(JobCategory jobCategory, IAssignableJob job, Action callback = null);
        void RegisterJobExecutor(IJobExecutorController jobExecutorController);
    }
}
