namespace PlugAndTrade.DieScheite.Client.Common
{
    public static class LogEntryLevelHelper
    {
        public static string GetCategory(int level)
        {
            var categoryId = (int) level / 100;
            switch (categoryId)
            {
                case 0: return "";
                case 1: return "DEBUG";
                case 2: return "INFO";
                case 3: return "WARNING";
                case 4: return "ERROR";
                case 5: return "CRITICAL";
                default: return $"{categoryId}";
            }
        }
    }

    public enum LogEntryLevel
    {
        Debug = 100,
        Info = 200,
        Warning = 300,
        Error = 400,
        Critical = 500,
    }
}
