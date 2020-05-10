using Assets.Scripts.Model.Data.Jobs.Parameters;
using Assets.Scripts.Model.Interfaces.Services;
using System.Drawing;

namespace Assets.Scripts.Model.Data.Jobs
{
    class BuildJob : Job
    {
        private readonly IBuildService _buildService;
        private readonly BuildJobParameter _parameter;

        public override float ExecuteTime { get; protected set; }
        public override JobCategory Category { get; protected set; }

        public BuildJob(BuildJobParameter parameter, IBuildService buildService) : base(parameter)
        {
            _parameter = parameter;
            _buildService = buildService;
            ExecuteTime = 0.3f;
            Category = JobCategory.Build;
        }

        public override void Execute()
        {
            _buildService.Build(_parameter.Coord, _parameter.TileContentType, Size.Empty);
        }
        public override bool Doable()
        {
            return _buildService.CanBuild(_parameter.Coord, _parameter.TileContentType, Size.Empty);
        }


    }
}
