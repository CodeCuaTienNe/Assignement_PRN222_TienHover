using NMS_Blazor.Components;
using NMS_Repositories;
using Microsoft.EntityFrameworkCore;
using NMS_Blazor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Log the admin account settings (email only, never log passwords)
var adminEmail = builder.Configuration.GetValue<string>("AdminAccount:Email");
var adminRole = builder.Configuration.GetValue<int>("AdminRole", 3);
Console.WriteLine($"Admin settings loaded - Email: {adminEmail}, Role: {adminRole}");

//DI 
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// Add AuthService
builder.Services.AddScoped<AuthService>();

// Add memory cache and use it as distributed cache
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IDistributedCache, MemoryDistributedCache>();

// Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseSession();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
