using Assets.Scripts.Model.Interfaces;
using Assets.Scripts.Model.Interfaces.Services;
using Assets.Scripts.Model.Interfaces.Services.WorldGenerator;
using Assets.Scripts.Model.Services.DataServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Model.Services.WorldGenerators
{
    class WorldGeneratorBaseService : IWorldGeneratorService, IWorldGeneratorBaseService
    {
        private readonly IMapDataService _mapService;
        private readonly IBuildService _buildService;

        private const float BaseArea = 100f;
        public int Order { get; } = 0;

        public WorldGeneratorBaseService(IMapDataService mapService, IBuildService buildService)
        {
            _mapService = mapService;
            _buildService = buildService;

            Center = new Point(0, 0);
        }

        public Point Center { get; private set; }

        public void Generate(IWorldService worldservice, bool debugGeneratorTime)
        {
            Stopwatch landStopwatch = null;
            
            if (debugGeneratorTime)
            {
                landStopwatch = new Stopwatch(); ;
                Debug.Log($"Generating World");
                landStopwatch.Start();
            }

            var baseStartX = Random.Range(15, _mapService.Size.Width - 16);
            var baseStartY = Random.Range(15, _mapService.Size.Height - 16);

            var lengthX = Random.Range(4, 25);
            var baseEndX = baseStartX + lengthX;

            var baseMiddleX = Mathf.RoundToInt(baseStartX + (lengthX / 2f));

            var baseEndY = Mathf.RoundToInt(BaseArea / lengthX) + baseStartY;

            var baseMiddleY = Mathf.RoundToInt(baseStartY + ((baseEndY - baseStartY) / 2f));

            Center = new Point(baseMiddleX, baseMiddleY);

            for (var x = 0; x < _mapService.Size.Width; x++)
            {
                for (var y = 0; y < _mapService.Size.Height; y++)
                {
                    var point = new Point(x, y);
                    _mapService.InitTileAt(point);

                    if(x >= baseStartX && x <= baseEndX && y >= baseStartY && y <= baseEndY)
                    {
                        var type = Tile.TileContentType.None;
                        if (x == baseStartX || x == baseEndX || y == baseStartY || y == baseEndY)
                        {
                            type = Tile.TileContentType.Wall;
                        }
                        _buildService.Build(point, type , Size.Empty);
                    }
                }
            }

            Debug.Log($"Base placed at [{baseStartX};{baseStartY}][{baseEndX};{baseEndY}]");

            if (debugGeneratorTime && landStopwatch != null)
            {
                landStopwatch.Stop();
                var _landGenerationTime = landStopwatch.Elapsed;
                Debug.Log($"Land generated in {_landGenerationTime}");
            }
        }
    }
}
