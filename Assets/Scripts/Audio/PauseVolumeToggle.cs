using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI toggle for the pause menu that mirrors and controls <see cref="AudioManager"/>'s
/// mute state. Note the toggle reads as "on = sound enabled", the inverse of
/// <c>isMusicMuted</c>, so the initial state and change handler both flip the value.
/// </summary>
public class PauseVolumeToggle : MonoBehaviour
{
    private Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();

        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance is NULL!");
            return;
        }

        toggle.isOn = !AudioManager.Instance.isMusicMuted;
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        if (AudioManager.Instance != null)
        {
            // isOn == isMusicMuted means the toggle's new state disagrees with the
            // current mute state, so flip it. (isOn=true/unmuted vs isMusicMuted=true
            // would mean "muted" is stale.)
            if (isOn == AudioManager.Instance.isMusicMuted)
                AudioManager.Instance.ToggleMusic();
        }
    }
}
