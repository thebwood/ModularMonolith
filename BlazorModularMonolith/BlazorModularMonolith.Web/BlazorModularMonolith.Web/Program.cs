using BlazorModularMonolith.Web.Components;
using BlazorModularMonolith.Web.Extensions;
using BlazorModularMonolith.Web.Services;
using BlazorModularMonolith.Web.ViewModels;
using BlazorModularMonolith.Web.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add HttpContextAccessor for token management
builder.Services.AddHttpContextAccessor();

// Register token provider as singleton (with circuit-based storage)
builder.Services.AddSingleton<ITokenProvider, TokenProvider>();

// Register the authentication handler
builder.Services.AddTransient<AuthenticationDelegatingHandler>();

// Configure HttpClient for API
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7188";

// Auth service with direct HttpClient (no delegating handler to avoid circular dependency)
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IAddressApiService, AddressApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services.AddHttpClient<IPersonApiService, PersonApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services.AddHttpClient<IBusinessApiService, BusinessApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthenticationDelegatingHandler>();

// Register ViewModels
builder.Services.AddScoped<PeopleViewModel>();
builder.Services.AddScoped<BusinessesViewModel>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseCustomMiddleware();

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorModularMonolith.Web.Client._Imports).Assembly);

app.Run();
