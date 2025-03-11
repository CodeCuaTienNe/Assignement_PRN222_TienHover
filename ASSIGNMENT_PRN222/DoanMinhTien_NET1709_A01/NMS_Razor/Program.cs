using NMS_Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//DI 
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();

//Session
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
//Use Session
app.UseSession();

app.Run();
