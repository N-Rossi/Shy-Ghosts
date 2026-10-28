using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Esc pauses: resume, restart room, volume, quit to title. Shows deaths and time.
public class PauseMenu : MonoBehaviour
{
    private const string VolumeKey = "volume";

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string titleSceneName = "Title";

    private void Awake()
    {
        panel.SetActive(false);

        // PlayerPrefs on Web is saved in the browser, so the volume is remembered between visits.
        float volume = PlayerPrefs.GetFloat(VolumeKey, 0.8f);
        AudioListener.volume = volume;
        volumeSlider.SetValueWithoutNotify(volume);
        volumeSlider.onValueChanged.AddListener(SetVolume);

        resumeButton.onClick.AddListener(Resume);
        restartButton.onClick.AddListener(RestartRoom);
        quitButton.onClick.AddListener(QuitToTitle);
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null || !kb.escapeKey.wasPressedThisFrame || GameState.IsFinished) return;

        SetPaused(!GameState.IsPaused);
    }

    private void OnDestroy()
    {
        // Leaving the scene while paused must not leave the next scene frozen.
        Time.timeScale = 1f;
        AudioListener.pause = false;
        GameState.IsPaused = false;
    }

    private void Resume() => SetPaused(false);

    private void RestartRoom()
    {
        SetPaused(false);
        RoomManager.Instance.RestartRoom();
    }

    private void QuitToTitle()
    {
        SetPaused(false);
        SceneManager.LoadScene(titleSceneName);
    }

    private void SetPaused(bool paused)
    {
        GameState.IsPaused = paused;
        panel.SetActive(paused);
        // timeScale 0 stops physics, animation, and everything driven by Time.deltaTime.
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;

        if (paused)
            statsText.text = $"Deaths: {RoomManager.Instance.Deaths}\nTime: {GameClock.Format(GameClock.Elapsed)}";
        else
            PlayerPrefs.Save();
    }

    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VolumeKey, volume);
    }
}
