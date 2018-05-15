using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

namespace Starship.Web.HttpResponseMessages {
    public class HtmlResponseMessage : HttpResponseMessage {
        public HtmlResponseMessage(string html) : base(HttpStatusCode.OK) {
            Content = new StringContent(html, Encoding.UTF8, MediaTypeNames.Text.Html);
        }
    }
}
