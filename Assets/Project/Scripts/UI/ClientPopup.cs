using UnityEngine;
using TMPro;
using DG.Tweening;  // Animation library
using UnityEngine.UI;

/// <summary>
/// Handles the popup/modal that shows detailed client information.
/// Uses DOTween for smooth fade and scale animations.
/// 
/// Structure in scene:
/// - Popup (this script + CanvasGroup for fading)
///   └── Background (dark overlay)
///   └── ContentBox (white window with actual content)
/// </summary>
public class ClientPopup : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI addressText;
    public TextMeshProUGUI pointsText;
    public Button closeButton;
    
    [Header("Animation References")]
    public CanvasGroup backgroundGroup;  // Controls opacity + interactivity of whole popup
    public Transform contentBox;         // The actual popup window that scales

    private void Awake()
    {
        // Wire up close button automatically
        if (closeButton != null) 
            closeButton.onClick.AddListener(Hide);

        // --- HIDE POPUP ON START ---
        // CanvasGroup lets us fade the whole popup and disable input
        backgroundGroup.alpha = 0;              // Invisible
        backgroundGroup.interactable = false;   // Can't click through
        backgroundGroup.blocksRaycasts = false; // Input passes through
        
        // Scale content to zero (invisible)
        // Safety check: make sure contentBox is actually a child of this popup
        if (contentBox != null && contentBox.IsChildOf(transform))
        {
            contentBox.localScale = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("[ClientPopup] contentBox is not assigned or is not a child of this popup! Please fix the reference in the Inspector.");
        }
    }

    /// <summary>
    /// Shows the popup with the given client data.
    /// Called when a row is clicked.
    /// </summary>
    public void Show(ClientProfile data)
    {
        // 1. Populate text fields with data
        nameText.text = $"Name: {data.name}";
        addressText.text = $"Address: {data.address}";
        pointsText.text = $"Points: {data.points}";

        // 2. Enable input blocking (so clicks don't go through to the list)
        backgroundGroup.interactable = true;
        backgroundGroup.blocksRaycasts = true;

        // 3. Animate in using DOTween
        // DOFade animates the CanvasGroup alpha
        backgroundGroup.DOFade(1f, 0.3f);
        
        // DOScale animates the transform scale
        // Ease.OutBack gives a nice "overshoot and bounce" effect
        contentBox.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// Hides the popup with animation.
    /// Called by close button or background click.
    /// </summary>
    public void Hide()
    {
        // Disable input immediately (don't wait for animation)
        backgroundGroup.interactable = false;
        backgroundGroup.blocksRaycasts = false;

        // Animate out
        backgroundGroup.DOFade(0f, 0.2f);
        
        // Ease.InBack is the reverse of OutBack - shrinks with a "pull in" effect
        contentBox.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
    }
}