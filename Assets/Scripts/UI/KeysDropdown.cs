using UnityEngine;
using UnityEngine.UI;
using TMPro;  // Add this for TextMeshPro

/// <summary>
/// A simple collapsible "Keys" help dropdown: one button toggles two panels and
/// swaps an arrow glyph in the label to show open/closed state.
/// </summary>
public class KeysDropdown : MonoBehaviour
{
    [Header("Panels to Toggle")]
    public GameObject keysInstructionPanel;
    public GameObject keysPanel2;

    [Header("Button")]
    public Button keysButton;

    [Header("Text to Change")]
    public TextMeshProUGUI keysText;  // For TextMeshPro
    // OR use this if you're using regular Text:
    // public Text keysText;

    private bool isOpen = false;

    void Start()
    {
        // Start with panels HIDDEN
        isOpen = false;

        if (keysInstructionPanel != null)
            keysInstructionPanel.SetActive(false);
        if (keysPanel2 != null)
            keysPanel2.SetActive(false);

        // Set initial text to show closed state
        if (keysText != null)
            keysText.text = "Keys ▼";

        // Add click listener to button
        if (keysButton != null)
            keysButton.onClick.AddListener(ToggleDropdown);
    }

    /// <summary>Flips the dropdown between open/closed, syncing both panels and the label arrow.</summary>
    public void ToggleDropdown()
    {
        isOpen = !isOpen;

        // Toggle panel visibility
        if (keysInstructionPanel != null)
            keysInstructionPanel.SetActive(isOpen);
        if (keysPanel2 != null)
            keysPanel2.SetActive(isOpen);

        // Change text based on state
        if (keysText != null)
            keysText.text = isOpen ? "Keys ▲" : "Keys ▼";
    }
}
