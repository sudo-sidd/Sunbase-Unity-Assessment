using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;  // Needed for Action<T> delegate type

/// <summary>
/// Represents a single row/item in the client list.
/// This is a reusable prefab - we create copies of it for each client.
/// 
/// [RequireComponent] ensures LayoutElement is always attached.
/// LayoutElement is needed so ScrollView knows how big each row should be.
/// </summary>
[RequireComponent(typeof(LayoutElement))]
public class ClientRow : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI labelText;   // Shows client label
    public TextMeshProUGUI pointsText;  // Shows points
    public Button rowButton;            // The clickable area

    [Header("Layout Settings")]
    [SerializeField] private float preferredHeight = 60f;

    // Private fields - only this class uses these
    private ClientProfile _data;                    // The data for THIS row
    private Action<ClientProfile> _onClickAction;   // Callback when clicked
    private LayoutElement _layoutElement;

    private void Awake()
    {
        // Get or add the LayoutElement component
        // This tells the ScrollView's layout system how tall this row should be
        _layoutElement = GetComponent<LayoutElement>();
        if (_layoutElement == null)
        {
            _layoutElement = gameObject.AddComponent<LayoutElement>();
        }
        _layoutElement.preferredHeight = preferredHeight;
        _layoutElement.minHeight = preferredHeight;
    }

    /// <summary>
    /// Called by ClientListManager to configure this row with data.
    /// 
    /// WHY pass a callback? So the row doesn't need to know about the popup.
    /// The Manager decides what happens on click - this is good separation!
    /// 
    /// Action<ClientProfile> is a delegate type - it's like a function pointer.
    /// It means "a function that takes ClientProfile and returns nothing"
    /// </summary>
    public void Setup(ClientProfile data, Action<ClientProfile> onClick)
    {
        _data = data;
        _onClickAction = onClick;

        // Populate UI text
        labelText.text = _data.label; 
        pointsText.text = _data.points.ToString();

        // Setup button click
        // RemoveAllListeners prevents duplicate callbacks if Setup() is called again
        rowButton.onClick.RemoveAllListeners();
        
        // Lambda expression: () => { ... } creates an inline function
        // When clicked, it calls the callback with our data
        rowButton.onClick.AddListener(() => _onClickAction?.Invoke(_data));
    }
}