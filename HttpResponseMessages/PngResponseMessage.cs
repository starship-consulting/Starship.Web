using System.Net;
using System.Net.Http;

namespace Starship.Web.HttpResponseMessages {
    public class PngResponseMessage : HttpResponseMessage {
        public PngResponseMessage(byte[] image) : base(HttpStatusCode.OK) {
            Content = new ByteArrayContent(image, 0, image.Length);
        }
    }
}
