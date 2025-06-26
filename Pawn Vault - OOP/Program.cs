using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pawn_Vault___OOP.Areas.Identity.Data;
using Pawn_Vault___OOP.Data;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;
using Pawn_Vault___OOP.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity setup
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await SeedRolesAndUsersAsync(app); // Seed roles and users
app.Run();

//  Seeding Roles and Users
async Task SeedRolesAndUsersAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Staff" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Admin
    string adminEmail = "admin@pawnvault.com";
    string adminPassword = "#Admin123";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail
        };
        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Staff (Display name as UserName)
    var staffUsers = new[]
    {
        new { Email = "staff1@pawnvault.com", UserName = "Staff 1" },// Renaming Staffs
        new { Email = "staff2@pawnvault.com", UserName = "Staff 2" }
    };
    string staffPassword = "#Staff123";

    foreach (var staff in staffUsers)
    {
        var existing = await userManager.FindByEmailAsync(staff.Email);
        if (existing == null)
        {
            var staffUser = new ApplicationUser
            {
                UserName = staff.UserName,
                Email = staff.Email
            };
            await userManager.CreateAsync(staffUser, staffPassword);
            await userManager.AddToRoleAsync(staffUser, "Staff");
        }
        else
        if (existing.UserName != staff.UserName)
        {
            existing.UserName = staff.UserName;
            await userManager.UpdateAsync(existing); //  this updates the DB
        }
    }
}
//PROBLEM:
//1. May AspNetRoles table na raw kapag nag-update database