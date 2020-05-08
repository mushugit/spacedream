using System;
using UnityEngine;

namespace Assets.Scripts.View.Interfaces
{
    public interface ICharacterView
    {
        void Move(Vector2 destination, Action callback = null);
    }
}
