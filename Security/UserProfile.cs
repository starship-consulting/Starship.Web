using System;
using System.Linq;
using System.Security.Claims;

namespace Starship.Web.Security {

    public class UserProfile {

        public UserProfile() {
        }

        public UserProfile(string email) {
            Email = email;
            IsImpersonating = true;
            IsAuthenticated = true;
        }

        public UserProfile(ClaimsPrincipal user) {

            Id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Name = user.Identity.Name;

            var firstName = user.Claims.FirstOrDefault(each => each.Type == ClaimTypes.GivenName);
            var lastName = user.Claims.FirstOrDefault(each => each.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");

            if(firstName != null) {
                Name = firstName.Value;
            }

            if(lastName != null) {
                Name += " " + lastName.Value;
            }

            if(!string.IsNullOrEmpty(Name)) {
                Name = Name.Trim();
            }

            Email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            Photo = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            IsAuthenticated = true;
        }

        public static UserProfile Guest() {
            return new UserProfile {
                Id = "guest",
                Name = "Guest",
                Email = "guest"
            };
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Photo { get; set; }

        public bool IsAuthenticated { get; set; }

        public bool IsImpersonating { get; set; }
    }
}