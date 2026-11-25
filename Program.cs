using HT_Web.Components;
using HT_Web.Services.State;
using MudBlazor.Services;
using HT_Web.Services.State;
using HT_Web.Services.Authentication;
using HT_Web.Services.Traceability;
using HT_Web.Repositories;
using HT_Web.Data;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<SqlConnectionFactory>();
builder.Services.AddScoped<ApplicationState>();
builder.Services.AddScoped<AuthenticationApiService>();
builder.Services.AddScoped<DummyTraceabilityService>();
builder.Services.AddScoped<IUserAccessService, UserAccessService>();

builder.Services.AddScoped<ITraceabilityRepository,
    TraceabilityRepository>();

builder.Services.AddScoped<ITraceabilityService,
    TraceabilityService>();
builder.Services.AddScoped<VisionReportService>();

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
app.MapDefaultEndpoints();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapGet("/api/vision-report", async (string hostname, DateTime testDate, VisionReportService visionService) =>
{
    var (filePath, errorMessage) = visionService.FindVisionReport(hostname, testDate);
    if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        return Results.NotFound(errorMessage ?? "Vision report not found.");

    var bytes = await File.ReadAllBytesAsync(filePath);
    return Results.File(bytes, "application/pdf");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
