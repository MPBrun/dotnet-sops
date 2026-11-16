namespace DotnetSops.CommandLine.Tests.Mocks;

internal sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _responseMessage;

    public MockHttpMessageHandler(HttpResponseMessage responseMessage)
    {
        _responseMessage = responseMessage;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(_responseMessage);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _responseMessage?.Dispose();
        }
        base.Dispose(disposing);
    }
}
