using System.Linq;
using System.Security.Claims;

namespace Starship.Web.Security {

    public class UserProfile {

        public UserProfile() {
        }

        public UserProfile(ClaimsPrincipal user) {
            Id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Name = user.Identity.Name;
            Email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            Photo = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
        }

        public static UserProfile Guest() {
            return new UserProfile {
                Id = "guest",
                Name = "Guest"
            };
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Photo { get; set; }
    }
}