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
    [SerializeField] private bool rotation;
    [SerializeField] private bool weirdFractal;
    [SerializeField] public float radiansPerSecond;

    private Material[,] materials;

    private float3 entityPosition;
    private int amountOfEntities;
    private NativeArray<Entity> entityArray;
    private EntityArchetype entityArchetype;
    private EntityManager entityManager;

    private int depth;
    Entity parentScale;
    Entity parent;
    

    int index;

    float3 newLoc;

    float4x4 loc;

    private System.Random rand = new System.Random();

    private float startTime;

    private quaternion newQuaternion;

    private static Vector3[] childDirections = {  // TO USE IN THE FUTURE
        Vector3.right,
        Vector3.left,
        Vector3.up,
        Vector3.down,
        Vector3.forward,       
        Vector3.back          
    };

    private static quaternion[] childOrientations = {  // TO USE IN THE FUTURE
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f),
        Quaternion.Euler(0f, 90f, 0f)
    };

    //MOVE SOME OF THIS INTO AWAKE INSTEAD FOR A BIT BETTER OPTIMIZATION
    private void Start()
    {
        amountOfEntities = (int)math.pow(5, maxDepth); // how many fractal children are spawned is 5^(maxDepth)

        entityArray= new NativeArray<Entity>((int)amountOfEntities, Allocator.Temp); 

        entityManager = World.Active.EntityManager;       

        // TRANSLATION, ROTATION AND SCALE CAN POSSIBLY BE DONE WITH JUST LOCALTOWORLD
          entityArchetype = entityManager.CreateArchetype(
          typeof(FractalComponent), typeof(Rotation), //Component class, Rotation
          typeof(RenderMesh), typeof(Translation),// Rendering, Location, Coords local to parent
          typeof(LocalToWorld), typeof(Scale)  // Coordinate conversion, Scale, Parent
          );

        startTime = Time.realtimeSinceStartup;

        entityPosition = this.transform.localPosition;

        if (materials == null)
            SetMaterialColors(); // Inits material array along with the color instructions that will be set to the fractal materials

        loc = new float4x4(float4.zero, float4.zero, float4.zero, new float4(entityPosition, 1)); // this.transform.position used for localtoworld, rotation values are set to zero for now        
               
            CreateEntities(entityArchetype); // Call entity spawn method

             
        
        Debug.Log(("Startup Time: " + (Time.realtimeSinceStartup - startTime) * 1000f) + "ms"); // Start up time debug        
    }

    private void CreateEntities(EntityArchetype entityArchetype)
    {
        float scale = 1;

        entityManager.CreateEntity(entityArchetype, entityArray); // Entity spawn   

        for (int i = 0; i < amountOfEntities; i++)
        {
            if (index > 5)
            {
                index = 0; // Used as a direction indexer
            }

            Entity entity = entityArray[i]; // Entity is created and added to array 

            if (i == 0)
            {
                parent = entity;
                parentScale = entity;
            }
            else
            {
                parentScale = entityArray[i - 1];
                parent = entityArray[i - 1];// If we reach the 5th entity, make a new parent for the next 5 entities      
            }

            if (i > 0)
            {
                scale = entityManager.GetComponentData<Scale>(parentScale).Value * childScale;
            }

            if (weirdFractal)
            {
                scale = childScale;
            }

            InterpolateColor(i);

            //entityManager.SetComponentData(entity, new Translation
            //{
            //    Value = entityPosition // EntityPosition is this.transform.localposition
            //});

            entityManager.SetComponentData(entity, new FractalComponent
            {
                radiansPerSecond = radiansPerSecond, // Fractals rotate at a rate of x radians per second
                translation = new float3(childDirections[depth] * (0.5f + 0.5f * childScale)),
                rotation = rotation,
                weirdFractal = weirdFractal,
                parent = parent
            });

            entityManager.SetComponentData(entity, new LocalToWorld
            {
                Value = loc
            });

            newLoc = new float3(entityManager.GetComponentData<Translation>(parent).Value.x, entityManager.GetComponentData<Translation>(parent).Value.y,
                entityManager.GetComponentData<Translation>(parent).Value.z);
            
            newLoc += new float3(childDirections[index].x, childDirections[index].y, childDirections[index].z) * (0.5f + 0.5f * childScale);

            if (!weirdFractal)
            {
                entityManager.SetComponentData(entity, new Translation
                {
                    Value = newLoc
                });
            }
            newQuaternion = entityManager.GetComponentData<Rotation>(parent).Value;

            newQuaternion.value = childOrientations[index].value;

            //translationTemp = entityManager.GetComponentData<Translation>(parentDirection).Value + new float3(childDirections[index].x, childDirections[index].y, childDirections[index].z) * (0.5f + 0.5f * childScale);
            //translationTemp = childDirections[index].y;

            entityManager.SetComponentData(entity, new Rotation
            {
                Value = newQuaternion
                /*Value = math.mul(math.normalize(defaultQuaternion), quaternion.AxisAngle(Vector3.up, rand.Next(-maxTwist, maxTwist)))*/ // Sets a random twist at start
            });

            entityManager.SetComponentData(entity, new Scale
            {
                Value = scale
            });

            //entityManager.SetComponentData(entity, new Parent
            //{
            //    Value = parentDirection // Parent == entityArray[i-1]
            //});

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

            index++;
            IterateDirections();
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

    private void IterateDirections()
    {
        childDirections[index] += childDirections[index];
    }


    //private void CreateEntities(EntityArchetype entityArchetype)
    //{
    //    float scale = 1;

    //    entityManager.CreateEntity(entityArchetype, entityArray); // Entity spawn   

    //    for (int i = 0; i < amountOfEntities; i++)
    //    {                      
    //        if(index > 5)
    //        {
    //            index = 0; // Used as a direction indexer
    //        }

    //        Entity entity = entityArray[i]; // Entity is created and added to array 

    //        if (i == 0)
    //        {
    //            parent = entity;
    //            parentScale = entity;
    //        }            
    //        else if(i % 5 == 0)
    //        {
    //            parentScale = entityArray[i - 5];                         
    //        }
    //        else
    //        {
    //            parent = entityArray[i - 1];// If we reach the 5th entity, make a new parent for the next 5 entities      
    //        }

    //        if (i > 0)
    //        {
    //            scale = entityManager.GetComponentData<Scale>(parentScale).Value * childScale;
    //        }

    //        if(weirdFractal)
    //        {
    //            scale = childScale;
    //        }

    //        InterpolateColor(i);

    //        //entityManager.SetComponentData(entity, new Translation
    //        //{
    //        //    Value = entityPosition // EntityPosition is this.transform.localposition
    //        //});

    //        entityManager.SetComponentData(entity, new FractalComponent
    //        {
    //            radiansPerSecond = radiansPerSecond, // Fractals rotate at a rate of x radians per second
    //            translation = new float3(childDirections[index] * (0.5f + 0.5f * childScale)),
    //            rotation = rotation,
    //            weirdFractal = weirdFractal,
    //            parent = parent                               
    //        });

    //        entityManager.SetComponentData(entity, new LocalToWorld
    //        {
    //            Value = loc
    //        });           

    //        newLoc = new float3(entityManager.GetComponentData<Translation>(parent).Value.x, entityManager.GetComponentData<Translation>(parent).Value.y, 
    //            entityManager.GetComponentData<Translation>(parent).Value.z);
    //        Vector3 aaa = new Vector3(childDirections[index].x, childDirections[index].y, childDirections[index].z);
    //        newLoc += new float3(childDirections[index].x, childDirections[index].y, childDirections[index].z);

    //        if (!weirdFractal)
    //        {
    //            entityManager.SetComponentData(entity, new Translation
    //            {
    //                Value = newLoc
    //            });
    //        }
    //        newQuaternion = entityManager.GetComponentData<Rotation>(parent).Value;

    //        newQuaternion.value = childOrientations[index].value;

    //        //translationTemp = entityManager.GetComponentData<Translation>(parentDirection).Value + new float3(childDirections[index].x, childDirections[index].y, childDirections[index].z) * (0.5f + 0.5f * childScale);
    //        //translationTemp = childDirections[index].y;

    //        entityManager.SetComponentData(entity, new Rotation
    //        {
    //            Value = newQuaternion
    //            /*Value = math.mul(math.normalize(defaultQuaternion), quaternion.AxisAngle(Vector3.up, rand.Next(-maxTwist, maxTwist)))*/ // Sets a random twist at start
    //        });

    //        entityManager.SetComponentData(entity, new Scale
    //        {
    //            Value = scale
    //        });

    //        //entityManager.SetComponentData(entity, new Parent
    //        //{
    //        //    Value = parentDirection // Parent == entityArray[i-1]
    //        //});

    //        //entityManager.SetComponentData(entity, new LocalToParent
    //        //{
    //        //    Value = loc
    //        //});

    //        entityManager.SetSharedComponentData(entity, new RenderMesh
    //        {
    //            mesh = cubeMesh[UnityEngine.Random.Range(0, cubeMesh.Length)], // Sets mesh to random out of the serialized list
    //            material = materials[depth, UnityEngine.Random.Range(0, 2)] // Sets material to random out of the serialized list
    //        }
    //        );

    //        OffsetEntitySpawn(3); // Adds a gap of x units between each entity on the z axis                    

    //        index++;
    //    }      
    //    entityArray.Dispose(); // Empty native array since garbage collector does not handle it (IMPORTANT)
    //}
}





