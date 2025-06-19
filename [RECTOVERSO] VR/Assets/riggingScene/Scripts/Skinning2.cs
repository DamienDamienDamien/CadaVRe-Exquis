
using Unity.Collections;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;
using UnityEngine.UIElements;

public class Skinning2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SkinnedMeshRenderer skinnedMeshRenderer;
    public GameObject targetObject;
    
    //[SerializeField] List<Vector3> bonesPos = new List<Vector3>();
    public float seuil = 0.1f;
    public float sigma = 0.1f; // Controls the radius of influence of the bones on the vertices

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
            //bones[i].transform.localRotation = Quaternion.identity; // Reset rotation to avoid skewing bind poses
            bindPoses[i] = bones[i].transform.worldToLocalMatrix * targetObject.transform.localToWorldMatrix;
        }
        skinnedMeshRenderer.sharedMesh.bindposes = bindPoses;
        Debug.Log(bones.Count + " bones found, bind poses set.");

        byte[] boneWeightsPerVertex = new byte[mesh.vertexCount];

        List<BoneWeight1> boneWeights = new List<BoneWeight1>();

        
        Matrix4x4 meshToWorld = targetObject.transform.localToWorldMatrix;

        for (int i = 0; i < bones.Count; i++)
        {
            Debug.Log($"Bone {i}: {bones[i].name}, Position: {bones[i].transform.position}");
        }
        
        var miny =float.MaxValue;
        var maxY = -float.MaxValue;

        for (int v = 0; v < mesh.vertexCount; v++)
        {
            var weights = new float[bones.Count-1];
            var total = 0f;
            int bonesPerVertex = 0; // Track the number of bones influencing this vertex
            var minDistance = float.MaxValue;
            int minDistanceBoneIndex = -1; // Track the index of the bone with the minimum distance

            Vector3 vertexLocalPos =meshToWorld.MultiplyPoint3x4(vertices[v]); // Convert vertex position to world space
            if (vertexLocalPos.y < miny)
            {
                miny = vertexLocalPos.y; // Update minimum Y value
            }
            if (vertexLocalPos.y > maxY)
            {
                maxY = vertexLocalPos.y; // Update maximum Y value
            }
            for (int i = 0; i < bones.Count-1; i++)
            {
                float d = DistanceToSegment(bones[i].transform.position, bones[i + 1].transform.position, vertexLocalPos);
                if (d < minDistance)
                {
                    minDistance = d; // Update the minimum distance
                    minDistanceBoneIndex = i; // Update the index of the bone with the minimum distance
                }
                float w = Mathf.Exp(-d * d / sigma);
                weights[i] = w;
                total += w;
                
            }
            //Debug.Log($"Vertex {v} min distance: {minDistance}, bone index: {minDistanceBoneIndex}");
            if (total > 0f)
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] /= total; // Normalize the weights
                }
            }
            else
            {
                Debug.LogWarning("This vertex has no influence from any bone!!");
                weights[minDistanceBoneIndex] = 1f; // Assign full weight to the bone with the minimum distance

            }

            /*
            for (int i = 0; i < bones.Count - 1; i++)
            {
                if (i == minDistanceBoneIndex)
                {
                    weights[i] = 1f; // Assign full weight to the bone with the minimum distance
                }
                else
                {
                    weights[i] = 0f; // Set other weights to zero
                }
            }*/

            colors[v] = new Color(0f, 0f, 0f, 1f); // Default color for the vertex
            for (int i=0; i < bones.Count-1; i++)
            {

                if (weights[i] > seuil)
                {
                    bonesPerVertex++;
                    var bw = new BoneWeight1();
                    bw.boneIndex = i; // Set the bone index
                    bw.weight = weights[i]; // Set the weight
                    boneWeights.Add(bw);
                    if (i == 0)
                    {
                        Color impact = new Color(weights[i],0f,0f,1f);
                        colors[v] = impact; // Set color for the first bone
                    }
                      
                    
                }
            }
            //Debug.Log($"Vertex {v} has {bonesPerVertex} bones influencing it.");

            boneWeightsPerVertex[v] = (byte)bonesPerVertex; // Store the number of bones influencing this vertex
        }
        Debug.Log($"Minimum Y: {miny}, Maximum Y: {maxY}"); // Log the min and max Y values

        mesh.colors = colors;
        var boneWeightsArray = boneWeights.ToArray(); 
        // Create NativeArray versions of the two arrays
        var bonesPerVertexArray = new NativeArray<byte>(boneWeightsPerVertex, Allocator.Temp);
        var weightsArray = new NativeArray<BoneWeight1>(boneWeightsArray, Allocator.Temp);

        mesh.SetBoneWeights(bonesPerVertexArray,weightsArray);

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
}
