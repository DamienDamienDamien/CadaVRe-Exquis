using System.IO;
using UnityEngine;
using System.Linq;

public class OBJ_IMPORT : MonoBehaviour
{
    [Header("FILE PATH")]
    public string folderA = "FolderA";
    public string folderB = "FolderB";
    public string fileBaseName = "test.json";

    [Header("MATERIAL SETTINGS")]
    public Material drawMaterial;

    [Header("SETTINGS")]
    public bool debugMode;

    [ContextMenu("Load Random JSON File")]
    public GameObject LoadRandomJsonFileAndCreateMesh(Transform targetParent = null)
    {
        string appDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        string fullFolderPath = Path.Combine(appDataPath, folderA, folderB);

        if (!Directory.Exists(fullFolderPath))
        {
            Debug.LogError("Folder not found!");
            return null;
        }

        string[] files = Directory.GetFiles(fullFolderPath, $"{Path.GetFileNameWithoutExtension(fileBaseName)}_*.json");

        if (files.Length == 0)
        {
            Debug.LogError("No matching files found!");
            return null;
        }

        string randomFile = files[Random.Range(0, files.Length)];

        string jsonContent = File.ReadAllText(randomFile);
        Debug.Log($"Random file chosen: {randomFile}");

        MeshList meshList = JsonUtility.FromJson<MeshList>(jsonContent);

        if (meshList.meshes.Length > 0)
        {
            var meshData = meshList.meshes[0];

            Debug.Log($"Creating Mesh {meshData.meshIndex}");

            GameObject newMeshObject = new GameObject($"Mesh_{meshData.meshIndex}");

            MeshFilter newMeshFilter = newMeshObject.AddComponent<MeshFilter>();
            MeshRenderer newMeshRenderer = newMeshObject.AddComponent<MeshRenderer>();
            newMeshRenderer.material = drawMaterial;

            // Convert world space back to local if a target parent is provided
            Vector3[] finalVertices = meshData.vertices;

            if (targetParent != null)
            {
                finalVertices = new Vector3[meshData.vertices.Length];
                for (int i = 0; i < finalVertices.Length; i++)
                {
                    finalVertices[i] = targetParent.InverseTransformPoint(meshData.vertices[i]);
                }
            }

            Mesh newMesh = new Mesh
            {
                vertices = finalVertices,
                triangles = meshData.triangles,
                colors = meshData.colors
            };

            newMesh.RecalculateNormals();
            newMeshFilter.mesh = newMesh;

            Debug.Log($"Mesh {meshData.meshIndex} created with {meshData.vertices.Length} vertices, {meshData.triangles.Length / 3} triangles.");

            return newMeshObject;
        }

        return null;
    }

    public GameObject LoadLastJsonFileAndCreateMesh(Transform targetParent = null)
{
        string appDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        string fullFolderPath = Path.Combine(appDataPath, folderA, folderB);

        if (!Directory.Exists(fullFolderPath))
        {
            Debug.LogError("Folder not found!");
            return null;
        }

        string[] files = Directory.GetFiles(fullFolderPath, $"{Path.GetFileNameWithoutExtension(fileBaseName)}_*.json");

        if (files.Length == 0)
        {
            Debug.LogError("No matching files found!");
            return null;
        }

        // Sort files by last write time (descending) and take the most recent one
        string latestFile = files
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .First();

        string jsonContent = File.ReadAllText(latestFile);

        MeshList meshList = JsonUtility.FromJson<MeshList>(jsonContent);

        if (meshList.meshes.Length > 0)
        {
            var meshData = meshList.meshes[0];

            Debug.Log($"Creating Mesh {meshData.meshIndex}");

            GameObject newMeshObject = new GameObject($"Mesh_{meshData.meshIndex}");

            MeshFilter newMeshFilter = newMeshObject.AddComponent<MeshFilter>();
            MeshRenderer newMeshRenderer = newMeshObject.AddComponent<MeshRenderer>();
            newMeshRenderer.material = drawMaterial;

            // Convert world space back to local if a target parent is provided
            Vector3[] finalVertices = meshData.vertices;

            if (targetParent != null)
            {
                finalVertices = new Vector3[meshData.vertices.Length];
                for (int i = 0; i < finalVertices.Length; i++)
                {
                    finalVertices[i] = targetParent.InverseTransformPoint(meshData.vertices[i]);
                }
            }

            Mesh newMesh = new Mesh
            {
                vertices = finalVertices,
                triangles = meshData.triangles,
                colors = meshData.colors
            };

            newMesh.RecalculateNormals();
            newMeshFilter.mesh = newMesh;

            Debug.Log($"Mesh {meshData.meshIndex} created with {meshData.vertices.Length} vertices, {meshData.triangles.Length / 3} triangles.");

            return newMeshObject;
        }

        return null;
    }

    [System.Serializable]
    public class MeshData
    {
        public int meshIndex;
        public Vector3[] vertices;
        public int[] triangles;
        public Color[] colors;
    }

    [System.Serializable]
    public class MeshList
    {
        public MeshData[] meshes;
    }

    void Start()
    {
        if (debugMode)
        {
            GameObject createdMesh = LoadRandomJsonFileAndCreateMesh();
            if (createdMesh != null)
            {
                Debug.Log($"Created Mesh: {createdMesh.name}");
            }
        }
    }
}
