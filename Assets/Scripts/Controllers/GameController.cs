using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.Model.Interfaces.Services;
using Assets.Scripts.View.Interfaces;
using LightInject;
using System;
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

        public ICharacterController CharacterController { get; private set; }

        void Awake()
        {
            _container = new ServiceContainer();
            _container.RegisterServices();

            _gameservice = _container.GetInstance<IGameService>();
            CharacterController = GetComponentInChildren<ICharacterController>();

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

        public ITileView GetTileAt(Point coord)
        {
            return _gameservice.GetTileAt(coord);
        }

        public Point GetTileCoord(ITileView tile)
        {
            return _gameservice.GetTileCoord(tile);
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

        public bool DoJob(JobCategory jobCategory, IAssignableJob jobReference)
        {
            return _gameservice.DoJob(jobCategory, jobReference);
        }

    }
}
