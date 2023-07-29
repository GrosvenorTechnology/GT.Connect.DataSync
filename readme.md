# Transaction Routing

To initially check how transactions look, you can use a site like https://typedwebhook.tools/ 

## Forwarded offline transaction

Offline transactions will be delivered with a one way call to a web hook.  The server should return a success code 200, 201 or 204 to tell GtConnect that the message can be marked as processed.  If the returns another code the message will be requeued for delivery till successful or the message times out.

![image](./docs/images/forward-example.png)

This will capture a transaction with the following format

```json
{
    "messageId": "33e97178-0594-4f5b-ba4a-a3c3f9f12d8e",
    "correlationId": "3efb6e81-6aad-4363-892d-fd972db4587b",
    "previousMessageId": null,
    "timestamp": "2023-07-29T00:05:06+01:00",
    "entity": "FP-GT8-WH~0101007626",
    "eventName": "transaction",
    "contents": {
        "data": "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<transaction>\n <transID>02e07a24-7242-4856-b08d-9246e990025d</transID>\n <deviceID>FP-GT8-WH~0101007626</deviceID>\n <employee>\n <empID>44825e03-7e23-42bf-4cfc-08da0b3f08ae</empID>\n <identifiedBy>\n <face>44825e03-7e23-42bf-4cfc-08da0b3f08ae</face>\n </identifiedBy>\n <verifiedBy>\n <none />\n </verifiedBy>\n </employee>\n <data>\n <clocking>\n <time>2023-07-29T00:05:06+0100</time>\n <type>in</type>\n </clocking>\n </data>\n</transaction>"
    }
}
```

## Server action online transaction

An online request is made synchronously, the server must respond to the clock with a response in real time.  If the server responds with anything else other than a valid 200 response a failure message will be sent to the clock.

![image](./docs/images/forward-example.png)

A sample of the message posted (the request content will be application/xml)

```xml
<interface>
    <action>Online-Transaction</action>
    <requestedOn>07/28/2023 22:58:49 +00:00</requestedOn>
    <requestType>POST</requestType>
    <payload>&lt;?xml version="1.0" encoding="utf-16"?&gt; &lt;transaction&gt; &lt;transID&gt;641e1e71-3cb8-427c-b439-cdbb5f303ef8&lt;/transID&gt; &lt;deviceID&gt;FP-GT8-WH~0101007739&lt;/deviceID&gt; &lt;employee&gt; &lt;empID&gt;{{employeeId}}&lt;/empID&gt; &lt;identifiedBy&gt; &lt;keypadID&gt;6141016&lt;/keypadID&gt; &lt;/identifiedBy&gt; &lt;verifiedBy&gt; &lt;pin&gt;12345&lt;/pin&gt; &lt;/verifiedBy&gt; &lt;/employee&gt; &lt;data&gt; &lt;clocking&gt; &lt;time&gt;2023-06-19T20:47:09&lt;/time&gt; &lt;type&gt;in&lt;/type&gt; &lt;jobCodes&gt; &lt;jobCode&gt; &lt;jobcodeId&gt;6873&lt;/jobcodeId&gt; &lt;jobCategoryId&gt;a&lt;/jobCategoryId&gt; &lt;/jobCode&gt; &lt;/jobCodes&gt; &lt;/clocking&gt; &lt;/data&gt; &lt;/transaction&gt;</payload>
    <sourceDeviceSerial>FP-GT8-WH~0101007739</sourceDeviceSerial>
    <sourceDeviceId>7f47d1ef-fb30-4552-84c4-014e2ed6bc13</sourceDeviceId>
    <tenantId>54458dda-ca3d-450b-9467-92a881a75539</tenantId>
</interface>
```

The response returned to the clock should again be XML

```xml
<response failed="false">
    <message>Thankyou for your clocking!</message>
</response>
```

## Testing with Visual Studio Dev Tunnels

The easiest way to test is to run our sample and set up Visual Studio dev tunnel, this
will give you a public endpoint that GtConnect can be pointed two.

![image](./docs/images/vs-create-a-dev-tunnel.png)

When you create the tunnel, create it as persistent (this will mean the URL will stay the same) and public will mean that there is no extra authentication needed.

![image](./docs/images/vs-create-a-dev-tunnel-2.png)

You can the click the button in the top left that will show your the URL it's created for you, this will give you a URL like this:

https://kxmnwtt0-7020.uks1.devtunnels.ms/

With this sample app, use this url for the forwarder for offline:

https://kxmnwtt0-7020.uks1.devtunnels.ms/api/transactions/offline

And for the server action based online transactions 

https://kxmnwtt0-7020.uks1.devtunnels.ms/api/transactions/online