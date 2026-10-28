using System.Collections.Generic;
using UnityEngine;

// Glowing eyes that creep toward the player in the dark and retreat from the lantern.
// Contact death comes from a KillZone on the same object. Passes through walls.
public class Watcher : MonoBehaviour
{
    // Every Watcher currently in play. The heartbeat audio will read this to find the nearest one.
    public static readonly List<Watcher> Active = new List<Watcher>();

    [SerializeField] private float creepSpeed = 1.2f;
    [SerializeField] private float lowBatterySpeedMultiplier = 1.6f;
    [SerializeField] private float retreatSpeed = 3f;
    [Tooltip("Seconds lit in a row before it vanishes. 0 = never vanishes, only retreats.")]
    [SerializeField] private float banishTime = 1.5f;

    private Transform player;
    private Lantern lantern;
    private LanternBattery battery;
    private float litTimer;

    // Switches off every Watcher in play, whichever room or spawner it came from.
    public static void DespawnAll()
    {
        // Copy first: switching a Watcher off removes it from Active.
        foreach (Watcher watcher in Active.ToArray())
            watcher.gameObject.SetActive(false);
    }

    private void Awake()
    {
        lantern = FindAnyObjectByType<Lantern>();
        battery = lantern.GetComponent<LanternBattery>();
        player = FindAnyObjectByType<PlayerController>().transform;
    }

    // OnEnable/OnDisable run every time the object is switched on or off.
    private void OnEnable()
    {
        Active.Add(this);
        litTimer = 0f;
    }

    private void OnDisable()
    {
        Active.Remove(this);
    }

    private void Update()
    {
        Vector2 current = transform.position;
        Vector2 playerPos = player.position;

        if (Lightning.IsFlashing || lantern.Illuminates(current))
        {
            Vector2 away = (current - playerPos).normalized;
            transform.position = current + away * (retreatSpeed * Time.deltaTime);

            litTimer += Time.deltaTime;
            if (banishTime > 0f && litTimer >= banishTime)
                gameObject.SetActive(false);
        }
        else
        {
            litTimer = 0f;

            float speed = creepSpeed;
            if (battery != null && battery.IsLow) speed *= lowBatterySpeedMultiplier;
            transform.position = Vector2.MoveTowards(current, playerPos, speed * Time.deltaTime);
        }
    }
}
