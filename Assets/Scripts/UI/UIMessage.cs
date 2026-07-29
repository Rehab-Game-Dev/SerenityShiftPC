using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Generic fade-in / hold / fade-out UI message. Calling <see cref="ShowMessage"/> again
/// while a message is already showing cancels the current fade and restarts the sequence
/// with the new text.
/// </summary>
public class UIMessage : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup canvasGroup;
    public TMP_Text messageText;

    [Header("Settings")]
    public float showTime = 5f;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    /// <summary>Displays the given message (or re-shows the current text if <paramref name="msg"/> is null), fading it in, holding, then fading out.</summary>
    public void ShowMessage(string msg = null)
    {
        if (msg != null && messageText != null)
            messageText.text = msg;

        StopAllCoroutines();
        StartCoroutine(MessageRoutine());
    }

    private IEnumerator MessageRoutine()
    {
        // Fade In
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // wait for show time
        yield return new WaitForSeconds(showTime);

        // Fade Out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
