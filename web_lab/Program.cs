using Auth;
using Auth.Service;
using Auth.Settings;
using DAL;
using Domain.Services;
using Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("TourAgencyDb");

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICatalogRepository>(provider => new CatalogRepository(connectionString));
builder.Services.AddScoped<IAccountRepository>(provider => new AccountRepository(connectionString));
builder.Services.AddScoped<ILogRepository>(provider => new LogRepository(connectionString));
builder.Services.AddSingleton<IFileService>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    var uploadProfilePictureDirectoryPath = Path.Combine(env.WebRootPath, "assets/profile_pictures");
    var uploadCatalogItemImageDirectoryPath = Path.Combine(env.WebRootPath, "assets");
    return new FileService(uploadProfilePictureDirectoryPath, uploadCatalogItemImageDirectoryPath);
});
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.AddAuth(builder.Configuration);

builder.Services.AddMvc(options =>
{
    options.Filters.Add<LoggingActionFilter>();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<AuthMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
