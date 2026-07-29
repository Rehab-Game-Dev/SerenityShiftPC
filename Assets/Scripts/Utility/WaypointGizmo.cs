using UnityEngine;

/// <summary>Draws a small sphere at this object's position in the Scene view, purely to make waypoint markers visible while editing.</summary>
public class WaypointGizmo : MonoBehaviour
{
    public float sphereRadius = 0.2f;

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, sphereRadius);
    }
}
