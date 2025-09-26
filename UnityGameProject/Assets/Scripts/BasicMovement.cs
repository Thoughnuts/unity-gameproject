using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float acceleration = 25f;
    public float deceleration = 30f;

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

    bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down,
                                           groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    void FixedUpdate()
    {
        HandleMovement();
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
}