using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;

public class FractalECS : MonoBehaviour
{
    [SerializeField] private Mesh[] cubeMesh;
    [SerializeField] private Material material;
    [SerializeField] private int maxDepth;
    [SerializeField] private float childScale;
    [SerializeField] private int maxTwist;

    private Material[,] materials;

    private float3 entityPosition;
    private int amountOfEntities;
    private NativeArray<Entity> entityArray;
    private EntityArchetype entityArchetype;
    private EntityManager entityManager;

    private int depth;
    Entity parent;

    float4x4 loc;

    private System.Random rand = new System.Random();

    private float startTime;

    private quaternion defaultQuaternion = new quaternion(1, 1, 1, 1);
    
    //private static Vector3[] childDirections = {  // TO USE IN THE FUTURE
    //    Vector3.up,
    //    Vector3.right,
    //    Vector3.left,
    //    Vector3.forward,
    //    Vector3.back
    //};

    //private static Quaternion[] childOrientations = {  // TO USE IN THE FUTURE
    //    Quaternion.identity,
    //    Quaternion.Euler(0f, 0f, -90f),
    //    Quaternion.Euler(0f, 0f, 90f),
    //    Quaternion.Euler(90f, 0f, 0f),
    //    Quaternion.Euler(-90f, 0f, 0f)
    //};

    //MOVE SOME OF THIS INTO AWAKE INSTEAD FOR A BIT BETTER OPTIMIZATION
    private void Awake()
    {
        amountOfEntities = (int)math.pow(5, maxDepth); // how many fractal children are spawned is 5^(maxDepth)

        entityArray= new NativeArray<Entity>((int)amountOfEntities, Allocator.Temp); 

        entityManager = World.Active.EntityManager;

        // TRANSLATION, ROTATION AND SCALE CAN POSSIBLY BE DONE WITH JUST LOCALTOWORLD
          entityArchetype = entityManager.CreateArchetype(
          typeof(FractalComponent), typeof(Rotation), //Component class, Rotation
          typeof(RenderMesh), typeof(Translation), typeof(LocalToParent), // Rendering, Location, Coords local to parent
          typeof(LocalToWorld), typeof(Scale), typeof(Parent)  // Coordinate conversion, Scale, Parent
          );

        startTime = Time.realtimeSinceStartup;

        entityPosition = this.transform.localPosition;

        if (materials == null)
            SetMaterialColors(); // Inits material array along with the color instructions that will be set to the fractal materials

        loc = new float4x4(float4.zero, float4.zero, float4.zero, new float4(entityPosition, 1)); // this.transform.position used for localtoworld, rotation values are set to zero for now        
               
        CreateEntities(entityArchetype); // Call entity spawn method
    }

    private void Start()
    {
        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "ms"); // Start up time debug        
    }

    private void CreateEntities(EntityArchetype entityArchetype)
    {
        entityManager.CreateEntity(entityArchetype, entityArray); // Entity spawn   

        for (int i = 0; i < amountOfEntities; i++)
        {            
            Entity entity = entityArray[i]; // Entity is created and added to array 

            if(i > 0)
            {
                parent = entityArray[i - 1];
            }
            else
            {
                parent = entity;
            }

            InterpolateColor(i);

            entityManager.SetComponentData(entity, new Translation
            {
                Value = entityPosition // EntityPosition is this.transform.localposition
            });

            entityManager.SetComponentData(entity, new FractalComponent
            {
                radiansPerSecond = 5, // Fractals rotate at a rate of 5 radians per second
            });

            // WE PROBABLY DON'T NEED THIS v
            entityManager.SetComponentData(entity, new LocalToWorld
            {
                Value = loc
            });

            entityManager.SetComponentData(entity, new Rotation
            {
                Value = math.mul(math.normalize(defaultQuaternion), quaternion.AxisAngle(Vector3.up, rand.Next(-maxTwist, maxTwist))) // Sets a random twist at start
            });

            entityManager.SetComponentData(entity, new Scale
            {
                Value = childScale 
            });

            entityManager.SetComponentData(entity, new Parent
            {
                Value = parent // Parent == entityArray[i-1]
            });

            //entityManager.SetComponentData(entity, new LocalToParent
            //{
            //    Value = loc
            //});

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = cubeMesh[UnityEngine.Random.Range(0, cubeMesh.Length)], // Sets mesh to random out of the serialized list
                material = materials[depth, UnityEngine.Random.Range(0, 2)] // Sets material to random out of the serialized list
            }
            );

            OffsetEntitySpawn(3); // Adds a gap of x units between each entity on the z axis                    
        }      
        entityArray.Dispose(); // Empty native array since garbage collector does not handle it (IMPORTANT)
    }

    private void OffsetEntitySpawn(int amount)
    {
        entityPosition.z += amount;
    }

    private void SetMaterialColors()
    {
        materials = new Material[maxDepth + 1, 2]; // Materials only contains one material duplicate per depth. 
        for (int i = 0; i <= maxDepth; i++)
        {
            float t = i / (maxDepth - 1f); // Color transition value; squaring t makes for a nice color transition
            t *= t;
            materials[i, 0] = new Material(material);
            materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
            materials[i, 1] = new Material(material);
            materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
        }
        materials[maxDepth, 0].color = Color.magenta;
        materials[maxDepth, 1].color = Color.red;
    }

    private void InterpolateColor(int i)
    {
        if (depth < maxDepth) // Depth is used as a material indexer, maxdepth is the max of the material array size
        {
            if (i % (amountOfEntities / 4) == 0) // Since we have 4 color segments, every 1/4 of the way we will increase depth, which shifts the color towards red/magenta
                depth++;
        }
    }
}





