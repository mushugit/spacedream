using System.Drawing;

namespace Assets.Scripts.View.Interfaces
{
    public interface IWallView
    {
        void RenderTile(Point coord, bool cascadeUpdateRender = false);
    }
}
