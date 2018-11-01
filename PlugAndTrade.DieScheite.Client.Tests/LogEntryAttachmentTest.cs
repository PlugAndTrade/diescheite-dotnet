using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using PlugAndTrade.DieScheite.Client.Common;

namespace PlugAndTrade.DieScheite.Client.Tests
{
    public class LogEntryAttachmentTest
    {
        [Fact]
        public void AddMessageAttachment()
        {
            var log = new LogEntry();
            log
                .Log(LogEntryLevel.Debug, "foo bar")
                .Attach(
                    "test name",
                    "content/type",
                    "encoding",
                    System.Text.Encoding.UTF8.GetBytes("someattacheddata"),
                    new Dictionary<string, object> { { "HeaderKey", "HeaderValue" } }.ToLookup(p => p.Key, p => p.Value)
                );

            Assert.Single(log.Messages);
            Assert.Single(log.Messages[0].Attachments);

            var att = log.Messages[0].Attachments[0];
            Assert.Equal("test name", att.Name);
            Assert.Equal("content/type", att.ContentType);
            Assert.Equal("encoding", att.ContentEncoding);
            Assert.Equal("someattacheddata", System.Text.Encoding.UTF8.GetString(att.Data));
            Assert.Equal(new Dictionary<string, object> { { "HeaderKey", "HeaderValue" } }, att.Headers.ToDictionary(p => p.Key, p => p.Value));
        }
    }
}
