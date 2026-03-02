using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Account
{
    public class ApplicationUserDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string LockoutEnd { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
    }
}
