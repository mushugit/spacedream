using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controllers
{
    public interface IJobExecutorController
    {
        void ExecuteJob(IExecutableJob job, Action callback = null);
    }
}
