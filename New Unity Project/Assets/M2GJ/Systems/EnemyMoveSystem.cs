using MoonSlicer.Components;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace MoonSlicer.Systems
{

    [UpdateAfter(typeof(RotateTowardsTargetSystem))]
    public class EnemyMoveSystem : JobComponentSystem
    {
        EntityQuery m_Group;
        List<MovementStateInfo> m_movementStateInfos = new List<MovementStateInfo>();

        protected override void OnCreate()
        {
            // Cached access to a set of ComponentData based on a specific query
            m_Group = GetEntityQuery(typeof(Translation), ComponentType.ReadOnly<MovementStateInfo>(), typeof(EnemyState));
            //m_Group = GetEntityQuery(new EntityQueryDesc()
            //{
            //All = new ComponentType[] { ComponentType.ReadWrite<Translation>(), ComponentType.ReadOnly<MovementStateInfo>(), ComponentType.ReadWrite<EnemyState>() }
            //});

        }

        [BurstCompile]
        struct MoveEnemyJob : IJobChunk
        {
            public float DeltaTime;
            public ArchetypeChunkComponentType<PhysicsVelocity> VelocityType;
            [ReadOnly]
            public ArchetypeChunkComponentType<Translation> TranslationType;
            public ArchetypeChunkComponentType<Rotation> RotationType;
            public ArchetypeChunkComponentType<MovementState> MovementType;
            public ArchetypeChunkComponentType<EnemyState> EnemyStateType;
            [DeallocateOnJobCompletion]
            [ReadOnly]
            public NativeArray<MovementStateInfo> MovementInfo;
            [ReadOnly]
            public ArchetypeChunkSharedComponentType<MovementStateInfo> MovementStateInfoType;

            public Unity.Mathematics.Random Random;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var chunkVelocities = chunk.GetNativeArray(VelocityType);
                var chunkTranslations = chunk.GetNativeArray(TranslationType);
                var chunkRotations = chunk.GetNativeArray(RotationType);
                var chunkMovementData = chunk.GetNativeArray(MovementType);
                var chunkEnemyData = chunk.GetNativeArray(EnemyStateType);
                int index = chunk.GetSharedComponentIndex(MovementStateInfoType);
                MovementStateInfo movementInfo = MovementInfo[index/2];
                quaternion rot90 = quaternion.Euler(0, math.PI / 2, 0);
                for (int i = 0; i < chunk.Count; i++)
                {
                    var enemyState = chunkEnemyData[i];
                    if (enemyState.CurrAIState != EnemyAIState.Dead)
                    {
                        var movementState = chunkMovementData[i];
                        var translation = chunkTranslations[i];
                        var velocity = chunkVelocities[i];
                        var rotation = chunkRotations[i];
                        float3 toTarget = movementState.Target - translation.Value;
                        float3 toPlayer = enemyState.PlayerPosition - translation.Value;
                        toPlayer.y = 0;
                        toTarget.y = 0;
                        float3 fromTargetToPlayer = movementState.Target - enemyState.PlayerPosition;
                        fromTargetToPlayer.y = 0;
                        if (enemyState.LookAtPlayer)
                        {
                            //rotation = new Rotation()
                            //{
                            //    Value = quaternion.identity
                            //};
                            rotation = new Rotation()
                            {
                                Value = quaternion.LookRotationSafe(toPlayer, new float3(0, 1, 0))
                            };
                        }

                        if (enemyState.CurrAIState != EnemyAIState.Idle)
                        {

                            float maxDist = enemyState.DesiredDistance + enemyState.DesiredDistanceLeeway;
                            float minDist = enemyState.DesiredDistance - enemyState.DesiredDistanceLeeway;
                            float currDistFromTargetToPlayer = math.lengthsq(fromTargetToPlayer);
                            if (currDistFromTargetToPlayer > maxDist * maxDist || currDistFromTargetToPlayer < minDist * minDist)
                            {
                                SetNewTarget(ref movementState, ref enemyState, ref translation, out toTarget, out fromTargetToPlayer, ref Random);
                            }
                            float3 fTTPNorm = math.normalizesafe(fromTargetToPlayer);
                            float3 tPNorm = math.normalizesafe(toPlayer);
                            float3 tTNorm = math.normalizesafe(toTarget);
                            //chunkTranslations[i] = new Translation()
                            //{
                            //    Value = translation.Value + tTNorm * movementInfo.Speed * DeltaTime
                            //};

                            if (math.dot(fTTPNorm, tPNorm) > 0)
                                velocity.Linear = tTNorm * movementInfo.Speed;// * math.min(1, math.length(toTarget));
                            else
                            {
                                bool tooFar = math.lengthsq(toPlayer) > maxDist * maxDist;
                                bool tooClose = math.lengthsq(toPlayer) < minDist * minDist;
                                if (tooFar || tooClose)
                                {
                                    velocity.Linear = math.normalizesafe(toPlayer * math.select(1, -1, tooClose)) * movementInfo.Speed;
                                }
                                else
                                {
                                    velocity.Linear = math.normalizesafe(math.mul(rot90, tPNorm *
                                        math.select(-1, 1,
                                        math.dot(math.mul(rot90, -tPNorm), fTTPNorm) < math.dot(math.mul(rot90, tPNorm), fTTPNorm))))
                                        * movementInfo.Speed;
                                }
                            }
                        }
                        velocity.Linear.y = -1;
                        chunkMovementData[i] = movementState;
                        chunkEnemyData[i] = enemyState;
                        chunkVelocities[i] = velocity;
                        chunkRotations[i] = rotation;
                    }
                    //chunkVelocities[i].Linear = 
                    //    chunkTranslations[i] = new Translation()
                    //    {
                    //        Value = chunkTranslations[i].Value + math.mul(chunkRotations[i].Value, new float3(0, 0, DeltaTime * MovementInfo[chunkIndex + 1].Speed))
                    //    };
                }
            }

            private static void SetNewTarget(ref MovementState movementState, ref EnemyState enemyState, ref Translation translation, out float3 toTarget, out float3 fromTargetToPlayer, ref Random Random)
            {
                float2 newDirection = Random.NextFloat2Direction();
                float3 newDirection3D = new float3(newDirection.x, 0, newDirection.y);
                enemyState.CurrAIState = EnemyAIState.Follow;
                movementState.Target = enemyState.PlayerPosition + newDirection3D * enemyState.DesiredDistance;
                UpdateTarget(movementState, enemyState, translation, out toTarget, out fromTargetToPlayer);
            }

            private static void ReturnToSpawn(ref MovementState movementState, ref EnemyState enemyState, ref Translation translation, out float3 toTarget, out float3 fromTargetToPlayer)
            {
                enemyState.CurrAIState = EnemyAIState.ReturnToSpawn;
                movementState.Target = enemyState.SpawnPosition;
                UpdateTarget(movementState, enemyState, translation, out toTarget, out fromTargetToPlayer);
                if (math.lengthsq(toTarget) < 0.25f)
                {
                    enemyState.CurrAIState = EnemyAIState.Idle;
                }
            }

            private static void UpdateTarget(MovementState movementState, EnemyState enemyState, Translation translation, out float3 toTarget, out float3 fromTargetToPlayer)
            {
                toTarget = movementState.Target - translation.Value;
                fromTargetToPlayer = movementState.Target - enemyState.PlayerPosition;
                toTarget.y = 0;
                fromTargetToPlayer.y = 0;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var velocityType = GetArchetypeChunkComponentType<PhysicsVelocity>();
            var rotationType = GetArchetypeChunkComponentType<Rotation>();
            var movementType = GetArchetypeChunkComponentType<MovementState>();
            var enemyStateType = GetArchetypeChunkComponentType<EnemyState>();
            var translationType = GetArchetypeChunkComponentType<Translation>(true);
            var movementInfoType = GetArchetypeChunkSharedComponentType<MovementStateInfo>();
            List<int> indices = new List<int>();
            EntityManager.GetAllUniqueSharedComponentData(m_movementStateInfos, indices);


            var job = new MoveEnemyJob()
            {
                DeltaTime = Time.deltaTime,
                VelocityType = velocityType,
                RotationType = rotationType,
                MovementType = movementType,
                EnemyStateType = enemyStateType,
                TranslationType = translationType,
                MovementStateInfoType = movementInfoType,
                MovementInfo = new NativeArray<MovementStateInfo>(m_movementStateInfos.ToArray(), Allocator.TempJob),
                Random = new Random((uint)UnityEngine.Random.Range(0, int.MaxValue))
            };

            m_movementStateInfos.Clear();
            return job.Schedule(m_Group, inputDeps);
        }
    }
}