using System.Collections.Generic;
using System.IO;
using Xunit;
using PlugAndTrade.DieScheite.Client.Common;
using PlugAndTrade.DieScheite.Client.Json;

namespace PlugAndTrade.DieScheite.Client.Tests
{
    public class LogEntryJsonTest
    {
        [Fact]
        public void AttachmentJson()
        {
            var att = new LogEntryAttachment
            {
                Id = "someid",
                Name = "somename",
                ContentType = "sometype",
                ContentEncoding = "someencoding",
                Headers = new [] { new KeyValuePair<string, object>("somekey", "somevalue") },
                Data = System.Text.Encoding.UTF8.GetBytes("somedata")
            };

            var mem = new MemoryStream();
            using (var writer = new StreamWriter(mem))
            {
                LogEntryJson.WriteJson(writer, att);
            }
            Assert.Equal(string.Join("", new []
                {
                    "{",
                        "\"id\":\"someid\",",
                        "\"name\":\"somename\",",
                        "\"contentType\":\"sometype\",",
                        "\"contentEncoding\":\"someencoding\",",
                        "\"headers\":[{\"somekey\":\"somevalue\"}],",
                        "\"data\":\"c29tZWRhdGE=\"",
                    "}",
                }),
                System.Text.Encoding.UTF8.GetString(mem.ToArray())
            );
        }
    }
}
