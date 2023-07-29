namespace GT.DataSync.Api.Models;

public class ForwardedDeviceMessage
{
    public Guid MessageId { get; set; }
    public Guid CorrelationId { get; set; }
    public Guid? PreviousMessageId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Entity { get; set; } = "";
    public string EventName { get; set; } = "";
    public ContentData? Contents { get; set; }
}
