using MoonSlicer.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace MoonSlicer.Systems
{


    public class DeathSystem : ComponentSystem
    {
        KillFeedController KillFeedController;

        protected override void OnCreate()
        {
            //gross, but time constraints
            KillFeedController = UnityEngine.GameObject.FindObjectOfType<KillFeedController>();
        }

        protected override void OnUpdate()
        {
            Random r = new Random((uint)UnityEngine.Random.Range(0, int.MaxValue));
            Entities.ForEach((Entity entity, ref DieComponent dieComponent, ref PhysicsVelocity velocity, ref EnemyState state) =>
            {
                state.CurrAIState = EnemyAIState.Dead;
                if (dieComponent.ApplyVelocity)
                {
                    float2 direction2D = r.NextFloat2Direction() * 5;
                    float3 direction3D = (new float3(direction2D.x, 5, direction2D.y));
                    velocity.Linear = direction3D;
                    velocity.Angular = r.NextFloat3Direction() * 2;
                    dieComponent.ApplyVelocity = false;
                }
                dieComponent.Timer -= UnityEngine.Time.deltaTime;
                if (dieComponent.Timer < 0)
                {
                    PostUpdateCommands.DestroyEntity(entity);
                    KillFeedController.RecordSlay(state.NameID);
                }
            }
            );
        }
    }
}