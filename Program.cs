using EcommProject.DataAccess.Data;
using EcommProject.DataAccess.Repository;
using EcommProject.DataAccess.Repository.IRepository;
using EcommProject.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var cs = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(cs));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<ICoverTypeRepository,CoverTypeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ISMSService, SMSService>();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<SMSSettings>(builder.Configuration.GetSection("SMSSettings"));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
});

builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "1737472480411339";
    options.AppSecret = "e1e5da924772637e09c50c56b1b0333d";
})
//.AddGoogle(options =>
//{
//    options.ClientId = "";
//    options.ClientSecret = "";
//})
.AddInstagram(options =>
{
    options.ClientId = "1101781631641418";
    options.ClientSecret = "fcdcd627c822e64cedf9b7126380dbee";
})
.AddTwitter(options =>
{
    options.ConsumerKey = "9pA32SnrRGPsLXyKa0oeINKie";
    options.ConsumerSecret = "nyd95em5D28o4ZwxtQw98xYYdu9DzvVRGyANaGmucC9M3LpwSi";
    //Bearer Token AAAAAAAAAAAAAAAAAAAAAJ17wAEAAAAAIqGJoCSgdK % 2BJEHij17jzYWgZCW0 % 3DhZ6xelKOJTTA50ZLkuXAi3d0Ivpfs4gdf8VMKFh1KCE0H6Rni4
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings")["SecretKey"];

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
