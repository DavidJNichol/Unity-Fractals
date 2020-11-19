using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//public class FractalSystem : ComponentSystem
//{
//    // Kinda slow, but works!
//    protected override void OnUpdate()
//    {
//        float deltaTime = Time.deltaTime;
//        Entities.ForEach((ref Rotation rotation, ref FractalComponent fractalComponent, ref LocalToWorld location, ref LocalToParent local) =>
//        {
//            rotation.Value = math.mul(math.normalize(rotation.Value),
//                quaternion.AxisAngle(Vector3.right, fractalComponent.radiansPerSecond * deltaTime));

//            location.Value = local.Value;
//        });
//    }
//}

public class FractalSystem : JobComponentSystem
{
    [BurstCompile]
    struct FractalJobSystem : IJobForEach<Rotation, FractalComponent, LocalToWorld, LocalToParent, Translation> // Gets each entity that has these 4 components attached
    {
        public float deltaTime;
           
        public void Execute(ref Rotation rotation, ref FractalComponent fractalComponent, ref LocalToWorld location, ref LocalToParent localToParent, ref Translation translation)
        {
            rotation.Value = math.mul(math.normalize(rotation.Value),
            quaternion.AxisAngle(Vector3.right, fractalComponent.radiansPerSecond * deltaTime)); // Rotation quaternion math (rip Euler)            

            if(fractalComponent.indexer < 1)
                translation.Value += fractalComponent.translation;

            fractalComponent.indexer++;

            location.Value = localToParent.Value; // Sets localtoworld coords to localtoparent coords (localtoparent coords do not do anything for now, might be a conflict between LTW and LTP)
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new FractalJobSystem()
        {
            deltaTime = Time.deltaTime, // Acess Time class outside of the Job
        };

        return job.Schedule(this, inputDeps); // Schedules job
    }
}


