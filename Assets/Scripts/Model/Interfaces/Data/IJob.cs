using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Data.Jobs.Parameters;

namespace Assets.Scripts.Model.Interfaces.Data
{
    public interface IJob
    {
        JobParameter Parameter { get; }
        Job.JobCategory Category { get; }

        bool Doable();
    }
}
