using Unity.Entities;
using UnityEngine;

public class FractalSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Iterate through all entities containing a LevelComponent
        Entities.ForEach((ref FractalComponent fractalComponent) =>
        {
            // Increment level by 1 per second
            fractalComponent.rotationSpeed += 5;
        });
    }


}
