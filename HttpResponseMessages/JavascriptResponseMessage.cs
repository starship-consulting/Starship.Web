using System.Net;
using System.Net.Http;
using System.Text;

namespace Starship.Web.HttpResponseMessages {
    public class JavascriptResponseMessage : HttpResponseMessage {
        public JavascriptResponseMessage(string html) : base(HttpStatusCode.OK) {
            Content = new StringContent(html, Encoding.UTF8, "application/javascript");
        }
    }
}
