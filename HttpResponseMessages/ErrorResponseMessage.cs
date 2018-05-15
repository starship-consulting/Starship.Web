using System.Net;

namespace Starship.Web.HttpResponseMessages {
    public class ErrorResponseMessage : JsonResponseMessage {
        public ErrorResponseMessage(string error)
            : base(error, HttpStatusCode.BadRequest)
        {
        }
    }
}