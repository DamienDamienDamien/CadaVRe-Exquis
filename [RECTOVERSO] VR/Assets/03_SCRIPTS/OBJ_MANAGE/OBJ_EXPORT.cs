using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class OBJ_EXPORT : MonoBehaviour
{
    [Header("FILE PATH")]
    public string folderA = "FolderA";
    public string folderB = "FolderB";
    public string fileName = "test.json"; // Base filename (without index)

    [Header("SETTINGS")]
    public string meshTag = "SaveMesh"; 
    public bool debugMode;

    [ContextMenu("Create JSON File")]
    public void CreateJsonFile()
    {
        string appDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        string fullFolderPath = Path.Combine(appDataPath, folderA, folderB);

        if (!Directory.Exists(fullFolderPath))
        {
            Directory.CreateDirectory(fullFolderPath);
        }
        Debug.Log($"Full folder path: {fullFolderPath}");
        string fileBaseName = Path.GetFileNameWithoutExtension(fileName);
        string fileExtension = Path.GetExtension(fileName);
        string filePath = Path.Combine(fullFolderPath, $"{fileBaseName}_01{fileExtension}");

        int fileIndex = 1;

        while (File.Exists(filePath))
        {
            fileIndex++;
            filePath = Path.Combine(fullFolderPath, $"{fileBaseName}_{fileIndex:00}{fileExtension}");
        }

        List<MeshData> meshDataList = new List<MeshData>();

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(meshTag);
        int meshIndex = 1;

        foreach (GameObject obj in taggedObjects)
        {
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                if (mesh != null)
                {
                    Vector3[] localVertices = mesh.vertices;
                    Vector3[] worldVertices = new Vector3[localVertices.Length];

                    // Transform local vertices to world space
                    for (int i = 0; i < localVertices.Length; i++)
                    {
                        worldVertices[i] = obj.transform.TransformPoint(localVertices[i]);
                    }

                    MeshData meshData = new MeshData
                    {
                        meshIndex = meshIndex,
                        vertices = worldVertices,
                        triangles = mesh.triangles,
                        colors = mesh.colors
                    };

                    meshDataList.Add(meshData);
                    meshIndex++;
                }
            }
        }

        if (meshDataList.Count == 0)
        {
            Debug.LogWarning("No mesh data found. JSON file not created.");
            return;
        }

        string json = JsonUtility.ToJson(new MeshList { meshes = meshDataList }, true);
        File.WriteAllText(filePath, json);

        Debug.Log($"JSON file created at: {filePath}");
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
        public List<MeshData> meshes;
    }

    void Start()
    {
        if (debugMode) { CreateJsonFile(); }
    }
}
