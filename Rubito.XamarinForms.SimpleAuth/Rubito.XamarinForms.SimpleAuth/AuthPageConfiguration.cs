namespace Rubito.SimpleFormsAuth
{
    /// <summary>
    /// Configure the appereance of the authentication page.
    /// </summary>
    public class AuthPageConfiguration
    {
        public string Title { get; set; } = "Authenticate";
        public string SubTitle { get; set; } = "Sign in to your Account";
        public bool ShowCloseButton { get; set; } = true;
        public bool ShowRegistrationButton { get; set; } = false;
        public uint MinUsernameLength { get; set; } = 2;
        public uint MinPasswordLength { get; set; } = 6;
    }
}
