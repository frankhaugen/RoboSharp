using Microsoft.Extensions.Logging;
using RoboSharp.Application.Teaching;
using RoboSharp.Hosting;
using RoboSharp.Language;
using RoboSharp.Locales;
using RoboSharp.Locales.English;
using RoboSharp.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.AddSimpleConsole(o => o.SingleLine = true).SetMinimumLevel(LogLevel.Warning);
builder.Services.AddRoboSharpHosting();
builder.Services.AddSingleton<ITeachingLocale, EnglishTeachingLocale>();
builder.Services.AddSingleton<ISyntaxTreeSerializer, SyntaxTreeSerializer>();
builder.Services.AddSingleton<IPipelineInspectionService, PipelineInspectionService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
