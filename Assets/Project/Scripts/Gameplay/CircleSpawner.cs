using UnityEngine;
using DG.Tweening;

/// <summary>
/// Responsible for spawning target circles at random positions.
/// Single Responsibility: Only handles circle spawning logic.
/// </summary>
public class CircleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private int minCircles = 5;
    [SerializeField] private int maxCircles = 10;
    
    [Header("Spawn Area")]
    [SerializeField] private float xRange = 8f;
    [SerializeField] private float yRange = 4f;

    /// <summary>
    /// Spawns random number of circles with pop-in animation.
    /// </summary>
    public void SpawnCircles()
    {
        int count = Random.Range(minCircles, maxCircles + 1);

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-xRange, xRange),
                Random.Range(-yRange, yRange),
                0
            );

            GameObject circle = Instantiate(circlePrefab, randomPos, Quaternion.identity);
            
            // Animate: Pop in from zero scale with overshoot
            circle.transform.localScale = Vector3.zero;
            circle.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack);
        }
    }
}
