using Assets.Scripts.Model.Data.Jobs.Parameters;
using System.Collections;
using static Assets.Scripts.Model.Data.Job;

namespace Assets.Scripts.Model.Interfaces.Services
{
    public interface IJobHandlerService
    {
        IEnumerator ExecuteQueues();
        void QueueJob(JobCategory jobCategory, JobParameter parameter);
    }
}
