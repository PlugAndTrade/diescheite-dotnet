using System.Threading.Tasks;

namespace PlugAndTrade.DieScheite.Client.Common
{
    public interface ILogger
    {
        Task Publish(LogEntry entry);
    }
}
