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

    private System.Random rand = new System.Random();

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
          typeof(Translation), typeof(RenderMesh),  // Rendering, Location
          typeof(LocalToWorld)  // Coordinate conversion
          );
           
        entityPosition = this.transform.localPosition;
        
        if(materials == null)
            SetMaterialColors(); // Inits material array along with the color instructions that will be set to the fractal materials

        CreateEntities(entityArchetype); // Main entity creation
    }

    private void CreateEntities(EntityArchetype entityArchetype)
    {
        entityManager.CreateEntity(entityArchetype, entityArray); // Entity spawn   

        for (int i = 0; i < amountOfEntities; i++)
        {            
            Entity entity = entityArray[i]; // Entity is created and added to array 

            InterpolateColor(i);

            entityManager.SetComponentData(entity, new Translation
            {              
                 Value = entityPosition // EntityPosition is this.transform.localposition
            }) ;

            entityManager.SetComponentData(entity, new FractalComponent
            {
                radiansPerSecond = 5, // Fractals rotate at a rate of 5 radians per second
            });

            entityManager.SetComponentData(entity, new Rotation
            {
                Value = math.mul(math.normalize(defaultQuaternion), quaternion.AxisAngle(Vector3.up, rand.Next(-maxTwist,maxTwist))) // Sets a random twist at start
            });

            OffsetEntitySpawn(2); // Adds a gap of 2 units between each entity on the z axis 

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {                
                mesh = cubeMesh[UnityEngine.Random.Range(0, cubeMesh.Length)], // Sets mesh to random out of the serialized list
                material = materials[depth, UnityEngine.Random.Range(0,2)] // Sets material to random out of the serialized list
            }
            );            
        }      
        entityArray.Dispose(); // Empty native array since garbage collector does not handle it (IMPORTANT)
    }

    private void OffsetEntitySpawn(int amount)
    {
        entityPosition.z += amount;
    }

    private void SetMaterialColors()
    {
        materials = new Material[maxDepth + 1, 2];
        for (int i = 0; i <= maxDepth; i++)
        {
            float t = i / (maxDepth - 1f);
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





