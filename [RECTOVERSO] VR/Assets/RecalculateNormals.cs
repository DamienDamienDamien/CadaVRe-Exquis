using UnityEngine;

public class RecalculateNormals : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [ExecuteInEditMode]
    void OnEnable()
    {
        // Recalculate normals when the script is enabled
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            meshFilter.sharedMesh.RecalculateNormals();
        }

        //calculate tangents
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh != null) {
            mesh.RecalculateTangents();
            }
        }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
