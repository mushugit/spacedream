using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
