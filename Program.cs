using HT_Web.Components;
using HT_Web.Services.State;
using MudBlazor.Services;
using HT_Web.Services.State;
using HT_Web.Services.Authentication;
using HT_Web.Services.Traceability;
using HT_Web.Repositories;
using HT_Web.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddScoped<SqlConnectionFactory>();
builder.Services.AddScoped<ApplicationState>();
builder.Services.AddScoped<AuthenticationApiService>();
builder.Services.AddScoped<DummyTraceabilityService>();
builder.Services.AddScoped<IUserAccessService, MockUserAccessService>();

builder.Services.AddScoped<ITraceabilityRepository,
    TraceabilityRepository>();

builder.Services.AddScoped<ITraceabilityService,
    TraceabilityService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
