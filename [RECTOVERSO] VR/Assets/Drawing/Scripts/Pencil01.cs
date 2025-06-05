using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pencil01 : MonoBehaviour
{



    [Header("Pen Properties")]
    public Transform penTip; // The tip of the pen, where the drawing starts
    public Material drawingMaterial; // Material used for the pen line
    public Material tipMaterial; // Material used for the pen tip
    [Range(0.01f, 0.1f)]
    public float penWidth = 0.05f; // Width of the pen line
    public Color[] penColors; // Color of the pen line

    [Header("Hands & Grabbable")]

    private LineRenderer currentDrawing; // Line renderer component for drawing
    private List<Vector3> positions = new();
    private int index;
    private int currentColorIndex;



    private void Start()
    {
        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];

    }

    private void Update()
    {
        // Vérifie la valeur de l'index trigger (par exemple, main droite)
        float rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

        if (rightTriggerValue > 0.1f || leftTriggerValue > 0.1f) // Seuil pour considérer que l'un des index triggers est pressé
        {
            Draw();
        }
        else if (currentDrawing != null)
        {
            // Convertit le LineRenderer en Mesh lorsque le trait est terminé
            ConvertLineRendererToMesh(currentDrawing);
            currentDrawing = null; // Réinitialise le LineRenderer
        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            SwitchColor(); // Change la couleur lorsque le bouton est pressé
        }
    }


    private void Draw()
    {
        if (currentDrawing == null)
        {
            index = 0;
            GameObject drawingObject = new GameObject("Drawing");
            currentDrawing = drawingObject.AddComponent<LineRenderer>(); // Ajoute un LineRenderer
            currentDrawing.material = drawingMaterial; // Set the material for the line renderer
            currentDrawing.startWidth = penWidth; // Set the start width of the line renderer
            currentDrawing.startColor = currentDrawing.endColor = penColors[currentColorIndex]; // Set the color of the line renderer
            currentDrawing.positionCount = 1;  // Initialize the position count to 1
            //Debug.Log($"Drawing at position: {penTip.position}");
            currentDrawing.SetPosition(0, penTip.position); // Set the initial position of the line renderer to the pen tip position

            // Ajoute un BoxCollider pour détecter les interactions avec la gomme
            BoxCollider collider = drawingObject.AddComponent<BoxCollider>();
            collider.isTrigger = true; // Définit le collider comme un trigger

            // Ajuste la taille et la position du collider en fonction du LineRenderer
            Vector3 start = currentDrawing.GetPosition(0);
            Vector3 end = currentDrawing.GetPosition(currentDrawing.positionCount - 1);
            collider.size = new Vector3(penWidth, penWidth, Vector3.Distance(start, end));
            collider.center = (start + end) / 2 - drawingObject.transform.position;
        }
        else
        {
            var currentPosition = currentDrawing.GetPosition(index); // Get the current position of the line renderer
            if (Vector3.Distance(currentPosition, penTip.position) > 0.01f) // Check if the distance between the current position and the pen tip position is greater than 0.01f
            {
                index++; // Increment the index for the line renderer
                currentDrawing.positionCount = index + 1; // Update the position count of the line renderer
                currentDrawing.SetPosition(index, penTip.position); // Set the new position of the line renderer to the pen tip position

                //Debug.Log($"Added point at position: {penTip.position}");
            }
        }


    }

    private void SwitchColor()
    {
        if (currentColorIndex == penColors.Length - 1)
        {
            currentColorIndex = 0; // Reset to the first color if at the last color
        }
        else
        {
            currentColorIndex++; // Increment the color index
        }
        tipMaterial.color = penColors[currentColorIndex]; // Update the color of the pen tip material
    }
    private void ConvertLineRendererToMesh(LineRenderer lineRenderer)
    {
        // Récupère les positions du LineRenderer
        int positionCount = lineRenderer.positionCount;
        Vector3[] positions = new Vector3[positionCount];
        lineRenderer.GetPositions(positions);

        // Crée un nouveau Mesh
        Mesh mesh = new Mesh();

        // Crée les vertices
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float halfWidth = penWidth / 2;

        for (int i = 0; i < positionCount - 1; i++)
        {
            Vector3 start = positions[i];
            Vector3 end = positions[i + 1];

            // Calcule les directions pour les vertices
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up) * halfWidth;

            // Ajoute les vertices pour ce segment
            vertices.Add(start - perpendicular);
            vertices.Add(start + perpendicular);
            vertices.Add(end - perpendicular);
            vertices.Add(end + perpendicular);

            // Ajoute les triangles pour ce segment
            int baseIndex = i * 4;
            triangles.Add(baseIndex);
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 2);

            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 2);
        }

        // Applique les vertices et triangles au Mesh
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        // Ajoute un MeshFilter et un MeshRenderer au GameObject
        GameObject meshObject = new GameObject("MeshDrawing");
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = mesh;
        meshRenderer.material = drawingMaterial; // Utilise le même matériau que le LineRenderer

        // Détruit le LineRenderer après conversion
        Destroy(lineRenderer.gameObject);
    }





}
