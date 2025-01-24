using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player; // Assign the player GameObject here
    [SerializeField] private float smoothSpeed = 0.3f; // Smoothing factor for camera movement
    [SerializeField, Range(0.1f, 1f)] private float thresholdPercentage = 0.5f; // Threshold area as a percentage of the screen
    [SerializeField, Range(0f, 1f)] private float playerMouseWeight = 0.5f; // Weight for averaging between player and mouse

    private Vector2 threshold; // Calculated in world space
    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        cam = Camera.main;
        CalculateThreshold();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f) return; // Do not follow when the game is paused
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        if (player == null || cam == null) return;

        // Calculate the target position based on the player and mouse positions
        Vector3 playerPosition = player.position;
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        Vector3 targetPosition;

        // Check if the mouse is outside the threshold
        Vector3 deltaMouse = mouseWorldPosition - transform.position;
        if (Mathf.Abs(deltaMouse.x) > threshold.x || Mathf.Abs(deltaMouse.y) > threshold.y)
        {
            // Weighted average between the player and mouse
            targetPosition = Vector3.Lerp(playerPosition, mouseWorldPosition, playerMouseWeight);
        }
        else
        {
            // Default to the player position if the mouse is within the threshold
            targetPosition = playerPosition;
        }

        // Maintain the Z position of the camera
        targetPosition.z = transform.position.z;

        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Mathf.Abs(cam.transform.position.z); // Use the camera's Z position
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = transform.position.z; // Maintain the camera's Z position
        return mouseWorldPosition;
    }

    private void CalculateThreshold()
    {
        if (cam == null) return;

        // Convert threshold percentage to world space
        float screenHalfHeight = cam.orthographicSize;
        float screenHalfWidth = screenHalfHeight * cam.aspect;

        threshold = new Vector2(screenHalfWidth * (1 - thresholdPercentage), screenHalfHeight * (1 - thresholdPercentage));
    }

    private void OnValidate()
    {
        CalculateThreshold(); // Recalculate threshold in the Editor when values are changed
    }

    private void OnDrawGizmos()
    {
        if (cam == null) return;

        // Draw the threshold area for visualization in the Editor
        Gizmos.color = Color.green;
        Vector3 cameraPosition = transform.position;
        Gizmos.DrawLine(cameraPosition + new Vector3(-threshold.x, -threshold.y, 0), cameraPosition + new Vector3(-threshold.x, threshold.y, 0));
        Gizmos.DrawLine(cameraPosition + new Vector3(threshold.x, -threshold.y, 0), cameraPosition + new Vector3(threshold.x, threshold.y, 0));
        Gizmos.DrawLine(cameraPosition + new Vector3(-threshold.x, -threshold.y, 0), cameraPosition + new Vector3(threshold.x, -threshold.y, 0));
        Gizmos.DrawLine(cameraPosition + new Vector3(-threshold.x, threshold.y, 0), cameraPosition + new Vector3(threshold.x, threshold.y, 0));
    }
}
