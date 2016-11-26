namespace Rubito.XamarinForms.SimpleAuth.Models
{
    public class AuthenticationResult
    {
        public readonly bool IsAuthenticated;
        public readonly BearerToken Token;
        public readonly string Message;

        public AuthenticationResult(bool authenticated, string message, BearerToken token = null)
        {
            this.IsAuthenticated = authenticated;
            this.Token = token;
            this.Message = message;
        }
    }
}
