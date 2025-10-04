using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Parcial.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

async Task SeedUsersAsync(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    var coordinador = await userManager.FindByEmailAsync("coordinador@uni.edu");
    if (coordinador == null)
    {
        coordinador = new IdentityUser 
        { 
            UserName = "coordinador@uni.edu", 
            Email = "coordinador@uni.edu", 
            EmailConfirmed = true 
        };
        await userManager.CreateAsync(coordinador, "Admin123!");
    }

    var alumno = await userManager.FindByEmailAsync("alumno1@uni.edu");
    if (alumno == null)
    {
        alumno = new IdentityUser
        {
            UserName = "alumno1@uni.edu",
            Email = "alumno1@uni.edu",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(alumno, "Alumno123!");
    }
}

using (var scope = app.Services.CreateScope())
{
    await SeedUsersAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
    app.UseMigrationsEndPoint();
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cursos}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();