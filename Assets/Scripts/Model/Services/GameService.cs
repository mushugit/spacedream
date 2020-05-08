using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Data.Jobs.Parameters;
using Assets.Scripts.Model.Interfaces;
using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.Model.Interfaces.Services;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Model.Data.Job;
using static Tile;

namespace Assets.Scripts.Model.Services
{
    class GameService : IGameService
    {
        private readonly IWorldService _worldService;
        private readonly IEventService _eventService;
        private readonly IJobHandlerService _jobHandlerService;

        public bool Playing { get; private set; } = false;

        public bool BuildMode { get; private set; }

        public Point WorldCenter
        {
            get { return _worldService.BaseCenter; }
        }

        public GameService(IEventService eventService, IWorldService worldService, IJobHandlerService jobHandlerService)
        {
            _eventService = eventService;
            _worldService = worldService;
            _jobHandlerService = jobHandlerService;
        }

        public void Play()
        {
            Debug.Log("Playing game !");
            _worldService.GenerateMap(new Size(255, 255), 42);
            Playing = true;
        }


        public void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler)
        {
            _eventService.SubscribeAllTileTypeChanged(tileTypeChangedEventHandler);
        }

        public void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler)
        {
            _eventService.SubscribeAllTileContentChanged(tileContentChangedEventHandler);
        }

        public ITile GetTileAt(Point coord)
        {
            return _worldService.GetTileAt(coord);
        }

        public Point GetTileCoord(ITileView tile)
        {
            return _worldService.GetTileCoord(tile);
        }

        public Size GetMapSize()
        {
            return _worldService.Size;
        }

        public bool CreateBuildJob(Point coord, TileContentType content, TileContentType templateContent, Size footprint)
        {
            _worldService.Build(coord, templateContent, footprint);
            _jobHandlerService.QueueJob(JobCategory.Build, new BuildJobParameter(coord, content));
            return true;
        }

        public bool CreateDestroyJob(Point coord, TileContentType targetContent, Size footprint)
        {
            //TODO display destroy job (red cross ?)
            _jobHandlerService.QueueJob(JobCategory.Destroy, new DestroyJobParameter(coord, targetContent));
            return true;
        }

        public IAssignableJob PeekJob(JobCategory jobCategory)
        {
            return _jobHandlerService.PeekJobQueue(jobCategory);
        }

        public bool DoJob(JobCategory jobCategory, IAssignableJob jobReference)
        {
            return _jobHandlerService.ExecuteJob(jobCategory, jobReference);
        }
    }
}
