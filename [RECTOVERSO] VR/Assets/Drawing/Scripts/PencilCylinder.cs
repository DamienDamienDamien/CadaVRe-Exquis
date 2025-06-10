using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PencilCylinder : MonoBehaviour
{
    [Header("SCRIPTS")]
    public SYSTEM_DRAWZONE drawZoneScript;
    [Header("Pen Properties")]
    public Transform penTip; // The tip of the pen, where the drawing starts 
    public Material tipMaterial; // Material used for the pen tip
    [Range(0.001f, 0.1f)]
    public float penWidth = 0.05f; // Width of the pen line
    GameObject meshObject;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    public Material coloredPaint;
    private Mesh mesh;
    GameObject outMeshObject;
    MeshFilter outMeshFilter;
    MeshRenderer outMeshRenderer;
    Mesh outMesh;

    [Header("Hands & Grabbable")]
    private bool isDrawing = false;
    List<Vector3> vertices;
    List<Vector3> verticesOut;
    List<Vector3> verticesTarget;
    List<int> triangles;
    List<int> trianglesOut;
    List<int> trianglesTarget;
    public float lineMinSize = 0.01f;
    [Range(0.001f, 1f)]
    public float baseBrushSize = 0.001f;
    public Color currentColor;
    private Vector3 LastFramePos;
    private Vector3 lastFramePosUpdated;
    public float minPressure = 0.01f; // Minimum pressure to start drawing
    List<Color> vertexColors = new List<Color>();
    List<Color> vertexColorsOut = new List<Color>();
    List<Color> vertexColorsTarget = new List<Color>();

    private void Start()
    {

         meshObject = new GameObject("CurrentDrawing");
         meshObject.tag = "SaveMesh";
         meshFilter = meshObject.AddComponent<MeshFilter>();
         meshRenderer = meshObject.AddComponent<MeshRenderer>();
        triangles = new List<int>();
        meshRenderer.material = coloredPaint;
        vertices = new List<Vector3>();
        mesh = new Mesh();

         outMeshObject = new GameObject("OutDrawing");
         outMeshFilter = outMeshObject.AddComponent<MeshFilter>();
         outMeshRenderer = outMeshObject.AddComponent<MeshRenderer>();
        trianglesOut = new List<int>();
        verticesOut = new List<Vector3>();
        outMeshRenderer.material = coloredPaint;
        outMesh = new Mesh();

        LastFramePos = penTip.position;
        lastFramePosUpdated = penTip.position;

    }

    private void Update()
    {
        float rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

        if (rightTriggerValue > 0 || leftTriggerValue > 0)
        {
            float brushPression =Mathf.Max(rightTriggerValue, leftTriggerValue);
            penWidth = Remap(brushPression, 0.001f, 1f, baseBrushSize, baseBrushSize+0.02f);


            //if(drawZoneScript.canDraw == true){Draw(mesh, meshFilter, vertices, triangles, vertexColors);}
            //else{return;}//Draw(outMesh, outMeshFilter, verticesOut, trianglesOut, vertexColorsOut);} je ne sais pas pourquoi ça ne réussit pas à créer dans le mesh outMeshFilter

            Draw(mesh, meshFilter, vertices, triangles, vertexColors,10);
            Debug.Log("Vertex Number : "+mesh.vertexCount);
            print(mesh.GetTriangles(0).Length.ToString() + " triangles in mesh");

        }
        else if (isDrawing)
        {
            
            meshFilter.mesh.colors = mesh.colors;
            outMeshFilter.mesh.colors = outMesh.colors;
            // Stop drawing when the trigger is released
            isDrawing = false;

        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Color random  = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            ChangeColor(random);
        }
        if(drawZoneScript.canDraw == true){
        lastFramePosUpdated = penTip.position;}

    }

    private void Draw(Mesh targetMesh, MeshFilter targetMeshFilter, List<Vector3> verticesTarget, List<int> trianglesTarget, List<Color> vertexColorsTarget, int cylinderRes = 6)
    {       
        
        Vector3 currentPosition = penTip.position;
        
        Vector3 direction = (currentPosition- LastFramePos).normalized;
           
        Vector3 perpendicular = transform.right * penWidth;
            if (verticesTarget.Count < 2 || !isDrawing)
            {
                for (int i = 0; i < cylinderRes; i++)
            {
                Vector3 vectorCylinder = transform.up*penWidth;
                float angle = i * (360f / cylinderRes);
                Quaternion rotation = Quaternion.AngleAxis(angle, direction);
                Vector3 rotatedVector = rotation * vectorCylinder;
                verticesTarget.Add(currentPosition+rotatedVector);
                vertexColorsTarget.Add(currentColor);
                
            }

            isDrawing = true;


            // Debug.Log(meshFilter.mesh.colors.Length.ToString() + "_" + vertexColors.Count.ToString());
        }

            else {

            for (int i = 0; i < cylinderRes; i++)
            {
                Vector3 vectorCylinder = transform.up * penWidth;
                float angle = i * (360f / cylinderRes);
                Quaternion rotation = Quaternion.AngleAxis(angle, direction);
                Vector3 rotatedVector = rotation * vectorCylinder;
                verticesTarget.Add(currentPosition + rotatedVector);
                vertexColorsTarget.Add(currentColor);
            }

            int prevBase = verticesTarget.Count - 2 * cylinderRes;
            int currBase = verticesTarget.Count - cylinderRes;
            for (int i = 0; i < cylinderRes; i++)
            {
                int next = (i + 1) % cylinderRes;
                // Premier triangle
                trianglesTarget.Add(prevBase + i);
                trianglesTarget.Add(currBase + next);
                trianglesTarget.Add(currBase + i);
                // Deuxième triangle
                trianglesTarget.Add(prevBase + i);
                
                trianglesTarget.Add(prevBase + next);
                trianglesTarget.Add(currBase + next);
            }



            // Apply vertices and triangles to the mesh
            

                
                    
                
            }
            targetMesh.SetVertices(verticesTarget);
            targetMesh.SetTriangles(trianglesTarget, 0);
            targetMesh.RecalculateNormals();
            targetMesh.colors = vertexColorsTarget.ToArray();
            targetMeshFilter.mesh = targetMesh;


        targetMeshFilter.mesh.RecalculateBounds();
        LastFramePos = penTip.position;
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return from2 + (value - from1) * (to2 - from2) / (to1 - from1);
    }


    public void ChangeColor (Color newColor){
        
        currentColor = newColor;
        tipMaterial.color = newColor;
    }

    public void CreateBaseMesh()
    {
        meshObject = null;
        meshFilter = null;
        meshRenderer = null;
        mesh = null;
        vertices = null;
        triangles = null;
        vertexColors = null;

        triangles = new List<int>();
        vertices = new List<Vector3>();
        vertexColors = new List<Color>();
         
        meshObject = new GameObject("CurrentDrawing");
        meshObject.tag = "SaveMesh";
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.material = coloredPaint;
        mesh = new Mesh();
    }

}
//Vector3 perpendicular = Vector3.Cross(direction, transform.up) * halfWidth;

