using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.View.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using static Tile;

namespace Assets.Scripts.Model.Services.DataServices
{
    public interface IMapDataService
    {
        bool MapInitialized { get; }
        double MapArea { get; }
        Size Size { get; }

        IBuildableMapService InitializeMap(Size Size);

        ITile GetTileAt(Point coord);
        void InitTileAt(Point coord);
        Point GetTileCoord(ITileView tile);
        int GetNearestContent(Point coord, TileContentType content);


        //Cells (Point + Tile) operations
        List<(Point Coord, ITile Tile)> GetAllCells();
        List<(Point Coord, ITile Tile)> GetCellsFromFootprint((Point Coord, ITile Tile) startingCell, Size footprint);
        List<(Point Coord, ITile Tile)> GetNeighbours(Point point);
    }
}