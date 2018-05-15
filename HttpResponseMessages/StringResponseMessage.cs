using System.Net;
using System.Net.Http;

namespace Starship.Web.HttpResponseMessages {
    public class StringResponseMessage : HttpResponseMessage {
        public StringResponseMessage() {
        }

        public StringResponseMessage(string content, HttpStatusCode status = HttpStatusCode.OK) {
            Content = new StringContent(content);
            StatusCode = status;
        }
    }
}