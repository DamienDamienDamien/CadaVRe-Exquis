using UnityEngine;

public class BoneVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    public Transform rootBone;                // The root of the bone hierarchy
    public float sphereSize = 0.02f;          // Size of the sphere representing each joint
    public Color sphereColor = Color.red;     // Color of the spheres
    public string spherePrefix = "BoneViz_";  // Prefix to identify created spheres

    void Start()
    {
        if (rootBone == null)
        {
            Debug.LogError("Root bone is not assigned.");
            return;
        }

        AddSpheresRecursively(rootBone);
    }

    void AddSpheresRecursively(Transform bone)
    {
        // Skip if a visualization sphere already exists
        if (bone.name.StartsWith(spherePrefix)) return;

        // Create sphere
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = spherePrefix + bone.name;
        sphere.transform.SetParent(bone, false);   // Local position stays at (0, 0, 0)
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.localRotation = Quaternion.identity;
        sphere.transform.localScale = Vector3.one * sphereSize;

        // Set color
        var renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = sphereColor;
            renderer.material = mat;
        }

        // Optional: Remove collider
        DestroyImmediate(sphere.GetComponent<Collider>());

        // Recurse into children
        foreach (Transform child in bone)
        {
            AddSpheresRecursively(child);
        }
    }
}