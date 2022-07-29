using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
     Controls controls;
     Rigidbody2D playerRB;
     Animator animator;
    [SerializeField] Camera playerCamera;
    [SerializeField] bool Grounded;
    [SerializeField] bool Attacking;

    [SerializeField] bool AirKicking;

    [SerializeField] bool Crouched;


    [SerializeField] float playerGravityScale;

    public int QueuedAttack { get; set; }

    [SerializeField] int JumpHeight;
    [SerializeField] Vector2 airKickSpeed;
    [SerializeField] Vector2 upperCutSpeed;
    [SerializeField] Vector2 slash1Speed;
    [SerializeField] Vector2 slideSpeed;
    [SerializeField] bool doubleJump;
    [SerializeField] int Speed;
    [SerializeField] int MaxSpeed;
    [SerializeField] int AirSpeed;
    [SerializeField] GameObject groundedCheckr;
    [SerializeField] LayerMask ground;
    public BoxCollider2D groundColldier;
    public BoxCollider2D wallCollider;


    [SerializeField] CapsuleCollider2D playerCollider;
    [SerializeField] CapsuleCollider2D standCollider;
    [SerializeField] CapsuleCollider2D crouchCollider;
    public bool FacingRight { get; set; }

    [SerializeField] Sprite baseSprite;
    Bounds playerBounds;
    Bounds playerColliderBounds;
    private void Awake()
    {
        FacingRight = true;
        playerBounds = baseSprite.bounds;

        playerRB = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        controls = new Controls();
        controls.Player.Enable();
        controls.Player.Attack.performed += Attack;
        controls.Player.HeavyAttack.performed += HeavyAttack;
        controls.Player.Jump.performed += Jump;
        controls.Player.Crouch.started += Crouch;
        controls.Player.Crouch.canceled += UnCrouch;
        controls.Player.Roll.performed += Roll;

        playerColliderBounds = playerCollider.bounds;
    }

   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        


    }
    private void FixedUpdate()
    {
        Vector2 pVelocity = playerRB.velocity;

        isGrounded();
        Vector2 inputVector = controls.Player.Movement.ReadValue<Vector2>();
      
        
        if (Grounded)
        {
            if (!Crouched)
            {

                playerRB.velocity = new Vector2(inputVector.x * Speed, pVelocity.y);

            }
            doubleJump = true;
            
        }
        else
        {
            if (AirKicking)
            {
                return;
            }

            playerRB.AddForce(new Vector2(inputVector.x, 0) * AirSpeed);
            pVelocity = playerRB.velocity;

            if (Mathf.Abs(pVelocity.x) > MaxSpeed)
            {
                playerRB.velocity = new Vector2(pVelocity.x * 0.90f, pVelocity.y);
            }
            else
            {
                playerRB.velocity = new Vector2(pVelocity.x * 0.99f, pVelocity.y);
            }
           
        }

        float absVelocityX = Mathf.Abs(playerRB.velocity.x);

        if (!Attacking)
        {
            if (Grounded)
            {
                if (Crouched)
                {
                    playerCollider.size = crouchCollider.size;
                    animator.Play("Crouch");
                }
                else if (inputVector.x == 0)
                {
                    animator.Play("Idle");
                }
                else
                {
                    animator.Play("Run");
                }
                
            }
            else
            {
                if (AirKicking)
                {
                    animator.Play("AirKick");

                }
                else
                {

                    animator.Play("Jump");
                }
                playerCollider.size = standCollider.size;

            }
        }
        else
        {
            playerCollider.size = standCollider.size;

        }

        if (inputVector.x > 0)
        {
            if (!FacingRight)
            {
                FacingRight = true;
                playerRB.transform.localScale = new Vector3(1, 1, 1);
                playerRB.transform.position -= new Vector3(1, 0, 0);
            }
        }
        else if(inputVector.x < 0)
        {
            if (FacingRight)
            {
                FacingRight = false;
                playerRB.transform.localScale = new Vector3(-1, 1, 1);
                playerRB.transform.position += new Vector3(1, 0, 0);
            }
        }
        PlayAttack();
        playerCamera.transform.position = new Vector3(playerCollider.transform.position.x + this.gameObject.transform.localScale.x * playerColliderBounds.size.x, playerCollider.transform.position.y, -10);
    }
    public void Attack(InputAction.CallbackContext context)
    {
        QueuedAttack = 1;

    }

    public void AttackEnd()
    {
        Debug.Log("end attack");
        Attacking = false;
        playerRB.gravityScale = playerGravityScale;
    }

    public void HeavyAttack(InputAction.CallbackContext context)
    {

        QueuedAttack = 2;
        
    }
    public void Roll(InputAction.CallbackContext context)
    {

    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (!Attacking)
        {
            if (Grounded)
            {
                if (Crouched)
                {
                    if (FacingRight)
                    {


                        playerRB.velocity = slideSpeed;
                    }
                    else
                    {
                        playerRB.velocity = new Vector2(slideSpeed.x * -1, slideSpeed.y);
                    }
                }
                else
                {
                    playerRB.velocity = new Vector3(playerRB.velocity.x, JumpHeight);
                }
            }
            else
            {
                wallJump();
            }
        }
       
    }

    private void wallJump()
    {
        if (!doubleJump)
        {
            return;
        }
        RaycastHit2D raycast = Physics2D.BoxCast(wallCollider.transform.position, wallCollider.bounds.size, 0f, Vector2.zero, wallCollider.transform.localScale.y, ground);

        if (raycast.collider != null)
        {
            if(raycast.collider.transform.position.x > this.transform.position.x)
            {
                playerRB.velocity = new Vector3(-3, JumpHeight);
               
            }
            else
            {
                playerRB.velocity = new Vector3(3, JumpHeight);

            }

            doubleJump = false;
        }

    }

    private void isGrounded()
    {
        
        RaycastHit2D raycast = Physics2D.BoxCast(groundColldier.transform.position, groundColldier.bounds.size, 0f, Vector2.zero, groundColldier.transform.localScale.y,ground);
        

        if (raycast.collider != null)
        {
            Grounded = true;

            if (AirKicking)
            {
                AirKicking = false;
                Attacking = false;
                playerRB.gravityScale = playerGravityScale;
            }
        }
        else
        {
            Grounded = false;
        }
    
    }

   

    public void Crouch(InputAction.CallbackContext context)
    {
        if (!Grounded)
        {
            QueuedAttack = 3;
            
        }
        Crouched = true;
    }


    private void UnCrouch(InputAction.CallbackContext obj)
    {
        Crouched = false;
        playerCollider.size = standCollider.size;
    }


    public void PlayAttack()
    {
        if (!Attacking)
        {

            
            if (QueuedAttack == 1 || QueuedAttack == 2 || QueuedAttack == 3) 
            {
                Attacking = true;
                if (QueuedAttack == 1)
                {
                    if (Grounded)
                    {
                        playerRB.gravityScale = 0f;
                        if (FacingRight)
                        {

                            playerRB.velocity = slash1Speed;
                        }
                        else
                        {
                            playerRB.velocity = new Vector2(slash1Speed.x * -1, slash1Speed.y);
                        }
                    }
                    animator.Play("Slash1");
                   
                }
                else if (QueuedAttack == 2)
                {
                    playerRB.gravityScale = 0f;
                    if (FacingRight)
                    {
                        playerRB.velocity = upperCutSpeed;
                    }
                    else
                    {
                        playerRB.velocity = new Vector2(upperCutSpeed.x * -1, upperCutSpeed.y);
                    }
                    animator.Play("UpperCut");
                    

                }
                else 
                {
                    animator.Play("AirKick");
                    playerRB.gravityScale = 0f;
                    AirKicking = true;
                    if (FacingRight)
                    {

                        playerRB.velocity = airKickSpeed;
                    }
                    else
                    {
                        playerRB.velocity = new Vector2(airKickSpeed.x * -1, airKickSpeed.y);
                    }
                }
                Debug.Log("start attack");
                QueuedAttack = 0;
                
            }
            
        }
    }
}
