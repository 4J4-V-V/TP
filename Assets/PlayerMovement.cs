using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask JumpableGround;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    private bool canDash = true;
    private bool isDashing;
    private float dashSpeed = 24f;
    private float dashTime = 0.2f;
    private float dashCooldown = 1f;
    private bool hasPickedUpItem = false;
    [SerializeField] private float jumpTime;
    private float jumpTimeCounter;
    private bool isJumping;

    private int doubleJump;
    [SerializeField] private int doubleJumpV;
    [SerializeField] private int doubleJumpF;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }


    void Update()
    {
        if (isDashing)
        {
            return;
        }

        float directionX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(directionX * moveSpeed, rb.velocity.y);

        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX, 1, 1);
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTimeCounter = jumpTime;
            isJumping = true;
        }
        else if (hasPickedUpItem && !IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if (Input.GetButtonDown("Dash") && canDash)
        {
            StartCoroutine(Dash());
        }
        extraJump();
    }

    private bool IsGrounded()
    {
        bool grounded = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, JumpableGround);
        return grounded;
    }

    public void PickupItem()
    {
        hasPickedUpItem = true;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
void extraJump()
{
    if (Input.GetButtonDown("Jump") && doubleJump > 0 && !IsGrounded())
    {
        rb.velocity = Vector2.up * doubleJumpF;
        doubleJump--; // Decrement double jump counter only for double jumps
    } 
    else if (Input.GetButtonDown("Jump") && IsGrounded())
    {
        rb.velocity = Vector2.up * jumpForce;
        doubleJump = doubleJumpV; // Reset double jump counter when grounded
    }
}
}


