using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f; // Speed of the player
    public Vector2 moveDir; // Stores input direction
    Rigidbody2D rb; // Reference to the Rigidbody2D component

    // Reference to the PlayerController to check if the player is dodging
    private PlayerController playerController;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Only update movement input if the player is not dodging.
        if (playerController == null || !playerController.IsDodging)
        {
            InputManagement();
        }
        else
        {
            // When dodging, you might want to clear moveDir.
            moveDir = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        // Handle input for movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        // If the player is dodging, ignore normal movement.
        if (playerController != null && playerController.IsDodging)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Apply normal movement
        rb.linearVelocity = moveDir * movementSpeed;
    }
}
