using System.Collections.Generic;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public class LogEntryHttpRequest
    {
        public string Method { get; set; }
        public string Uri { get; set; }
        public string Host { get; set; }
        public IList<KeyValuePair<string, object>> Headers { get; }
        public byte[] Body { get; set; }

        public LogEntryHttpRequest()
        {
            Headers = new List<KeyValuePair<string, object>>();
        }
    }
}
