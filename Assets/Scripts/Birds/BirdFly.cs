using UnityEngine;

/// <summary>
/// Flies a bird forward at a constant speed with a subtle sine-wave bob added on top,
/// so a flock of these doesn't read as perfectly straight, robotic movement.
/// </summary>
public class BirdFly : MonoBehaviour
{
    [Header("הגדרות טיסה")] // Flight settings
    public float flySpeed = 15f;     // flight speed of the bird
    public float wobbleAmount = 1f; // amount of vertical wobble

    // Randomized per-bird so a flock doesn't wobble in perfect unison.
    private float randomOffset;

    void Start()
    {
        randomOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        // Move forward along the bird's own facing direction.
        transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);

        // Add a gentle vertical sine wobble on top of the forward motion to make the flight feel alive.
        float wobble = Mathf.Sin(Time.time + randomOffset) * wobbleAmount * Time.deltaTime;
        transform.Translate(Vector3.up * wobble);
    }
}
