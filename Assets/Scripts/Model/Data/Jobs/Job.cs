using Assets.Scripts.Model.Data.Jobs.Parameters;
using Assets.Scripts.Model.Interfaces.Data;

namespace Assets.Scripts.Model.Data
{
    public abstract class Job : IAssignableJob, IExecutableJob
    {
        public enum JobState { Todo, Doing, Done }
        //TODO not fixed list of category, each category as a "theme" : rest, exercice, recreation, creation
        public enum JobCategory { Build, Destroy }

        public bool Assigned { get; private set; }

        public JobParameter Parameter { get; private set; }

        public Job(JobParameter parameter)
        {
            Parameter = parameter;
        }
        public void Assign()
        {
            Assigned = true;
        }

        public void Unassign()
        {
            Assigned = false;
        }

        public abstract void Execute();
        public abstract bool Doable();
    }
}
