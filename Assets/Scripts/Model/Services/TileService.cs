using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.Model.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.Model.Services
{
    class TileService : ITileService
    {
        public TileType GetDefaultTileType()
        {
            return TileType.Space;
        }

        public IBuildableTile GetFromPrototype(TileType type, ITile mainContentTile)
        {
            return new Tile(type, mainContentTile);
        }
    }
}
