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
    [SerializeField] private float childScale;
    [SerializeField] private int maxTwist;

    private float3 entityPosition;
    private float amountOfEntities;
    private NativeArray<Entity> entityArray;
    private EntityArchetype entityArchetype = new EntityArchetype(); 
    private EntityManager entityManager;

    private float spawnProbability;
    private float maxRotationSpeed;
    

    private System.Random rand = new System.Random();

    private quaternion defaultQuaternion = new quaternion(1, 1, 1, 1);
    
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

        // TRANSLATION, ROTATION AND SCALE CAN POSSIBLY BE DONE WITH JUST LOCALTOWORLD
        entityArchetype = entityManager.CreateArchetype(
          typeof(FractalComponent), typeof(Rotation), //Component class, Rotation
          typeof(Translation), typeof(RenderMesh),  // Rendering, Location
          typeof(LocalToWorld), typeof(Scale)  // Coordinate conversion
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

            entityManager.SetComponentData(entity, new FractalComponent
            {
                radiansPerSecond = 5
            });

            entityManager.SetComponentData(entity, new Rotation
            {
                //Value = defaultQuaternion

                Value = math.mul(math.normalize(defaultQuaternion), quaternion.AxisAngle(-maxTwist, (float)rand.Next(-maxTwist,maxTwist))) // Sets a random twist at start
            });

            OffsetEntitySpawn(2); // Adds a gap of 2 units between each entity on the z axis 

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = cubeMesh[UnityEngine.Random.Range(0, cubeMesh.Length)], // Sets mesh to random out of the UI list
                material = material[UnityEngine.Random.Range(0, material.Length)], // Sets material to random out of the UI list
            }
            );

            entityManager.SetComponentData(entity, new Scale
            {
                Value = childScale
            });
        }      
        entityArray.Dispose(); // Empty native array since garbage collector does not handle it (IMPORTANT)
    }

    private void OffsetEntitySpawn(int amount)
    {
        entityPosition.z += amount;
    }

}





