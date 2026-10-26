using UnityEngine;
using UnityEngine.InputSystem;

// Tracks which room the player is in, moves the camera, and handles death and restart.
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private Rigidbody2D player;
    [SerializeField] private LanternBattery battery;
    [SerializeField] private RoomCamera roomCamera;

    private Room[] rooms;
    private Room currentRoom;
    private float roomStartCharge;
    private Lantern lantern;

    public int Deaths { get; private set; }
    public Room CurrentRoom => currentRoom;

    // Fires whenever a room begins: on entering it and on every restart or death.
    public event System.Action<Room> RoomStarted;

    private void Awake()
    {
        Instance = this;
        rooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
        lantern = battery.GetComponent<Lantern>();
    }

    private void Start()
    {
        Room startRoom = FindRoomAt(player.position);
        if (startRoom == null)
        {
            Debug.LogWarning("RoomManager: player does not start inside any Room.");
            return;
        }
        EnterRoom(startRoom, instantCamera: true);
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.rKey.wasPressedThisFrame && !GameState.InputBlocked)
            RestartRoom();

        // Outside every room (e.g. falling into a pit) keeps the current room.
        Room room = FindRoomAt(player.position);
        if (room != null && room != currentRoom)
            EnterRoom(room, instantCamera: false);
    }

    public void KillPlayer()
    {
        Deaths++;
        RestartRoom();
    }

    public void RestartRoom()
    {
        if (currentRoom == null) return;

        Watcher.DespawnAll();
        currentRoom.ResetRoom();
        battery.SetCharge(roomStartCharge);

        // Set both so the interpolated Rigidbody doesn't visibly slide to the spawn.
        Vector2 spawn = currentRoom.SpawnPosition;
        player.position = spawn;
        player.transform.position = spawn;
        player.linearVelocity = Vector2.zero;

        RoomStarted?.Invoke(currentRoom);
    }

    private void EnterRoom(Room room, bool instantCamera)
    {
        // Every room starts fresh: no Watcher follows you in, and the room you left is restored.
        Watcher.DespawnAll();
        if (currentRoom != null) currentRoom.ResetRoom();
        currentRoom = room;

        if (room.UsesFixedStartCharge)
            battery.SetCharge(room.FixedStartCharge);
        roomStartCharge = battery.Charge;

        room.ResetRoom();
        lantern.SetBroken(room.LanternBroken);
        roomCamera.MoveTo(room.CameraPosition, instantCamera);

        RoomStarted?.Invoke(room);
    }

    private Room FindRoomAt(Vector2 point)
    {
        foreach (Room room in rooms)
            if (room.Contains(point)) return room;
        return null;
    }
}
