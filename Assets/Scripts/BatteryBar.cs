using UnityEngine;
using UnityEngine.UI;

// Shows lantern charge on a UI Image set to Image Type: Filled.
public class BatteryBar : MonoBehaviour
{
    [SerializeField] private LanternBattery battery;
    [SerializeField] private Image fill;
    [SerializeField] private Color normalColor = new Color(1f, 0.78f, 0.47f);
    [SerializeField] private Color lowColor = new Color(0.85f, 0.25f, 0.2f);

    private void Update()
    {
        if (battery == null || fill == null) return;

        fill.fillAmount = battery.Charge;
        fill.color = battery.IsLow ? lowColor : normalColor;
    }
}
