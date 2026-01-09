using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // Requires DOTween imported

public class LineGameManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject circlePrefab;
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;
    
    [Header("Game Config")]
    public int minCircles = 5;
    public int maxCircles = 10;

    // State
    private List<Vector2> linePoints = new List<Vector2>();
    private bool isDrawing = false;

    void Start()
    {
        SpawnCircles();
    }

    void Update()
    {
        HandleInput();
    }

    void SpawnCircles()
    {
        int count = Random.Range(minCircles, maxCircles + 1);
        
        // Boundaries for Camera Size 5
        float xRange = 8f; 
        float yRange = 4f;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-xRange, xRange),
                Random.Range(-yRange, yRange),
                0
            );

            GameObject c = Instantiate(circlePrefab, randomPos, Quaternion.identity);
            
            // Animation: Pop in from zero scale
            c.transform.localScale = Vector3.zero;
            c.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack);
        }
    }

    void HandleInput()
    {
        // 1. START DRAWING
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            linePoints.Clear();
            UpdateLine();
        }

        // 2. CONTINUE DRAWING
        if (Input.GetMouseButton(0) && isDrawing)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Optimization: Only add point if mouse moved enough
            if (linePoints.Count == 0 || Vector2.Distance(linePoints[linePoints.Count - 1], mousePos) > 0.2f)
            {
                linePoints.Add(mousePos);
                UpdateLine();
            }
        }

        // 3. STOP DRAWING
        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            CheckHits();
        }
    }

    void UpdateLine()
    {
        // Visuals
        lineRenderer.positionCount = linePoints.Count;
        Vector3[] points3D = new Vector3[linePoints.Count];
        for(int i=0; i<linePoints.Count; i++) points3D[i] = linePoints[i];
        lineRenderer.SetPositions(points3D);

        // Physics
        edgeCollider.SetPoints(linePoints);
    }

    void CheckHits()
    {
        // <--- ADD THIS LINE! Forces Unity to update the collider shape NOW.
        Physics2D.SyncTransforms(); 

        List<Collider2D> hitColliders = new List<Collider2D>();
        edgeCollider.GetContacts(hitColliders);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Target"))
            {
                hit.enabled = false;
                hit.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => {
                    Destroy(hit.gameObject);
                });
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}