using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MoonSlicer.Components
{
    namespace Authoring
    {
        [RequiresEntityConversion]
        public class MovementState : ECSStoreMonoBehaviour<Components.MovementState>
        {
        }
    }
    [Serializable]
    public struct MovementState : IComponentData
    {
        public float StunnedTimer;
        public float3 Target;
        public float3 Velocity;
        public float3 Forward;

    }
}