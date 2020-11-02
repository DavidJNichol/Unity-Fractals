using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;

public class RotateSystem : JobComponentSystem
{
    [BurstCompile]
    private struct RotateJob : IJobForEach<RotationEulerXYZ, Rotate>
    {
        public float deltaTime;
        public void Execute(ref RotationEulerXYZ rotation, ref Rotate rotate)
        {
            rotation.Value.y += rotate.radiansPerSecond * deltaTime;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {        
        var job = new RotateJob { deltaTime = Time.deltaTime };
        return job.Schedule(this, inputDeps);
    }
}