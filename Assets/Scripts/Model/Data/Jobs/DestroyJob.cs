using Assets.Scripts.Model.Data.Jobs.Parameters;
using Assets.Scripts.Model.Interfaces.Services;
using System.Drawing;

namespace Assets.Scripts.Model.Data.Jobs
{
    class DestroyJob : Job
    {
        private readonly DestroyJobParameter _parameter;
        private readonly IBuildService _buildService;

        public override float ExecuteTime { get; protected set; }
        public override JobCategory Category { get; protected set; }

        public DestroyJob(DestroyJobParameter parameter, IBuildService buildService) : base(parameter)
        {
            _parameter = parameter;
            _buildService = buildService;
            ExecuteTime = 0.2f;
            Category = JobCategory.Destroy;
        }

        public override bool Doable()
        {
            return _buildService.CanDestroy(_parameter.Coord, _parameter.TileTargetContentType, Size.Empty);
        }

        public override void Execute()
        {
            _buildService.Destroy(_parameter.Coord, _parameter.TileTargetContentType, Size.Empty);
        }
    }
}
