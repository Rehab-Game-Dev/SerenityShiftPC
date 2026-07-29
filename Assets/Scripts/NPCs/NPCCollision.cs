using UnityEngine;

/// <summary>
/// Attached to a catchable NPC. On player contact, plays a sound, reports the catch
/// to <see cref="GoalMessageController"/> and <see cref="GameManager"/>, then removes
/// the NPC. Mirrors <see cref="BirdCatchable"/> but for the NPC-catching phase of a level.
/// </summary>
public class NPCCollision : MonoBehaviour
{
    public bool isCatchable = false;
    private bool hasBeenCaught = false;

    [Header("Sound Effect")]
    public AudioClip catchSound;
    private AudioSource audioSource;

    private void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure AudioSource for sound effects
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if already caught
        if (hasBeenCaught) return;

        if (other.CompareTag("Player") && isCatchable)
        {
            hasBeenCaught = true;

            // Play catch sound
            if (catchSound != null)
            {
                AudioSource.PlayClipAtPoint(catchSound, transform.position);
            }

            // Show the goal message for 3 seconds
            if (GoalMessageController.Instance != null)
            {
                GoalMessageController.Instance.OnNPCCaught();
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NPCCaught();
            }
            else
            {
                Debug.LogError("GameManager.Instance is NULL!");
            }

            Destroy(gameObject);
        }
    }
}
