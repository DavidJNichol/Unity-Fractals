using Unity.Entities;
using Unity.Mathematics;

public struct FractalComponent : IComponentData
{
	public quaternion rotationSpeed;
	public int depth;
}


