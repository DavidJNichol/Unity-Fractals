using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct FractalComponent : IComponentData
{
	public float radiansPerSecond;
	public int depth;

	//parent entity, you can name it entity instead of value
	//public Entity parentEntity;
	//public Entity currentEntity;

	//public NativeArray<Entity> entityArray;

	//public EntityManager entityManager;

	//public Scale scale;
}


/* so we have a fractal component which has all component data for the entitities, we should seperate each variable or a few variables into separate structs
 * We get a reference to entity through Fractal ecs, (entitymanager[i] hopefully, entitymanager[0] for the parent), and try to attach parent values to that first entity. Then we iterate 
 * down the line of entities and set their values based on the entity before it's values, (we should probably make our own components instead of using Unity's Rot, Transl, etc.
 * so that we can add more values than one
 * */
