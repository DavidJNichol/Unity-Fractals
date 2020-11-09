using Unity.Entities;
using Unity.Mathematics;

public struct FractalComponent : IComponentData
{
	public float radiansPerSecond;
	public int depth;
}


