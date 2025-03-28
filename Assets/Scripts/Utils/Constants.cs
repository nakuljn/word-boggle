public static class Constants
{
    public static class PlayerPrefsConstants
    {
        public const string GameMode = "GameMode";
    }
    
    public static class SceneNames
    {
        public static string HomeScene = "HomeScene";
        public static string MainScene = "MainScene";
    }

    public static int EndlessModeGridSizeX = 4;
    public static int EndlessModeGridSizeY = 4;

}

public static class UIStrings
{
    //Error messages
    public static string WordNotFound = "Word was not found";
    public static string WordTooShort = "Word is too short";
    public static string WordAlreadyFound = "Word already found";
    
    //Display messages
    public const string WordsTarget = "Total words to find ";
    public const string CurrentWords = "Current words count ";
    public const string ScoreTarget = "Total score to reach ";
    public const string CurrentScore = "Current Score ";
    public const string TotalScore = "Total Score ";
    public const string AverageScore = "Average Score ";
}