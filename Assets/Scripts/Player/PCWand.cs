using UnityEngine;

/// <summary>
/// PC/mouse equivalent of a VR pointer for catching birds: on left-click, raycasts
/// forward from this transform and catches the first uncaught bird it hits.
/// </summary>
public class PCWand : MonoBehaviour
{
    public float range = 100f; // range of the raycast
    private bool hasBeenCaught = false; // prevent double counting

    void Update()
    {
        // Check: was the left mouse button clicked?
        if (Input.GetMouseButtonDown(0))
        {
            ShootRay();
        }
    }

    void ShootRay()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, range);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.CompareTag("Bird"))
            {
                BirdCatchable birdScript = hit.transform.GetComponent<BirdCatchable>();
                if (birdScript != null && !birdScript.hasBeenCaught)
                {
                    birdScript.CatchBird();
                    return; // catch only one per click
                }
            }
        }
    }
}
