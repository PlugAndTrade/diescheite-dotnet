namespace PlugAndTrade.DieScheite.Client.Common
{
    public class LogEntryAttachment
    {
      public string Id { get; set; }
      public string Name { get; set; }
      public string ContentType { get; set; }
      public string ContentEncoding { get; set; }
      public byte[] Data { get; set; }
    }
}
