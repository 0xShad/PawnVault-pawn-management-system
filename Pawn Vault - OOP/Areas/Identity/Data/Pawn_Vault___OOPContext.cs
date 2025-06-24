using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pawn_Vault___OOP.Areas.Identity.Data;

namespace Pawn_Vault___OOP.Data;

public class Pawn_Vault___OOPContext : IdentityDbContext<ApplicationUser>
{
    public Pawn_Vault___OOPContext(DbContextOptions<Pawn_Vault___OOPContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
