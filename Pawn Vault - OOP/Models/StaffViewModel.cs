namespace Pawn_Vault___OOP.Models.ViewModels
{
    public class StaffViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }

        // Only for editing
        public string? NewPassword { get; set; }
    }
}
