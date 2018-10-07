using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Starship.Core.Csv;
using Starship.Core.Extensions;

namespace Starship.Web.Formatters {
    public class CsvMediaFormatter : MediaTypeFormatter {

        public CsvMediaFormatter() {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
        }

        public override bool CanReadType(Type type) {
            return false;
        }

        public override bool CanWriteType(Type type) {
            return typeof(IEnumerable).IsAssignableFrom(type) || typeof(IList).IsAssignableFrom(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext) {
            var taskSource = new TaskCompletionSource<object>();
            var outputStream = new StreamWriter(writeStream);
            var list = value as IEnumerable;
      
            if (list != null) {
                foreach (var item in list) {
                    var line = new StringBuilder();
                    var index = 0;
                    var quotationDelimitedStrings = false;

                    item.GetType().WithAttribute<CsvFormatAttribute>(attribute => quotationDelimitedStrings = attribute.QuotationDelimitedStrings);

                    foreach (var property in item.GetType().GetProperties()) {
                        if (index > 0) {
                            line.Append(",");
                        }

                        var propertyValue = property.GetValue(item);

                        if (quotationDelimitedStrings) {
                            line.Append("\"");
                        }

                        line.Append(propertyValue);

                        if (quotationDelimitedStrings) {
                            line.Append("\"");
                        }

                        index += 1;
                    }

                    outputStream.WriteLine(line.ToString());
                }
            }
      
            outputStream.Close();
            taskSource.SetResult(null);
            return taskSource.Task;
        }
    }
}