using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FractalSystem : ComponentSystem
{
    // Kinda slow, but works!
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, ref FractalComponent fractalComponent) =>
        {
            var deltaTime = Time.deltaTime;
            rotation.Value = math.mul(math.normalize(rotation.Value),
                quaternion.AxisAngle(Vector3.right, fractalComponent.radiansPerSecond * deltaTime));
        });
    }
}
