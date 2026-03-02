using Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Account
{
    public class RegisterInput: Input<RegisterInput>
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [DataType(DataType.Text)]
        public string Role { get; set; }
    }
}
