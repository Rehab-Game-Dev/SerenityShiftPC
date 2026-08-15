using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Tracks how many birds have been caught in the current level and keeps the
/// on-screen counter in sync. One instance per level scene (not persistent).
/// </summary>
public class BirdGameManager : MonoBehaviour
{
    public static BirdGameManager Instance;

    public int caughtCount = 0;
    public int totalBirds = 3;

    [Header("UI Reference")]
    public TextMeshProUGUI birdCounterText; // Reference to your bird counter text

    [Header("UI To Hide On Completion")]
    public GameObject instructionPanel;
    public GameObject counterPanel;
    public GameObject counterIcon;

    [Header("Timing")]
    [Tooltip("Delay before hiding the instruction/counter UI, so the completion toast has time to be read first.")]
    public float completionDelay = 3f;

    private void Awake()
    {
        Instance = this;
        caughtCount = 0;
        UpdateCounterUI();
    }

    /// <summary>Called by <see cref="BirdCatchable"/> each time a bird is caught.</summary>
    public void BirdCaught()
    {
        caughtCount++;
        Debug.Log("caught bird " + caughtCount + "/" + totalBirds);
        UpdateCounterUI();

        if (caughtCount >= totalBirds)
        {
            Debug.Log("All birds caught! You win!");
            StartCoroutine(HideCompletionUIAfterDelay());
        }
    }

    // Waits for the completion toast to be read before hiding the instruction/counter
    // UI, instead of cutting the toast off immediately.
    private IEnumerator HideCompletionUIAfterDelay()
    {
        yield return new WaitForSeconds(completionDelay);

        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (counterPanel != null) counterPanel.SetActive(false);
        if (counterIcon != null) counterIcon.SetActive(false);
    }

    private void UpdateCounterUI()
    {
        if (birdCounterText != null)
        {
            birdCounterText.text = caughtCount + " / " + totalBirds;
        }
        else
        {
            Debug.LogWarning("Bird Counter Text is not assigned in BirdGameManager!");
        }
    }
}
