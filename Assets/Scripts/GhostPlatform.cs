using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class GhostPlatform : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float litAlpha = 0.5f;
    [SerializeField] private AudioClip shriekClip;
    [SerializeField] private float flinchScale = 0.85f;
    [SerializeField] private float flinchDuration = 0.15f;
    [SerializeField] private float sampleSpacing = 0.25f;

    private SpriteRenderer sprite;
    private Collider2D col;
    private AudioSource audioSource;
    private Lantern lantern;
    private Vector3 baseScale;
    private Bounds bounds;
    private Vector2[] samplePoints;
    private bool isLit;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        baseScale = transform.localScale;
        // Cached: a disabled collider reports empty bounds.
        bounds = col.bounds;
        // Built once: allocating an array every frame makes GC stutter on Web.
        samplePoints = BuildSamplePoints();
    }

    private Vector2[] BuildSamplePoints()
    {
        var points = new List<Vector2>();
        Vector2 bottomLeft = bounds.min;
        Vector2 topRight = bounds.max;
        Vector2 bottomRight = new Vector2(topRight.x, bottomLeft.y);
        Vector2 topLeft = new Vector2(bottomLeft.x, topRight.y);

        AddEdge(points, bottomLeft, bottomRight);
        AddEdge(points, bottomRight, topRight);
        AddEdge(points, topRight, topLeft);
        AddEdge(points, topLeft, bottomLeft);
        points.Add(bounds.center);

        samplePoints = points.ToArray();
        return points.ToArray();
    }

    // Adds points from 'from' up to (not including) 'to'. The next edge adds 'to'.
    private void AddEdge(List<Vector2> points, Vector2 from, Vector2 to)
    {
        int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(from, to) / sampleSpacing));
        for (int i = 0; i < steps; i++)
            points.Add(Vector2.Lerp(from, to, (float)i / steps));
    }

    private void Start()
    {
        lantern = FindAnyObjectByType<Lantern>();
        isLit = CheckLit();
        ApplyState();   // no shriek on load
    }

    private void Update()
    {
        bool lit = CheckLit();
        if (lit == isLit) return;

        isLit = lit;
        ApplyState();
        if (isLit) Flinch();
    }

    private bool CheckLit()
    {
        if (lantern == null || !lantern.IsOn) return false;
        if (IsPointInCone(bounds.ClosestPoint(lantern.Origin))) return true;

        foreach (Vector2 point in samplePoints)
        {
            if (IsPointInCone(point)) return true;
        }

        return false;
    }

    private bool IsPointInCone(Vector2 point)
    {
        Vector2 toPoint = point - lantern.Origin;
        if (toPoint.sqrMagnitude > lantern.Range * lantern.Range) return false;
        return Vector2.Angle(lantern.Direction, toPoint) <= lantern.HalfAngle;
    }

    private void ApplyState()
    {
        col.enabled = !isLit;
        Color c = sprite.color;
        c.a = isLit ? litAlpha : 0;
        sprite.color = c;
    }

    private void Flinch()
    {
        if (audioSource != null && shriekClip != null)
            audioSource.PlayOneShot(shriekClip);

        StopAllCoroutines();
        StartCoroutine(FlinchRoutine());
    }

    private IEnumerator FlinchRoutine()
    {
        float t = 0f;
        while (t < flinchDuration)
        {
            t += Time.deltaTime;
            // 0 -> 1 -> 0 over the duration: shrink then recover
            float squash = Mathf.Sin(t / flinchDuration * Mathf.PI);
            transform.localScale = baseScale * Mathf.Lerp(1f, flinchScale, squash);
            yield return null;
        }
        transform.localScale = baseScale;
    }
}