using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerMovementController : MonoBehaviour
{
    public enum PlayerState
    {
        Grounded,
        Walking,
        Jumping,
        Falling
    };

    [SerializeField]
    PlayerState playerState_;

    [SerializeField] 
    float       moveSpeed_ = 5.0f;
    
    float       jumpSpeed_ = 120.0f;

    Rigidbody2D       rigidbody_;
    SpriteRenderer  spriteRenderer_;
    Animator        animator_;
    
    void Awake()
    {
        rigidbody_      = GetComponent<Rigidbody2D>();
        animator_       = GetComponent<Animator>();
        spriteRenderer_ = GetComponent<SpriteRenderer>();

        playerState_ = PlayerState.Grounded;
       
    }   

    void Update()
    { 
        if(Mathf.Approximately(rigidbody_.velocity.y, 0))
        {
            playerState_ = PlayerState.Grounded;

            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                playerState_ = PlayerState.Walking;
            }
        }
        else
        {
            playerState_ = rigidbody_.velocity.y < 0 ? PlayerState.Falling : PlayerState.Jumping;
        }

        //  animator_.SetInteger("PlayerState", (int)playerState_);

        if(Input.GetKey(KeyCode.A))
        {
            rigidbody_.MovePosition(transform.position + Vector3.left * moveSpeed_ * Time.deltaTime);

            spriteRenderer_.flipX = true;
        }
        
        if(Input.GetKey(KeyCode.D))
        {
            rigidbody_.MovePosition(transform.position + Vector3.right * moveSpeed_ * Time.deltaTime);

            spriteRenderer_.flipX = false;
        }
            
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(playerState_ != PlayerState.Falling && playerState_ != PlayerState.Jumping)
            {
                rigidbody_.MovePosition(transform.position + Vector3.up * jumpSpeed_ * Time.deltaTime);
            }
        }   
    }

    void FixedUpdate()
    {

    }
}
