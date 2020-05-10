using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.View.Interfaces
{
    public interface IGameView
    {
        void AddGameobjectReference(Point coord, GameObject gameobject);
        List<(Point coord, ITileView tile)> GetNeighbours(Point coord);
        ITileView GetTileAt(Point coord);
        void OnTileContentChanged(object sender, Tile.TileContentChangedEventArgs e);
        void RegisterWallView(IWallView wallView);
        void RemoveGameObjectAt(Point point);
        void SubscribeSpecificContentTypeChanged(Tile.TileContentType contentType, EventHandler<Tile.TileContentChangedEventArgs> tileContentChangedEventHandler);
    }
}
