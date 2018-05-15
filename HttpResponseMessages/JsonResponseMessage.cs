using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Starship.Web.HttpResponseMessages {
    public class JsonResponseMessage : HttpResponseMessage {
        public JsonResponseMessage(string json, HttpStatusCode statusCode)
            : base(statusCode) {
            Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        public JsonResponseMessage(string json)
            : base(HttpStatusCode.OK) {
            Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        public JsonResponseMessage(object json, HttpStatusCode statusCode, JsonSerializerSettings settings)
            : this(JsonConvert.SerializeObject(json, settings), statusCode) {
        }

        public JsonResponseMessage(object json, JsonSerializerSettings settings)
            : this(JsonConvert.SerializeObject(json, settings), HttpStatusCode.OK) {
        }
    }
}