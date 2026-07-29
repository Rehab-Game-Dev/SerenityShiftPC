using UnityEngine;

/// <summary>
/// Trigger volume representing a single musical note in a "play the sequence" puzzle.
/// Plays its sound when the player steps on it and reports itself to the scene's
/// <see cref="SequenceChecker"/>, which validates note order.
/// </summary>
public class NoteDetector : MonoBehaviour
{
    public string noteName;
    public AudioClip noteSound;

    private AudioSource audioSource;
    private SequenceChecker sequenceChecker;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Find the SequenceChecker in the scene
        sequenceChecker = FindObjectOfType<SequenceChecker>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(noteName + " pressed");

            // Play the sound
            if (noteSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(noteSound);
            }

            // Register the note with the sequence checker
            if (sequenceChecker != null)
            {
                sequenceChecker.NotePressed(noteName);
            }
        }
    }
}
