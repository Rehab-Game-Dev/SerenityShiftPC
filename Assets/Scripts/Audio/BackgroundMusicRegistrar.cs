using UnityEngine;

/// <summary>
/// Attach to the background-music GameObject in each scene. On start, hands this
/// scene's <see cref="AudioSource"/> to the persistent <see cref="AudioManager"/> singleton
/// so mute/unmute state carries over across scene loads.
/// </summary>
public class BackgroundMusicRegistrar : MonoBehaviour
{
    void Start()
    {
        AudioSource musicSource = GetComponent<AudioSource>();

        if (musicSource != null && AudioManager.Instance != null)
        {
            // Register this music source with the AudioManager
            AudioManager.Instance.RegisterBackgroundMusic(musicSource);
        }
    }
}
