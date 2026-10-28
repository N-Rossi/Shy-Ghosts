using UnityEngine;

// Put in the last room. Touching it ends the game and opens the end screen.
[RequireComponent(typeof(Collider2D))]
public class EndTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Found at touch time: room prefabs can't hold references to objects in the scene.
        EndScreen endScreen = FindAnyObjectByType<EndScreen>();
        if (endScreen != null) endScreen.Show();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
