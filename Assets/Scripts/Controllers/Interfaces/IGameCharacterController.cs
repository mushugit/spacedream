using Assets.Scripts.View.Interfaces;
using System.Drawing;

namespace Assets.Scripts.Controllers.Interfaces
{
    public interface IGameCharacterController
    {
        Point WorldPosition { get; }
        bool Ready { get; }
        float SpeedMetersPerSecond { get; }
        int EnergyLevel { get; }
        int Condition { get; }

        void ForceMove(Point destination);
        void RegisterView(ICharacterView view);
    }
}
