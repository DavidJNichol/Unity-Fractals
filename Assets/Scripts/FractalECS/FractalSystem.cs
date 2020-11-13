using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class FractalSystem : ComponentSystem
{
    // Kinda slow, but works!
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, ref FractalComponent fractalComponent) =>
        {
            var deltaTime = Time.deltaTime;
            rotation.Value = math.mul(math.normalize(rotation.Value),
                quaternion.AxisAngle(Vector3.right, fractalComponent.radiansPerSecond * deltaTime));

           
        });
    }
}

//public class FractalSystemJobs : JobComponentSystem
//{
//    //public FractalComponent fractalComponent = new FractalComponent();
//    //[BurstCompile]
//    //struct FractalJobSystem : IJob
//    //{
//    //    public void Execute()
//    //    {
            
//    //    }
//    //}

//    //protected override JobHandle OnUpdate(JobHandle inputDeps)
//    //{
//    //    var job = new FractalJobSystem()
//    //    {
//    //    };

//    //    return job.Schedule();
//    //}
//}


