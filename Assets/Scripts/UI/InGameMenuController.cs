using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Toggles the in-game pause/options menu when the bound input action fires, and
/// repositions the menu in front of the player each time it opens.
/// </summary>
public class InGameMenuController : MonoBehaviour
{
    public GameObject menuObject;
    public InputActionReference menuButtonAction; // reference to the controller button

    void OnEnable()
    {
        // Register for the button press event
        menuButtonAction.action.performed += ToggleMenu;
    }

    void OnDisable()
    {
        // Unregister to prevent errors
        menuButtonAction.action.performed -= ToggleMenu;
    }

    void ToggleMenu(InputAction.CallbackContext context)
    {
        // Toggle the display state (if on - turn off, if off - turn on)
        bool isActive = !menuObject.activeSelf;
        menuObject.SetActive(isActive);

        // Bonus: position the menu in front of the player when it opens
        if (isActive)
        {
            menuObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
            menuObject.transform.LookAt(Camera.main.transform);
            // LookAt orients -Z toward the camera; flip so the menu's front (+Z) faces the player instead.
            menuObject.transform.Rotate(0, 180, 0);
        }
    }
}
