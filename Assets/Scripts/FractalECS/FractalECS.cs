using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine.Animations;

public class FractalECS : MonoBehaviour
{
    [SerializeField] private Mesh[] cubeMesh;
    [SerializeField] private Material[] material;
    [SerializeField] private int maxDepth;

    private float3 entityPosition;
    private float amountOfEntities;
    private NativeArray<Entity> entityArray;
    private EntityArchetype entityArchetype = new EntityArchetype(); 
    private EntityManager entityManager;
    
    private static Vector3[] childDirections = {  // TO USE IN THE FUTURE
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] childOrientations = {  // TO USE IN THE FUTURE
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
    };

    private void Start()
    {
        amountOfEntities = math.pow(5, maxDepth); // how many fractal children are spawned is 5^(maxDepth)

        entityArray= new NativeArray<Entity>((int)amountOfEntities, Allocator.Temp);

        entityManager = World.Active.EntityManager;

        entityArchetype = entityManager.CreateArchetype(
          typeof(FractalComponent), typeof(Rotation), //Component class, Rotation
          typeof(Translation), typeof(RenderMesh),  // Rendering, Location
          typeof(LocalToWorld) // Coordinate conversion
          );        

        entityPosition = this.transform.localPosition;

        CreateEntities(entityArchetype, 0); // Main entity creation
    }

    private void CreateEntities(EntityArchetype entityArchetype, int childIndex)
    {        
        entityManager.CreateEntity(entityArchetype, entityArray); // Entity spawn

        for (int i = 0; i < amountOfEntities; i++)
        {
            Entity entity = entityArray[i]; // Entity is created and added to array 

            entityManager.SetComponentData(entity, new Translation
            {              
                 Value = entityPosition // EntityPosition is this.transform.localposition
            }) ;

            OffsetEntitySpawn(2); // Adds a gap of 2 units between each entity on the z axis 

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = cubeMesh[UnityEngine.Random.Range(0, cubeMesh.Length)], // Sets mesh to random out of the UI list
                material = material[UnityEngine.Random.Range(0, material.Length)], // Sets material to random out of the UI list
            }
            );
        }      
        entityArray.Dispose(); // Empty native array since garbage collector does not handle it (IMPORTANT)
    }

    private void OffsetEntitySpawn(int amount)
    {
        entityPosition.z += amount;
    }

}





