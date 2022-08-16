using System;

public static class GameEvents
{
    public static Action<int[]> GameOverEvent;
    public static void GameOverInvoke(int[] winnerIds)
    {
        GameOverEvent?.Invoke(winnerIds);
    }
}
