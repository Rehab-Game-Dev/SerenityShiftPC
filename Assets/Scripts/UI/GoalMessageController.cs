using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Shared "N more to go" style progress message controller used by hybrid levels that
/// track both NPC and bird catches (unlike <see cref="BirdMessageController"/>, which
/// only handles birds). Each catch type keeps its own count and message text. Once the
/// final catch is made and the progress toast hides, the "completed mission" prompt
/// fades in, stays for a few seconds, then fades back out.
/// </summary>
public class GoalMessageController : MonoBehaviour
{
    public static GoalMessageController Instance;

    public GameObject goalMessage;
    public TextMeshProUGUI goalText;
    public float displayDuration = 3f;

    [Header("Completed Mission Prompt")]
    public CanvasGroup completedMissionPrompt;
    public float promptDisplayDuration = 5f;
    public float promptFadeDuration = 1f;

    private int redShirtsCaught = 0;
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
        if (goalMessage != null)
        {
            goalMessage.SetActive(false);
        }

        if (completedMissionPrompt != null)
        {
            completedMissionPrompt.alpha = 0f;
            completedMissionPrompt.gameObject.SetActive(false);
        }
    }

    /// <summary>Called each time an NPC is caught.</summary>
    public void OnNPCCaught()
    {
        redShirtsCaught++;
        ShowNPCMessage();
    }

    /// <summary>Called each time a bird is caught.</summary>
    public void OnBirdCaught()
    {
        birdsCaught++;
        ShowBirdMessage();
    }

    void ShowNPCMessage()
    {
        bool missionComplete = false;

        if (goalText != null)
        {
            if (redShirtsCaught == 1)
            {
                goalText.text = "Great, 2 more to go!";
            }
            else if (redShirtsCaught == 2)
            {
                goalText.text = "Nice, 1 more!";
            }
            else if (redShirtsCaught >= 3)
            {
                TimerManager timer = FindFirstObjectByType<TimerManager>();
                if (timer != null) timer.StopTimer();
                string timeString = timer != null ? timer.GetFormattedTime() : "";
                goalText.text = "Excellent, you caught all of them!\n" + timeString;
                missionComplete = true;
            }
        }

        if (goalMessage != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowMessageForDuration(missionComplete));
        }
    }

    void ShowBirdMessage()
    {
        bool missionComplete = false;

        if (goalText != null)
        {
            if (birdsCaught == 1)
            {
                goalText.text = "Great job! 2 more birds to go!";
            }
            else if (birdsCaught == 2)
            {
                goalText.text = "Nice catch! 1 more bird!";
            }
            else if (birdsCaught >= 3)
            {
                TimerManager timer = FindFirstObjectByType<TimerManager>();
                if (timer != null) timer.StopTimer();
                string timeString = timer != null ? timer.GetFormattedTime() : "";
                goalText.text = "Excellent! You caught all the birds!\n" + timeString;
                missionComplete = true;
            }
        }

        if (goalMessage != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowMessageForDuration(missionComplete));
        }
    }

    IEnumerator ShowMessageForDuration(bool missionComplete)
    {
        goalMessage.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        goalMessage.SetActive(false);

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