using System.Xml.Serialization;

namespace GT.DataSync.Api.Models;

[XmlRoot("interface")]
public class DeviceRequest
{
    [XmlElement("action")]
    public string Action { get; set; }

    [XmlElement("requestedOn")]
    public string RequestedOn { get; set; }

    [XmlElement("requestType")]
    public string RequestType { get; set; }

    [XmlElement("payload")]
    public string Payload { get; set; }
}
