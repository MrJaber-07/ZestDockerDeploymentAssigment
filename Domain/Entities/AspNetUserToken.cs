using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class AspNetUserToken
    {
        public string UserId { get; set; } = null!;

        public string LoginProvider { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Value { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
    }

}
