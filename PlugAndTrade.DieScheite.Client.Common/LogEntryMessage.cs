using System.Collections.Generic;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public class LogEntryMessage
    {
      public int Level { get; set; }
      public string Message { get; set; }
      public string TraceId { get; set; }
      public string Stacktrace { get; set; }
      public IList<LogEntryAttachment> Attachments { get; }

      public LogEntryMessage()
      {
          Attachments = new List<LogEntryAttachment>();
      }
    }
}
