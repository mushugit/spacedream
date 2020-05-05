using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.Model.Interfaces.Services;
using Assets.Scripts.Model.Services.DataServices;
using Assets.Scripts.View.Interfaces;
using System;
using System.Drawing;
using UnityEngine;
using static Tile;

namespace Assets.Scripts.Model.Services
{
    class BuildService : IBuildService
    {
        private readonly IMapDataService _mapDataService;
        private bool _initialized;
        private IBuildableMapService _buildableMapService;

        public event EventHandler<TileTypeChangedEventArgs> TileTypeChanged;
        public event EventHandler<TileContentChangedEventArgs> TileContentChanged;

        public BuildService(IMapDataService mapDataService)
        {
            _mapDataService = mapDataService;
            _initialized = false;
        }

        public void Initialize(IBuildableMapService buildableMapService)
        {
            if (!_initialized)
            {
                _buildableMapService = buildableMapService;
                _initialized = true;
            }
        }

        protected virtual void OnTileTypeChanged(ITileView tile, TileTypeChangedEventArgs e)
        {
            TileTypeChanged?.Invoke(tile, e);
        }

        protected virtual void OnTileContentChanged(ITileView tile, TileContentChangedEventArgs e)
        {
            TileContentChanged?.Invoke(tile, e);
        }

        public bool CanDestroy(Point coord, TileContentType targetContent, Size footprint, ITile mainContentTile = null)
        {
            if (!_initialized)
            {
                Debug.LogError($"[BuildService] Trying to destroy before initialisation");
                return false;
            }

            var originTile = _mapDataService.GetTileAt(coord);
            if (!CanDestroy(coord, targetContent, mainContentTile))
            {
                return false;
            }
            for (var dx = 0; dx < footprint.Width; dx++)
            {
                for (var dy = 0; dy < footprint.Height; dy++)
                {
                    if (!(dx == 0 && dy == 0))
                    {
                        var point = new Point(coord.X + dx, coord.Y + dy);
                        if (!CanDestroy(point, targetContent, originTile))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public bool Destroy(Point coord, TileContentType targetContent, Size footprint, ITile mainContentTile = null)
        {
            if (!_initialized)
            {
                Debug.LogError($"[BuildService] Trying to destroy before initialisation");
                return false;
            }

            var originTile = _mapDataService.GetTileAt(coord);
            if (!Destroy(coord, targetContent, mainContentTile))
            {
                return false;
            }
            for (var dx = 0; dx < footprint.Width; dx++)
            {
                for (var dy = 0; dy < footprint.Height; dy++)
                {
                    if (!(dx == 0 && dy == 0))
                    {
                        var point = new Point(coord.X + dx, coord.Y + dy);
                        if (!Destroy(point, targetContent, originTile))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool CanBuild(Point coord, TileContentType content, Size footprint, ITile mainContentTile = null)
        {
            if (!_initialized)
            {
                Debug.LogError($"[BuildService] Trying to build before initialisation");
                return false;
            }

            var originTile = _mapDataService.GetTileAt(coord);
            if (!CanBuild(coord, content, mainContentTile))
            {
                return false;
            }
            for (var dx = 0; dx < footprint.Width; dx++)
            {
                for (var dy = 0; dy < footprint.Height; dy++)
                {
                    if (!(dx == 0 && dy == 0))
                    {
                        var point = new Point(coord.X + dx, coord.Y + dy);
                        if (!CanBuild(point, content, originTile))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool Build(Point coord, TileContentType content, Size footprint, ITile mainContentTile = null)
        {
            if (!_initialized)
            {
                Debug.LogError($"[BuildService] Trying to build before initialisation");
                return false;
            }

            var originTile = _mapDataService.GetTileAt(coord);
            if (!Build(coord, content, mainContentTile))
            {
                return false;
            }
            for (var dx = 0; dx < footprint.Width; dx++)
            {
                for (var dy = 0; dy < footprint.Height; dy++)
                {
                    if (!(dx == 0 && dy == 0))
                    {
                        var point = new Point(coord.X + dx, coord.Y + dy);
                        if (!Build(point, content, originTile))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CanDestroy(Point coord, TileContentType targetContent, ITile mainContentTile)
        {
            var tile = _mapDataService.GetTileAt(coord);

            var oldTileType = tile.Type;
            var oldContentType = tile.Content;

            var tileTypeChanged = true;
            var contentChanged = true;

            if (oldTileType == TileType.Space)
            {
                tileTypeChanged = false;
            }
            if (oldContentType == targetContent)
            {
                contentChanged = false;
            }

            return tileTypeChanged || contentChanged;
        }

        private bool Destroy(Point coord, TileContentType targetContent, ITile mainContentTile)
        {
            //TODO : return void
            var tile = _mapDataService.GetTileAt(coord);

            var oldTileType = tile.Type;
            var oldContentType = tile.Content;

            var tileTypeChanged = true;
            var contentChanged = true;

            if (oldTileType == TileType.Space)
            {
                tileTypeChanged = false;
            }
            if (oldContentType == targetContent)
            {
                contentChanged = false;
            }
            var typeChangedEvent = new TileTypeChangedEventArgs
            {
                OldTileType = oldTileType,
                NewTileType = oldContentType == TileContentType.None ? TileType.Space : TileType.Floor,
                TileCoord = coord
            };
            var contentChangedEvent = new TileContentChangedEventArgs
            {
                OldContent = oldContentType,
                NewContent = targetContent,
                TileCoord = coord
            };

            if (tileTypeChanged || contentChanged)
            {
                _buildableMapService.DestroyTile(coord, targetContent, mainContentTile);
            }

            if (tileTypeChanged)
            {
                OnTileTypeChanged(tile, typeChangedEvent);
            }
            if (contentChanged)
            {
                OnTileContentChanged(tile, contentChangedEvent);
            }
            return true;
        }
        private bool CanBuild(Point coord, TileContentType content, ITile mainContentTile)
        {
            var tile = _mapDataService.GetTileAt(coord);

            var oldTileType = tile.Type;
            var oldContentType = tile.Content;

            var tileTypeChanged = true;
            var contentChanged = true;

            if (oldTileType != TileType.Space)
            {
                tileTypeChanged = false;
            }
            if (oldContentType == content)
            {
                contentChanged = false;
            }
            return tileTypeChanged || contentChanged;
        }

        private bool Build(Point coord, TileContentType content, ITile mainContentTile)
        {
            //TODO : return void
            var tile = _mapDataService.GetTileAt(coord);

            var oldTileType = tile.Type;
            var oldContentType = tile.Content;

            var tileTypeChanged = true;
            var contentChanged = true;

            if (oldTileType != TileType.Space)
            {
                tileTypeChanged = false;
            }
            if (oldContentType == content)
            {
                contentChanged = false;
            }
            var typeChangedEvent = new TileTypeChangedEventArgs
            {
                OldTileType = oldTileType,
                NewTileType = TileType.Floor,
                TileCoord = coord
            };
            var contentChangedEvent = new TileContentChangedEventArgs
            {
                OldContent = oldContentType,
                NewContent = content,
                TileCoord = coord
            };

            if (tileTypeChanged || contentChanged)
            {
                _buildableMapService.BuildOnTile(coord, content, mainContentTile);
            }

            if (tileTypeChanged)
            {
                OnTileTypeChanged(tile, typeChangedEvent);
            }
            if (contentChanged)
            {
                OnTileContentChanged(tile, contentChangedEvent);
            }
            return true;
        }
    }
}
