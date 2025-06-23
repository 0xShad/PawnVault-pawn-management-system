using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pawn_Vault___OOP.Data;
using Pawn_Vault___OOP.Interfaces;
using Pawn_Vault___OOP.Models;
using Pawn_Vault___OOP.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => 
    sqlOptions.EnableRetryOnFailure()
    ));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ?? Configure where unauthenticated users should be redirected (Login Page)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // ? This is the default login URL when using Razor Identity
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await SeedRolesAsync(app);

//TEST!! para lang sa customer profiles
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // apply pending migrations, opitional lang
    //db.Database.Migrate();

    // Add a test customer if none exist
    if (!db.Customers.Any())
    {
        db.Customers.Add(new Customer
        {
            CustomerFN = "Maria",
            CustomerLN = "Reyes",
            TelephoneNo = "09991234567",
            Street = "Burgos St",
            Municipality = "Quezon City",
            ZipCode = "1100"
        });
        db.SaveChanges();
    }
    if (!db.Employees.Any())
    {
        db.Employees.AddRange( // adding employees pero di pa maayos na actions
            new Employee
            {
                EmpFN = "Juan",
                EmpLN = "Dela Cruz",
                Role = "Employee",
                Status = "Active",
                Username = "juandelacruz",
                Password = "admin123" 
            },
            new Employee
            {
                EmpFN = "Ana",
                EmpLN = "Santos",
                Role = "Employee",
                Status = "Active",
                Username = "anasantos",
                Password = "employee123"
            }
        );
        db.SaveChanges();
    }
}


app.Run();

async Task SeedRolesAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new[] { "Admin", "Staff" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
   

    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        string email = "admin@pawnvault.com";
        string password = "#Admin123";

        if(await userManager.FindByEmailAsync(email) == null)
        {
            var user = new IdentityUser();
            user.UserName = email;
            user.Email = email;

            await userManager.CreateAsync(user, password);

            await userManager.AddToRoleAsync(user, "Admin");
        }

      
    }
      
}