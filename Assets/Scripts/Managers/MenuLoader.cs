using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Button-facing scene loader for the main/pause menus - one method per destination
/// so each can be wired directly to a UI Button's OnClick event. The difficulty
/// levels load asynchronously behind a progress bar since their scenes take a
/// few seconds to load.
/// </summary>
public class MenuLoader : MonoBehaviour
{
    [Header("Loading UI")]
    public GameObject loadingPanel;
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    [Tooltip("How fast the bar animates toward real progress, in fill-fraction per second. Lower = slower/smoother fill. Keeps the bar from snapping straight to 100% when the scene loads almost instantly (e.g. in the Editor).")]
    public float progressFillSpeed = 0.6f;
    [Tooltip("Menu elements (difficulty buttons, title text, etc.) to hide as soon as loading starts, so only the loading panel is visible.")]
    public GameObject[] menuElementsToHide;

    public void LoadTutorial()
    {
        StartCoroutine(LoadSceneAsync("StreetScene - tutorial"));
    }
    public void LoadEasy()
    {
        StartCoroutine(LoadSceneAsync("StreetScene - easy"));
    }
    public void LoadMedium()
    {
        StartCoroutine(LoadSceneAsync("StreetScene - medium"));
    }
    public void LoadHard()
    {
        StartCoroutine(LoadSceneAsync("StreetScene - hard"));
    }

    /// <summary>
    /// Loads a scene in the background while driving the loading panel/progress bar.
    /// Unity's AsyncOperation only reports progress up to 0.9 while loading (the
    /// remaining 0.1 is the scene activation step), so progress is rescaled to a
    /// clean 0-1 range and activation is held back until the bar visually reaches
    /// full, avoiding an abrupt jump at the end.
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (progressBar != null) progressBar.value = 0f;
        if (progressText != null) progressText.text = "0%";

        foreach (GameObject element in menuElementsToHide)
        {
            if (element != null) element.SetActive(false);
        }

        // Let the engine actually render the loading panel / hidden buttons before
        // kicking off the scene load, which can briefly block the main thread and
        // would otherwise delay this frame from ever being shown.
        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Animate toward the real progress instead of snapping to it, so the bar
            // still reads as gradual motion even when the scene itself loads almost
            // instantly (e.g. in the Editor, where assets are already cached).
            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * progressFillSpeed);

            if (progressBar != null) progressBar.value = displayedProgress;
            if (progressText != null) progressText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";

            // Only activate once the scene has actually finished loading AND the bar
            // has visually caught up to full, so the player always sees a complete fill.
            if (operation.progress >= 0.9f && displayedProgress >= 0.999f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public void LoadMainMenu()
    {
        // Reset systems a paused/slowed gameplay scene may have left in a non-default state.
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
    public void Street()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
    public void Train()
    {
        // Do nothing for now
    }
}
