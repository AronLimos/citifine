using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CitiFine.Areas.Identity.Data;
using Stripe;
using CitiFine.Models;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CitiFineDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'CitiFineDbContextConnection' not found.");

builder.Services.AddDbContext<CitiFineDbContext>(options => options.UseSqlServer(connectionString));

// Registration Confirmation not needed (set to false)
builder.Services.AddDefaultIdentity<CitiFineUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CitiFineDbContext>();

// Stripe Config
//StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
// Bind Stripe settings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add policies for authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("RequireOfficer", policy => policy.RequireRole("Officer"));
});

// Inject the Email Service
builder.Services.AddTransient<EmailService>();


// Building the app
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

// To link Register/Login Pages
app.MapRazorPages();

app.Run();
