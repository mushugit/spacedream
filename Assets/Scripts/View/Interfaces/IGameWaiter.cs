using Assets.Scripts.Controllers.Interfaces;
using System.Collections;

namespace Assets.Scripts.View.Interfaces
{
    public interface IGameWaiter
    {
        void WaitForGame(IGameController game);

        IEnumerator WaitForGamePlaying();
    }
}
