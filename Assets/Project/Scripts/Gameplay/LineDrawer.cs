using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles line drawing input and rendering.
/// Single Responsibility: Only handles drawing the line.
/// </summary>
public class LineDrawer : MonoBehaviour
{
    [Header("Line Components")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private EdgeCollider2D edgeCollider;
    
    [Header("Settings")]
    [SerializeField] private float minPointDistance = 0.2f;

    // Event fired when drawing completes (mouse up)
    public event Action OnDrawComplete;

    private List<Vector2> linePoints = new List<Vector2>();
    private bool isDrawing = false;
    private Vector2 lastPoint;

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Start drawing
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }

        // Continue drawing
        if (Input.GetMouseButton(0) && isDrawing)
        {
            ContinueDrawing();
        }

        // Stop drawing
        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            StopDrawing();
        }
    }

    private void StartDrawing()
    {
        isDrawing = true;
        linePoints.Clear();
        lastPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        UpdateLineVisuals();
    }

    private void ContinueDrawing()
    {
        Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector2 smoothedPos = Vector2.Lerp(lastPoint, currentMousePos, 0.5f);

        if (Vector2.Distance(lastPoint, smoothedPos) > 0.05f)
        {
            linePoints.Add(smoothedPos);
            lastPoint = smoothedPos;
            UpdateLineVisuals();
        }
    }

    private void StopDrawing()
    {
        isDrawing = false;
        OnDrawComplete?.Invoke();
    }

    private void UpdateLineVisuals()
    {
        // Update LineRenderer
        lineRenderer.positionCount = linePoints.Count;
        Vector3[] points3D = new Vector3[linePoints.Count];
        for (int i = 0; i < linePoints.Count; i++)
        {
            points3D[i] = linePoints[i];
        }
        lineRenderer.SetPositions(points3D);

        // Update EdgeCollider for physics
        if (linePoints.Count >= 2)
        {
            edgeCollider.SetPoints(linePoints);
        }
    }

    /// <summary>
    /// Returns the edge collider for hit detection.
    /// </summary>
    public EdgeCollider2D GetEdgeCollider()
    {
        return edgeCollider;
    }
}
