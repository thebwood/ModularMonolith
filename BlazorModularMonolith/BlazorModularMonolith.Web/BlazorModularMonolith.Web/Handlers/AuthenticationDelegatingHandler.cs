using System.Net.Http.Headers;
using BlazorModularMonolith.Web.Services;

namespace BlazorModularMonolith.Web.Handlers;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;

    public AuthenticationDelegatingHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        // Get the token from the current scope
        using var scope = _serviceProvider.CreateScope();
        var tokenProvider = scope.ServiceProvider.GetService<ITokenProvider>();
        var token = tokenProvider?.GetToken();

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
