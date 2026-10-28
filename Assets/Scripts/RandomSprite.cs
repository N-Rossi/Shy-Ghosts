using UnityEngine;

// Picks one sprite from a list for this object, e.g. a different view behind each window.
// The pick depends on the object's position, so a window always shows the same view every play.
[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : MonoBehaviour
{
    [SerializeField] private Sprite[] options;

    private void Awake()
    {
        if (options == null || options.Length == 0) return;

        // Position-based instead of Random.Range, so it doesn't change between runs or on room reset.
        Vector2 p = transform.position;
        int hash = Mathf.RoundToInt(p.x * 7f) * 73856093 ^ Mathf.RoundToInt(p.y * 7f) * 19349663;
        int index = (int)((uint)hash % (uint)options.Length);

        GetComponent<SpriteRenderer>().sprite = options[index];
    }
}
