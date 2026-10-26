using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Title scene. Any click or key starts the game. The click is also what lets the browser start audio.
public class TitleScreen : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";

    private void Update()
    {
        Mouse mouse = Mouse.current;
        Keyboard kb = Keyboard.current;

        bool clicked = mouse != null && (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame);
        bool keyed = kb != null && kb.anyKey.wasPressedThisFrame;

        if (clicked || keyed)
            SceneManager.LoadScene(gameSceneName);
    }
}
