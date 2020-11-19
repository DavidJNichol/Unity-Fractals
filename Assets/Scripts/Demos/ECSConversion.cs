using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ECSConversion : MonoBehaviour
{
    private List<GameObject> CubeGOList;

    public GameObject Cube;

    EntityManager manager;
    Entity AsteroidEntityPrefab;
    Vector3 offset = new Vector3(0,0,0);

    void Awake()
    {
        if(manager == null)
            manager = World.Active.EntityManager;

        AsteroidEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Cube, World.Active);
    }

    void Update()
    {
        if (Input.GetKey("g"))
        {
            for (int i = 0; i < 5; i++)
                SpawnTheAsteroids();
        }
    }

    private void SpawnTheAsteroids()
    {
        Entity asteroid = manager.Instantiate(AsteroidEntityPrefab);
        offset.z+=2;
        manager.SetComponentData(asteroid, new Translation { Value = transform.position + offset });
        manager.SetComponentData(asteroid, new Rotation { Value = Quaternion.identity });
    }
}
