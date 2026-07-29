using UnityEngine;

/// <summary>
/// Attached to a catchable bird instance. Handles the one-time "caught" transition:
/// plays a sound, reports the catch to whichever level-progress controller is present,
/// and removes the bird.
/// </summary>
public class BirdCatchable : MonoBehaviour
{
    public bool hasBeenCaught = false;

    [Header("Sound Effect")]
    public AudioClip catchSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    /// <summary>
    /// Marks the bird as caught, notifies the active level controller, and destroys it.
    /// Safe to call multiple times - only the first call has any effect.
    /// </summary>
    public void CatchBird()
    {
        if (hasBeenCaught) return;

        hasBeenCaught = true;

        // Play catch sound at player position
        if (catchSound != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                AudioSource.PlayClipAtPoint(catchSound, player.transform.position);
            }
        }

        // Different levels use different message controllers, so try each in turn.
        // Try GoalMessageController first (for demo/hybrid levels)
        if (GoalMessageController.Instance != null)
        {
            GoalMessageController.Instance.OnBirdCaught();
        }
        // Fall back to BirdMessageController (for medium level)
        else if (BirdMessageController.Instance != null)
        {
            BirdMessageController.Instance.OnBirdCaught();
        }

        if (BirdGameManager.Instance != null)
        {
            BirdGameManager.Instance.BirdCaught();
        }
        else
        {
            Debug.LogError("BirdGameManager.Instance is NULL!");
        }

        Destroy(gameObject);
    }
}
