using System;
using Unity.Entities;
using Unity.Mathematics;

namespace MoonSlicer.Components
{
    namespace Authoring
    {
        public class EnemyState : ECSStoreMonoBehaviour<Components.EnemyState>
        {
            public int MaxHealth;
            public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddBuffer<CollisionInfoElement>(entity);
                dstManager.AddComponentData(entity, new TeamComponent() { TeamIndex = Team.Enemy });
                dstManager.AddComponentData(entity, new HealthState() { MaxHealth = MaxHealth, Health = MaxHealth });
                base.Convert(entity, dstManager, conversionSystem);
            }
        }
    }
    [Serializable]
    public struct EnemyState : IComponentData
    {
        public float DesiredDistance;
        public float3 PlayerPosition;
        public float LeashDistance;
        public float3 SpawnPosition;
        public EnemyAIState CurrAIState;
        public float DesiredDistanceLeeway;
        public bool LookAtPlayer;
        public int NameID;
    }

    public enum EnemyAIState
    {
        Idle,
        Follow,
        Attack,
        ReturnToSpawn,
        Dead,
        //???
    }
}