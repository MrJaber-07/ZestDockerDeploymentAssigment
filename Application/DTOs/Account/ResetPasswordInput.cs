using Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Account
{
    public class ResetPasswordInput : Input<ResetPasswordInput>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Code { get; set; }
    }
}
