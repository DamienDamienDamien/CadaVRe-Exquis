using UnityEngine;

public class WeightsViz: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Renderer renderer;
    [SerializeField] Vector3 bounds;
    public Transform rootBone;
    void Start()
    {
        renderer = GetComponent<Renderer>();
        bounds = renderer.bounds.size;


        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        Destroy(meshFilter);

        SkinnedMeshRenderer skinnedRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
        skinnedRenderer.sharedMesh = mesh;
        skinnedRenderer.rootBone = rootBone;



        // Get the number of vertices
        int vertexCount = mesh.vertexCount;

        // Create a color array with a random color per vertex
        Color[] colors = new Color[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            colors[i] = new Color(Random.value, Random.value, Random.value);
        }

        // Assign colors to the mesh
        mesh.colors = colors;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
