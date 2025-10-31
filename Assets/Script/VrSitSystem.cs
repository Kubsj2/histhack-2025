using UnityEngine;

using UnityEngine.InputSystem;

public class VRSitSystem : MonoBehaviour
{
    [Header("References")]
    public Transform sitPoint;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rightHandRay;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor leftHandRay;
    public Transform xrOrigin;
    public float sitTransitionSpeed = 3f;

    [Header("Input")]
    public InputActionProperty sitAction;

    private bool isSitting = false;
    private bool isInTransition = false;
    private Vector3 targetPosition;

    void OnEnable()
    {
        sitAction.action.performed += OnSitPressed;
    }

    void OnDisable()
    {
        sitAction.action.performed -= OnSitPressed;
    }

    void OnSitPressed(InputAction.CallbackContext ctx)
    {
        if (isInTransition) return;

        if (!isSitting && IsLookingAtChair())
        {
            StartCoroutine(SmoothSit());
        }
        else if (isSitting)
        {
            StartCoroutine(SmoothStand());
        }
    }

    bool IsLookingAtChair()
    {
        RaycastHit hit;
        if (rightHandRay.TryGetCurrent3DRaycastHit(out hit) || leftHandRay.TryGetCurrent3DRaycastHit(out hit))
        {
            return hit.collider != null && hit.collider.gameObject == gameObject;
        }
        return false;
    }

    System.Collections.IEnumerator SmoothSit()
    {
        isInTransition = true;
        Vector3 startPos = xrOrigin.position;
        targetPosition = sitPoint.position;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * sitTransitionSpeed;
            xrOrigin.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        isSitting = true;
        isInTransition = false;
    }

    System.Collections.IEnumerator SmoothStand()
    {
        isInTransition = true;
        Vector3 startPos = xrOrigin.position;
        targetPosition = new Vector3(startPos.x, startPos.y + 0.5f, startPos.z); // wstań 0.5 m wyżej
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * sitTransitionSpeed;
            xrOrigin.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        isSitting = false;
        isInTransition = false;
    }
}
