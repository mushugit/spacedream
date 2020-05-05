using System.Drawing;
using static Tile;

namespace Assets.Scripts.Model.Data.Jobs.Parameters
{
    class DestroyJobParameter : JobParameter
    {
        public DestroyJobParameter(Point coord, TileContentType tileTargetContentType) : base(coord)
        {
            TileTargetContentType = tileTargetContentType;
        }

        public TileContentType TileTargetContentType { get; }
    }
}
