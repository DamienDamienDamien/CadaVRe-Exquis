using System.Collections.Generic;
using UnityEngine;

public class ReadBoneWeights : MonoBehaviour
{
    //[SerializeField] BoneWeight1[] boneWeights;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int weightnum = 0; // 0 for weight0, 1 for weight1, 2 for weight2

    int vertexCount;
    Mesh mesh;
    SkinnedMeshRenderer skinnedMeshRenderer;
    int prevWeight = -1;
    void Start()
    {
        
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        
        mesh = skinnedMeshRenderer.sharedMesh;
        vertexCount = mesh.vertexCount;
        viewWeight();

    }

    // Update is called once per frame
    [ContextMenu("View Weight")]

    void viewWeight()
    {


        var bonesPerVertex = mesh.GetBonesPerVertex();


        var boneWeights = skinnedMeshRenderer.sharedMesh.GetAllBoneWeights();

        // Keep track of where we are in the array of BoneWeights, as we iterate over the vertices
        var boneWeightIndex = 0;

        Color[] colors = new Color[vertexCount];
        // Iterate over the vertices
        for (var vertIndex = 0; vertIndex < vertexCount; vertIndex++)
        {
            var totalWeight = 0f;
            var numberOfBonesForThisVertex = bonesPerVertex[vertIndex];
            Debug.Log("This vertex has " + numberOfBonesForThisVertex + " bone influences");
            var color = Color.black;
            // For each vertex, iterate over its BoneWeights
            for (var i = 0; i < numberOfBonesForThisVertex; i++)
            {
                var currentBoneWeight = boneWeights[boneWeightIndex];

                totalWeight += currentBoneWeight.weight;
                if (currentBoneWeight.boneIndex == weightnum)
                {
                    color = new Color(currentBoneWeight.weight, currentBoneWeight.weight, currentBoneWeight.weight);
                }

                if (i > 0)
                {
                    Debug.Assert(boneWeights[boneWeightIndex - 1].weight >= currentBoneWeight.weight);
                }
                boneWeightIndex++;
            }
            
            Debug.Assert(Mathf.Approximately(1f, totalWeight));

            colors[vertIndex] = color;
        }
        // Assign colors to the mesh
        mesh.colors = colors;
    }
    void Update()
    {
        if (weightnum != prevWeight)
        {
            viewWeight();
            prevWeight = weightnum;
        }
    }
}
