using JsonBasedLocalization.Web;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddMvc().AddRazorRuntimeCompilation();
builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizerFactory , JsonStringLcalizerFactory>();
builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization (option =>
    {
        option.DataAnnotationLocalizerProvider = (Type, factory) =>
        factory.Create(typeof(JsonStringLcalizerFactory));
    });
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var SupportedCultures = new[]
    {
        new CultureInfo(name : "en-US"),
        new CultureInfo(name : "ar-EG")
    };
    options.DefaultRequestCulture = new RequestCulture(culture : SupportedCultures[0] , uiCulture : SupportedCultures[0]);
    options.SupportedCultures = SupportedCultures;
    options.SupportedUICultures = SupportedCultures;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
var SupportedCultures = new[] { "en-US", "ar-EG" };
var LocalizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(SupportedCultures[0])
    .AddSupportedCultures(SupportedCultures)
    .AddSupportedUICultures(SupportedCultures);
app.UseRequestLocalization(LocalizationOptions);
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
