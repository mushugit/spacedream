using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tile;

namespace Assets.Scripts.View.Interfaces
{
    public interface ITileView
    {
        TileType Type { get; }
        TileContentType Content { get; }


        event EventHandler<TileTypeChangedEventArgs> TileTypeChanged;
        event EventHandler<TileContentChangedEventArgs> TileContentChanged;

    }
}
