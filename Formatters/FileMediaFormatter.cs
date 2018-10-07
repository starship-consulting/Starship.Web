using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Starship.Web.Http;

namespace Starship.Web.Formatters {
    public class FileMediaFormatter : MediaTypeFormatter {

        public FileMediaFormatter() {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }

        public override bool CanReadType(Type type) {
            return type == typeof(MediaFileData);
        }

        public override bool CanWriteType(Type type) {
            return false;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger) {

            if (!content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var directory = Path.GetTempPath();
            var provider = new MultipartFormDataStreamProvider(directory);

            return content.ReadAsMultipartAsync(provider).ContinueWith(t => {
                var file = provider.FileData.First();
                var filename = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));

                return new MediaFileData(File.ReadAllBytes(file.LocalFileName), file.Headers.ContentType.MediaType, filename) as object;
            });
        }
    }
}