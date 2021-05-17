using System.Collections;
using System.Collections.Generic;

// A static class that stores data as the scenes change
public static class GameData
{
    // The current level of the current play session
    public static int level;
    // The previous scene that was active (used to tell if it's an AI game)
    public static int previousScene;
}
