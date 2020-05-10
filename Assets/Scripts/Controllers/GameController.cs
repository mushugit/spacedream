using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.Model.Interfaces.Services;
using Assets.Scripts.View.Interfaces;
using LightInject;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Assets.Scripts.Model.Data.Job;
using static Tile;

namespace Assets.Scripts.Controllers
{
    class GameController : MonoBehaviour, IGameController
    {
        private IGameService _gameservice;

        private ServiceContainer _container;
        private IJobExecutorController _jobExecutorController;

        public IGameCharacterController CharacterController { get; private set; }

        void Awake()
        {
            _container = new ServiceContainer();
            _container.RegisterServices();

            _gameservice = _container.GetInstance<IGameService>();
            CharacterController = GetComponentInChildren<IGameCharacterController>();

            DelayedRegisterJobExecutor();

            _gameservice.Play();
        }

        public bool Playing
        {
            get
            {
                return _gameservice.Playing;
            }
        }

        public Point WorldCenter
        {
            get { return _gameservice.WorldCenter; }
        }

        public void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler)
        {
            _gameservice.SubscribeAllTileTypeChanged(tileTypeChangedEventHandler);
        }

        public void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler)
        {
            _gameservice.SubscribeAllTileContentChanged(tileContentChangedEventHandler);
        }

        public void SubscribeSpecificTileContentChanged(TileContentType contentType, EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler)
        {
            _gameservice.SubscribeSpecificTileContentChanged(contentType, tileContentChangedEventHandler);
        }

        public ITileView GetTileAt(Point coord)
        {
            return _gameservice.GetTileAt(coord);
        }

        public Point GetTileCoord(ITileView tile)
        {
            return _gameservice.GetTileCoord(tile);
        }

        public List<(Point coord, ITileView tile)> GetNeighbours(Point coord)
        {
            var originalList = _gameservice.GetNeighbours(coord);
            var newList = new List<(Point coord, ITileView tile)>(originalList.Count);

            foreach ((var localCoord, var tile) in originalList)
            {
                newList.Add((localCoord, tile));
            }
            return newList;


            //return _gameservice.GetNeighbours(coord).Cast<(Point coord, ITileView tile)>().ToList();
        }

        public Size GetMapSize()
        {
            return _gameservice.GetMapSize();
        }

        public bool CreateBuildJob(Point coord, TileContentType content, TileContentType templateContent, Size footprint)
        {
            return _gameservice.CreateBuildJob(coord, content, templateContent, footprint);
        }

        public bool CreateDestroyJob(Point coord, TileContentType targetContent, Size footprint)
        {
            return _gameservice.CreateDestroyJob(coord, targetContent, footprint);
        }

        public IAssignableJob PeekJob(JobCategory jobCategory)
        {
            return _gameservice.PeekJob(jobCategory);
        }

        public bool DoJob(JobCategory jobCategory, IAssignableJob jobReference, Action callback = null)
        {
            return _gameservice.DoJob(jobCategory, jobReference, callback);
        }

        public void RegisterJobExecutor(IJobExecutorController jobExecutorController)
        {
            _jobExecutorController = jobExecutorController;
        }

        private void DelayedRegisterJobExecutor()
        {
            _gameservice.RegisterJobExecutor(_jobExecutorController);
        }
    }
}
