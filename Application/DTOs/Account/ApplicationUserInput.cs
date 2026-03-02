using Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Account
{
    public class ApplicationUserInput : Input<ApplicationUserInput>
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
