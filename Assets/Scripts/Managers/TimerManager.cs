using UnityEngine;
using TMPro;

/// <summary>
/// Runs a simple up-counting level timer (MM:SS) and exposes the final/current time to
/// other systems (e.g. win-message controllers append <see cref="GetFormattedTime"/> to
/// their text). Visibility of the timer panel is restored from the player's saved preference.
/// </summary>
public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float elapsedTime = 0f;
    private bool isRunning = true;

    public GameObject timerPanel;

    void Start()
    {
        bool saved = PlayerPrefs.GetInt("TimerVisible", 1) == 1;
        timerPanel.SetActive(saved);
    }

    void Update()
    {
        if (!isRunning) return;
        elapsedTime += Time.deltaTime;
        DisplayTime(elapsedTime);
    }

    void DisplayTime(float time)
    {
        timerText.text = FormatTime(time);
    }

    /// <summary>Freezes the timer at its current value (called once the level's objective is complete).</summary>
    public void StopTimer()
    {
        isRunning = false;
    }

    /// <summary>Raw elapsed time in seconds, for callers that need the numeric value rather than a formatted string.</summary>
    public float GetFinalTime()
    {
        return elapsedTime;
    }

    /// <summary>Current elapsed time formatted as MM : SS, for display in completion messages.</summary>
    public string GetFormattedTime()
    {
        return FormatTime(elapsedTime);
    }

    /// <summary>
    /// Formats a duration in seconds as "MM : SS". Minutes are not capped or converted to
    /// hours, so a duration over an hour renders as e.g. "61 : 05" rather than "1:01:05".
    /// </summary>
    public static string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        return string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}
