// Shared game-flow flags. Gameplay scripts skip input while the game is paused or over.
public static class GameState
{
    public static bool IsPaused { get; set; }
    public static bool IsFinished { get; set; }

    public static bool InputBlocked => IsPaused || IsFinished;

    // Statics survive scene loads, so the game scene clears them when it starts.
    public static void Reset()
    {
        IsPaused = false;
        IsFinished = false;
    }
}
