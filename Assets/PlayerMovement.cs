using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask JumpableGround;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float maxFallSpeed = 20f; // Maximum fall speed
    [SerializeField] private float gravityMultiplier = 2f; // Multiplier to increase fall speed
    private bool canDash = true;
    private bool isDashing;
    private float dashSpeed = 24f;
    private float dashTime = 0.2f;
    private float dashCooldown = 0.2f;
    private bool hasPickedUpItem = false;
    [SerializeField] private float jumpTime;
    private float jumpTimeCounter;
    private bool isJumping;
    private float groundDashCooldown = 0f;
    private bool hasDashedInAir = false;

    private int doubleJump;
    [SerializeField] private int doubleJumpV;
    [SerializeField] private int doubleJumpF;

    // New variables for apex modifier
    private float _jumpApexThreshold = 0.7f; // Adjust as needed
    private float _apexBonus = 13f; // Adjust as needed
    private float _minFallSpeed = 5f; // Adjust as needed
    private float _maxFallSpeed = 20f; // Adjust as needed

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

        // Calculate apex point
        float _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(rb.velocity.y));

        // Calculate apex bonus only if the player is jumping or falling
        float apexBonus = 0f;
        if (_apexPoint > 0 && !IsGrounded())
        {
            apexBonus = Mathf.Sign(rb.velocity.y) * _apexBonus * (1 - Mathf.Abs(_apexPoint - 0.5f) * 2); // Adjust this formula as needed
        }

        // Apply apexBonus to vertical velocity
        rb.velocity += Vector2.up * apexBonus * Time.deltaTime;

        // Calculate current horizontal speed
        float _currentHorizontalSpeed = moveSpeed + apexBonus;

        rb.velocity = new Vector2(directionX * _currentHorizontalSpeed, rb.velocity.y);

        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX, 1, 1);
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTimeCounter = jumpTime;
            isJumping = true;
            hasDashedInAir = false;
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

        if (Input.GetButtonDown("Dash"))
        {
            if (!IsGrounded() && canDash && !hasDashedInAir) // Add condition to check if already dashed in the air
            {
                StartCoroutine(Dash());
                canDash = false;
                hasDashedInAir = true; // Set flag to true after dashing in the air
            }
            else if (IsGrounded() && Time.time >= groundDashCooldown)
            {
                StartCoroutine(Dash());
                groundDashCooldown = Time.time + 2f; // Set cooldown for dashing on the ground
            }

        }
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
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