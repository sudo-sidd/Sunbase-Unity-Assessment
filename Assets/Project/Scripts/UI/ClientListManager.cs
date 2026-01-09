using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;  // Needed for .Where() and .ToList() filtering
using DG.Tweening; // DOTween animation library

/// <summary>
/// Main controller for the client list screen.
/// Manages fetching data, filtering, and spawning list items.
/// 
/// This is like the "brain" of the list - it coordinates everything.
/// </summary>
public class ClientListManager : MonoBehaviour
{
    [Header("Core Dependencies")]
    public ClientService dataService;   // The service that fetches data from API
    public ClientPopup popupWindow;     // The popup we show when a row is clicked

    [Header("UI List References")]
    public Transform listContentContainer;  // The "Content" child of ScrollView
    public ClientRow rowPrefab;             // Template for each list item
    public TMP_Dropdown filterDropdown;     // All/Managers/Non-Managers filter

    // Private field to store the full unfiltered list
    // We keep this so we can filter without re-fetching from API
    private List<ClientProfile> _fullClientList = new List<ClientProfile>();

    private void Start()
    {
        // Subscribe to dropdown changes - when user selects a filter option
        filterDropdown.onValueChanged.AddListener(OnFilterChanged);
        
        // Kick off the API request as soon as the scene starts
        dataService.FetchClients(OnDataSuccess, OnDataFail);
    }

    /// <summary>
    /// Callback when API returns data successfully
    /// </summary>
    private void OnDataSuccess(List<ClientProfile> clients)
    {
        _fullClientList = clients;       // Store for later filtering
        RefreshDisplay(_fullClientList); // Show all initially
    }

    /// <summary>
    /// Callback when API fails
    /// </summary>
    private void OnDataFail(string error)
    {
        Debug.LogError($"Failed to load data: {error}");
        // TODO: Show error message to user in the UI
    }

    /// <summary>
    /// Called when dropdown selection changes.
    /// mode 0 = All, mode 1 = Managers, mode 2 = Non-Managers
    /// </summary>
    public void OnFilterChanged(int mode)
    {
        List<ClientProfile> filtered = new List<ClientProfile>();

        switch (mode)
        {
            case 0: 
                filtered = _fullClientList; 
                break;
            case 1: 
                // LINQ Where() filters items that match the condition
                filtered = _fullClientList.Where(x => x.isManager).ToList(); 
                break;
            case 2: 
                filtered = _fullClientList.Where(x => !x.isManager).ToList(); 
                break;
        }

        RefreshDisplay(filtered);
    }

    /// <summary>
    /// Clears old list items and spawns new ones with animations.
    /// This is called whenever we need to update what's displayed.
    /// </summary>
    private void RefreshDisplay(List<ClientProfile> listToShow)
    {
        // Kill any running DOTween animations on this container
        // This prevents visual glitches if we refresh mid-animation
        DOTween.Kill(listContentContainer);

        // Destroy all old row items
        foreach (Transform child in listContentContainer)
        {
            Destroy(child.gameObject);
        }

        // Spawn new items with staggered animations
        for (int i = 0; i < listToShow.Count; i++)
        {
            var profile = listToShow[i];
            
            // Instantiate = create a copy of the prefab
            ClientRow newRow = Instantiate(rowPrefab, listContentContainer);

            // --- ANIMATION SETUP ---
            // Start invisible (scale 0) with slight random rotation
            newRow.transform.localScale = Vector3.zero;
            float randomRotation = Random.Range(-3f, 3f);
            newRow.transform.localRotation = Quaternion.Euler(0, 0, randomRotation);

            // Setup the row's data and click handler
            // We pass a lambda (inline function) that opens the popup
            newRow.Setup(profile, (clickedProfile) => {
                popupWindow.Show(clickedProfile); 
            });

            // --- ANIMATION PLAYBACK ---
            // Each row animates in with a delay based on its index (cascade effect)
            float delay = i * 0.08f;
            
            // DOTween Sequence lets us chain multiple animations together
            Sequence popSequence = DOTween.Sequence();
            popSequence.SetDelay(delay);
            
            // Scale up with overshoot (goes to 1.15 then settles to 1.0)
            popSequence.Append(newRow.transform.DOScale(1.15f, 0.2f).SetEase(Ease.OutQuad));
            popSequence.Append(newRow.transform.DOScale(1f, 0.15f).SetEase(Ease.InOutQuad));
            
            // Rotation straightens out at the same time as the second scale
            // Join() runs this animation parallel to the previous one
            popSequence.Join(newRow.transform.DOLocalRotate(Vector3.zero, 0.15f).SetEase(Ease.OutQuad));
        }
    }
}