using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Shows a short congratulatory message each time a bird is caught (used by the
/// medium-difficulty level). On the final bird, also stops the level timer and
/// appends the player's completion time to the message.
/// </summary>
public class BirdMessageController : MonoBehaviour
{
    public static BirdMessageController Instance;

    public GameObject birdMessage;
    public TextMeshProUGUI birdText;
    public float displayDuration = 3f;

    private int birdsCaught = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (birdMessage != null)
        {
            birdMessage.SetActive(false);
        }
    }

    /// <summary>Called by <see cref="BirdCatchable"/> each time a bird is caught.</summary>
    public void OnBirdCaught()
    {
        birdsCaught++;
        ShowMessage();
    }

    void ShowMessage()
    {
        if (birdText != null)
        {
            // Same messages as NPCs
            if (birdsCaught == 1)
            {
                birdText.text = "Great, 2 more to go!";
            }
            else if (birdsCaught == 2)
            {
                birdText.text = "Nice, 1 more!";
            }
            else if (birdsCaught >= 3)
            {
                TimerManager timer = FindFirstObjectByType<TimerManager>();
                if (timer != null) timer.StopTimer();
                string timeString = timer != null ? timer.GetFormattedTime() : "";
                birdText.text = "Excellent, you caught all of them!\n" + timeString;
            }
        }

        if (birdMessage != null)
        {
            // Cancel any in-progress hide-after-delay so back-to-back catches don't
            // hide the newest message early.
            StopAllCoroutines();
            StartCoroutine(ShowMessageForDuration());
        }
    }

    IEnumerator ShowMessageForDuration()
    {
        birdMessage.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        birdMessage.SetActive(false);
    }
}
