using UnityEngine;

// Kills the player on contact. Use for pits, later spikes.
[RequireComponent(typeof(Collider2D))]
public class KillZone : MonoBehaviour
{
    // TODO: OnTriggerEnter2D(Collider2D other)
    //       - ignore anything not tagged "Player"
    //       - RoomManager.Instance.KillPlayer()

    private void OnTriggerEnter2D(Collider2D other) => TryKill(other);

    private void TryKill(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        RoomManager.Instance.KillPlayer();
    }

}