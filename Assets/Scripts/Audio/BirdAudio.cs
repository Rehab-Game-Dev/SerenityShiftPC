using UnityEngine;
using System.Collections;

/// <summary>
/// Plays a random ambient bird song in 3D space on loop. Waits a frame before starting
/// so the <see cref="AudioSource"/> component (added/configured elsewhere) is ready.
/// </summary>
public class BirdAudio : MonoBehaviour
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
        // Short delay so other components (e.g. pooling/spawning) finish setting this
        // object up before we start reading its AudioSource.
        yield return new WaitForSeconds(0.1f);

        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.loop = true;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        PlaySong();
    }

    void PlaySong()
    {
        if (audioSource == null) return;
        audioSource.clip = Random.value < .5f ? song1 : song2;
        audioSource.Play();
    }
}
