using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Shows a short congratulatory message each time a bird is caught (used by the
/// medium-difficulty level). On the final bird, also stops the level timer, appends
/// the player's completion time to the message, and — once the message hides —
/// fades in the "completed mission" prompt, holds it, then fades it back out.
/// </summary>
public class BirdMessageController : MonoBehaviour
{
    public static BirdMessageController Instance;

    public GameObject birdMessage;
    public TextMeshProUGUI birdText;
    public float displayDuration = 3f;

    [Header("Completed Mission Prompt")]
    public CanvasGroup completedMissionPrompt;
    public float promptDisplayDuration = 5f;
    public float promptFadeDuration = 1f;

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

        if (completedMissionPrompt != null)
        {
            completedMissionPrompt.alpha = 0f;
            completedMissionPrompt.gameObject.SetActive(false);
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
        bool missionComplete = false;

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
                missionComplete = true;
            }
        }

        if (birdMessage != null)
        {
            // Cancel any in-progress hide-after-delay so back-to-back catches don't
            // hide the newest message early.
            StopAllCoroutines();
            StartCoroutine(ShowMessageForDuration(missionComplete));
        }
    }

    IEnumerator ShowMessageForDuration(bool missionComplete)
    {
        birdMessage.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        birdMessage.SetActive(false);

        if (missionComplete && completedMissionPrompt != null)
        {
            yield return StartCoroutine(ShowCompletedMissionPrompt());
        }
    }

    IEnumerator ShowCompletedMissionPrompt()
    {
        completedMissionPrompt.gameObject.SetActive(true);
        completedMissionPrompt.alpha = 1f;

        yield return new WaitForSeconds(promptDisplayDuration);

        yield return StartCoroutine(FadeCanvasGroup(completedMissionPrompt, 1f, 0f, promptFadeDuration));

        completedMissionPrompt.gameObject.SetActive(false);
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;
        cg.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        cg.alpha = to;
    }
}