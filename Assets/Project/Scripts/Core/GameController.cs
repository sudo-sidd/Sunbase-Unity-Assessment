using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main game controller that coordinates all Task 2 components.
/// Single Responsibility: Orchestrates game flow and handles restart.
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private CircleSpawner circleSpawner;
    [SerializeField] private LineDrawer lineDrawer;
    [SerializeField] private HitDetector hitDetector;

    private void Start()
    {
        // Subscribe to line drawing completion
        lineDrawer.OnDrawComplete += OnLineDrawComplete;
        
        // Start the game
        circleSpawner.SpawnCircles();
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (lineDrawer != null)
        {
            lineDrawer.OnDrawComplete -= OnLineDrawComplete;
        }
    }

    /// <summary>
    /// Called when player finishes drawing a line.
    /// </summary>
    private void OnLineDrawComplete()
    {
        hitDetector.CheckHits(lineDrawer.GetEdgeCollider());
    }

    /// <summary>
    /// Restarts the game. Called by UI button.
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
