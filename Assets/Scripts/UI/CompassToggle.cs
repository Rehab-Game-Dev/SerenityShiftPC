using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Settings-menu toggle for showing/hiding the compass. Persists the choice to
/// <see cref="PlayerPrefs"/> and applies it immediately via <see cref="CompassController"/>.
/// </summary>
public class CompassToggle : MonoBehaviour
{
    public GameObject compassObject;

    void OnEnable()
    {
        bool saved = PlayerPrefs.GetInt("CompassToggle", 1) == 1;
        GetComponent<Toggle>().isOn = saved;
        GetComponent<Toggle>().onValueChanged.AddListener(OnToggleChanged);
        ApplyVisibility();
    }

    void OnDisable()
    {
        GetComponent<Toggle>().onValueChanged.RemoveListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("CompassToggle", isOn ? 1 : 0);
        PlayerPrefs.Save();
        ApplyVisibility();
    }

    void ApplyVisibility()
    {
        if (compassObject != null)
        {
            CompassController compass = compassObject.GetComponent<CompassController>();
            if (compass != null)
            {
                compass.UpdateVisibility();
            }
        }
    }
}
