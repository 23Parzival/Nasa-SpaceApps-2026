using UnityEngine;

public static class GameState
{
    public static bool powerOn = false;
    public static bool minigameOpen = false;
    public static bool uiOpen = false;

    [RuntimeInitializeOnLoadMethod]
    static void Reset()
    {
        powerOn = false;
        minigameOpen = false;
        uiOpen = false;
    }
}