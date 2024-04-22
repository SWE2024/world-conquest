/// <summary>
/// <c>Preferences</c> holds all the current game state information and loads / saves settings.
/// </summary>
public static class Preferences
{
    public static int PlayerCount = 2; // default
    public static int MapNumber = 1; // default
    public static bool AutoPopulateFlag = false; // sees if the setup phase should be skipped
    public static float CurrentVolume = 0.5f;
    public static bool isShownFPS = true;

    public static void LoadPreferences()
    {
        // load a settings file that holds the player's preferences
    }

    public static void SavePreferences()
    {
        // create a settings file that holds the player's preferences
    }
}
