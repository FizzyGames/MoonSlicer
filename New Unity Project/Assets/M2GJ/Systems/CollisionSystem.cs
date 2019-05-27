
using MoonSlicer.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace MoonSlicer.Systems
{
    //courtesy https://forum.unity.com/threads/can-anyone-give-some-example-of-triggerevent.664732/#post-4452577
    public class CollisionSystem : JobComponentSystem
    {
        private BuildPhysicsWorld m_BuildPhysicsWorld;
        private StepPhysicsWorld m_StepPhysicsWorld;
        private EndSimulationEntityCommandBufferSystem m_EndSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorld = Unity.Entities.World.Active.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorld = Unity.Entities.World.Active.GetOrCreateSystem<StepPhysicsWorld>();
            m_EndSimulationEntityCommandBufferSystem = Unity.Entities.World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        [BurstCompile]
        private struct CollisionJob : IJob
        {
            [ReadOnly] public PhysicsWorld PhysicsWorld;
            [ReadOnly] public TriggerEvents CollisionEvents;
            [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityData;
            [NativeDisableParallelForRestriction] public BufferFromEntity<CollisionInfoElement> CollisionInfoBuffer;
            public EntityCommandBuffer EntityCommandBuffer;

            public void Execute()
            {
                foreach (TriggerEvent collisionEvent in CollisionEvents)
                {
                    Entity entityA = PhysicsWorld.Bodies[collisionEvent.BodyIndices.BodyAIndex].Entity;
                    Entity entityB = PhysicsWorld.Bodies[collisionEvent.BodyIndices.BodyBIndex].Entity;
                    if (CollisionInfoBuffer.Exists(entityA))
                        CollisionInfoBuffer[entityA].Add(new CollisionInfo() { other = entityB });
                    if (CollisionInfoBuffer.Exists(entityB))
                        CollisionInfoBuffer[entityB].Add(new CollisionInfo() { other = entityA });
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var combineDependencies = JobHandle.CombineDependencies(inputDeps, m_BuildPhysicsWorld.FinalJobHandle,
                m_StepPhysicsWorld.FinalSimulationJobHandle);

            var collisionJob = new CollisionJob
            {
                PhysicsWorld = m_BuildPhysicsWorld.PhysicsWorld,
                CollisionEvents = m_StepPhysicsWorld.Simulation.TriggerEvents,
                PhysicsVelocityData = GetComponentDataFromEntity<PhysicsVelocity>(),
                EntityCommandBuffer = m_EndSimulationEntityCommandBufferSystem.CreateCommandBuffer(),
                CollisionInfoBuffer = GetBufferFromEntity<CollisionInfoElement>()
            };

            var collisionJobHandle = collisionJob.Schedule(combineDependencies);
            return collisionJobHandle;
        }
    }
}

