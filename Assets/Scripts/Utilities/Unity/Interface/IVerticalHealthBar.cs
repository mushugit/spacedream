using UnityEngine;

namespace Assets.Scripts.Utilities.Unity.Interface
{
    public interface IVerticalHealthBar
    {
        float Value { get; set; }
        float MaxValue { get; set; }

        void SetFlashingColor(Color color);
        void SetFlashingFrequency(float frequency);
        void SetPercentThresholdFlashing(float threshold);
    }
}
