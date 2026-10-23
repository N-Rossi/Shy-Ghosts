using UnityEngine;

// Placed in a room. Spawns its Watcher once the lantern has been dark for a moment in that room.
public class WatcherSpawner : MonoBehaviour, IResettable
{
    [SerializeField] private Watcher watcherPrefab;
    [Tooltip("Seconds the lantern must stay off before the Watcher appears.")]
    [SerializeField] private float darknessDelay = 1f;
    [Tooltip("Won't spawn while the player is closer than this, so it never appears on top of you.")]
    [SerializeField] private float minPlayerDistance = 3f;

    private Watcher watcher;
    private Room room;
    private Lantern lantern;
    private Transform player;
    private float darkTimer;

    private void Start()
    {
        room = GetComponentInParent<Room>();
        lantern = FindAnyObjectByType<Lantern>();
        player = FindAnyObjectByType<PlayerController>().transform;

        // One Watcher per spawner, created once and reused: no Instantiate during play.
        watcher = Instantiate(watcherPrefab, transform.position, Quaternion.identity, transform);
        watcher.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (watcher.gameObject.activeSelf) return;

        bool inThisRoom = RoomManager.Instance != null && RoomManager.Instance.CurrentRoom == room;
        if (!inThisRoom || lantern.IsOn)
        {
            darkTimer = 0f;
            return;
        }

        darkTimer += Time.deltaTime;
        if (darkTimer < darknessDelay) return;
        if (Vector2.Distance(player.position, transform.position) < minPlayerDistance) return;

        watcher.transform.position = transform.position;
        watcher.gameObject.SetActive(true);
        darkTimer = 0f;
    }

    public void ResetState()
    {
        if (watcher != null) watcher.gameObject.SetActive(false);
        darkTimer = 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
