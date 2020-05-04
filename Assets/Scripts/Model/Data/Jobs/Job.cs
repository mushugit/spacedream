using Assets.Scripts.Model.Data.Jobs.Parameters;
using Assets.Scripts.Model.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.Data
{
    public abstract class Job : IAssignableJob
    {
        public enum JobState { Todo, Doing, Done }
        //TODO not fixed list of category, each category as a "theme" : rest, exercice, recreation, creation
        public enum JobCategory { Build }

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
    }
}
