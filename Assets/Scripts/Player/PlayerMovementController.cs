using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EntityScript))]
public class PlayerMovementController : MonoBehaviour
{
    public enum PlayerState : int
    {
        Grounded = 0,
        Walking  = 1, 
        Jumping  = 2,
        Falling  = 3
    };

    [SerializeField] 
    Collider2D groundCheck;

    Entity entity;

    Rigidbody2D      rigidbody2d;
    SpriteRenderer   spriteRenderer;
    Animator         animator;

    PlayerState playerState;

    float       moveSpeed = 5.0f;
    float       jumpSpeed = 5.0f;
    float       fallSpeed = 10.0f;

    float       horizontalAxis = 0.0f;

    Vector2     lastPos;

    void Awake()
    {
        rigidbody2d    = GetComponent<Rigidbody2D>();
        animator       = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerState = PlayerState.Grounded;
        entity      = GetComponent<EntityScript>().Entity;

    }

    private void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        horizontalAxis = Input.GetAxis("Horizontal");

        List<Collider2D> colliders = new List<Collider2D>();

        groundCheck.GetContacts(colliders);  

        // Check if player doesn't collide with any surface
        if(colliders.Count == 0) 
        {
            if(playerState != PlayerState.Jumping || rigidbody2d.velocity.y <= 0)
            {
                playerState = PlayerState.Falling;
            }
        }
        else
        {
            if(playerState == PlayerState.Falling)
            {
                float fallDistance = (lastPos.y - transform.position.y);
                entity.OnEntityFall(fallDistance);
            }
            else
            {
                lastPos = transform.position;
            }

            if(playerState != PlayerState.Jumping)
            {
                playerState = horizontalAxis != 0 ? PlayerState.Walking : PlayerState.Grounded;

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    playerState = PlayerState.Jumping;
                }
            }

        }

        animator.SetInteger("PlayerState", (int)playerState);

        if(horizontalAxis > 0)      spriteRenderer.flipX = false;
        else if(horizontalAxis < 0) spriteRenderer.flipX = true;
    }

    float fallLerp = 0;
    float jumpLerp = 0;

    void FixedUpdate()
    {
        if(playerState == PlayerState.Walking)
        {
            rigidbody2d.velocity = new Vector2(moveSpeed * horizontalAxis, 0);
        }
        else if (playerState == PlayerState.Falling)
        {
            rigidbody2d.velocity = new Vector2(moveSpeed * horizontalAxis,  -Mathf.Lerp(fallSpeed/2, fallSpeed, fallLerp / 50.0f));
        }
        else if (playerState == PlayerState.Jumping)
        {
            rigidbody2d.velocity = new Vector2(moveSpeed * horizontalAxis, Mathf.Lerp(jumpSpeed, 0, jumpLerp / 25.0f));
        }
        else
        {
            rigidbody2d.velocity = Vector2.zero;
        }

        fallLerp = playerState == PlayerState.Falling ? (fallLerp + 1) : 0;
        jumpLerp = playerState == PlayerState.Jumping ? (jumpLerp + 1) : 0;

    }

}
