using UnityEngine;
using UnityEngine.UI;

// Blinks a UI element on and off, e.g. "Click to start". Unscaled time, so it also blinks while paused.
[RequireComponent(typeof(Graphic))]
public class Blink : MonoBehaviour
{
    [SerializeField] private float onTime = 0.8f;
    [SerializeField] private float offTime = 0.4f;

    private Graphic graphic;

    private void Awake() => graphic = GetComponent<Graphic>();

    private void Update()
    {
        graphic.enabled = Time.unscaledTime % (onTime + offTime) < onTime;
    }
}
