using UnityEngine;

/// <summary>
/// Scene-persistent singleton that owns the mute/unmute state for background music.
/// Survives scene loads so the player's mute preference stays applied as new
/// music sources register themselves via <see cref="RegisterBackgroundMusic"/>.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Volume State")]
    public bool isMusicMuted = false;

    [Header("Background Music")]
    public AudioSource backgroundMusicSource;

    void Awake()
    {
        // Singleton pattern - keep only one instance across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Apply saved music state
            UpdateMusicState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Find background music if not assigned
        if (backgroundMusicSource == null)
        {
            GameObject musicObj = GameObject.Find("CityAudioSource"); // Change this to your music object name
            if (musicObj != null)
            {
                backgroundMusicSource = musicObj.GetComponent<AudioSource>();
            }
        }

        UpdateMusicState();
    }

    /// <summary>Flips the mute state and immediately applies it to the current music source.</summary>
    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        UpdateMusicState();
    }

    /// <summary>Explicitly sets the mute state (used by UI toggles that show an on/off state rather than a button).</summary>
    public void SetMusicMute(bool mute)
    {
        isMusicMuted = mute;
        UpdateMusicState();
    }

    private void UpdateMusicState()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.mute = isMusicMuted;
        }
    }

    /// <summary>
    /// Called by <see cref="BackgroundMusicRegistrar"/> when a new scene loads, so the
    /// singleton's mute state applies to that scene's music source instead of the old one.
    /// </summary>
    public void RegisterBackgroundMusic(AudioSource musicSource)
    {
        backgroundMusicSource = musicSource;
        UpdateMusicState();
    }
}
