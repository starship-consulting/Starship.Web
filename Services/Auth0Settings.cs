using System;

namespace Starship.Web.Services {

    public class Auth0Settings {

        public string Domain { get; set; }

        public string Identifier { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AccessToken { get; set; }

        public bool UseCookies { get; set; }

        public bool UseJwtBearer { get; set; }

        public string TokenUrl => $"https://{Identifier}.auth0.com/oauth/token";

        public string Audience => $"https://{Identifier}.auth0.com/api/v2/";
    }
}