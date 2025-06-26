using Microsoft.AspNetCore.Identity;

namespace Pawn_Vault___OOP.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
