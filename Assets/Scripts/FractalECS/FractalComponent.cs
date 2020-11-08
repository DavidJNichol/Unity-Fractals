using Unity.Entities;
using UnityEngine;

public struct FractalComponent : IComponentData
{
	public float rotationSpeed;
	public int depth;
}

