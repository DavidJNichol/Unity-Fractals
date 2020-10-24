using Unity.Entities;

public class EntitySystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref EntityComponent entityComponent) =>
        {
            // Increment level by 1 per second
            entityComponent.componentFloat += 1f;
        });
    }
}