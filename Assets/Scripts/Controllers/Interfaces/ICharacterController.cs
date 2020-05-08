using Assets.Scripts.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controllers.Interfaces
{
    public interface ICharacterController
    {
        Point WorldPosition { get; }
        bool Ready { get; }
        float SpeedMetersPerSecond { get; }

        void ForceMove(Point destination);
        void RegisterView(ICharacterView view);
    }
}
