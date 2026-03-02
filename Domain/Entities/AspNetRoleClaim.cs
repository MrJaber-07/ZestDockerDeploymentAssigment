using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class AspNetRoleClaim
    {
        public int Id { get; set; }

        public string RoleId { get; set; } = null!;

        public string? ClaimType { get; set; }

        public string? ClaimValue { get; set; }

        public virtual AspNetRole Role { get; set; } = null!;
    }

}
