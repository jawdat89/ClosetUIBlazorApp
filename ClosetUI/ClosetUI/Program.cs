using ClosetUI.Components;
using ClosetUI.Models.Locales;
using ClosetUI.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPartCalculationService, PartCalculationService>();
builder.Services.AddScoped<IBoardService, BoardService>();

builder.Services.AddLocalization();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(PartGeneratorDtoResource));
    });

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["AppSettings:GeneratePDFAzureFunctionURL"])
});

var app = builder.Build();

string[] supportedCultures = ["en", "he"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[1])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ClosetUI.Client._Imports).Assembly);

app.MapControllers();

QuestPDF.Settings.License = LicenseType.Community;

app.Run();
