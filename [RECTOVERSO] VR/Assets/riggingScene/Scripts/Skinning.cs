
using Unity.Collections;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;

public class Skinning : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SkinnedMeshRenderer skinnedMeshRenderer;
    public GameObject targetObject;
    
    //[SerializeField] List<Vector3> bonesPos = new List<Vector3>();
    public float seuil = 0.1f;

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
        
        targetObject.GetComponent<SkinnedMeshRenderer>().rootBone = bones[0].transform; // Set the root bone of the SkinnedMeshRenderer
        //Setting bind poses

        Matrix4x4[] bindPoses = new Matrix4x4[bones.Count];
        skinnedMeshRenderer.bones = bones.Select(b => b.transform).ToArray();
        for (int i = 0; i < bones.Count; i++)
        {
            bones[i].transform.localRotation = Quaternion.identity; // Reset rotation to avoid skewing bind poses
            bindPoses[i] = bones[i].transform.worldToLocalMatrix * targetObject.transform.localToWorldMatrix;
        }
        skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
        Debug.Log(bones.Count + " bones found, bind poses set.");

        byte[] boneWeightsPerVertex = new byte[mesh.vertexCount];

        List<BoneWeight1> boneWeights = new List<BoneWeight1>();

        for (int v = 0; v < mesh.vertexCount; v++)
        {
 

            var dis1 = DistanceToSegment(bones[0].transform.position, bones[1].transform.position, vertices[v]);
            var dis2 = DistanceToSegment(bones[1].transform.position, bones[2].transform.position, vertices[v]);

            if (Mathf.Abs(dis1 - dis2) < seuil)
            {
                boneWeightsPerVertex[v] = 2; // Both bones influence the vertex
                var lerpFac = math.remap(-seuil,seuil,0f,1f, dis2 - dis1);
                lerpFac = Mathf.SmoothStep(0f, 1f, lerpFac); // Smooth transition factor
                lerpFac = Mathf.SmoothStep(0f, 1f, lerpFac); // Smooth transition factor

                lerpFac = Mathf.SmoothStep(0f, 1f, lerpFac); // Smooth transition factor

                Debug.Log("fac ="+ lerpFac);
                colors[v] = Color.Lerp(color1, color2, lerpFac); // Blend color if distances are similar
                var bw = new BoneWeight1();
                bw.boneIndex = 0; // First bone
                bw.weight = lerpFac; // Weight for the first bone
                boneWeights.Add(bw);
                bw.boneIndex = 1; // Second bone
                bw.weight = 1 - lerpFac; // Weight for the second bone
                boneWeights.Add(bw);
                
            }
           
            else if (dis1 >= dis2)
            {
                boneWeightsPerVertex[v] = 1; // Only the first bone influences the vertex
                var bw= new BoneWeight1();
                bw.boneIndex = 1; // First bone
                bw.weight = 1f; // Full influence from the first bone
                boneWeights.Add(bw);
                colors[v] = color1; // Red for vertices close to the first bone segment
            }
            else
            {
                boneWeightsPerVertex[v] = 1; // Only the first bone influences the vertex
                var bw = new BoneWeight1();
                bw.boneIndex = 0; // First bone
                bw.weight = 1f; // Full influence from the first bone
                boneWeights.Add(bw);
                colors[v] = color2; // Green for other vertices


            }

        }

        var boneWeightsArray = boneWeights.ToArray(); 
        // Create NativeArray versions of the two arrays
        var bonesPerVertexArray = new NativeArray<byte>(boneWeightsPerVertex, Allocator.Temp);
        var weightsArray = new NativeArray<BoneWeight1>(boneWeightsArray, Allocator.Temp);

        mesh.SetBoneWeights(bonesPerVertexArray,weightsArray);

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
