using System;
using System.Diagnostics;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public class LogEntryActiveTrace : IDisposable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long Timestamp { get; set; }
        public LogEntryActiveTrace Parent { get; set; }
        private readonly Action<LogEntryTrace> _onDone;
        private readonly Stopwatch _timer;

        public LogEntryActiveTrace(string name, LogEntryActiveTrace parent, Action<LogEntryTrace> onDone)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Timestamp = DateTime.UtcNow.AsEpochMillis();
            Parent = parent;
            _onDone = onDone;
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Stop();
            _onDone(new LogEntryTrace
            {
              Id = Id,
              ParentId = Parent?.Id,
              Name = Name,
              Timestamp = Timestamp,
              Duration = _timer.ElapsedMilliseconds
            });
        }
    }
}
