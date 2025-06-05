using System.Collections.Generic;
using UnityEngine;


public class Pencil03 : MonoBehaviour
{
    [Header("Pen Properties")]
    public Transform penTip; // The tip of the pen, where the drawing starts
    public Material drawingMaterial; // Material used for the pen line
    public Material tipMaterial; // Material used for the pen tip
    [Range(0.01f, 0.1f)]
    public float penWidth = 0.05f; // Width of the pen line
    public Color[] penColors; // Color of the pen line

    [Header("Hands & Grabbable")]
    private List<Vector3> positions = new();
    private Mesh currentMesh;
    private GameObject currentMeshObject;
    private int currentColorIndex;

    private void Start()
    {
        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];
    }

    private void Update()
    {
        float rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

        if (rightTriggerValue > 0.1f || leftTriggerValue > 0.1f)
        {
            Draw();
        }
        else if (positions.Count > 1)
        {
            // Finalize the mesh when the drawing stops
            FinalizeMesh();
        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            SwitchColor();
        }
    }

    private void Draw()
    {
        Vector3 currentPosition = penTip.position;

        if (positions.Count == 0 || Vector3.Distance(positions[^1], currentPosition) > 0.01f)
        {
            positions.Add(currentPosition);

            if (currentMesh == null)
            {
                // Create a new mesh and GameObject for the drawing
                currentMesh = new Mesh();
                currentMeshObject = new GameObject("MeshDrawing");
                MeshFilter meshFilter = currentMeshObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = currentMeshObject.AddComponent<MeshRenderer>();

                meshFilter.mesh = currentMesh;                
                meshRenderer.material = new Material(drawingMaterial) { color = penColors[currentColorIndex] };
            }

            UpdateMesh();
        }
    }

    private void UpdateMesh()
    {
        if (positions.Count < 2) return;

        // Récupérer les vertices, triangles et couleurs existants
        List<Vector3> vertices = new List<Vector3>(currentMesh.vertices);
        List<int> triangles = new List<int>(currentMesh.triangles);
        List<Color> colors = new List<Color>(currentMesh.colors);

        float halfWidth = penWidth / 2;

        // Ajouter uniquement les nouvelles faces à partir de la dernière position
        int startIndex = positions.Count - 2; // Commencer à l'avant-dernier point
        Vector3 start = positions[startIndex];
        Vector3 end = positions[startIndex + 1];

        // Calculer la direction et l'orientation basées sur le penTip
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, penTip.up).normalized * halfWidth;

        // Ajouter les nouveaux vertices pour ce segment
        vertices.Add(start - perpendicular);
        vertices.Add(start + perpendicular);
        vertices.Add(end - perpendicular);
        vertices.Add(end + perpendicular);

        // Ajouter les couleurs pour les nouveaux vertices
        colors.Add(penColors[currentColorIndex]);
        colors.Add(penColors[currentColorIndex]);
        colors.Add(penColors[currentColorIndex]);
        colors.Add(penColors[currentColorIndex]);

        // Ajouter les nouveaux triangles pour ce segment
        int baseIndex = vertices.Count - 4; // Les 4 derniers vertices ajoutés
        triangles.Add(baseIndex);
        triangles.Add(baseIndex + 1);
        triangles.Add(baseIndex + 2);

        triangles.Add(baseIndex + 1);
        triangles.Add(baseIndex + 3);
        triangles.Add(baseIndex + 2);

        // Appliquer les vertices, triangles et couleurs au mesh
        currentMesh.Clear();
        currentMesh.SetVertices(vertices);
        currentMesh.SetTriangles(triangles, 0);
        currentMesh.SetColors(colors);
        currentMesh.RecalculateNormals();
    }

    private void FinalizeMesh()
    {
        currentMesh = null;
        currentMeshObject = null;
        positions.Clear();
    }

    private void SwitchColor()
    {
      //  mesh.colors = new Color[mesh.vertexCount];
        if (currentColorIndex == penColors.Length - 1)
        {
            currentColorIndex = 0;
        }
        else
        {
            currentColorIndex++;
        }
        tipMaterial.color = penColors[currentColorIndex];
    }
}
