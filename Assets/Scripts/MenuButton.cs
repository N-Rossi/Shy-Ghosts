using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Text-only menu button: the label lights up in the lantern color and gets "> <" pointers
// when hovered or selected. Replaces Unity's default white boxes.
[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color normalColor = new Color32(0xB8, 0xAE, 0xC8, 0xFF);
    [SerializeField] private Color highlightColor = new Color32(0xFF, 0xC8, 0x78, 0xFF);
    [Tooltip("Shown around the label when highlighted. Both sides, so centered text doesn't jump.")]
    [SerializeField] private string pointerLeft = "> ";
    [SerializeField] private string pointerRight = " <";

    private string baseText;

    private void Awake()
    {
        if (label == null) label = GetComponentInChildren<TMP_Text>();
        baseText = label.text;
        SetHighlighted(false);
    }

    // A hidden panel must not come back with a button still lit.
    private void OnDisable() => SetHighlighted(false);

    public void OnPointerEnter(PointerEventData eventData) => SetHighlighted(true);
    public void OnPointerExit(PointerEventData eventData) => SetHighlighted(false);
    public void OnSelect(BaseEventData eventData) => SetHighlighted(true);
    public void OnDeselect(BaseEventData eventData) => SetHighlighted(false);

    private void SetHighlighted(bool highlighted)
    {
        if (label == null) return;
        label.color = highlighted ? highlightColor : normalColor;
        label.text = highlighted ? pointerLeft + baseText + pointerRight : baseText;
    }
}
