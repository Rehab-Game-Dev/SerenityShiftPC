using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Ambient background NPC behavior: wanders to random points within a radius via
/// NavMesh, pausing briefly at each stop, with an Animator speed parameter driven by
/// actual agent velocity so it blends between Idle and Walk.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class RandomStreetWalker : MonoBehaviour
{
    [Header("Settings")]
    public float walkRadius = 20f; // how far it can walk from the current point
    public float waitTime = 3f;    // how long to wait when it reaches the destination before continuing

    [Header("Animation")]
    public Animator animator;
    public string speedParam = "speed"; // the name of the parameter in your Animator

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        // Start the timer so it begins walking immediately
        timer = waitTime;
    }

    void Update()
    {
        // Drive Idle/Walk blend directly from the agent's current speed.
        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat(speedParam, speed);
        }

        timer += Time.deltaTime;

        // Once the wait has elapsed and the previous destination is reached (or there
        // was none), pick a new random point and head there.
        if (timer >= waitTime && (!agent.hasPath || agent.remainingDistance < 0.5f))
        {
            Vector3 newPos = RandomNavSphere(transform.position, walkRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    /// <summary>Finds a random NavMesh-valid point within <paramref name="dist"/> of <paramref name="origin"/>.</summary>
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
