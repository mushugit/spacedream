using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.View.Interfaces;
using System;
using System.Drawing;
using static Assets.Scripts.Model.Data.Job;
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
        void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler);
        void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler);
        bool CreateDestroyJob(Point coord, TileContentType targetContent, Size footprint);
        bool CreateBuildJob(Point coord, TileContentType content, TileContentType templateContent, Size footprint);
        IAssignableJob PeekJob(JobCategory jobCategory);
        bool DoJob(JobCategory jobCategory, IAssignableJob jobReference);
    }
}
