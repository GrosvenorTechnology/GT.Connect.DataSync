using Azure.Core;
using GT.DataSync.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace GT.DataSync.Api.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ILogger<TransactionController> logger)
    {
        _logger = logger;
    }

    [HttpPost("online")]
    [Produces("application/xml")]
    [ProducesResponseType(typeof(XElement), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostOnlineTransaction([FromBody] DeviceRequest data)
    {
        //Dump the body to the log
        Request.Body.Position = 0;
        var body = await new StreamReader(Request.Body).ReadToEndAsync();
        _logger.LogInformation(body);

        var transaction = XElement.Parse(data.Payload);

        var sn = transaction.Element("deviceID")?.Value;

        if (string.IsNullOrEmpty(sn))
        {
            _logger.LogError("No deviceId in request");
            _logger.LogError(data.Payload);
            return BadRequest();
        }

        _logger.LogInformation("Online transaction accepted.");

        return Ok(new XElement("response", new XAttribute("failed", "false"),
            new XElement("message", $"Accepted at: {DateTime.Now}")));
    }

    [HttpPost("offline")]
    //[Consumes("application/xml")]
    [Produces("application/xml")]
    [ProducesResponseType(typeof(XElement), StatusCodes.Status200OK)]
    public async Task<IActionResult> PostOfflineTransaction([FromBody] ForwardedDeviceMessage message)
    {
        //Dump the body to the log
        Request.Body.Position = 0;
        var body = await new StreamReader(Request.Body).ReadToEndAsync();
        _logger.LogInformation(body);

        if (message?.EventName != "transaction")
        {
            _logger.LogWarning("Not a transaction: {deviceId} / {eventName}", message?.Entity, message?.EventName);
            return Ok();
        }

        if (message?.Contents?.Data is null)
        {
            _logger.LogWarning("Message body is null: {deviceId} / {eventName}", message?.Entity, message?.EventName);
            return Ok();
        }

        var transaction = XElement.Parse(message.Contents.Data);

        var sn = transaction.Element("deviceID")?.Value;

        if (sn == null)
        {
            _logger.LogError("No device Id, transaction discarded");
            return Ok();
        }

        _logger.LogInformation("Offline transaction accepted.");
        return Ok();
    }
}
