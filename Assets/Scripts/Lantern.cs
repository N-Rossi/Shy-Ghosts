using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Lantern : MonoBehaviour
{
    private Light2D light2D;
    private Camera cam;
    private LanternBattery battery;

    // Ghost platforms will read these next step.
    public bool IsOn => light2D.enabled;
    public Vector2 Origin => transform.position;
    public Vector2 Direction => transform.up;
    public float Range => light2D.pointLightOuterRadius;
    public float HalfAngle => light2D.pointLightOuterAngle * 0.5f;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        cam = Camera.main;
        battery = GetComponent<LanternBattery>();
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        Aim(mouse);
        if (mouse.rightButton.wasPressedThisFrame)
        {
            bool wantsOn = !light2D.enabled;
            // A dead battery keeps the lantern off until a candle refills it.
            if (wantsOn && battery != null && battery.IsEmpty) return;
            light2D.enabled = wantsOn;
        }
    }

    public void TurnOff() => light2D.enabled = false;

    private void Aim(Mouse mouse)
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouse.position.ReadValue());
        Vector2 dir = mouseWorld - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}