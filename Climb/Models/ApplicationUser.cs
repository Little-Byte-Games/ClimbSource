using Microsoft.AspNetCore.Identity;

namespace Climb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int UserID { get; set; }

        public User User { get; set; }
    }
}
