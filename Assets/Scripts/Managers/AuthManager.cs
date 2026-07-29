using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System.Collections.Generic;

/// <summary>
/// Drives the login screen: sign-up, sign-in, and guest entry via Unity's Authentication
/// and Cloud Save services. On success, loads a short "Environment Menu" transition scene
/// while the player's saved progress is fetched, then routes to the correct level.
/// </summary>
public class AuthManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inputPanel;
    public TMP_InputField userField;
    public TMP_InputField passField;
    public TextMeshProUGUI statusText;
    public GameObject initUIParent;
    public TextMeshProUGUI detailsText;

    // Tracks whether the open input panel is being used for sign-up or sign-in,
    // since both flows share the same fields and submit button.
    private bool isSigningUp = false;

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    /// <summary>Opens the shared input panel in sign-up mode.</summary>
    public void OpenSignUp()
    {
        detailsText.fontSize = 100;
        initUIParent.SetActive(false);
        isSigningUp = true;
        inputPanel.SetActive(true);
        statusText.text = "Create Account:";
    }

    /// <summary>Opens the shared input panel in sign-in mode.</summary>
    public void OpenSignIn()
    {
        detailsText.fontSize = 100;
        initUIParent.SetActive(false);
        isSigningUp = false;
        inputPanel.SetActive(true);
        statusText.text = "Welcome Back!";
    }

    /// <summary>Skips authentication entirely and dismisses the login UI.</summary>
    public void GuestLogin()
    {
        initUIParent.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>Cancels the input panel and returns to the initial login screen.</summary>
    public void ClosePanel()
    {
        inputPanel.SetActive(false);
        initUIParent.SetActive(true);
    }

    /// <summary>
    /// Submits the username/password fields for either sign-up or sign-in (based on
    /// <see cref="isSigningUp"/>), then hands off to <see cref="LoadLevelData"/> to resume
    /// the player's saved progress.
    /// </summary>
    public async void OnSubmitPressed()
    {
        string u = userField.text;
        string p = passField.text;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            detailsText.text = "Fields cannot be empty";
            return;
        }

        statusText.text = "Processing...";

        try
        {
            if (isSigningUp)
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(u, p);
                // New accounts start at level 1 so LoadLevelData has a save to read back.
                SaveLevelData(1);
                inputPanel.SetActive(false);
                SceneManager.LoadScene("Environment Menu");
                LoadLevelData();
            }
            else
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(u, p);
                inputPanel.SetActive(false);
                SceneManager.LoadScene("Environment Menu");
                LoadLevelData();
            }
        }
        catch (System.Exception e)
        {
            statusText.text = "Error: " + e.Message;

            // Display error in DetailsText with font size 40
            if (detailsText != null)
            {
                detailsText.text = e.Message;
                detailsText.fontSize = 40;
            }

            Debug.LogError(e.Message);
        }
    }

    /// <summary>Persists the player's furthest-reached level to Unity Cloud Save.</summary>
    private async void SaveLevelData(int level)
    {
        var data = new Dictionary<string, object> { { "saved_level", level } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }

    /// <summary>
    /// Reads the player's saved progress and loads the matching scene. Players with no
    /// save yet (or an unrecognized level value) fall back to the tutorial.
    /// </summary>
    private async void LoadLevelData()
    {
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "saved_level" });

        if (data.ContainsKey("saved_level"))
        {
            int levelToLoad = data["saved_level"].Value.GetAs<int>();

            switch (levelToLoad)
            {
                case 1:
                    SceneManager.LoadScene("StreetScene - tutorial");
                    break;
                case 2:
                    SceneManager.LoadScene("StreetScene - easy");
                    break;
                case 3:
                    SceneManager.LoadScene("StreetScene - medium");
                    break;
                case 4:
                    SceneManager.LoadScene("StreetScene - hard");
                    break;
                default:
                    SceneManager.LoadScene("MainMenu");
                    break;
            }
        }
        else
        {
            SceneManager.LoadScene("StreetScene - tutorial");
        }
    }
}
