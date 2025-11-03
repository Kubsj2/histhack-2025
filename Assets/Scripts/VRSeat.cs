using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils; // XROrigin

[DisallowMultipleComponent]
public class VRSeat : MonoBehaviour
{
    [Header("Refs")]
    public XROrigin xrOrigin;
    public Transform seatAnchor;
    public MovementLocker locker;
    [Tooltip("Zablokuj ruch i obrót po usadzeniu")]
    public bool lockTurningAndMovement = true;

    bool _seated;
    Vector3 _prevPos;
    Quaternion _prevRot;

    // Opcjonalnie: wy��cz kolizje/auto-wysoko�� gdy siedzimy
    CharacterController _cc;
    CharacterControllerDriver _ccDriver;
    bool _ccPrevEnabled, _ccDriverPrevEnabled;

    void Awake()
    {
        if (xrOrigin)
        {
            _cc = xrOrigin.GetComponent<CharacterController>();
            _ccDriver = xrOrigin.GetComponent<CharacterControllerDriver>();
        }
    }

    [ContextMenu("Sit")]
    public void Sit()
    {
        if (_seated || !xrOrigin || !seatAnchor) return;

        _seated = true;
        _prevPos = xrOrigin.transform.position;
        _prevRot = xrOrigin.transform.rotation;

        if (lockTurningAndMovement) locker?.SetLocked(true);

        // Wyr�wnaj yaw do anchor.forward i przenie� kamer� w punkt kotwicy
        xrOrigin.MatchOriginUpCameraForward(Vector3.up, seatAnchor.forward);
        xrOrigin.MoveCameraToWorldLocation(seatAnchor.position);

        // (opcjonalnie) wy��cz CC i auto-height, �eby nic nas nie �pcha�o�
        if (_cc)
        {
            _ccPrevEnabled = _cc.enabled;
            _cc.enabled = false;
        }
        if (_ccDriver)
        {
            _ccDriverPrevEnabled = _ccDriver.enabled;
            _ccDriver.enabled = false;
        }
    }

    [ContextMenu("Stand")]
    public void Stand()
    {
        if (!_seated || !xrOrigin) return;

        // Przywr�� poprzedni� pozycj� rig-a (albo mo�esz zostawi� gracza na krze�le)
        xrOrigin.transform.SetPositionAndRotation(_prevPos, _prevRot);

        if (lockTurningAndMovement) locker?.SetLocked(false);

        if (_cc) _cc.enabled = _ccPrevEnabled;
        if (_ccDriver) _ccDriver.enabled = _ccDriverPrevEnabled;

        _seated = false;
    }
}
