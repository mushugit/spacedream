using Assets.Scripts.Model.Interfaces.Data;
using System.Drawing;
using static Tile;

namespace Assets.Scripts.Model.Services.DataServices
{
    public interface IBuildableMapService : IMapDataService
    {
        void BuildOnTile(Point coord, TileContentType content, ITile mainContentTile);
        void DestroyTile(Point coord, TileContentType targetContent, ITile mainContentTile);
    }
}