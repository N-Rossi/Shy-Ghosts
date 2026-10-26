using UnityEngine;
using UnityEngine.InputSystem;

// Picks the player's animation from movement state and flips the sprite to face the right way.
// The Animator Controller needs states named exactly Idle, Run, Jump and Fall; no transitions or parameters.
// Optional: an IdleFront state is used after standing still for a moment, if the controller has one.
[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    // Hashed once: Animator looks states up by int, and hashing a string every frame is wasted work.
    private static readonly int IdleState = Animator.StringToHash("Idle");
    private static readonly int IdleFrontState = Animator.StringToHash("IdleFront");
    private static readonly int RunState = Animator.StringToHash("Run");
    private static readonly int JumpState = Animator.StringToHash("Jump");
    private static readonly int FallState = Animator.StringToHash("Fall");

    [Tooltip("Horizontal speed below this counts as standing still.")]
    [SerializeField] private float runThreshold = 0.1f;
    [Tooltip("Seconds standing still before turning to face the camera (IdleFront).")]
    [SerializeField] private float frontIdleDelay = 1f;
    [Tooltip("Pixels the mouse must move in one frame to turn a standing player toward the light.")]
    [SerializeField] private float mouseMoveThreshold = 1f;

    private Animator animator;
    private SpriteRenderer sprite;
    private Rigidbody2D body;
    private PlayerController controller;
    private Lantern lantern;
    private int currentState;
    private float stillTimer;
    private bool hasFrontIdle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
        lantern = GetComponentInChildren<Lantern>();
        // Layer 0 is the controller's only layer. Without the state, standing still just stays on Idle.
        hasFrontIdle = animator.HasState(0, IdleFrontState);
    }

    private void Update()
    {
        if (GameState.InputBlocked) return;

        Vector2 velocity = body.linearVelocity;
        bool standingStill = controller.IsGrounded && Mathf.Abs(velocity.x) <= runThreshold;
        stillTimer = standingStill ? stillTimer + Time.deltaTime : 0f;

        int state;
        if (!controller.IsGrounded)
            state = velocity.y > 0f ? JumpState : FallState;
        else if (!standingStill)
            state = RunState;
        else if (hasFrontIdle && stillTimer >= frontIdleDelay)
            state = IdleFrontState;
        else
            state = IdleState;

        // Play only on change: calling Play every frame would restart the clip at frame 0 forever.
        if (state != currentState)
        {
            animator.Play(state);
            currentState = state;
        }

        UpdateFacing();
    }

    private void UpdateFacing()
    {
        // Front-facing frames are never mirrored.
        if (currentState == IdleFrontState)
        {
            sprite.flipX = false;
            return;
        }

        // Moving: face the way you move. Standing still: keep that facing, and only turn
        // toward the light when the player moves the mouse. (Checking the lantern's angle instead
        // would also fire while sliding to a stop, since the player moves under a still cursor.)
        float direction = controller.MoveInput;
        Mouse mouse = Mouse.current;
        bool mouseMoved = mouse != null && mouse.delta.ReadValue().sqrMagnitude > mouseMoveThreshold * mouseMoveThreshold;
        if (direction == 0f && mouseMoved && lantern != null && lantern.IsOn)
            direction = lantern.Direction.x;

        // The side-view frames face right, so flip when facing left. Near 0 keeps the last facing.
        if (Mathf.Abs(direction) > 0.01f)
            sprite.flipX = direction < 0f;
    }
}
