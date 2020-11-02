using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

public class Entities : MonoBehaviour
{
    //[SerializeField] private Mesh cubeMesh;
    [SerializeField] private Mesh[] cubeMesh;
    [SerializeField] private Material[] material;
    [SerializeField] private int amountOfEntities;


    private void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(EntityComponent),
            typeof(Translation), typeof(RenderMesh),  // Rendering
            typeof(LocalToWorld) // Coordinate conversion
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(amountOfEntities, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);
      
        for (int i = 0; i < amountOfEntities; i++)
        {


            Entity entity = entityArray[i];
            entityManager.SetComponentData(entity, new EntityComponent { componentFloat = UnityEngine.Random.Range(10f, 20f) });
            entityManager.SetComponentData(entity, new Translation { Value = new float3(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-100f, 100f), (UnityEngine.Random.Range(-500f, 500f))) });     

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = cubeMesh[UnityEngine.Random.Range(0, cubeMesh.Length)],
                material = material[UnityEngine.Random.Range(0, material.Length)],
            }
            ); ;
        }
        entityArray.Dispose();
    }

    
}
