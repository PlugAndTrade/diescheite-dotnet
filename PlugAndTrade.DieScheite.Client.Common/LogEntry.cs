using System.Collections.Generic;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public class LogEntry
    {
      public string Id { get; set; }
      public string ParentId { get; set; }
      public string CorrelationId { get; set; }
      public string ServiceId { get; set; }
      public string ServiceInstanceId { get; set; }
      public string ServiceVersion { get; set; }
      public long Timestamp { get; set; }
      public long Duration { get; set; }
      public IList<KeyValuePair<string, object>> Headers { get; }
      public IList<LogEntryTrace> Trace { get; }
      public IList<LogEntryMessage> Messages { get; }
      public LogEntryHttpData Http { get; set; }

      public LogEntry()
      {
          Headers = new List<KeyValuePair<string, object>>();
          Trace = new List<LogEntryTrace>();
          Messages = new List<LogEntryMessage>();
      }
    }
}
