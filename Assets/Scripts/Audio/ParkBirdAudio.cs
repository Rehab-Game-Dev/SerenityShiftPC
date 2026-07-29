using UnityEngine;
using System.Collections;

/// <summary>
/// Ambient bird song for the park area: unlike <see cref="BirdAudio"/>, this plays one
/// clip at a time with a randomized pause in between, rather than looping continuously,
/// so nearby birds don't all sing in a constant overlapping drone.
/// </summary>
public class ParkBirdAudio : MonoBehaviour
{
    public AudioClip song1;
    public AudioClip song2;

    private AudioSource audioSource;

    void OnEnable()
    {
        StartCoroutine(InitAudio());
    }

    IEnumerator InitAudio()
    {
        // Randomized startup delay so nearby birds don't all begin in sync.
        yield return new WaitForSeconds(0.1f + Random.Range(0f, 8f));

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.loop = false; // No loop - we handle it manually
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = 20f;

        StartCoroutine(PlayWithPauses());
    }

    IEnumerator PlayWithPauses()
    {
        while (true)
        {
            PlaySong();
            // Wait for clip to finish + random pause of 2-5 seconds
            yield return new WaitForSeconds(audioSource.clip.length + Random.Range(2f, 5f));
        }
    }

    void PlaySong()
    {
        if (audioSource == null) return;
        audioSource.clip = Random.value < .5f ? song1 : song2;
        audioSource.Play();
    }
}
