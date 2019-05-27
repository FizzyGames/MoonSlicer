using Unity.Entities;
using Unity.Jobs;

namespace MoonSlicer.Systems
{
    public class RotateTowardsTargetSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return default(JobHandle);
        }
    }
}