using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class RoundedFrame : MaskableGraphic
{
    [Header("Dimensions du cadre")]
    [SerializeField] private float frameWidth = 300f;
    [SerializeField] private float frameHeight = 200f;
    [SerializeField] private float borderThickness = 10f;

    [Header("Arrondis")]
    [SerializeField][Range(0f, 300f)] private float cornerRadius = 20f;
    [SerializeField][Range(4, 32)] private int cornerSegments = 8;

    [Header("Couleurs")]
    public Color borderColor = Color.white;
    [SerializeField] private Color fillColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    [SerializeField] private bool fillFrame = true;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float halfWidth = frameWidth * 0.5f;
        float halfHeight = frameHeight * 0.5f;
        float radius = Mathf.Min(cornerRadius, Mathf.Min(halfWidth, halfHeight));

        // Dessiner le remplissage si activé
        if (fillFrame)
        {
            DrawRoundedRect(vh, -halfWidth, -halfHeight, frameWidth, frameHeight, radius, fillColor);
        }

        // Dessiner la bordure
        DrawRoundedBorder(vh, -halfWidth, -halfHeight, frameWidth, frameHeight, radius, borderThickness, borderColor);
    }

    private void DrawRoundedRect(VertexHelper vh, float x, float y, float width, float height, float radius, Color col)
    {
        int startVertex = vh.currentVertCount;

        // Coins (centres des arcs)
        Vector2[] corners = new Vector2[]
        {
            new Vector2(x + width - radius, y + height - radius), // Coin haut-droite
            new Vector2(x + radius, y + height - radius),          // Coin haut-gauche
            new Vector2(x + radius, y + radius),                   // Coin bas-gauche
            new Vector2(x + width - radius, y + radius)            // Coin bas-droite
        };

        float[] startAngles = { 0f, 90f, 180f, 270f };

        // Générer tous les points du contour
        for (int corner = 0; corner < 4; corner++)
        {
            float angleStart = startAngles[corner] * Mathf.Deg2Rad;
            float angleStep = (90f * Mathf.Deg2Rad) / cornerSegments;

            for (int i = 0; i <= cornerSegments; i++)
            {
                float angle = angleStart + angleStep * i;
                float vx = corners[corner].x + Mathf.Cos(angle) * radius;
                float vy = corners[corner].y + Mathf.Sin(angle) * radius;

                vh.AddVert(new Vector3(vx, vy, 0), col, Vector2.zero);
            }
        }

        // Centre pour les triangles en éventail
        vh.AddVert(new Vector3(x + width * 0.5f, y + height * 0.5f, 0), col, Vector2.zero);
        int centerIndex = vh.currentVertCount - 1;

        // Créer les triangles en éventail depuis le centre
        int totalVertices = centerIndex - startVertex;
        for (int i = 0; i < totalVertices; i++)
        {
            int next = (i + 1) % totalVertices;
            vh.AddTriangle(centerIndex, startVertex + i, startVertex + next);
        }
    }

    private void DrawRoundedBorder(VertexHelper vh, float x, float y, float width, float height, float radius, float thickness, Color col)
    {
        int startVertex = vh.currentVertCount;

        // Coins pour les arcs
        Vector2[] corners = new Vector2[]
        {
            new Vector2(x + width - radius, y + height - radius), // Coin haut-droite
            new Vector2(x + radius, y + height - radius),          // Coin haut-gauche
            new Vector2(x + radius, y + radius),                   // Coin bas-gauche
            new Vector2(x + width - radius, y + radius)            // Coin bas-droite
        };

        float innerRadius = Mathf.Max(0, radius - thickness);
        float[] startAngles = { 0f, 90f, 180f, 270f };

        // Générer les vertices pour chaque coin
        for (int corner = 0; corner < 4; corner++)
        {
            float angleStart = startAngles[corner] * Mathf.Deg2Rad;
            float angleStep = (90f * Mathf.Deg2Rad) / cornerSegments;

            for (int i = 0; i <= cornerSegments; i++)
            {
                float angle = angleStart + angleStep * i;

                // Vertex extérieur
                float outerX = corners[corner].x + Mathf.Cos(angle) * radius;
                float outerY = corners[corner].y + Mathf.Sin(angle) * radius;
                vh.AddVert(new Vector3(outerX, outerY, 0), col, Vector2.zero);

                // Vertex intérieur
                float innerX = corners[corner].x + Mathf.Cos(angle) * innerRadius;
                float innerY = corners[corner].y + Mathf.Sin(angle) * innerRadius;
                vh.AddVert(new Vector3(innerX, innerY, 0), col, Vector2.zero);
            }
        }

        // Créer les triangles pour la bordure (quad strip)
        int totalSegments = (cornerSegments + 1) * 4;
        for (int i = 0; i < totalSegments; i++)
        {
            int current = i * 2;
            int next = ((i + 1) % totalSegments) * 2;

            // Triangle 1
            vh.AddTriangle(
                startVertex + current,      // Outer current
                startVertex + current + 1,  // Inner current
                startVertex + next          // Outer next
            );

            // Triangle 2
            vh.AddTriangle(
                startVertex + next,         // Outer next
                startVertex + current + 1,  // Inner current
                startVertex + next + 1      // Inner next
            );
        }
    }

    // Méthodes pour modifier les paramètres via code
    public void SetCornerRadius(float radius)
    {
        cornerRadius = Mathf.Clamp(radius, 0f, 100f);
        SetVerticesDirty();
    }

    public void SetBorderThickness(float thickness)
    {
        borderThickness = thickness;
        SetVerticesDirty();
    }

    public void SetFrameSize(float width, float height)
    {
        frameWidth = width;
        frameHeight = height;
        SetVerticesDirty();
    }

    public void SetBorderColor(Color col)
    {
        borderColor = col;
        SetVerticesDirty();
    }

    public void SetFillColor(Color col)
    {
        fillColor = col;
        SetVerticesDirty();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();
    }
#endif
}