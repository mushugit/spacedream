using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.Model.Interfaces.Services
{
    interface IGameService
    {
        void Play();
        bool Playing { get; }
        ITile GetTileAt(Point coord);
        Point GetTileCoord(ITileView tile);
        Size GetMapSize();
        Point WorldCenter { get; }
        bool Build(Point coord, TileContentType content, Size footprint);
        void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler);
        void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler);
        IEnumerator ExecuteJobs();
    }
}
