using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

public class Jobs : MonoBehaviour
{
    [SerializeField] private bool useJobs;
    [SerializeField] private int amountOfObjects;
    [SerializeField] private Transform prefabLime; //Init via inspector
    private List<Lime> limeList;

    //JOB
    [BurstCompile]
    public struct ReallyToughParallelJob : IJobParallelFor
    {
        public NativeArray<float3> positionArray;
        public NativeArray<float> moveYArray;
        public float deltaTime;

        public void Execute(int index)
        {
            positionArray[index] += new float3(0, moveYArray[index] * deltaTime, 0f);

            if (positionArray[index].y > 5f)
            {
                moveYArray[index] = -math.abs(moveYArray[index]);
            }

            if (positionArray[index].y < -5f)
            {
                moveYArray[index] = +math.abs(moveYArray[index]);
            }
        }
    }
    //LIME
    public class Lime
    {
        public Transform transform;
        public float moveY;
    }

    private void Start()
    {
        limeList = new List<Lime>();

        // Instantiate and randomly position limes
        for (int i = 0; i < amountOfObjects; i++)
        {
            Transform limeTransform = Instantiate(prefabLime, new Vector3(UnityEngine.Random.Range(-12f, 25f), UnityEngine.Random.Range(-5f, 30f), UnityEngine.Random.Range(-8f, 20f)), Quaternion.identity);
            limeList.Add(new Lime
            {
                transform = limeTransform,
                moveY = UnityEngine.Random.Range(1f, 2f)
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        float startTime = Time.realtimeSinceStartup;

        // Create arrays for copying data into
        NativeArray<float3> positionArray = new NativeArray<float3>(limeList.Count, Allocator.TempJob);
        NativeArray<float> moveYArray = new NativeArray<float>(limeList.Count, Allocator.TempJob);

        // Populate the arrays 
        for (int i = 0; i < limeList.Count; i++)
        {
            positionArray[i] = limeList[i].transform.position;
            moveYArray[i] = limeList[i].moveY;
        }

        // Create job w. data
        ReallyToughParallelJob reallyToughParallelJob = new ReallyToughParallelJob
        {
            deltaTime = Time.deltaTime,
            positionArray = positionArray,
            moveYArray = moveYArray,
        };

        // Schedule job
        JobHandle jobHandle = reallyToughParallelJob.Schedule(limeList.Count, 100);
        jobHandle.Complete();

        // Update to calculated values
        for (int i = 0; i < limeList.Count; i++)
        {
            limeList[i].transform.position = positionArray[i];
            limeList[i].moveY = moveYArray[i];
        }

        // Dispose arrays
        positionArray.Dispose();
        moveYArray.Dispose();

        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
    }
}