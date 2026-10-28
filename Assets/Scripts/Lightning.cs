using UnityEngine;
using UnityEngine.Rendering.Universal;

// Storm rooms: lightning lights the whole room on a timer. While a flash is up, everything counts as lit:
// ghost platforms show and lose their collision, Watchers get pushed back.
public class Lightning : MonoBehaviour
{
    public static bool IsFlashing { get; private set; }

    [SerializeField] private Light2D globalLight;
    [Tooltip("Global light intensity during a flash. Kept below 1 so it isn't a harsh full-white strobe.")]
    [SerializeField] private float flashIntensity = 0.9f;
    [Tooltip("Seconds the light takes to fade back after a flash. Visual only: platforms go solid the instant the flash ends.")]
    [SerializeField] private float fadeOutTime = 0.35f;

    [Header("Thunder (optional)")]
    [SerializeField] private AudioSource thunderSource;
    [SerializeField] private AudioClip[] thunderClips;
    [SerializeField] private float thunderDelay = 0.4f;

    private float baseIntensity;
    private Room activeRoom;
    private float nextFlashTimer;
    private float flashTimer;
    private float fadeTimer;
    private float thunderTimer = -1f;

    private void Awake()
    {
        baseIntensity = globalLight.intensity;
        IsFlashing = false;
    }

    private void Start()
    {
        // Restart the rhythm on death, so a room always plays out the same way.
        RoomManager.Instance.RoomStarted += ResetStorm;
    }

    private void OnDestroy()
    {
        if (RoomManager.Instance != null) RoomManager.Instance.RoomStarted -= ResetStorm;
        IsFlashing = false;
    }

    private void Update()
    {
        Room room = RoomManager.Instance.CurrentRoom;
        // Also catches the first room, which can start before Start() subscribed to RoomStarted.
        if (room != activeRoom) ResetStorm(room);

        if (room == null || !room.HasStorm)
        {
            IsFlashing = false;
            fadeTimer = 0f;
            globalLight.intensity = baseIntensity;
            return;
        }

        if (IsFlashing)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
                IsFlashing = false;
                fadeTimer = fadeOutTime;
            }
        }
        else
        {
            nextFlashTimer -= Time.deltaTime;
            if (nextFlashTimer <= 0f) StartFlash(room);
        }

        UpdateLight();
        UpdateThunder();
    }

    private void StartFlash(Room room)
    {
        IsFlashing = true;
        flashTimer = room.FlashDuration;
        // Measured from the start of this flash.
        nextFlashTimer = room.FlashInterval + Random.Range(-room.FlashJitter, room.FlashJitter);
        thunderTimer = thunderDelay;
    }

    private void UpdateLight()
    {
        if (IsFlashing)
        {
            globalLight.intensity = flashIntensity;
        }
        else if (fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime;
            globalLight.intensity = Mathf.Lerp(baseIntensity, flashIntensity, Mathf.Max(0f, fadeTimer) / fadeOutTime);
        }
        else
        {
            globalLight.intensity = baseIntensity;
        }
    }

    private void UpdateThunder()
    {
        if (thunderTimer < 0f) return;

        thunderTimer -= Time.deltaTime;
        if (thunderTimer >= 0f) return;

        if (thunderSource != null && thunderClips != null && thunderClips.Length > 0)
            thunderSource.PlayOneShot(thunderClips[Random.Range(0, thunderClips.Length)]);
    }

    private void ResetStorm(Room room)
    {
        activeRoom = room;
        IsFlashing = false;
        fadeTimer = 0f;
        thunderTimer = -1f;
        globalLight.intensity = baseIntensity;
        // A full interval of darkness first, so the player gets their bearings.
        nextFlashTimer = room != null ? room.FlashInterval : 0f;
    }
}
