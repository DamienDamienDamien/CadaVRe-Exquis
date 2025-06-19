using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class viewSelection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SkinnedMeshRenderer skinnedMeshRenderer;
    public GameObject targetObject;
    //[SerializeField] List<Vector3> bones = new List<Vector3>();


    void Start()
    {
        var bones = GetComponent<getBones>().bones;
        skinnedMeshRenderer = targetObject.gameObject.AddComponent<SkinnedMeshRenderer>();
        Mesh mesh = targetObject.GetComponent<MeshFilter>().sharedMesh;
        skinnedMeshRenderer.sharedMesh = mesh;
        Destroy(GetComponent<MeshFilter>());
        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[mesh.vertexCount];
        Color color1 = new Color(1f, 0f, 0f);
        Color color2 = new Color(0f, 1f, 0f);

        for (int v = 0; v < mesh.vertexCount; v++)
        {
            var dis1 = DistanceToSegment(bones[0].transform.position, bones[1].transform.position, vertices[v]);
            var dis2 = DistanceToSegment(bones[1].transform.position, bones[2].transform.position, vertices[v]);

           /* if (Mathf.Abs(dis1 - dis2) < 0.1f)
            {
                var lerpFac = Mathf.Clamp01(dis1 / (dis1 + dis2));
                Debug.Log("fac ="+ lerpFac);
                colors[v] = Color.Lerp(color1, color2, lerpFac); // Blend color if distances are similar
            }
           */
            if (dis1 >= dis2)
            {

                colors[v] = color1; // Red for vertices close to the first bone segment
            }
            else
            {
                colors[v] = color2; // Green for other vertices


            }

        }
            
           
        mesh.colors = colors;
    }

    float DistanceToSegment(Vector3 pointA, Vector3 pointB, Vector3 vertexPos)
    {
        Vector3 ab = pointB - pointA;
       
        Vector3 ap = vertexPos - pointA;

        float projection = (Vector3.Dot(ab, ap)/ab.sqrMagnitude);
        projection = Mathf.Clamp01(projection); // Clamp to the segment



        Vector3 closestPoint = pointA + projection*ab;
        float distance = Vector3.Distance(closestPoint, vertexPos);
        return distance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
