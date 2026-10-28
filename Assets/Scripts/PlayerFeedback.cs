using UnityEngine;

// Game-feel extras: dust puffs and sounds when the player jumps and lands.
[RequireComponent(typeof(PlayerController))]
public class PlayerFeedback : MonoBehaviour
{
    [Header("Dust (child Particle Systems at the feet)")]
    [SerializeField] private ParticleSystem jumpDust;
    [SerializeField] private ParticleSystem landDust;

    [Header("Sound")]
    [SerializeField] private AudioSource source;
    [Tooltip("Several clips = one picked at random each time, so it doesn't get repetitive.")]
    [SerializeField] private AudioClip[] jumpClips;
    [SerializeField] private AudioClip[] landClips;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField, Range(0f, 1f)] private float jumpVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float landVolume = 0.7f;
    [SerializeField, Range(0f, 1f)] private float footstepVolume = 0.3f;
    [Tooltip("Random pitch +/- per play. Small variation keeps repeated sounds from feeling robotic.")]
    [SerializeField, Range(0f, 0.3f)] private float pitchVariation = 0.08f;

    [Header("Landing")]
    [Tooltip("Landings slower than this (units/sec) are silent, e.g. stepping off a 1-tile ledge.")]
    [SerializeField] private float minLandSpeed = 4f;
    [Tooltip("Fall speed that gives the full landing volume and dust.")]
    [SerializeField] private float hardLandSpeed = 16f;

    [Header("Footsteps")]
    [Tooltip("Seconds between steps at full run speed. The run clip is 6 frames at 12 fps = 0.5 s with 2 steps, so 0.25 matches it.")]
    [SerializeField] private float stepInterval = 0.25f;
    [Tooltip("Slower than this (units/sec) counts as standing: no steps.")]
    [SerializeField] private float minStepSpeed = 1f;
    [SerializeField] private float fullRunSpeed = 7f;

    private PlayerController controller;
    private Rigidbody2D body;
    private float stepTimer;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float speed = Mathf.Abs(body.linearVelocityX);
        if (!controller.IsGrounded || speed < minStepSpeed || GameState.InputBlocked)
        {
            // First step lands a moment after you start moving, not on the same frame.
            stepTimer = stepInterval * 0.5f;
            return;
        }

        // Steps come faster as you speed up, like real feet.
        stepTimer -= Time.deltaTime * Mathf.Clamp01(speed / fullRunSpeed);
        if (stepTimer > 0f) return;

        stepTimer += stepInterval;
        PlayRandom(footstepClips, footstepVolume);
    }

    private void OnEnable()
    {
        controller.Jumped += OnJumped;
        controller.Landed += OnLanded;
    }

    private void OnDisable()
    {
        controller.Jumped -= OnJumped;
        controller.Landed -= OnLanded;
    }

    private void OnJumped()
    {
        if (jumpDust != null) jumpDust.Play();
        PlayRandom(jumpClips, jumpVolume);
    }

    private void OnLanded(float fallSpeed)
    {
        // The landing sound is the first step; don't double it with a footstep.
        stepTimer = stepInterval;
        if (fallSpeed < minLandSpeed) return;

        // 0 for a soft landing, 1 for a long fall: louder sound and more dust.
        float impact = Mathf.InverseLerp(minLandSpeed, hardLandSpeed, fallSpeed);

        if (landDust != null)
        {
            var burst = landDust.emission.GetBurst(0);
            burst.count = Mathf.RoundToInt(Mathf.Lerp(4f, 12f, impact));
            landDust.emission.SetBurst(0, burst);
            landDust.Play();
        }
        PlayRandom(landClips, landVolume * Mathf.Lerp(0.5f, 1f, impact));
    }

    private void PlayRandom(AudioClip[] clips, float volume)
    {
        if (source == null || clips == null || clips.Length == 0) return;
        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
    }
}
