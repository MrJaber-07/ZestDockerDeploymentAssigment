using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class RefreshToken
    {
        public int Id { get; set; }

        public string Token { get; set; } = null!;

        public DateTime Expires { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Revoked { get; set; }

        public string UserId { get; set; } = null!;

        public virtual AspNetUser User { get; set; } = null!;
    }
}
