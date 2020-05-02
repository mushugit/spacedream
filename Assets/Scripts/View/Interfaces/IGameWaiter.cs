using Assets.Scripts.Controllers.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.View.Interfaces
{
    public interface IGameWaiter
    {
        void WaitForGame(IGameController game);

        IEnumerator WaitForGamePlaying();
    }
}
