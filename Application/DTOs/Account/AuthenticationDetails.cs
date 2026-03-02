using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.ViewModels.Account
{
    public class AuthenticationDetails
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }

        //[JsonIgnore] set in cookie only remove in production
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
