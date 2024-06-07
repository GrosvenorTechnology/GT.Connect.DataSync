using System.Net.Http.Headers;
using System.Text;

namespace GT.DataSync.Core.Service;

public class BasicAuthHandler : DelegatingHandler
{
    private readonly string _username;
    private readonly string _password;

    public BasicAuthHandler(string username, string password)
    {
        InnerHandler = new HttpClientHandler();
        _username = username;
        _password = password;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}")));

        return await base.SendAsync(request, cancellationToken)
            .ConfigureAwait(false);
    }
}
