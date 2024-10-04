using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;  // The object to follow (e.g., the player)
    
    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 0, -10);  // Offset from the target (z should be -10 for 2D)
    
    [Header("Follow Settings")]
    public float smoothTime = 0.3f;  // How smoothly the camera follows the target
    private Vector3 velocity = Vector3.zero;  // Used by SmoothDamp to track velocity
    
    [Header("Boundaries")]
    public bool enableBounds = false;  // Enable camera bounds
    public Vector2 minBounds;  // Minimum bounds for the camera (x, y)
    public Vector2 maxBounds;  // Maximum bounds for the camera (x, y)

    void LateUpdate()
    {
        if (target != null)
        {
            // Target position plus the offset
            Vector3 targetPosition = target.position + offset;

            // Smoothly move the camera towards the target position
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            
            // Clamp the camera position within bounds if enabled
            if (enableBounds)
            {
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
            }

            // Update the camera's position
            transform.position = smoothedPosition;
        }
    }
}
