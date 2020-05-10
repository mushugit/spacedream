
using Assets.Scripts.Model.Interfaces;
using Assets.Scripts.Model.Interfaces.Data;
using Assets.Scripts.Model.Interfaces.Services;
using Assets.Scripts.Model.Interfaces.Services.WorldGenerator;
using Assets.Scripts.Model.Services.DataServices;
using Assets.Scripts.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static Tile;
using Random = UnityEngine.Random;

public class WorldService : IWorldService
{
    private readonly IMapDataService _mapService;
    private IBuildableMapService _buildableMapService;
    private readonly IEnumerable<IWorldGeneratorService> _worldGeneratorServices;
    private readonly IBuildService _buildService;
    private TimeSpan _mapGenerationTime;

    private Point baseCenter;

    public Size Size
    {
        get
        {
            return _mapService.Size;
        }
    }

    public WorldService(IMapDataService mapService, IEnumerable<IWorldGeneratorService> worldGeneratorServices, IBuildService buildService)
    {
        _mapService = mapService;
        _worldGeneratorServices = worldGeneratorServices.OrderBy(generator => generator.Order);
        _buildService = buildService;
    }

    public void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler)
    {
        _buildService.TileTypeChanged += tileTypeChangedEventHandler;
    }
    public void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler)
    {
        _buildService.TileContentChanged += tileContentChangedEventHandler;
    }

    public void GenerateMap(Size size, int seed)
    {
        Random.InitState(seed);
        var mapStopwatch = new System.Diagnostics.Stopwatch();

        InitializeMap(size);

        Debug.Log($"Generating map of size {size}");
        mapStopwatch.Start();

        //Default Center
        baseCenter = new Point(0, 0);

        foreach (var generator in _worldGeneratorServices)
        {
            generator.Generate(this, true); 
            if (generator is IWorldGeneratorBaseService)
            {
                var baseGenerator = generator as IWorldGeneratorBaseService;
                baseCenter = baseGenerator.Center;
            }
        }

        mapStopwatch.Stop();
        _mapGenerationTime = mapStopwatch.Elapsed;

        Debug.Log($"Map generated in {_mapGenerationTime}");
    }

    public Point BaseCenter
    {
        get { return baseCenter; }
    }

    private void InitializeMap(Size size)
    {
        _buildableMapService = _mapService.InitializeMap(size);
        _buildService.Initialize(_buildableMapService);
    }

    public ITile GetTileAt(Point coord)
    {
        return _mapService.GetTileAt(coord);
    }

    public Point GetTileCoord(ITileView tile)
    {
        return _mapService.GetTileCoord(tile);
    }

    public List<(Point coord, ITile tile)> GetNeighbours(Point coord)
    {
        return _mapService.GetNeighbours(coord);
    }

    public bool Build(Point coord, TileContentType content, Size footprint, ITile mainContentTile = null)
    {
        return _buildService.Build(coord, content, footprint, mainContentTile);
    }
}
