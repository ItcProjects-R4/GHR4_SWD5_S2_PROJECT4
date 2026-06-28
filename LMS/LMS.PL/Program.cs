// Import the required packages
using AutoMapper;
using CloudinaryDotNet;
using dotenv.net;
using LMS.BLL.Extensions;
using LMS.BLL.Services.Implementation;
using LMS.BLL.Services.Interfaces;
using LMS.DAL.Data;
using LMS.DAL.Repositories.Implementation;
using LMS.DAL.Repositories.Interfaces;
using LMS.DAL.Seeding;
using LMS.Domain.Models;
using LMS.PL.Helpers;
using LMS.PL.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

// Set your Cloudinary credentials



var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
}

builder.Services.AddHttpClient<IPaymobService, PaymobService>();

// Register Repositories and Services
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymobService, PaymobService>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICourseService, CourseService>();


builder.Services.AddScoped<ISubmissionService, SubmissionService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

//DbContext
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))

);


// Register Cloudinary
Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
cloudinary.Api.Secure = true;


builder.Services.AddSingleton(cloudinary);

// Register the CloudinaryService

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Register AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<CourseProfile>();
});

// Register Identity with roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// add this block to fix the SameSite cookie redirects
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
});

// add emailsender
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddBLLServices();

//adding custom claims
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsPrincipalFactory>();



var app = builder.Build();

try
{
    await DbInitializer.SeedAsync(app.Services);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding the database.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Home/NotFoundPage");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//Register IdentitySeeder

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedIdentityAsync(scope.ServiceProvider);
}

app.Run();



