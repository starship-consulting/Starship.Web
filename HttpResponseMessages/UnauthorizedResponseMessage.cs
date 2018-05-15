using System.Net;

namespace Starship.Web.HttpResponseMessages {
    public class UnauthorizedResponseMessage : StringResponseMessage {
        public UnauthorizedResponseMessage() : base("You are not logged in.", HttpStatusCode.Unauthorized) {
        }
    }
}