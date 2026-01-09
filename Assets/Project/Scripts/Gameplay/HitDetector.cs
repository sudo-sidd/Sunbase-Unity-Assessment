using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Detects and handles hits between the line and target circles.
/// Single Responsibility: Only handles collision detection and target destruction.
/// </summary>
public class HitDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string targetTag = "Target";
    [SerializeField] private float destroyAnimDuration = 0.2f;

    /// <summary>
    /// Checks for hits using the provided edge collider and destroys hit targets.
    /// </summary>
    public void CheckHits(EdgeCollider2D edgeCollider)
    {
        // Force Unity to update collider shape immediately
        Physics2D.SyncTransforms();

        List<Collider2D> hitColliders = new List<Collider2D>();
        edgeCollider.GetContacts(hitColliders);

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag(targetTag))
            {
                DestroyTarget(hit);
            }
        }
    }

    private void DestroyTarget(Collider2D target)
    {
        // Disable collider to prevent double hits
        target.enabled = false;
        
        // Animate shrink then destroy
        target.transform.DOScale(Vector3.zero, destroyAnimDuration)
            .OnComplete(() => Destroy(target.gameObject));
    }
}
