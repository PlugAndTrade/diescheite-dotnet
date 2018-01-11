namespace PlugAndTrade.DieScheite.Client.Common
{
    public class LogEntryTrace
    {
      public string Id { get; set; }
      public string ParentId { get; set; }
      public string Name { get; set; }
      public long Timestamp { get; set; }
      public long Duration { get; set; }
    }
}
