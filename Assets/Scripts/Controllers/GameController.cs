﻿using Assets.Scripts.Controllers.Interfaces;
using Assets.Scripts.Model.Interfaces.Services;
using Assets.Scripts.View.Interfaces;
using LightInject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Tile;

namespace Assets.Scripts.Controllers
{
    class GameController : MonoBehaviour, IGameController
    {
        private IGameService _gameservice;

        public ServiceContainer Container { get; private set; }

        void Awake()
        {
            Container = new ServiceContainer();

            Container.RegisterServices();

            _gameservice = Container.GetInstance<IGameService>();

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

        private bool Build(Point coord, TileContentType content, Size footprint)
        {
            return _gameservice.Build(coord, content, footprint);
        }

    }
}
