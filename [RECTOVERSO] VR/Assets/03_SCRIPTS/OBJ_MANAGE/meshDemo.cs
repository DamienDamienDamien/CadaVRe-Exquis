using UnityEngine;

public class meshDemo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        /*GameObject newSphere = new GameObject("newSphere");
        MeshFilter newMeshFilter = newSphere.AddComponent<MeshFilter>();
        MeshRenderer newMeshRenderer = newSphere.AddComponent<MeshRenderer>();
        newMeshRenderer.material = new Material(Shader.Find("Standard"));
        newMeshFilter.mesh = new Mesh();
        newMeshFilter.mesh.vertices = vertices;
        newMeshFilter.mesh.triangles = triangles;
        newMeshFilter.mesh.RecalculateNormals();

        newSphere.transform.position = new Vector3(0, 5, 0);*/



        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
