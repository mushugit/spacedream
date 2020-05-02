using Assets.Scripts.Model.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.Model.Interfaces.Services
{
    public interface ITileService
    {
        TileType GetDefaultTileType();
        IBuildableTile GetFromPrototype(TileType type,  ITile mainContentTile);
    }
}
