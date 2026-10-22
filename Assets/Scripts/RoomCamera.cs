using UnityEngine;

// Moves the camera between rooms. Ease Duration 0 = instant snap.
public class RoomCamera : MonoBehaviour
{
    [SerializeField] private float easeDuration = 0.25f;

    private Vector3 from;
    private Vector3 to;
    private float progress = 1f;

    public void MoveTo(Vector2 position, bool instant)
    {
        to = new Vector3(position.x, position.y, transform.position.z);

        if (instant || easeDuration <= 0f)
        {
            transform.position = to;
            progress = 1f;
            return;
        }

        from = transform.position;
        progress = 0f;
    }

    // LateUpdate runs after every Update, so the camera moves after the player has.
    private void LateUpdate()
    {
        if (progress >= 1f) return;

        progress = Mathf.Min(1f, progress + Time.deltaTime / easeDuration);
        transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, progress));
    }
}
