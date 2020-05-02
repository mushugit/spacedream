using Assets.Scripts.View.Interfaces;
using LightInject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.Controllers.Interfaces
{
    public interface IGameController
    {
        ServiceContainer Container { get; }
        ITileView GetTileAt(Point coord);
        Point GetTileCoord(ITileView tile);
        Size GetMapSize();
        Point WorldCenter { get; }
        bool Playing { get; }
        void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler);
        void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler);
    }
}
