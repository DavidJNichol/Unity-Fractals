using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public int maxDepth;
    private int depth;
    public float childScale;

    private static Vector3[] childDirections = 
    { 
        Vector3.up, 
        Vector3.right, 
        Vector3.left, 
        Vector3.forward, 
        Vector3.back
    };

    private static Quaternion[] childOritentations = 
    { 
        Quaternion.identity, 
        Quaternion.Euler(0f, 0f, -90f), 
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)

    };

    private Material[] materials;   
    
    void Start()
    {
        if(materials == null)
        {
            InitializeMaterials();
        }
        Debug.Log(materials[0]);
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = materials[depth];       
        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }      
    }
    // DYNAMIC BATCHING vvv
    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1];
        for (int i = 0; i <= maxDepth; i++)
        {
            materials[i] = new Material(material);
            materials[i].color = Color.Lerp(Color.white, Color.yellow, (float)i / maxDepth);
        }
    }
    // DYNAMIC BATCHING ^^^

    private void Initialize(Fractal parent, int childIndex)
    {
        mesh = parent.mesh;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        transform.parent = parent.transform;
        childScale = parent.childScale;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childDirections[childIndex] * (.5f + .5f * childScale);
        transform.localRotation = childOritentations[childIndex];
    }

    private IEnumerator CreateChildren()
    {
        for(int i = 0; i < childDirections.Length; i++)
        {
            yield return new WaitForSeconds(Random.Range(.1f,.5f));
            new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, i);
         
        }        
    }

    
    void Update()
    {
        
    }
}
