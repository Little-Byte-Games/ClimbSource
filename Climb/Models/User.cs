using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Climb.Models
{
    public class User : IProfile
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [RegularExpression(@"((\S+)( *))*", ErrorMessage = "Username needs at least 1 non-whitespace character.")]
        public string Username { get; set; }
        public string ProfilePicKey { get; set; }
        public string BannerPicKey { get; set; }

        public HashSet<LeagueUser> LeagueUsers { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}