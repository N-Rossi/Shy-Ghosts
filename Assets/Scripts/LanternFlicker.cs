using UnityEngine;
using UnityEngine.Rendering.Universal;

// Jitters the lantern's intensity while the battery is low. Visual only: ghost platforms ignore it.
[RequireComponent(typeof(Light2D), typeof(LanternBattery))]
public class LanternFlicker : MonoBehaviour
{
    [Tooltip("Lowest intensity during normal flicker, as a fraction of the base intensity.")]
    [SerializeField, Range(0f, 1f)] private float minIntensityFactor = 0.5f;
    [SerializeField] private float flickerSpeed = 12f;
    [Tooltip("Chance per second of a short near-blackout dip at the low threshold. Doubles as charge nears 0.")]
    [SerializeField] private float dipChancePerSecond = 0.6f;
    [SerializeField] private float dipDuration = 0.06f;
    [SerializeField, Range(0f, 1f)] private float dipIntensityFactor = 0.15f;

    private Light2D light2D;
    private LanternBattery battery;
    private float baseIntensity;
    private float dipTimer;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        battery = GetComponent<LanternBattery>();
        baseIntensity = light2D.intensity;
    }

    private void Update()
    {
        if (!battery.IsLow)
        {
            light2D.intensity = baseIntensity;
            dipTimer = 0f;
            return;
        }

        // 0 at the low threshold, 1 when empty.
        float severity = 1f - battery.Charge / battery.LowThreshold;

        // Perlin noise wanders smoothly between 0 and 1, so the light wavers instead of strobing.
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        float factor = Mathf.Lerp(minIntensityFactor, 1f, noise);

        if (dipTimer > 0f)
        {
            dipTimer -= Time.deltaTime;
            factor *= dipIntensityFactor;
        }
        else if (Random.value < dipChancePerSecond * (1f + severity) * Time.deltaTime)
        {
            dipTimer = dipDuration;
        }

        light2D.intensity = baseIntensity * factor;
    }
}
