using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Drives a UI arrow that points from the player toward the current objective.
/// The target depends on the active scene: the hard level points at the street
/// performer, the tutorial points at the goal zone, and every other level points at
/// whichever is relevant right now - NPCs first, then birds once NPCs are cleared.
/// The arrow freezes once <see cref="missionComplete"/> is set.
/// </summary>
public class CompassController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform arrowTransform;

    [Header("Settings")]
    public float updateInterval = 0.2f;

    // Set to true by GoalZoneTrigger (tutorial) and StreetPerformerDetection (hard)
    // when the mission is complete - freezes the arrow
    public static bool missionComplete = false;

    private Transform playerTransform;
    private float timer;
    private Transform closestTarget;
    private string currentSceneName;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        missionComplete = false; // fresh start on every scene load
        currentSceneName = SceneManager.GetActiveScene().name;
        FindPlayer();
        UpdateVisibility();
    }

    /// <summary>
    /// Shows or hides the compass based on the player's saved "CompassToggle" preference.
    /// Called on start and whenever <see cref="CompassToggle"/> changes the setting.
    /// </summary>
    public void UpdateVisibility()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        bool isEnabled = PlayerPrefs.GetInt("CompassToggle", 1) == 1;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = isEnabled ? 1 : 0;
            canvasGroup.interactable = isEnabled;
            canvasGroup.blocksRaycasts = isEnabled;
        }
        else
        {
            gameObject.SetActive(isEnabled);
        }
    }

    // Locates the player transform to measure distances/angles from. Falls back to
    // searching by name if no main camera is tagged yet (e.g. right after scene load).
    void FindPlayer()
    {
        if (Camera.main != null)
        {
            playerTransform = Camera.main.transform;
        }
        else
        {
            GameObject player = GameObject.Find("Player_PC");
            if (player == null) player = GameObject.Find("XR Origin (XR Rig)");
            if (player != null) playerTransform = player.transform;
        }
    }

    void Update()
    {
        // Skip all work while the compass is hidden.
        if (canvasGroup != null && canvasGroup.alpha == 0) return;

        if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
        {
            FindPlayer();
            return;
        }

        // Re-scan for the closest target periodically rather than every frame.
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            FindClosestTarget();
            timer = 0;
        }

        UpdateArrow();
    }

    // Picks what the arrow should point at. Scenes with a single scripted objective
    // (hard/tutorial) override the default "nearest NPC or bird" behavior.
    void FindClosestTarget()
    {
        closestTarget = null;
        float minDistance = float.MaxValue;

        // Scene-specific overrides: a single fixed objective instead of "nearest target".
        if (currentSceneName.Contains("hard"))
        {
            if (!missionComplete)
            {
                // Point to Django (Street Performer)
                var performer = Object.FindFirstObjectByType<StreetPerformerDetection>();
                if (performer != null)
                {
                    closestTarget = performer.transform;
                    return;
                }
            }
            // Mission complete (or no performer found) - no target, arrow freezes
            closestTarget = null;
            return;
        }
        else if (currentSceneName.Contains("tutorial"))
        {
            if (!missionComplete)
            {
                // Point to Goal Zone
                var goal = Object.FindFirstObjectByType<GoalZoneTrigger>();
                if (goal != null)
                {
                    closestTarget = goal.transform;
                    return;
                }
            }
            // Mission complete - no target, arrow freezes
            closestTarget = null;
            return;
        }

        // Default logic: point at NPCs until they're all caught, then switch to birds.
        bool lookForBirds = false;

        if (BirdGameManager.Instance != null)
        {
            lookForBirds = true;
        }
        else if (GameManager.Instance != null)
        {
            lookForBirds = GameManager.Instance.caughtCount >= GameManager.Instance.totalNPCs;
        }
        else
        {
            if (Object.FindFirstObjectByType<BirdCatchable>() != null)
            {
                lookForBirds = true;
            }
        }

        if (lookForBirds)
        {
            BirdCatchable[] birds = Object.FindObjectsByType<BirdCatchable>(FindObjectsSortMode.None);
            foreach (var bird in birds)
            {
                if (bird.gameObject.activeInHierarchy)
                {
                    float dist = Vector3.Distance(playerTransform.position, bird.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestTarget = bird.transform;
                    }
                }
            }
        }
        else
        {
            NPCCollision[] npcs = Object.FindObjectsByType<NPCCollision>(FindObjectsSortMode.None);
            foreach (var npc in npcs)
            {
                if (npc.gameObject.activeInHierarchy)
                {
                    float dist = 0f;
                    dist = Vector3.Distance(playerTransform.position, npc.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestTarget = npc.transform;
                    }
                }
            }
        }
    }

    // Rotates the arrow to point at closestTarget, measured as a flat (Y-ignored)
    // angle relative to the player's current facing direction.
    void UpdateArrow()
    {
        if (closestTarget == null || arrowTransform == null)
        {
            return;
        }

        Vector3 directionToTarget = closestTarget.position - playerTransform.position;
        directionToTarget.y = 0;

        if (directionToTarget.sqrMagnitude < 0.01f) return;

        Vector3 playerForward = playerTransform.forward;
        playerForward.y = 0;

        float angle = Vector3.SignedAngle(playerForward, directionToTarget, Vector3.up);
        arrowTransform.localRotation = Quaternion.Euler(0, 0, -angle);
    }
}
