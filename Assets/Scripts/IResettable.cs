// Anything inside a room that returns to its starting state when the room resets (candles, Watchers).
public interface IResettable
{
    void ResetState();
}
