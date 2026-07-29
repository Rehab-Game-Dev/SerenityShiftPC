using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Global pause toggle (Tab key): freezes time, stops all NavMeshAgents, pauses every
/// currently-playing AudioSource (remembering which ones so only those resume), and
/// shows the pause overlay UI.
/// </summary>
public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;
    private List<AudioSource> pausedAudioSources = new List<AudioSource>();
    public GameObject pauseOverlay;

    void Start()
    {
        if (pauseOverlay == null)
            pauseOverlay = GameObject.Find("PauseOverlay");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    /// <summary>Freezes gameplay: stops time, NPC/car navigation, and audio, then shows the pause overlay.</summary>
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseOverlay.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (NavMeshAgent agent in FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None))
        {
            agent.isStopped = true;
        }

        // Only remember sources that were actually playing, so Resume() doesn't
        // start audio that was already silent before the pause.
        pausedAudioSources.Clear();
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audio in allAudioSources)
        {
            if (audio.isPlaying)
            {
                audio.Pause();
                pausedAudioSources.Add(audio);
            }
        }
    }

    /// <summary>Reverses <see cref="Pause"/>: resumes time, NPC/car navigation, and the audio sources that were playing before the pause.</summary>
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseOverlay.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (NavMeshAgent agent in FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None))
        {
            agent.isStopped = false;
        }
        foreach (AudioSource audio in pausedAudioSources)
        {
            if (audio != null)
                audio.UnPause();
        }
        pausedAudioSources.Clear();
    }
}
