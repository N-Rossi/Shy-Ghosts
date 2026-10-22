using UnityEngine;

// Pickup that refills the lantern battery. Deactivates itself when taken; room reset calls ResetCandle.
[RequireComponent(typeof(Collider2D))]
public class Candle : MonoBehaviour
{
    [Tooltip("Fraction of a full battery restored. 0.5 = half.")]
    [SerializeField, Range(0f, 1f)] private float refillAmount = 0.5f;
    [Tooltip("Leave the candle in place if the battery is already full.")]
    [SerializeField] private bool skipWhenFull = true;

    [Header("Idle bob")]
    [SerializeField] private float bobHeight = 0.1f;
    [SerializeField] private float bobSpeed = 2f;

    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPosition + Vector3.up * offset;
    }

    // Unity calls these when a collider enters / stays inside this trigger.
    // Stay matters for skipWhenFull: standing on a candle collects it once the battery drops.
    private void OnTriggerEnter2D(Collider2D other) => TryCollect(other);
    private void OnTriggerStay2D(Collider2D other) => TryCollect(other);

    private void TryCollect(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        LanternBattery battery = other.GetComponentInChildren<LanternBattery>();
        if (battery == null) return;
        if (skipWhenFull && battery.Charge >= 1f) return;

        battery.Refill(refillAmount);
        gameObject.SetActive(false);
    }

    public void ResetCandle()
    {
        transform.position = startPosition;
        gameObject.SetActive(true);
    }
}
