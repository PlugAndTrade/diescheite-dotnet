using System.Collections.Generic;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public class LogEntryHttpResponse
    {
        public int? StatusCode { get; set; }
        public IList<KeyValuePair<string, object>> Headers { get; }
        public byte[] Body { get; set; }

        public LogEntryHttpResponse()
        {
            Headers = new List<KeyValuePair<string, object>>();
        }
    }
}
