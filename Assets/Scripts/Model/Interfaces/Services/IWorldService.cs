using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.Model.Interfaces
{
    public interface IWorldService
    {
        void GenerateMap(Size size, int seed);
        Size Size { get; }
        ITile GetTileAt(Point coord);
        Point GetTileCoord(ITileView tile);
        bool Build(Point coord, TileContentType content, Size footprint, ITile mainContentTile = null);
        void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler);
        void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler);

        Point BaseCenter { get; }
    }
}
