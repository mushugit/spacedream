using System.Drawing;

namespace Assets.Scripts.Model.Data.Jobs.Parameters
{
    public abstract class JobParameter
    {
        public Point Coord { get; }
        public JobParameter(Point coord)
        {
            Coord = coord;
        }
    }
}
