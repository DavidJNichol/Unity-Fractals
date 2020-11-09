using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FractalSystem : JobComponentSystem
{
    FractalComponent fractalComponent;

    [BurstCompile]
    struct FractalJobSystem : IJobForEach<Rotation>
    {
        public quaternion rotationQuaternion;

        public void Execute(ref Rotation rotation)
        {
            rotation.Value = rotationQuaternion;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new FractalJobSystem()
        {
            rotationQuaternion  = new quaternion(0, 50 * Time.deltaTime, 0, .5f)
        };

        

        return job.Schedule(this, inputDeps);
    }
}
