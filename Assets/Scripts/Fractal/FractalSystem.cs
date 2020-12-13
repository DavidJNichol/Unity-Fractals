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
    struct FractalJobSystem : IJobForEach<Rotation, FractalComponent, LocalToWorld> // Gets each entity that has these 4 components attached
    {
        public float deltaTime;
           
        public void Execute(ref Rotation rotation, ref FractalComponent fractalComponent, ref LocalToWorld localToWorld)
        {
            if (fractalComponent.rotation)
            {
                rotation.Value = math.mul(math.normalize(rotation.Value),
                quaternion.AxisAngle(Vector3.right, fractalComponent.radiansPerSecond * deltaTime)); // Rotation quaternion math (rip Euler)            
            }
            else
            {
                rotation.Value = quaternion.identity;
            }            

            //if(fractalComponent.weirdFractal)
            //{
            //    //translation.Value = LocalToWorld


            //    //if (fractalComponent.indexer2 == 0)
            //    //{
            //    //    if (fractalComponent.indexer % 2 == 0)
            //    //        translation.Value += fractalComponent.translation * Vector3.up;
            //    //    else if (fractalComponent.indexer % 3 == 0)
            //    //        translation.Value += fractalComponent.translation * Vector3.right;
            //    //    else if (fractalComponent.indexer % 4 == 0)
            //    //        translation.Value += fractalComponent.translation * Vector3.down;
            //    //    else if (fractalComponent.indexer % 5 == 0)
            //    //        translation.Value += fractalComponent.translation * Vector3.left;
            //    //    else
            //    //        translation.Value += fractalComponent.translation * Vector3.forward;
            //    //}

            //    //fractalComponent.indexer++;
            //    //fractalComponent.indexer2++;
            //}

            //localToWorld.Value = localToParent.Value; // Sets localtoworld coords to localtoparent coords (localtoparent coords do not do anything for now, might be a conflict between LTW and LTP)
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


