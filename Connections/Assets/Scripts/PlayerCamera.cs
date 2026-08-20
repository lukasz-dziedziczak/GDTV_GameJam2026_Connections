using UnityEngine;

[ExecuteAlways]
public class PlayerCamera : MonoBehaviour
{
    [Header("Orbit / view")]
    [Tooltip("Distance from this camera's parent in local space")]
    [SerializeField] float zoom = 15f;

    [Tooltip("Angle above the ground (degrees). 0 = horizontal, 90 = straight down")]
    [SerializeField] float viewAngle = 45f;

    [Tooltip("Rotation around the parent (degrees)")]
    [SerializeField] float rotation = 45f;

    void Start()
    {
        UpdateTransform();
    }

    void LateUpdate()
    {
        // Keep camera positioned relative to parent each frame (allows parent to move)
        UpdateTransform();
    }

    void OnValidate()
    {
        // Update in editor when values change
        UpdateTransform();
    }

    void UpdateTransform()
    {
        if (transform.parent == null) return;

        // Compute local offset using spherical-style rotation:
        // - First tilt by viewAngle around the local X axis (pitch)
        // - Then rotate around Y by rotation (yaw)
        Quaternion rot = Quaternion.Euler(viewAngle, rotation, 0f);

        // Start from a vector pointing backwards along local Z and offset by zoom.
        // Applying rot gives the desired local position relative to the parent.
        Vector3 localOffset = rot * new Vector3(0f, 0f, -zoom);

        // Apply as localPosition so the camera follows the parent transform.
        transform.localPosition = localOffset;

        // Make the camera look at the parent's world position
        transform.LookAt(transform.parent.position);
    }
}
