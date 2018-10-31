using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.TracingScope;


namespace PlugAndTrade.DieScheite.Client.Tests
{
    public class TestException : Exception
    {
        public TestException(string message) : base(message) {}
    }

    public class LogEntryFactoryTest
    {
        [Fact]
        public async Task LoggedActionLogException()
        {
            var logger = new Mock<ILogger>();
            var factory = new LogEntryFactory("", "", "");
            await Assert.ThrowsAsync<TestException>(async () => await factory.LoggedActionAsync(new ILogger[] { logger.Object }, new StaticTracingScope(), async (log) =>
            {
                throw new TestException("Foobar");
            }));

            logger.Verify(x => x.Publish(It.Is<LogEntry>(log =>
                400 >= log.Level
                && log.Messages.Count == 1
                && string.Equals(log.Messages[0].Message, "Foobar")
                && !string.IsNullOrEmpty(log.Messages[0].Stacktrace)
            )));
        }
    }
}
