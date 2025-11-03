using UnityEngine;

[DisallowMultipleComponent]
public class BoardSurface : MonoBehaviour
{
    public BoxCollider bounds;

    void Reset()
    {
        if (!bounds) bounds = GetComponent<BoxCollider>();
    }

    public Vector3 ProjectToSurface(Vector3 worldPoint)
    {
        var pLocal = transform.InverseTransformPoint(worldPoint);
        pLocal.y = bounds ? bounds.center.y : 0f;
        return transform.TransformPoint(pLocal);
    }

    public Vector3 ClampToBounds(Vector3 worldPoint)
    {
        if (!bounds) return ProjectToSurface(worldPoint);
        var pLocal = transform.InverseTransformPoint(worldPoint);
        var c = bounds.center;
        var h = bounds.size * 0.5f;
        pLocal.x = Mathf.Clamp(pLocal.x, c.x - h.x, c.x + h.x);
        pLocal.z = Mathf.Clamp(pLocal.z, c.z - h.z, c.z + h.z);
        pLocal.y = c.y;
        return transform.TransformPoint(pLocal);
    }

    public Quaternion AlignUp(Quaternion current, float yawDegrees = 0f)
    {
        var up = transform.up;
        var forward = Quaternion.AngleAxis(yawDegrees, up) * transform.forward;
        return Quaternion.LookRotation(forward, up);
    }

    void OnDrawGizmosSelected()
    {
        if (!bounds) return;
        Gizmos.color = new Color(0, 1, 1, 0.25f);
        var m = Matrix4x4.TRS(transform.TransformPoint(bounds.center), transform.rotation, transform.lossyScale);
        Gizmos.matrix = m;
        Gizmos.DrawCube(Vector3.zero, bounds.size);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, bounds.size);
    }
}
