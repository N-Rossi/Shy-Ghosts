using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Shown when the player reaches the EndTrigger: total deaths, total time, thanks, credits.
public class EndScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private Button titleButton;
    [SerializeField] private string titleSceneName = "Title";

    private void Awake()
    {
        panel.SetActive(false);
        titleButton.onClick.AddListener(() => SceneManager.LoadScene(titleSceneName));
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    public void Show()
    {
        if (GameState.IsFinished) return;

        GameState.IsFinished = true;
        Time.timeScale = 0f;
        Watcher.DespawnAll();

        statsText.text = $"Deaths: {RoomManager.Instance.Deaths}\nTime: {GameClock.Format(GameClock.Elapsed)}";
        panel.SetActive(true);
    }
}
