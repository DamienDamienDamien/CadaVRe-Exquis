using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
public class SYSTEM_CHARACTER : MonoBehaviour
{
    [Header("SCRIPTS")]
    public SYSTEM_DRAWZONE drawZoneScript;
    public OBJ_IMPORT objImportScript;
    public OBJ_EXPORT objExportScript;
    public SpawnUncompletedCharacter spawnScript;
    [Header("REFERENCES")]
    public GameObject drawZone;
    public GameObject fileManager;
    public BodyPartsData[] bodyParts;
    public string[] folderNames;
    public Material paintMaterial;
    public GameObject createdMissingPart;

    [Header("SETTINGS")]
    public bool createdFull;
    public bool canDraw;

    //PRIVATE VARIABLES

    
    [System.Serializable]
    public enum BodyPartsEnum{
        Body,
        Head,
        R_Arm,
        L_Arm,
        R_Leg,
        L_Leg
    }

    [System.Serializable]
    public class BodyPartsData
    {
        public GameObject Bone;
        public GameObject missingPartReference;
        public BodyPartsEnum BodyPart;
        public GameObject Mesh;
        public bool Missing;
    }
    

    void Awake()
    {
        drawZone = GameObject.FindWithTag("drawZone");
        drawZoneScript = drawZone.GetComponent<SYSTEM_DRAWZONE>();
        fileManager =  GameObject.FindWithTag("fileManager");
        objImportScript = fileManager.GetComponent<OBJ_IMPORT>();
        objExportScript = fileManager.GetComponent<OBJ_EXPORT>();

        
        if (createdFull)
        {
            CreateFull();
        }
        //CreateFull();
        //CreateDamaged();
        //FinishDraw();

    }

    public void CreateFull()
    {    
        foreach (BodyPartsData bodyPartData in bodyParts)
            {

                objImportScript.folderB = bodyPartData.BodyPart.ToString();
                GameObject createdMesh = objImportScript.LoadRandomJsonFileAndCreateMesh(bodyPartData.Bone.transform);

                if (createdMesh == null)
                {
                    continue;
                }
                
                createdMesh.transform.SetParent(bodyPartData.Bone.transform);
                createdMesh.transform.localPosition = Vector3.zero;
                createdMesh.transform.localRotation = Quaternion.identity;
                createdMesh.name = bodyPartData.BodyPart.ToString();
                createdMesh.GetComponent<Renderer>().material = paintMaterial;
                bodyPartData.Mesh = createdMesh;
                //createdMesh.transform.localScale = new Vector3(1f, 1f, 1f);

                SpawnUncompletedCharacter spawnScript = gameObject.GetComponent<SpawnUncompletedCharacter>();
                spawnScript.enabled = false;
            }
    }

    public void CreateDamaged()
    {
        //int randomIndex = Random.Range(0, bodyParts.Length);
        //bodyParts[randomIndex].Missing = true;

        int index = GetFolderWithFewestFiles();
        bodyParts[index].Missing = true;

        foreach (BodyPartsData bodyPartData in bodyParts)
            {
                if (bodyPartData.Missing)
                {
                    Renderer targetRenderer = bodyPartData.missingPartReference.GetComponent<Renderer>();
                    targetRenderer.enabled = true;
                    if (targetRenderer != null)
                    {
                        targetRenderer.enabled = false;
                        continue; // SKIP THE MISSING PART HERE SINCE NEW FUNCTION IS MADE TO BE CALLED
                        Bounds targetBounds = targetRenderer.bounds;

                        drawZone.transform.position = targetBounds.center;
                        //drawZone.transform.rotation = bodyPartData.missingPartReference.transform.rotation;

                        Vector3 worldSize = targetBounds.size;
                        Mesh mesh = drawZone.GetComponent<MeshFilter>().sharedMesh;
                        Vector3 meshSize = mesh.bounds.size;

                        drawZone.transform.localScale = new Vector3(
                            worldSize.x / meshSize.x,
                            worldSize.y / meshSize.y,
                            worldSize.z / meshSize.z);
                        targetRenderer.enabled = false;

                        drawZone.transform.SetParent(bodyPartData.Bone.transform);
                        continue;
                    }
                }

                objImportScript.folderB = bodyPartData.BodyPart.ToString();
                GameObject createdMesh = objImportScript.LoadRandomJsonFileAndCreateMesh(bodyPartData.Bone.transform);

                if (createdMesh == null)
                {
                    continue;
                }
                
                createdMesh.transform.SetParent(bodyPartData.Bone.transform);
                createdMesh.transform.localPosition = Vector3.zero;
                createdMesh.transform.localRotation = Quaternion.identity;
                createdMesh.name = bodyPartData.BodyPart.ToString();
                createdMesh.GetComponent<Renderer>().material = paintMaterial;
                bodyPartData.Mesh = createdMesh;
                drawZone.SetActive(false);
                //createdMesh.transform.localScale = new Vector3(1f, 1f, 1f);
            }
    }

