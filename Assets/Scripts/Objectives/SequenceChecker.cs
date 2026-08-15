using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Validates the "Django's music puzzle" note sequence: each <see cref="NoteDetector"/>
/// reports its note here via <see cref="NotePressed"/>. A wrong note (relative to
/// <see cref="correctSequence"/>) resets progress and shows a mistake message; completing
/// the full sequence stops the level timer, shows a success message, and after a delay
/// resumes Django's ambient music and hides the puzzle's UI. Once that cleanup is done,
/// the "completed mission" prompt fades in, holds, then fades back out.
/// </summary>
public class SequenceChecker : MonoBehaviour
{
    [SerializeField]
    private List<string> correctSequence = new List<string>
    { "Do", "Mi", "Fa", "Sol", "Fa", "Mi", "Re", "Do" };

    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private float dialogDisplayTime = 5f;

    [SerializeField] private string mistakeMessage = "Django (street performer) : \"Don't worry you can try again you got this!\"";
    [SerializeField] private string successMessage = "Django (street performer) : \"Well done, I recognize a talented musician when i see one\"";

    [SerializeField] private AudioSource djangoMusicSource;
    [SerializeField] private GameObject cubeNotes;
    [SerializeField] private GameObject instructionPanel;

    [Header("Completed Mission Prompt")]
    [SerializeField] private CompletedMissionPrompt completedMissionPrompt;

    private List<string> playerSequence = new List<string>();
    private bool puzzleSolved = false;
    private bool isShowingDialog = false;

    /// <summary>Whether the full correct sequence has been played.</summary>
    public bool IsPuzzleSolved => puzzleSolved;

    /// <summary>
    /// Registers the next note the player played. Ignored once the puzzle is solved or
    /// while a dialog is already showing, so rapid note presses can't overlap messages.
    /// </summary>
    public void NotePressed(string noteName)
    {
        if (puzzleSolved || isShowingDialog) return;

        playerSequence.Add(noteName);
        Debug.Log("Current sequence: " + string.Join(", ", playerSequence));

        if (!IsSequencePrefixMatch(playerSequence, correctSequence))
        {
            Debug.Log("Wrong note! Sequence reset.");
            playerSequence.Clear();
            ShowDialog(mistakeMessage);
            return;
        }

        if (playerSequence.Count == correctSequence.Count)
        {
            Debug.Log("Puzzle solved! Correct sequence completed!");
            puzzleSolved = true;

            TimerManager timer = FindFirstObjectByType<TimerManager>();
            if (timer != null) timer.StopTimer();
            string timeString = timer != null ? timer.GetFormattedTime() : "";
            ShowDialog(successMessage + "\n" + timeString);

            StartCoroutine(RestartMusicAfterDialog());
        }
    }

    /// <summary>
    /// Checks whether <paramref name="played"/> matches the start of <paramref name="correct"/>,
    /// note-for-note. Called after every note so a mistake is caught immediately rather than
    /// only once the full sequence has been entered. A <paramref name="played"/> list longer
    /// than <paramref name="correct"/> cannot match (it can never be a prefix).
    /// </summary>
    public static bool IsSequencePrefixMatch(IReadOnlyList<string> played, IReadOnlyList<string> correct)
    {
        if (played.Count > correct.Count) return false;

        for (int i = 0; i < played.Count; i++)
        {
            if (played[i] != correct[i])
            {
                return false;
            }
        }
        return true;
    }

    private void ShowDialog(string message)
    {
        if (dialogPanel != null && dialogText != null)
        {
            dialogText.text = message;
            dialogPanel.SetActive(true);
            StartCoroutine(HideDialogAfterDelay());
        }
    }

    private IEnumerator HideDialogAfterDelay()
    {
        isShowingDialog = true;
        yield return new WaitForSeconds(dialogDisplayTime);

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
        isShowingDialog = false;
    }

    // Gives the success dialog time to be read before resuming ambient music and
    // tidying away the puzzle's props/instructions.
    private IEnumerator RestartMusicAfterDialog()
    {
        yield return new WaitForSeconds(5f);

        if (djangoMusicSource != null)
        {
            djangoMusicSource.volume = 0.5f;

            if (!djangoMusicSource.isPlaying)
            {
                djangoMusicSource.Play();
            }

            Debug.Log("Django's music resumed at half volume");
        }

        if (cubeNotes != null)
        {
            cubeNotes.SetActive(false);
            Debug.Log("Cube notes hidden");
        }

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
            Debug.Log("Instruction panel hidden");
        }

        // Lives on the prompt object itself, so it keeps running even though
        // SequenceManager (this script's GameObject) gets deactivated around here.
        if (completedMissionPrompt != null)
        {
            completedMissionPrompt.Show();
        }
    }

    /// <summary>Clears player progress without affecting <see cref="puzzleSolved"/> (used to let a solved puzzle be tried again).</summary>
    public void ResetSequence()
    {
        playerSequence.Clear();
        puzzleSolved = false;
        Debug.Log("Sequence reset");
    }
}