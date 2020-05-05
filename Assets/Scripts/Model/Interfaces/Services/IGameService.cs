using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections;
using System.Drawing;
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
        bool Destroy(Point coord, TileContentType targetContent, Size footprint);
    }
}
