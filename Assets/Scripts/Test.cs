using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

public class Test : MonoBehaviour
{
    [SerializeField] private Mesh cubeMesh;
    [SerializeField] private Material material;
    [SerializeField] private int amountOfEntities;

    //JOB    
    [BurstCompile]
    public struct JobStruct : IJobParallelFor
    {
        //public Mesh theMesh;
        //public Material theMaterial;

        public int amountOfEntities;


        public void Execute(int index)
        {
            EntityManager entityManager = World.Active.EntityManager;

            EntityArchetype entityArchetype = entityManager.CreateArchetype(
                typeof(EntityComponent),
                typeof(Translation), typeof(RenderMesh),  // Rendering
                typeof(LocalToWorld) // Coordinate conversion
                );

            NativeArray<Entity> entityArray = new NativeArray<Entity>(amountOfEntities, Allocator.Temp);
            entityManager.CreateEntity(entityArchetype, entityArray);

                Entity entity = entityArray[index];
                entityManager.SetComponentData(entity, new EntityComponent { componentFloat = UnityEngine.Random.Range(10f, 20f) });
                entityManager.SetComponentData(entity, new Translation { Value = new float3(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-100f, 100f), (UnityEngine.Random.Range(-500f, 500f))) });

                //entityManager.SetSharedComponentData(entity, new RenderMesh
                //{
                //    mesh = theMesh,
                //    material = theMaterial
                //}
                //); ;
            entityArray.Dispose();
        }
    }

    private void Start()
    {
        JobStruct jobStruct = new JobStruct
        {
            amountOfEntities = this.amountOfEntities,
            //theMesh = cubeMesh,
            //theMaterial = material
        };

        JobHandle jobHandle = jobStruct.Schedule(amountOfEntities, 100);
        jobHandle.Complete();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}