/// <summary>
/// <c>Preferences</c> holds all the current game state information and loads / saves settings.
/// </summary>
public static class Preferences
{
    public static int PlayerCount = 2; // default
    public static int AgentCount = 1; // default
    public static int MapNumber = 1; // default
    public static bool AutoPopulateFlag = false; // sees if the setup phase should be skipped
    public static float CurrentVolume = 0.5f;
    public static bool isShownFPS = true;

    /// <summary>
    /// <c>LoadPreferences</c> searches the file system for a config file and reads it if found.
    /// </summary>
    public static void LoadPreferences()
    {
        // load the file
        // parse the preferences and set them here
    }

    /// <summary>
    /// <c>SavePreferences</c> gets the current game preferences and saves them to an external file to save state.
    /// </summary>
    public static void SavePreferences()
    {
        // read current player's preferences
        // create a file
        // layout this file with the preference data
    }
}
