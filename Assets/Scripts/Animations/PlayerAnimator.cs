using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{


    Animator am; // Reference to the Animator component
    PlayerMovement pm; // Reference to the PlayerMovement script
    SpriteRenderer sr; // Reference to the Sprite Renderer component


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.moveDir != Vector2.zero)
        {
            am.SetBool("Move", true);
        }
        else
        {
            am.SetBool("Move", false);
        }
    }
}
