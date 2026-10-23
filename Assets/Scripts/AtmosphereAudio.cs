using UnityEngine;

// Drives the reactive audio layers: a heartbeat that speeds up as the nearest Watcher closes in,
// and whispers that fade in while the lantern is off. The ambient loop needs no code (Play On Awake + Loop).
public class AtmosphereAudio : MonoBehaviour
{
    [Header("Heartbeat")]
    [SerializeField] private AudioSource heartbeatSource;
    [Tooltip("A single heartbeat (one 'ba-dum'). Played repeatedly, faster when a Watcher is near.")]
    [SerializeField] private AudioClip heartbeatClip;
    [Tooltip("No heartbeat when the nearest Watcher is farther than this.")]
    [SerializeField] private float heartbeatMaxDistance = 12f;
    [SerializeField] private float slowestInterval = 1.2f;
    [Tooltip("Keep above the gap between the clip's two thumps (~0.35 s for Heartbeat.wav) or beats overlap.")]
    [SerializeField] private float fastestInterval = 0.55f;
    [SerializeField, Range(0f, 1f)] private float heartbeatMaxVolume = 0.8f;

    [Header("Whispers")]
    [Tooltip("Looping AudioSource with the whisper clip assigned.")]
    [SerializeField] private AudioSource whisperSource;
    [SerializeField, Range(0f, 1f)] private float whisperVolume = 0.35f;
    [Tooltip("Volume change per second while fading in or out.")]
    [SerializeField] private float whisperFadeSpeed = 0.5f;

    private Transform player;
    private Lantern lantern;
    private float beatTimer;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        lantern = FindAnyObjectByType<Lantern>();

        if (whisperSource != null)
        {
            whisperSource.loop = true;
            whisperSource.volume = 0f;
            if (whisperSource.clip != null && !whisperSource.isPlaying) whisperSource.Play();
        }
    }

    private void Update()
    {
        UpdateHeartbeat();
        UpdateWhispers();
    }

    private void UpdateHeartbeat()
    {
        float nearest = NearestWatcherDistance();
        if (nearest > heartbeatMaxDistance)
        {
            // Zero, so the first beat lands the moment a Watcher comes in range.
            beatTimer = 0f;
            return;
        }

        beatTimer -= Time.deltaTime;
        if (beatTimer > 0f) return;

        // 0 at max distance, 1 when touching.
        float closeness = 1f - nearest / heartbeatMaxDistance;
        beatTimer = Mathf.Lerp(slowestInterval, fastestInterval, closeness);

        if (heartbeatSource != null && heartbeatClip != null)
            heartbeatSource.PlayOneShot(heartbeatClip, Mathf.Lerp(0.2f, 1f, closeness) * heartbeatMaxVolume);
    }

    private void UpdateWhispers()
    {
        if (whisperSource == null) return;

        float target = lantern.IsOn ? 0f : whisperVolume;
        whisperSource.volume = Mathf.MoveTowards(whisperSource.volume, target, whisperFadeSpeed * Time.deltaTime);
    }

    private float NearestWatcherDistance()
    {
        float nearest = float.PositiveInfinity;
        foreach (Watcher watcher in Watcher.Active)
        {
            float distance = Vector2.Distance(player.position, watcher.transform.position);
            if (distance < nearest) nearest = distance;
        }
        return nearest;
    }
}
