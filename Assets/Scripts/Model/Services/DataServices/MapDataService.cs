using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.Model.Interfaces.Services;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Tile;

namespace Assets.Scripts.Model.Services.DataServices
{
    class MapDataService : IBuildableMapService
    {
        public double MapArea => Size.Width * Size.Height;
        public Size Size { get; private set; }
        public bool MapInitialized { get; private set; } = false;
        public float[,] NoiseMap { get; private set; }

        private IBuildableTile[,] _map;

        private readonly ITileService _tileService;
        private readonly Dictionary<TileContentType, List<Point>> _contentList;


        public MapDataService(ITileService tileService)
        {
            _tileService = tileService;
            _contentList = new Dictionary<TileContentType, List<Point>>();
        }

        public ITile GetTileAt(Point coord)
        {
            return MapInitialized ? _map[coord.X, coord.Y] : null;
        }

        public List<(Point Coord, ITile Tile)> GetNeighbours(Point coord)
        {
            var neighbours = new List<(Point Coord, ITile Tile)>();
            for (var dx = -1; dx <= 1; dx++)
            {
                var newX = coord.X + dx;
                if (newX > 0 && newX < Size.Width && dx != 0)
                {
                    var neighbourPoint = new Point(newX, coord.Y);
                    neighbours.Add((neighbourPoint, _map[neighbourPoint.X, neighbourPoint.Y]));
                }
            }
            for (var dy = -1; dy <= 1; dy++)
            {
                var newY = coord.Y + dy;
                if (newY > 0 && newY < Size.Height && dy != 0)
                {
                    var neighbourPoint = new Point(coord.X, newY);
                    neighbours.Add((neighbourPoint, _map[neighbourPoint.X, neighbourPoint.Y]));
                }
            }
            return neighbours;
        }

        public int GetNearestContent(Point coord, TileContentType content)
        {
            var distance = int.MaxValue;

            if (_contentList.ContainsKey(content))
            {
                foreach (var contentPoint in _contentList[content])
                {
                    var currentDistance = coord.ManhattanDistance(contentPoint);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                    }
                }
            }

            return distance;
        }

        public List<(Point Coord, ITile Tile)> GetAllCells()
        {
            if (!MapInitialized)
            {
                return null;
            }

            var cellList = new List<(Point Coord, ITile Tile)>();
            for (var x = 0; x < Size.Width; x++)
            {
                for (var y = 0; y < Size.Height; y++)
                {
                    var point = new Point(x, y);
                    cellList.Add((point, _map[x, y]));
                }
            }
            return cellList;
        }

        public void InitTileAt(Point coord)
        {
            //TODO : Init all map at once
            if (IsInMap(coord))
            {
                _map[coord.X, coord.Y] = _tileService.GetFromPrototype(_tileService.GetDefaultTileType(), null);
            }
        }

        public Point GetTileCoord(ITileView tile)
        {
            if (!MapInitialized)
            {
                return Point.Empty;
            }
            for (var x = 0; x < Size.Width; x++)
            {
                for (var y = 0; y < Size.Height; y++)
                {
                    if (_map[x, y] == tile)
                    {
                        return new Point(x, y);
                    }
                }
            }
            return Point.Empty;
        }

        public IBuildableMapService InitializeMap(Size size)
        {
            if (MapInitialized)
            {
                return null;
            }

            Size = size;
            _map = new Tile[size.Width, size.Height];
            MapInitialized = true;

            return this;
        }

        public List<(Point Coord, ITile Tile)> GetCellsFromFootprint((Point Coord, ITile Tile) startingCell, Size footprint)
        {
            var returnList = new List<(Point Coord, ITile Tile)>();

            for (var dx = 0; dx < footprint.Width; dx++)
            {
                for (var dy = 0; dy < footprint.Height; dy++)
                {
                    var newPoint = new Point(startingCell.Coord.X + dx, startingCell.Coord.Y + dy);
                    if (IsInMap(newPoint))
                    {
                        var newTile = _map[newPoint.X, newPoint.Y];
                        returnList.Add((newPoint, newTile));
                    }
                }
            }

            return returnList;
        }

        private bool IsInMap(Point coord)
        {
            return coord.X >= 0 && coord.X < Size.Width && coord.Y >= 0 && coord.Y < Size.Height;
        }

        private void AddPointToContent(TileContentType content, Point coord)
        {
            if (!_contentList.ContainsKey(content))
            {
                _contentList.Add(content, new List<Point>() { coord });
            }
            else
            {
                _contentList[content].Add(coord);
            }
        }

        private void RemovePointToContent(TileContentType content, Point coord)
        {
            if (!_contentList.ContainsKey(content))
            {
                Debug.LogError($"[MapService] Trying to remove a unknown reference of content type {content} ({coord})");
            }
            else
            {
                _contentList[content].Remove(coord);
            }
        }

        public void BuildOnTile(Point coord, TileContentType newContent, ITile mainContentTile)
        {
            if (IsInMap(coord))
            {
                var tile = _map[coord.X, coord.Y];
                var oldContent = tile.Content;

                if (oldContent != TileContentType.None && oldContent == TileContentType.None)
                {
                    RemovePointToContent(oldContent, coord);
                }

                if (newContent != TileContentType.None && newContent == TileContentType.None)
                {
                    AddPointToContent(newContent, coord);
                }

                tile.Build(newContent, mainContentTile);
            }
        }
    }
}