    string GetMissingBodyPartAsString()
    {
        foreach (var part in bodyParts)
        {
            if (part.Missing)
            {
                return part.BodyPart.ToString(); 
            }
        }

        return null; 
    }

    public void FinishDraw()
    {
        drawZone.transform.SetParent(null);
        drawZone.transform.position = new Vector3(0, -10, 0);

        string missingPart = GetMissingBodyPartAsString();
        objExportScript.folderB = missingPart;
        objExportScript.CreateJsonFile();
        print("Exported: " + missingPart);

        GameObject[] objs = GameObject.FindGameObjectsWithTag(objExportScript.meshTag);
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }

        foreach (BodyPartsData bodyPartData in bodyParts)
        {
            if (bodyPartData.Missing)
            {
                bodyPartData.Missing = false;
                objImportScript.folderB = bodyPartData.BodyPart.ToString();
                createdMissingPart = objImportScript.LoadLastJsonFileAndCreateMesh(bodyPartData.Bone.transform);
                
                createdMissingPart.transform.SetParent(bodyPartData.Bone.transform);
                createdMissingPart.transform.localPosition = Vector3.zero;
                createdMissingPart.transform.localRotation = Quaternion.identity;
                createdMissingPart.name = bodyPartData.BodyPart.ToString();
                createdMissingPart.GetComponent<Renderer>().material = paintMaterial;
                bodyPartData.Mesh = createdMissingPart;
            }

        }

        //SpawnUncompletedCharacter spawnScript = gameObject.GetComponent<SpawnUncompletedCharacter>();
        spawnScript.isCompleted = true;
    }


    public void MoveDrawBox()
    {
        foreach (BodyPartsData bodyPartData in bodyParts)
        {
            if (bodyPartData.Missing)
            {
                Renderer targetRenderer = bodyPartData.missingPartReference.GetComponent<Renderer>();
                targetRenderer.enabled = true;
                if (targetRenderer != null)
                {
                    Bounds targetBounds = targetRenderer.bounds;

                    drawZone.transform.position = targetBounds.center;
                    //drawZone.transform.rotation = bodyPartData.missingPartReference.transform.rotation;

                    Vector3 worldSize = targetBounds.size;
                    Mesh mesh = drawZone.GetComponent<MeshFilter>().sharedMesh;
                    Vector3 meshSize = mesh.bounds.size;

                    drawZone.transform.localScale = new Vector3(
                        worldSize.x / meshSize.x,
                        worldSize.y / meshSize.y,
                        worldSize.z / meshSize.z);
                    targetRenderer.enabled = false;
                }
            }
        }
    }

        public int GetFolderWithFewestFiles()
    {
        string basePath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
            objImportScript.folderA
        );

        Dictionary<int, int> folderFileCounts = new Dictionary<int, int>();

        for (int i = 0; i < folderNames.Length; i++)
        {
            string fullPath = Path.Combine(basePath, folderNames[i]);

            if (Directory.Exists(fullPath))
            {
                int count = Directory.GetFiles(fullPath).Length;
                folderFileCounts[i] = count;
            }
            else
            {
                folderFileCounts[i] = int.MaxValue;
            }
        }

        int minCount = folderFileCounts.Values.Min();
        var indexesWithMin = folderFileCounts
            .Where(kvp => kvp.Value == minCount)
            .Select(kvp => kvp.Key)
            .ToList();

        int chosenIndex = indexesWithMin[Random.Range(0, indexesWithMin.Count)];
        return chosenIndex;
    }


}
