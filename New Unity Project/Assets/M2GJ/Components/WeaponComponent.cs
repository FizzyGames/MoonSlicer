using System;
using Unity.Entities;

namespace MoonSlicer.Components
{
    namespace Authoring
    {
        public class WeaponComponent : ECSStoreMonoBehaviour<Components.WeaponComponent>
        {

        }
    }

    [Serializable]
    public struct WeaponComponent : IComponentData
    {
        public float DamagePerSecond;
        public bool IsActive;
    }
}