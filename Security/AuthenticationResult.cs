namespace Starship.Web.Security {
    public class AuthenticationResult {

        public string Token { get; set; }

        public Session Session { get; set; }
    }
}