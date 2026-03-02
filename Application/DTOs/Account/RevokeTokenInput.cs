using Application.Validators;

namespace Application.ViewModels.Account
{
    public class RevokeTokenInput : Input<RevokeTokenInput>
    {
        public string Token { get; set; }
    }
}
