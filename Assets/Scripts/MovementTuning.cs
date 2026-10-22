using UnityEngine;

// All player movement tuning in one asset. Edits made in Play mode persist after stopping.
[CreateAssetMenu(fileName = "MovementTuning", menuName = "Shy Ghosts/Movement Tuning")]
public class MovementTuning : ScriptableObject
{
    [Header("Run")]
    [Tooltip("Top horizontal speed, units/sec.")]
    public float maxRunSpeed = 7f;
    [Tooltip("Units/sec² toward max speed while holding a direction on the ground.")]
    public float groundAcceleration = 80f;
    [Tooltip("Units/sec² toward zero with no input on the ground.")]
    public float groundDeceleration = 90f;
    public float airAcceleration = 50f;
    public float airDeceleration = 30f;

    [Header("Jump")]
    [Tooltip("Upward velocity applied on jump.")]
    public float jumpVelocity = 14f;
    [Tooltip("Upward velocity is multiplied by this when Space is released early.")]
    [Range(0f, 1f)] public float jumpCutMultiplier = 0.5f;
    [Tooltip("Seconds after leaving a ledge where a jump still works.")]
    public float coyoteTime = 0.1f;
    [Tooltip("Seconds a jump press is remembered before landing.")]
    public float jumpBufferTime = 0.1f;

    [Header("Gravity")]
    [Tooltip("Rigidbody2D gravity scale while moving up.")]
    public float riseGravityScale = 3f;
    [Tooltip("Rigidbody2D gravity scale while falling. Higher than rise = snappier arc.")]
    public float fallGravityScale = 4.5f;
    [Tooltip("Max downward speed, units/sec.")]
    public float maxFallSpeed = 18f;

    [Header("Ground check")]
    [Tooltip("How far below the collider to probe for ground.")]
    public float groundCheckDistance = 0.05f;
}
