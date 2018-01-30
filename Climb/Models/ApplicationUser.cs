using Microsoft.AspNetCore.Identity;

namespace Climb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public const int RequiredChars = 6;
        public const int MaxChars = 100;

        public int UserID { get; set; }

        public User User { get; set; }
    }
}
