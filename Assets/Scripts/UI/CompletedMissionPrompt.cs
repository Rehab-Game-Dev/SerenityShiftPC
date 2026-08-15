using UnityEngine;
using System.Collections;

/// <summary>
/// Sits directly on the "completed mission" prompt object (needs a CanvasGroup).
/// Call <see cref="Show"/> from any script to fade it in, hold, then fade it out.
/// Living on the prompt object itself means the fade still runs even if whatever
/// triggered it (e.g. a puzzle manager) gets deactivated right afterward.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class CompletedMissionPrompt : MonoBehaviour
{
    public float displayDuration = 5f;
    public float fadeDuration = 1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}