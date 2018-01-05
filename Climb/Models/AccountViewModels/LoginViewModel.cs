using System.ComponentModel.DataAnnotations;
using Climb.ViewModels;

namespace Climb.Models.AccountViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public LoginViewModel()
            : base(null)
        {
        }
    }
}
