using static Tile;

namespace Assets.Scripts.Model.Interfaces.Data
{
    public interface IBuildableTile : ITile
    {
        void Build(TileContentType content, ITile mainContentTile);

        void DestroyAll();
        void DestroyContent();
    }
}
