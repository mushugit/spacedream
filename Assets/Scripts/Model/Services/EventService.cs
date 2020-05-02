using Assets.Scripts.Model.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.Model.Services
{
    class EventService : IEventService
    {
        private readonly Dictionary<TileContentType, EventHandler<TileContentChangedEventArgs>> _tileSpecifiContentChanged;
        private readonly IBuildService _buildService;

        public EventService(IBuildService buildService)
        {
            _tileSpecifiContentChanged = new Dictionary<TileContentType, EventHandler<TileContentChangedEventArgs>>();
            _buildService = buildService;

            _buildService.TileContentChanged += FilterOnTileContentChangedEventHandler;
        }

        public void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler)
        {
            _buildService.TileContentChanged += tileContentChangedEventHandler;
        }

        public void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler)
        {
            _buildService.TileTypeChanged += tileTypeChangedEventHandler;
        }

        public void SubscribeAllTileSpecificContentChanged(TileContentType contentType, EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler)
        {
            if (!_tileSpecifiContentChanged.ContainsKey(contentType))
            {
                _tileSpecifiContentChanged.Add(contentType, tileContentChangedEventHandler);
            }
            else
            {
                _tileSpecifiContentChanged[contentType] += tileContentChangedEventHandler;
            }

        }

        private void FilterOnTileContentChangedEventHandler(object sender, TileContentChangedEventArgs e)
        {
            FireTileSpecificContentChanged(e.NewContent, sender, e);
            FireTileSpecificContentChanged(e.OldContent, sender, e);
        }

        private void FireTileSpecificContentChanged(TileContentType contentType, object sender, TileContentChangedEventArgs e)
        {
            if (_tileSpecifiContentChanged.ContainsKey(contentType))
            {
                _tileSpecifiContentChanged[contentType].Invoke(sender, e);
            }
        }
    }
}
