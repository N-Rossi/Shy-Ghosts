using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementTuning tuning;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody2D body;
    private BoxCollider2D box;

    private float moveInput;          // -1, 0, 1
    private bool jumpHeld;
    private float coyoteTimer;        // counts down after leaving ground
    private float jumpBufferTimer;    // counts down after pressing jump
    private bool isGrounded;
    private  bool jumpCutApplied;

    private float fastestFall;        // most negative y velocity since leaving the ground

    // Read by PlayerAnimator.
    public bool IsGrounded => isGrounded;
    public float MoveInput => moveInput;

    // Read by PlayerFeedback. Landed passes how fast you were falling (units/sec, positive).
    public event System.Action Jumped;
    public event System.Action<float> Landed;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }

    private void Update()
    { 
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (GameState.InputBlocked)
        {
            // Drop held input so nothing fires the moment the game resumes.
            moveInput = 0f;
            jumpHeld = false;
            jumpBufferTimer = 0f;
            return;
        }

        float input = 0f;
        if (keyboard.aKey.isPressed) input -=1f;
        if (keyboard.dKey.isPressed) input +=1f;
        moveInput = input;
        
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpBufferTimer = tuning.jumpBufferTime;
        }
        jumpHeld = keyboard.spaceKey.isPressed;
        jumpBufferTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyRun();
        ApplyJump();
        ApplyGravity();
    }

    private void CheckGround()
    {
        Vector2 size = new Vector2(box.bounds.size.x * 0.9f, box.bounds.size.y);
        RaycastHit2D hit = Physics2D.BoxCast(box.bounds.center, size, 0f, Vector2.down, tuning.groundCheckDistance, groundMask);
        bool wasGrounded = isGrounded;
        isGrounded = hit.collider != null && body.linearVelocityY <= 0.01f;

        // By the time we're grounded the collision has already zeroed the velocity,
        // so remember the fastest fall while airborne and report that.
        if (!isGrounded) fastestFall = Mathf.Min(fastestFall, body.linearVelocityY);
        if (isGrounded && !wasGrounded)
        {
            Landed?.Invoke(-fastestFall);
            fastestFall = 0f;
        }

        if (isGrounded)
        {
            coyoteTimer = tuning.coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
        }
    }

    private void ApplyRun()
    {
        Vector2 v = body.linearVelocity;
        float target = moveInput * tuning.maxRunSpeed;
        bool hasInput = moveInput != 0f;

        float rate;
        if (isGrounded)
            rate = hasInput ? tuning.groundAcceleration : tuning.groundDeceleration;
        else
            rate = hasInput ? tuning.airAcceleration : tuning.airDeceleration;

        v.x = Mathf.MoveTowards(v.x, target, rate * Time.fixedDeltaTime);
        body.linearVelocity = v;
    }

    private void ApplyJump()
    {
        Vector2 v = body.linearVelocity;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            v.y = tuning.jumpVelocity;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            jumpCutApplied = false;
            Jumped?.Invoke();
        }

        if (!jumpCutApplied && v.y > 0f && !jumpHeld)
        {
            v.y *= tuning.jumpCutMultiplier;
            jumpCutApplied = true;
        }

        body.linearVelocity = v;
    }

    private void ApplyGravity()
    {
        Vector2 v = body.linearVelocity;
        body.gravityScale = v.y > 0f ? tuning.riseGravityScale : tuning.fallGravityScale;

        if (v.y < -tuning.maxFallSpeed)
        {
            v.y = -tuning.maxFallSpeed;
            body.linearVelocity = v;
        }
    }
}