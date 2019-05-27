using System;
using Unity.Entities;
using UnityEngine;

namespace MoonSlicer.Components
{
    [Serializable]
    public struct MovementStateInfo : ISharedComponentData
    {
        public float Speed;
        public bool IsFlying;
        public bool CanBeKnockedBack;
    }
    namespace Authoring
    {
        [RequiresEntityConversion]
        public class MovementStateInfo : ECSStoreMonoBehaviourShared<Components.MovementStateInfo>
        {
        }
    }
}