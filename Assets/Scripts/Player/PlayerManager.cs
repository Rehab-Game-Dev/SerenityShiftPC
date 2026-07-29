using UnityEngine;

/// <summary>
/// Activates the PC player and its control panel at scene start. (Historically this
/// also toggled between a PC and VR player rig; the VR path was removed when the
/// project split into separate PC/VR versions.)
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [Header("Player References")]
    public GameObject pcPlayer;
    public GameObject PC_Controls_Panel;

    private void Start()
    {
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        pcPlayer.SetActive(true);
        if (PC_Controls_Panel != null)
        {
            PC_Controls_Panel.SetActive(true);
        }
    }
}
