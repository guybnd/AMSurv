using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player
    private Vector2 movement; // Stores input direction

    void Update()
    {
        // Get input from the player
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // Apply movement
        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime);
    }
}
