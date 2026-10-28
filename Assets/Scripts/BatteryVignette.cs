using UnityEngine;
using UnityEngine.UI;

// Darkens the screen edges as the battery drains. Goes on a full-screen UI RawImage.
// The gradient texture is generated here in code, so there's no art asset to make.
[RequireComponent(typeof(RawImage))]
public class BatteryVignette : MonoBehaviour
{
    [SerializeField] private LanternBattery battery;
    [Tooltip("Texture resolution. Low = chunky pixel bands that match the art.")]
    [SerializeField] private Vector2Int textureSize = new Vector2Int(60, 34);
    [Tooltip("Number of brightness steps in the gradient. 0 = smooth.")]
    [SerializeField] private int bands = 5;

    [Header("Full battery -> empty battery")]
    [SerializeField, Range(0f, 1f)] private float alphaFull = 0.35f;
    [SerializeField, Range(0f, 1f)] private float alphaEmpty = 0.9f;
    [Tooltip("Scale of the image. Above 1 pushes the dark edge off-screen, so the clear middle looks wider.")]
    [SerializeField] private float scaleFull = 1.5f;
    [SerializeField] private float scaleEmpty = 1f;
    [Tooltip("How fast the vignette catches up, in full-range sweeps per second. Softens the jump when a candle refills.")]
    [SerializeField] private float followSpeed = 1.5f;

    private RawImage image;
    private float shown;   // 0 = full battery look, 1 = empty battery look

    private void Awake()
    {
        image = GetComponent<RawImage>();
        image.texture = BuildTexture();
        image.raycastTarget = false;   // never block clicks on the pause menu
    }

    private void Start()
    {
        shown = Drain();
        Apply();
    }

    private void Update()
    {
        shown = Mathf.MoveTowards(shown, Drain(), followSpeed * Time.deltaTime);
        Apply();
    }

    private float Drain() => battery != null ? 1f - battery.Charge : 0f;

    private void Apply()
    {
        Color color = image.color;
        color.a = Mathf.Lerp(alphaFull, alphaEmpty, shown);
        image.color = color;
        transform.localScale = Vector3.one * Mathf.Lerp(scaleFull, scaleEmpty, shown);
    }

    private Texture2D BuildTexture()
    {
        var texture = new Texture2D(textureSize.x, textureSize.y, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        var pixels = new Color32[textureSize.x * textureSize.y];
        for (int y = 0; y < textureSize.y; y++)
        {
            for (int x = 0; x < textureSize.x; x++)
            {
                // -1..1 across the image; the image stretches to the screen, so this is an ellipse on screen.
                float nx = (x + 0.5f) / textureSize.x * 2f - 1f;
                float ny = (y + 0.5f) / textureSize.y * 2f - 1f;
                float distance = Mathf.Sqrt(nx * nx + ny * ny) / Mathf.Sqrt(2f);   // 0 center, 1 corner

                float alpha = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.35f, 1f, distance));
                if (bands > 0) alpha = Mathf.Round(alpha * bands) / bands;

                pixels[y * textureSize.x + x] = new Color32(0, 0, 0, (byte)(alpha * 255f));
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
        return texture;
    }
}
