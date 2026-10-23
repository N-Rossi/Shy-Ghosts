using UnityEngine;

// Charge drains while the lantern is lit. At 0 the lantern is forced off until refilled.
[RequireComponent(typeof(Lantern))]
public class LanternBattery : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float startCharge = 1f;
    [Tooltip("Fraction of a full battery used per second while lit. 0.05 = 20 seconds.")]
    [SerializeField] private float drainPerSecond = 0.05f;
    [SerializeField, Range(0f, 1f)] private float lowThreshold = 0.25f;

    private Lantern lantern;

    // 0 = empty, 1 = full.
    public float Charge { get; private set; }
    public float LowThreshold => lowThreshold;
    public bool IsEmpty => Charge <= 0f;
    public bool IsLow => Charge < lowThreshold;

    private void Awake()
    {
        lantern = GetComponent<Lantern>();
        Charge = startCharge;
    }

    private void Update()
    {
        if (!lantern.IsOn) return;

        Charge = Mathf.Max(0f, Charge - drainPerSecond * RoomDrainMultiplier() * Time.deltaTime);
        if (IsEmpty) lantern.TurnOff();
    }

    private static float RoomDrainMultiplier()
    {
        RoomManager manager = RoomManager.Instance;
        if (manager == null || manager.CurrentRoom == null) return 1f;
        return manager.CurrentRoom.BatteryDrainMultiplier;
    }

    // Used by room reset to restore the charge the room started with.
    public void SetCharge(float value)
    {
        Charge = Mathf.Clamp01(value);
    }

    // amount is a fraction of a full battery: 0.5 = half.
    public void Refill(float amount)
    {
        Charge = Mathf.Min(1f, Charge + amount);
    }
}
