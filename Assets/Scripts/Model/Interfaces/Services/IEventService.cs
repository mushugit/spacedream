using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.Model.Interfaces.Services
{
    public interface IEventService
    {
        void SubscribeAllTileTypeChanged(EventHandler<TileTypeChangedEventArgs> tileTypeChangedEventHandler);
        void SubscribeAllTileContentChanged(EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler);
        void SubscribeAllTileSpecificContentChanged(TileContentType contentType, EventHandler<TileContentChangedEventArgs> tileContentChangedEventHandler);
    }
}
