using Assets.Scripts.Model.Data.Jobs.Parameters;
using Assets.Scripts.Model.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.Data.Jobs
{
    class DestroyJob : Job
    {
        private readonly DestroyJobParameter _parameter;
        private readonly IBuildService _buildService;

        public DestroyJob(DestroyJobParameter parameter, IBuildService buildService) : base(parameter)
        {
            _parameter = parameter;
            _buildService = buildService;
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
