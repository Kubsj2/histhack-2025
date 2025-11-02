using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class PlanarGrabConstraint : MonoBehaviour
{
    public BoardSurface board;
    public float gridSize = 0f;
    public bool followInteractorYaw = false;

    XRGrabInteractable _grab;
    IXRSelectInteractor _interactor;
    Vector3 _grabOffsetOnBoard;

    void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        _grab.movementType = XRBaseInteractable.MovementType.Instantaneous;
        _grab.trackRotation = false;
    }

    void OnEnable()
    {
        _grab.selectEntered.AddListener(OnGrab);
        _grab.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        _grab.selectEntered.RemoveListener(OnGrab);
        _grab.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        _interactor = args.interactorObject;
        if (!board) return;
        var hitPoint = GuessInteractorPointOnBoard(_interactor);
        var onBoard = board.ProjectToSurface(hitPoint);
        _grabOffsetOnBoard = transform.position - onBoard;
        _grabOffsetOnBoard = Vector3.ProjectOnPlane(_grabOffsetOnBoard, board.transform.up);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        _interactor = null;
    }

    void Update()
    {
        if (!board || _interactor == null) return;

        var hitPoint = GuessInteractorPointOnBoard(_interactor);
        var target = board.ProjectToSurface(hitPoint) + _grabOffsetOnBoard;
        target = board.ClampToBounds(target);
        if (gridSize > 0f) target = SnapToGrid(target, gridSize);
        transform.position = target;

        float yaw = 0f;
        if (followInteractorYaw)
        {
            var fwd = Vector3.ProjectOnPlane(_interactor.transform.forward, board.transform.up).normalized;
            if (fwd.sqrMagnitude > 0.0001f)
                yaw = Vector3.SignedAngle(board.transform.forward, fwd, board.transform.up);
        }
        transform.rotation = board.AlignUp(transform.rotation, yaw);
    }

    Vector3 SnapToGrid(Vector3 world, float step)
    {
        var t = board.transform;
        var local = t.InverseTransformPoint(world);
        local.x = Mathf.Round(local.x / step) * step;
        local.z = Mathf.Round(local.z / step) * step;
        return t.TransformPoint(local);
    }

    Vector3 GuessInteractorPointOnBoard(IXRSelectInteractor interactor)
    {
        if (interactor is XRRayInteractor ray && ray.TryGetCurrent3DRaycastHit(out var hit)) return hit.point;
        return interactor.transform.position;
    }
}
