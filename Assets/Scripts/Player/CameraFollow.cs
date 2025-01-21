using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float smoothSpeed = 0.125f;
    public Vector3 offset; // Offset from the player

    void LateUpdate()
    {
        if (player == null) return;

        // Smoothly follow the player
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
