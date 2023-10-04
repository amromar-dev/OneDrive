using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

using Microsoft.Identity.Web.UI;
using OneDrive.Web.Logic.Authentication;
using OneDrive.Web.Logic.Files;
using OneDrive.Web.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Bind auth options from appSettings
var authOptions = new MicrosoftAuthOptions();
builder.Configuration.GetSection("MicrosoftAuth").Bind(authOptions);
builder.Services.AddSingleton(authOptions);

// Add authentication configurations
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = MicrosoftAccountDefaults.AuthenticationScheme;
}).AddCookie().AddMicrosoftAccount(o =>
{
    o.ClientId = authOptions.ClientId;
    o.ClientSecret = authOptions.ClientSecret;
    o.Scope.Add(authOptions.Scope);
    o.SaveTokens = true;
});

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorPages().AddMicrosoftIdentityUI();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Add Custom Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapControllers();

app.Run();
