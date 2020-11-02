using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class EntitySystem : JobComponentSystem
{
    [BurstCompile]
    struct EntityJob : IJobForEach<EntityComponent>
    {
        public float deltaTime; // not needed yet
         
        public void Execute(ref EntityComponent entityComponent)
        {            
            entityComponent.componentFloat = math.exp10(math.sqrt(entityComponent.componentFloat));
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new EntityJob()
        {
            deltaTime = Time.deltaTime            
        };
       
        return job.Schedule(this, inputDeps);
    }
}