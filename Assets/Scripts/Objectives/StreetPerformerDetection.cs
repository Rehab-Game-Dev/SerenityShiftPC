using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Hard-level trigger: when the player finds the street performer (Django), freezes
/// the compass (the follow-up puzzle should be solved by exploring, not by the arrow),
/// shows dialog, fades out his music, and reveals the note-sequence puzzle objects.
/// </summary>
public class StreetPerformerDetection : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private ParticleSystem musicParticles;
    [SerializeField] private GameObject cubeNotes;
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private string newInstructionMessage = "Walk on the colored notes to play the sequence: red, blue, green, purple, green, blue, yellow, red";
    [SerializeField] private float dialogDisplayTime = 10f; // Time before dialogue disappears

    private AudioSource musicAudioSource;
    private bool missionStarted = false;

    private void Start()
    {
        musicAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !missionStarted)
        {
            Debug.Log("Found the street performer");

            missionStarted = true;

            // Player found Django - compass job done, freeze it
            // (the note riddle should be solved by looking around, not by arrow)
            CompassController.missionComplete = true;

            if (dialogPanel != null)
            {
                dialogPanel.SetActive(true);
                StartCoroutine(HideDialogAfterDelay());
            }

            // Update the instruction text
            if (instructionText != null)
            {
                instructionText.text = newInstructionMessage;
            }

            // Fade out the music instead of stopping immediately
            if (musicAudioSource != null)
            {
                StartCoroutine(FadeOutMusic());
            }

            if (musicParticles != null)
            {
                musicParticles.Stop();
            }

            startMissionTwo();
        }
    }

    private IEnumerator HideDialogAfterDelay()
    {
        yield return new WaitForSeconds(dialogDisplayTime);

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = musicAudioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        musicAudioSource.Stop();
        // Restore the configured volume so a future re-trigger doesn't fade in from zero.
        musicAudioSource.volume = startVolume;
    }

    private void startMissionTwo()
    {
        Debug.Log("Mission Two Started");

        if (cubeNotes != null)
        {
            cubeNotes.SetActive(true);
        }
    }
}
