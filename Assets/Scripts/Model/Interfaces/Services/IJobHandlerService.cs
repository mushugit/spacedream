using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Data.Jobs.Parameters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.Model.Data.Job;

namespace Assets.Scripts.Model.Interfaces.Services
{
    public interface IJobHandlerService
    {
        IEnumerator ExecuteQueues();
        void QueueJob(JobCategory jobCategory, JobParameter parameter);
    }
}
