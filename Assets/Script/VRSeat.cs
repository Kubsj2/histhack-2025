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

    // Opcjonalnie: wy³¹cz kolizje/auto-wysokoœæ gdy siedzimy
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

        // Wyrównaj yaw do anchor.forward i przenieœ kamerê w punkt kotwicy
        xrOrigin.MatchOriginUpCameraForward(Vector3.up, seatAnchor.forward);
        xrOrigin.MoveCameraToWorldLocation(seatAnchor.position);

        // (opcjonalnie) wy³¹cz CC i auto-height, ¿eby nic nas nie „pcha³o”
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

        // Przywróæ poprzedni¹ pozycjê rig-a (albo mo¿esz zostawiæ gracza na krzeœle)
        xrOrigin.transform.SetPositionAndRotation(_prevPos, _prevRot);

        if (lockTurningAndMovement) locker?.SetLocked(false);

        if (_cc) _cc.enabled = _ccPrevEnabled;
        if (_ccDriver) _ccDriver.enabled = _ccDriverPrevEnabled;

        _seated = false;
    }
}
