using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers.Interfaces
{
    public interface IUpdatableCharacterController : IGameCharacterController
    {
        void UpdatePosition(Vector2 position);
    }
}
