using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    [Header("Double Tap Settings")]
    public float doubleTapTime = 0.3f; // Time window for double tap
    public bool enableDoubleTap = true; // Toggle double tap feature

    [Header("Visual Effects")]
    public ParticleSystem dashParticles;
    public TrailRenderer dashTrail;
    public Color dashColor = new Color(1f, 0.8f, 0.2f, 1f);

    [Header("Input Settings")]
    public KeyCode dashKey = KeyCode.C;

    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;
    private PlayerJump playerJump;

    // Dash state variables
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimeCounter;
    private float dashCooldownCounter;
    private Vector2 dashDirection;
    private Color originalColor;

    // Double tap detection variables
    private float leftTapTimer = 0f;
    private float rightTapTimer = 0f;
    private int leftTapCount = 0;
    private int rightTapCount = 0;

    // Input state
    private bool isHoldingLeft = false;
    private bool isHoldingRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        playerJump = GetComponent<PlayerJump>();

        // Store original color
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Set up trail renderer if it exists
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }
    }

    void Update()
    {
        UpdateInputState();
        HandleInput();
        UpdateDash();
        UpdateCooldown();
        UpdateDoubleTapTimers();
    }

    void UpdateInputState()
    {
        // Check if we're currently holding movement keys
        isHoldingLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        isHoldingRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    }

    void HandleInput()
    {
        // Original C key dash
        if (Input.GetKeyDown(dashKey) && canDash && !isDashing)
        {
            StartDash();
        }

        // Double tap detection
        if (enableDoubleTap && canDash && !isDashing)
        {
            HandleDoubleTap();
        }
    }

    void HandleDoubleTap()
    {
        // Check for left double tap (A key or Left Arrow)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Prevent left dash if currently holding right
            if (isHoldingRight)
            {
                ResetDoubleTapCounters();
                return;
            }

            leftTapCount++;
            leftTapTimer = doubleTapTime;

            // If this is the second tap within the time window
            if (leftTapCount >= 2)
            {
                dashDirection = Vector2.left;
                TriggerDoubleTapDash();
                leftTapCount = 0;
                leftTapTimer = 0f;
            }
        }

        // Check for right double tap (D key or Right Arrow)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Prevent right dash if currently holding left
            if (isHoldingLeft)
            {
                ResetDoubleTapCounters();
                return;
            }

            rightTapCount++;
            rightTapTimer = doubleTapTime;

            // If this is the second tap within the time window
            if (rightTapCount >= 2)
            {
                dashDirection = Vector2.right;
                TriggerDoubleTapDash();
                rightTapCount = 0;
                rightTapTimer = 0f;
            }
        }
    }

    void ResetDoubleTapCounters()
    {
        leftTapCount = 0;
        rightTapCount = 0;
        leftTapTimer = 0f;
        rightTapTimer = 0f;
    }

    void TriggerDoubleTapDash()
    {
        if (canDash && !isDashing)
        {
            StartDash();
        }
    }

    void UpdateDoubleTapTimers()
    {
        // Update left tap timer
        if (leftTapTimer > 0f)
        {
            leftTapTimer -= Time.deltaTime;
            if (leftTapTimer <= 0f)
            {
                leftTapCount = 0; // Reset if time window expires
            }
        }

        // Update right tap timer
        if (rightTapTimer > 0f)
        {
            rightTapTimer -= Time.deltaTime;
            if (rightTapTimer <= 0f)
            {
                rightTapCount = 0; // Reset if time window expires
            }
        }
    }

    void StartDash()
    {
        // If dashDirection wasn't set by double tap, use current input or facing direction
        if (dashDirection == Vector2.zero)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (horizontalInput != 0)
            {
                dashDirection = new Vector2(horizontalInput, 0f).normalized;
            }
            else
            {
                dashDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            }
        }

        // Additional safety check for opposite direction dash
        if (IsAttemptingOppositeDirectionDash())
        {
            dashDirection = Vector2.zero;
            return;
        }

        isDashing = true;
        canDash = false;
        dashTimeCounter = dashDuration;
        dashCooldownCounter = dashCooldown;

        // Disable other movement during dash
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Visual effects
        StartDashEffects();
    }

    bool IsAttemptingOppositeDirectionDash()
    {
        // Check if trying to dash opposite to held direction
        if (dashDirection.x < 0 && isHoldingRight) // Trying to dash left while holding right
        {
            return true;
        }

        if (dashDirection.x > 0 && isHoldingLeft) // Trying to dash right while holding left
        {
            return true;
        }

        return false;
    }

    void UpdateDash()
    {
        if (isDashing)
        {
            dashTimeCounter -= Time.deltaTime;

            // Apply dash velocity - horizontal only, but preserve vertical physics
            rb.velocity = new Vector2(dashDirection.x * dashSpeed, rb.velocity.y);

            // End dash when timer expires
            if (dashTimeCounter <= 0f)
            {
                EndDash();
            }
        }
    }

    void EndDash()
    {
        isDashing = false;
        dashDirection = Vector2.zero; // Reset dash direction

        // Re-enable movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Stop visual effects
        EndDashEffects();

        // Maintain some horizontal momentum after dash (optional)
        if (playerMovement != null)
        {
            rb.velocity = new Vector2(dashDirection.x * dashSpeed * 0.3f, rb.velocity.y);
        }
    }

    void UpdateCooldown()
    {
        if (!canDash && !isDashing)
        {
            dashCooldownCounter -= Time.deltaTime;
            if (dashCooldownCounter <= 0f)
            {
                canDash = true;
            }
        }
    }

    void StartDashEffects()
    {
        // Color change
        if (spriteRenderer != null)
        {
            spriteRenderer.color = dashColor;
        }

        // Trail renderer
        if (dashTrail != null)
        {
            dashTrail.emitting = true;
        }

        // Particles
        if (dashParticles != null)
        {
            // Make particles face dash direction
            var shape = dashParticles.shape;
            shape.rotation = dashDirection.x > 0 ? new Vector3(0, 90, 0) : new Vector3(0, -90, 0);
            dashParticles.Play();
        }
    }

    void EndDashEffects()
    {
        // Restore original color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Trail renderer
        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }
    }

    // Public methods for other scripts to check dash state
    public bool IsDashing()
    {
        return isDashing;
    }

    public bool CanDash()
    {
        return canDash;
    }

    public float GetCooldownPercent()
    {
        return Mathf.Clamp01(1f - (dashCooldownCounter / dashCooldown));
    }

    // Reset dash when grounded (optional feature)
    public void ResetDash()
    {
        if (!isDashing)
        {
            canDash = true;
            dashCooldownCounter = 0f;
        }
    }

    // Call this from your PlayerJump script when landing
    public void OnGrounded()
    {
        ResetDash();
    }
}