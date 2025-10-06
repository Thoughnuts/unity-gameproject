using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float acceleration = 25f;
    public float deceleration = 30f;

    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    
    [Header("Physics Settings")]
    public float gravityScale = 3f;
    public float jumpForce = 15f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private float horizontalInput;
    private Vector2 targetVelocity;
    
    // Dash variables
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float dashCooldownLeft = 0f;
    private Vector2 dashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        SetupRigidbody();
    }

    void SetupRigidbody()
    {
        rb.gravityScale = gravityScale;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // ← CRITICAL FOR SMOOTHNESS
    }

    void Update()
    {
        GetInput();
        HandleSpriteFlip();
        HandleJump();
        HandleDash();
        UpdateDashTimers();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // Use GetAxis for smoother input
    }

    void HandleSpriteFlip()
    {
        if (horizontalInput > 0.1f)
            spriteRenderer.flipX = false;
        else if (horizontalInput < -0.1f)
            spriteRenderer.flipX = true;
    }

    void HandleJump()
    {
        isGrounded = CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void HandleDash()
    {
        // Check if dash is available and player presses Shift
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownLeft <= 0f && !isDashing)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownLeft = dashCooldown;
        
        // Determine dash direction based on input or facing direction
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            dashDirection = new Vector2(horizontalInput, 0f).normalized;
        }
        else
        {
            // Dash in facing direction if no horizontal input
            dashDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        }
        
        // Apply dash force
        rb.velocity = dashDirection * dashForce;
        
        // Optional: Disable gravity during dash for better control
        rb.gravityScale = 0f;
    }

    void EndDash()
    {
        isDashing = false;
        rb.gravityScale = gravityScale; // Restore gravity
    }

    void UpdateDashTimers()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
            {
                EndDash();
            }
        }
        
        if (dashCooldownLeft > 0f)
        {
            dashCooldownLeft -= Time.deltaTime;
        }
    }

    bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down,
                                           groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    void FixedUpdate()
    {
        // Only handle normal movement if not dashing
        if (!isDashing)
        {
            HandleMovement();
        }
        LimitVerticalVelocity();
    }

    void HandleMovement()
    {
        // Calculate target velocity
        targetVelocity.x = horizontalInput * moveSpeed;
        targetVelocity.y = rb.velocity.y; // Keep current Y velocity

        // Smoothly interpolate towards target velocity
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Quick stop when no input
        if (Mathf.Abs(horizontalInput) < 0.1f)
        {
            rb.velocity = new Vector2(
                Mathf.Lerp(rb.velocity.x, 0, deceleration * Time.fixedDeltaTime),
                rb.velocity.y
            );
        }
    }

    void LimitVerticalVelocity()
    {
        // Prevent excessive falling speed
        if (rb.velocity.y < -20f)
        {
            rb.velocity = new Vector2(rb.velocity.x, -20f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
    
    // Public methods to check dash state (useful for other scripts)
    public bool IsDashing()
    {
        return isDashing;
    }
    
    public float GetDashCooldownPercent()
    {
        return Mathf.Clamp01(1f - (dashCooldownLeft / dashCooldown));
    }
}