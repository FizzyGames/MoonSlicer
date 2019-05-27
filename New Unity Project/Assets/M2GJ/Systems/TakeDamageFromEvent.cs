using Unity.Entities;
using Unity.Jobs;
using MoonSlicer.Components;
using Unity.Collections;

namespace MoonSlicer.Systems
{
    public class TakeDamageFromEvent : JobComponentSystem
    {
        EntityQuery m_Group;
        EntityCommandBufferSystem m_bufferSystem;
        protected override void OnCreate()
        {
            m_Group = EntityManager.CreateEntityQuery(typeof(HealthState), ComponentType.ReadOnly(typeof(TeamComponent)));
            m_bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        public struct TakeDamageCheckJob : IJobForEachWithEntity<HealthState, TeamComponent>
        {
            [NativeDisableParallelForRestriction]
            public BufferFromEntity<CollisionInfoElement> CollisionInfos;
            [ReadOnly]
            public ComponentDataFromEntity<WeaponComponent> WeaponComponents;
            [ReadOnly]
            public ComponentDataFromEntity<DieComponent> DieComponents;
            public EntityCommandBuffer.Concurrent CommandBuffer;
            public float DeltaTime;
            public void Execute(Entity entity, int index, ref HealthState healthState, [ReadOnly]ref TeamComponent c1)
            {
                DynamicBuffer<CollisionInfoElement> collisionInfos = CollisionInfos[entity];
                for (int i = 0; i < collisionInfos.Length; i++)
                {
                    Entity otherEntity = collisionInfos[i].Value.other;
                    if (WeaponComponents.Exists(otherEntity))
                    {
                        healthState.Health -= WeaponComponents[otherEntity].DamagePerSecond * DeltaTime;
                        if (healthState.Health < 0)
                        {
                            CommandBuffer.RemoveComponent(index, entity, typeof(HealthState));
                            CommandBuffer.AddComponent(index, entity, new DieComponent() { Timer = 3.0f, ApplyVelocity = true });
                        }
                    }
                }
                collisionInfos.Clear();
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new TakeDamageCheckJob()
            {
                CollisionInfos = GetBufferFromEntity<CollisionInfoElement>(),
                WeaponComponents = GetComponentDataFromEntity<WeaponComponent>(true),
                DieComponents = GetComponentDataFromEntity<DieComponent>(true),
                CommandBuffer = m_bufferSystem.CreateCommandBuffer().ToConcurrent(),
                DeltaTime = UnityEngine.Time.deltaTime
            };
            var handle = job.Schedule(m_Group, inputDeps);
            m_bufferSystem.AddJobHandleForProducer(handle);
            return handle;
        }
    }
}