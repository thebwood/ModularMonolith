using System.Collections.Concurrent;

namespace BlazorModularMonolith.Web.Services;

public class TokenProvider : ITokenProvider
{
    private static readonly ConcurrentDictionary<string, string?> _tokens = new();
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCircuitId()
    {
        // Use connection ID or session ID as circuit identifier
        var context = _httpContextAccessor.HttpContext;
        return context?.Connection.Id ?? "default";
    }

    public string? GetToken()
    {
        var circuitId = GetCircuitId();
        _tokens.TryGetValue(circuitId, out var token);
        return token;
    }

    public void SetToken(string? token)
    {
        var circuitId = GetCircuitId();
        if (string.IsNullOrEmpty(token))
        {
            _tokens.TryRemove(circuitId, out _);
        }
        else
        {
            _tokens[circuitId] = token;
        }
    }

    public void ClearToken()
    {
        var circuitId = GetCircuitId();
        _tokens.TryRemove(circuitId, out _);
    }
}
