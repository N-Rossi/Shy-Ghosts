using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Tracks which room the player is in, moves the camera, and handles death and restart.
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [SerializeField] private Rigidbody2D player;
    [SerializeField] private LanternBattery battery;
    [SerializeField] private RoomCamera roomCamera;

    [Header("Editor testing only (ignored in builds)")]
    [Tooltip("Start Play mode in this room instead of where the Player is placed. Leave empty for the normal start.")]
    [SerializeField] private Room debugStartRoom;
    [Tooltip("In the editor, [ and ] jump to the previous / next room.")]
    [SerializeField] private bool debugRoomSkipKeys = true;

    private Room[] rooms;
    private Room currentRoom;
    private float roomStartCharge;
    private Lantern lantern;

    public int Deaths { get; private set; }
    public Room CurrentRoom => currentRoom;

    // Fires whenever a room begins: on entering it and on every restart or death.
    public event Action<Room> RoomStarted;

    private void Awake()
    {
        Instance = this;
        rooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
        // Play order: left to right, then bottom to top. Used by the editor room-skip keys.
        Array.Sort(rooms, (a, b) =>
        {
            int byX = a.transform.position.x.CompareTo(b.transform.position.x);
            return byX != 0 ? byX : a.transform.position.y.CompareTo(b.transform.position.y);
        });
        lantern = battery.GetComponent<Lantern>();
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (debugStartRoom != null)
        {
            MovePlayerTo(debugStartRoom.SpawnPosition);
            EnterRoom(debugStartRoom, instantCamera: true);
            return;
        }
#endif
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

#if UNITY_EDITOR
        if (debugRoomSkipKeys && kb != null && !GameState.InputBlocked)
        {
            if (kb.leftBracketKey.wasPressedThisFrame) SkipRoom(-1);
            if (kb.rightBracketKey.wasPressedThisFrame) SkipRoom(+1);
        }
#endif

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
        MovePlayerTo(currentRoom.SpawnPosition);

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

    private void MovePlayerTo(Vector2 position)
    {
        // Set both so the interpolated Rigidbody doesn't visibly slide there.
        player.position = position;
        player.transform.position = position;
        player.linearVelocity = Vector2.zero;
    }

#if UNITY_EDITOR
    private void SkipRoom(int step)
    {
        int index = Array.IndexOf(rooms, currentRoom);
        int target = Mathf.Clamp(index + step, 0, rooms.Length - 1);
        if (target == index) return;

        // Skipping ahead with an empty battery would strand you, so top it up.
        battery.SetCharge(1f);
        MovePlayerTo(rooms[target].SpawnPosition);
        EnterRoom(rooms[target], instantCamera: true);
    }
#endif

    private Room FindRoomAt(Vector2 point)
    {
        foreach (Room room in rooms)
            if (room.Contains(point)) return room;
        return null;
    }
}
