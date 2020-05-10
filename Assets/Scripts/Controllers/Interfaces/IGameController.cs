using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using static Tile;

namespace Assets.Scripts.Controllers.Interfaces
{
    public interface IGameController
    {
        //ServiceContainer Container { get; }
        ITileView GetTileAt(Point coord);
        Point GetTileCoord(ITileView tile);
        Size GetMapSize();
        Point WorldCenter { get; }
        bool Playing { get; }
        IGameCharacterController CharacterController { get; }

        void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler);
        void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler);
        bool CreateBuildJob(Point coord, TileContentType content, TileContentType templateContent, Size footprint);
        bool CreateDestroyJob(Point coord, TileContentType targetContent, Size footprint);
        IAssignableJob PeekJob(Job.JobCategory jobCategory);
        bool DoJob(Job.JobCategory jobCategory, IAssignableJob jobReference, Action callback = null);
        void RegisterJobExecutor(IJobExecutorController jobExecutorController);
        void SubscribeSpecificTileContentChanged(TileContentType contentType, EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler);
        List<(Point coord, ITileView tile)> GetNeighbours(Point coord);
    }
}
