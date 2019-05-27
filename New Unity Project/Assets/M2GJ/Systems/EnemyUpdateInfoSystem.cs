using MoonSlicer.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace MoonSlicer.Systems
{
    public class EnemyUpdateInfoSystem : JobComponentSystem
    {
        EntityQuery m_Group;

        protected override void OnCreate()
        {
            m_Group = GetEntityQuery(typeof(EnemyState));
        }

        public struct SetPlayerPositionJob : IJobForEach<EnemyState>
        {
            public float3 PlayerPosition;

            public void Execute(ref EnemyState enemyState)
            {
                enemyState.PlayerPosition = PlayerPosition;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            SetPlayerPositionJob job = new SetPlayerPositionJob()
            {
                PlayerPosition = PlayerSingleton.Instance.PlayerPosition.Position
            };
            return job.Schedule(m_Group, inputDeps);
        }
    }
}