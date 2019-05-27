using System;
using Unity.Entities;

namespace MoonSlicer.Components
{
    [Serializable]
    public struct DieComponent : IComponentData
    {
        public float Timer;
        public bool ApplyVelocity;
    }
}