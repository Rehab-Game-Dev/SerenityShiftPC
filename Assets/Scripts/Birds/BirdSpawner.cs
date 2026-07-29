using UnityEngine;
using System.Collections;

/// <summary>
/// Continuously spawns bird prefabs at randomized positions/rotations within a box
/// volume around this object, each with a limited lifetime. Starts spawning immediately
/// on <see cref="Start"/>; <see cref="StartSpawning"/> exists for callers that want to
/// (re)trigger spawning explicitly (e.g. after all NPCs are caught).
/// </summary>
public class BirdSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] birdPrefabs;

    public float spawnInterval = 5f; // How often to spawn a bird?
    public Vector3 spawnArea = new Vector3(20f, 2f, 20f); // Size of the area from which birds spawn
    public float birdLifetime = 20f; // How long before a bird is destroyed
    private bool isSpawning = false; // Flag indicating whether we should spawn birds

    void Start()
    {
        StartCoroutine(SpawnBirds());
    }

    /// <summary>Starts a second spawn loop if one isn't already running. Call this to (re)activate spawning at runtime.</summary>
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnBirds());
            Debug.Log("Bird spawning started!");
        }
    }

    IEnumerator SpawnBirds()
    {
        while (true)
        {
            SpawnRandomBird();
            // Wait for a random time to make it feel natural
            yield return new WaitForSeconds(spawnInterval + Random.Range(0f, 3f));
        }
    }

    void SpawnRandomBird()
    {
        // safety check to avoid errors (no bird prefabs assigned)
        if (birdPrefabs.Length == 0) return;

        Vector3 randomPos = transform.position + new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            Random.Range(-spawnArea.z, spawnArea.z)
        );

        Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);

        int randomIndex = Random.Range(0, birdPrefabs.Length);
        GameObject selectedBird = birdPrefabs[randomIndex];

        GameObject newBird = Instantiate(selectedBird, randomPos, randomRot);

        // Auto-cleanup so spawned birds don't accumulate indefinitely.
        Destroy(newBird, birdLifetime);
    }

    // Visualizes the spawn volume in the Scene view for easy tuning.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, spawnArea * 2);
    }
}
