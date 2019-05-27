using Unity.Entities;

namespace MoonSlicer
{
    public enum LivingEntityState
    {
        Normal = 0,
        InAir = 1 << 0,
        Prone = 1 << 1,
        Stunned = 1 << 2,
        OnFire = 1 << 3,

    }

    public struct HealthState : IComponentData
    {
        public float Health;
        public LivingEntityState State;
        public float MaxHealth;
    }
}