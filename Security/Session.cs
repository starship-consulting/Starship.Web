using System;

namespace Starship.Web.Security {
    public class Session {

        /*public static Session Lookup(SessionCredentials credentials) {
            return new Session {
                Id = Cryptography.HmacSha1(AppConfiguration.SecretKey, credentials.Email),
                Email = credentials.Email
            };
        }*/

        public string GetPassword() {
            return Password;
        }
        
        public bool IsAuthenticated { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        private string Password { get; set; }
    }
}