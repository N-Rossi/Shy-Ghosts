using UnityEngine;

// Total play time for the pause menu and end screen. Put one in the game scene (on GameManager).
public class GameClock : MonoBehaviour
{
    public static float Elapsed { get; private set; }

    private void Awake()
    {
        Elapsed = 0f;
        GameState.Reset();
    }

    private void Update()
    {
        // deltaTime is 0 while paused (timeScale 0), so pauses don't count.
        if (!GameState.IsFinished) Elapsed += Time.deltaTime;
    }

    // 83.456 -> "1:23.45"
    public static string Format(float seconds)
    {
        int minutes = (int)(seconds / 60f);
        return $"{minutes}:{seconds - minutes * 60f:00.00}";
    }
}
