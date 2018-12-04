using System.Linq;
using System.Security.Claims;

namespace Starship.Web.Security {

    public class UserProfile {

        public UserProfile() {
        }

        public UserProfile(ClaimsPrincipal user) {
            Name = user.Identity.Name;
            Email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            Photo = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Photo { get; set; }
    }
}