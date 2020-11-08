using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using Unity.Physics.Extensions;
using UnityEditor.Experimental.GraphView;

public class FractalECS : MonoBehaviour
{
    //[SerializeField] private Mesh cubeMesh;
    [SerializeField] private Mesh[] cubeMesh;
    [SerializeField] private Material[] material;
    [SerializeField] private int maxDepth;

    private float3 entityPosition;
    //[SerializeField] private float childScale;
    //[SerializeField] private float spawnProbability;
    //[SerializeField] private int maxRotationSpeed;
    //[SerializeField] private float maxTwist;

    private float amountOfEntities;
    private System.Random rand;
    private int depth;

    private static Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] childOrientations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
    };

    private void Start()
    {
        amountOfEntities = math.pow(5, maxDepth); // how many fractal children are spawned is 5^maxDepth
        entityPosition = this.transform.localPosition;


        if (depth < maxDepth)
        {
            CreateEntities(this, depth);
        }
    }

    private void CreateEntities(FractalECS parent, int childIndex)
    {
        
        EntityManager entityManager = World.Active.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(EntityComponent), typeof(Rotation),
            typeof(Translation), typeof(RenderMesh),  // Rendering
            typeof(LocalToWorld), typeof(Scale) // Coordinate conversion
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>((int)amountOfEntities, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        for (int i = 0; i < amountOfEntities; i++)
        {
            Entity entity = entityArray[i];

            entityManager.SetComponentData(entity, new Translation
            {              
                 Value = entityPosition // That last .5 is the equiv of childScale, can be turned into a serialized variable in future
            }) ;

            entityManager.SetComponentData(entity, new Scale
            {
                //Value = .5f
            });

            entityPosition.z += 2;

            //entityManager.SetComponentData(entity, new Rotation
            //{
            //    Value = childOrientations[childIndex]
            //});

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = cubeMesh[UnityEngine.Random.Range(0, cubeMesh.Length)],
                material = material[UnityEngine.Random.Range(0, material.Length)],
            }
            );

            if(depth < maxDepth-1)
                depth++;
        }
        entityArray.Dispose();
    }

    //meshes = parent.meshes;
    //materials = parent.materials;
    //maxDepth = parent.maxDepth;
    //depth = parent.depth + 1;
    //childScale = parent.childScale;
    //spawnProbability = parent.spawnProbability;
    //maxRotationSpeed = parent.maxRotationSpeed;
    //maxTwist = parent.maxTwist;
    //transform.parent = parent.transform;
    //transform.localScale = Vector3.one* childScale;
    //  transform.localPosition =
    //	childDirections[childIndex] * (0.5f + 0.5f * childScale);
    //transform.localRotation = childOrientations[childIndex];

}



