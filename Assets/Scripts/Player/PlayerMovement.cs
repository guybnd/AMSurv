using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the player
    public Vector2 moveDir; // Stores input direction
    Rigidbody2D rb; // Reference to the Rigidbody2D component



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        // Apply movement
        Move();
    }

    void InputManagement()
    {
        // Handle input for movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDir = new Vector2(moveX,moveY).normalized;
    }

    void Move()
    {
        // Move the player
        rb.linearVelocity = new Vector2 (moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }
}
