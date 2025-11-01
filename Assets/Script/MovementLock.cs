using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

[DisallowMultipleComponent]
public class MovementLocker : MonoBehaviour
{
    [Header("Providers to control")]
    public TeleportationProvider teleport;
    public ContinuousMoveProviderBase move;
    public SnapTurnProviderBase snapTurn;
    public ContinuousTurnProviderBase continuousTurn;

    public bool Locked { get; private set; }

    public void SetLocked(bool locked)
    {
        Locked = locked;
        if (teleport) teleport.enabled = !locked;
        if (move) move.enabled = !locked;
        if (snapTurn) snapTurn.enabled = !locked;
        if (continuousTurn) continuousTurn.enabled = !locked;
    }

    public void Toggle() => SetLocked(!Locked);
}
