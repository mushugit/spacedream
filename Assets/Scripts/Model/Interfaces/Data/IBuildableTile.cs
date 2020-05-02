using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.Model.Interfaces.Data
{
    public interface IBuildableTile : ITile
    {
        void Build(TileContentType content, ITile mainContentTile);

        void Destroy();
    }
}
