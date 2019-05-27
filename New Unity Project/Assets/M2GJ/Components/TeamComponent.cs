using System;
using Unity.Entities;

namespace MoonSlicer.Components
{
    public enum Team
    {
        Player,
        Enemy,
        Hostile,
    }

    [Serializable]
    public struct TeamComponent : IComponentData
    {
        public Team TeamIndex;
    }
}