using UnityEngine;

/// <summary>
/// Drives the player's Idle/Walk/Run animator blend from the actual movement speed
/// reported by their <see cref="CharacterController"/>.
/// </summary>
public class PlayerAnimationLinker : MonoBehaviour
{
    [Header("Connections")]
    public Animator bodyAnimator;          // the Animator component of the player body
    public CharacterController playerController; // the CharacterController component that moves the player

    [Header("Settings")]
    public string speedParameterName = "Speed"; // speed parameter name in the Animator
    public float animationSmoothTime = 0.1f;    // to make the transition smooth and not jumpy

    void Update()
    {
        // Ignore vertical velocity (jumps/falls) - only horizontal movement should affect the walk/run blend.
        Vector3 horizontalVelocity = playerController.velocity;
        horizontalVelocity.y = 0;

        float currentSpeed = horizontalVelocity.magnitude;

        // 0 -> Idle, >0 -> Walk/Run, smoothed so the transition doesn't snap between poses.
        bodyAnimator.SetFloat(speedParameterName, currentSpeed, animationSmoothTime, Time.deltaTime);
    }
}
