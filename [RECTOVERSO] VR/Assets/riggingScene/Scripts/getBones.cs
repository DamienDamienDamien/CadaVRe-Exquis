using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class getBones : MonoBehaviour
{
    public List<GameObject> bones = new List<GameObject>();
    public string BonePrefix = "Bone"; // Prefix to filter bone names, can be set in the inspector  
    public Transform rootBone; // Assign the root bone in the inspector if needed
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GetChildrenRecursive(rootBone);
    }

    void GetChildrenRecursive(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name.StartsWith("Bone")||child.name.StartsWith("Tail"))
            {
                bones.Add(child.transform.gameObject);
            }
            GetChildrenRecursive(child);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
