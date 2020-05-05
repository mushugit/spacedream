using System.Drawing;
using static Tile;

namespace Assets.Scripts.Model.Data.Jobs.Parameters
{
    class BuildJobParameter : JobParameter
    {
        public TileContentType TileContentType { get; }
        public BuildJobParameter(Point coord, TileContentType tileContentType) : base(coord)
        {
            TileContentType = tileContentType;
        }
    }
}
