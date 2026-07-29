using UnityEngine;

/// <summary>Continuously scales an object up and down between min/max scale on a sine wave, to draw the eye to an objective.</summary>
public class TargetPulse : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Remap sin(-1..1) to (0..1) so Lerp stays within [minScale, maxScale].
        float scale = Mathf.Lerp(minScale, maxScale,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        transform.localScale = originalScale * scale;
    }
}
