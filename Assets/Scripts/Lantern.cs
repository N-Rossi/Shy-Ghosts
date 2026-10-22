using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Lantern : MonoBehaviour
{
    private Light2D light2D;
    private Camera cam;

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
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        Aim(mouse);
        if(mouse.rightButton.wasPressedThisFrame)
        {
            light2D.enabled = !light2D.enabled;
        }
    }

    private void Aim(Mouse mouse)
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouse.position.ReadValue());
        Vector2 dir = mouseWorld - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}