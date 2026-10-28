using UnityEngine;

// One screen of the level. The root sits at the room's bottom-left corner; rooms tile every 30 x 17 units.
public class Room : MonoBehaviour
{
    public static readonly Vector2 Size = new Vector2(30f, 17f);
    // 480x270 at 16 PPU shows 30 x 16.875 units; this keeps the bottom row fully on screen.
    private static readonly Vector2 CameraOffset = new Vector2(15f, 8.4375f);

    [SerializeField] private Transform spawnPoint;
    [Tooltip("Set the battery to a fixed charge on entering or resetting this room. Off = keep the charge you entered with.")]
    [SerializeField] private bool useFixedStartCharge;
    [SerializeField, Range(0f, 1f)] private float fixedStartCharge = 1f;
    [Tooltip("Scales battery drain in this room. 1 = normal, 0 = lantern never drains (tutorial rooms).")]
    [SerializeField, Min(0f)] private float batteryDrainMultiplier = 1f;

    [Header("Storm")]
    [Tooltip("Lightning flashes light the whole room on a timer.")]
    [SerializeField] private bool hasStorm;
    [Tooltip("Seconds from one flash to the next. Keep it steady so players can learn the rhythm.")]
    [SerializeField, Min(0.5f)] private float flashInterval = 3f;
    [Tooltip("Random +/- seconds added to each interval. 0 = perfectly regular.")]
    [SerializeField, Min(0f)] private float flashJitter = 0f;
    [Tooltip("Seconds each flash counts as light (ghost platforms visible and not solid).")]
    [SerializeField, Min(0.05f)] private float flashDuration = 0.25f;
    [Tooltip("Lantern can't be turned on in this room.")]
    [SerializeField] private bool lanternBroken;

    public Rect Bounds => new Rect(transform.position, Size);
    public Vector2 CameraPosition => (Vector2)transform.position + CameraOffset;
    public Vector2 SpawnPosition => spawnPoint != null ? (Vector2)spawnPoint.position : Bounds.center;
    public bool UsesFixedStartCharge => useFixedStartCharge;
    public float FixedStartCharge => fixedStartCharge;
    public float BatteryDrainMultiplier => batteryDrainMultiplier;
    public bool HasStorm => hasStorm;
    public float FlashInterval => flashInterval;
    public float FlashJitter => flashJitter;
    public float FlashDuration => flashDuration;
    public bool LanternBroken => lanternBroken;

    public bool Contains(Vector2 point) => Bounds.Contains(point);

    public void ResetRoom()
    {
        // true = include inactive objects, so collected candles come back.
        foreach (IResettable resettable in GetComponentsInChildren<IResettable>(true))
            resettable.ResetState();
    }

    // Editor-only drawing: room outline and spawn point in the Scene view.
    private void OnDrawGizmos()
    {
        Rect bounds = Bounds;
        // Cyan outline marks storm rooms.
        Gizmos.color = hasStorm ? Color.cyan : Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);

        Gizmos.color = spawnPoint != null ? Color.green : Color.red;
        Gizmos.DrawWireSphere(SpawnPosition, 0.4f);
    }
}
