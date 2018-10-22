using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Starship.Web.HttpResponseMessages {
    public class HttpResponseMessage<T> : HttpResponseMessage {
        public HttpResponseMessage(JsonSerializerSettings settings) {
            Settings = settings;
        }

        public HttpResponseMessage(JsonSerializerSettings settings, T content) : this(settings) {
            SetContent(content);
        }

        public HttpResponseMessage(JsonSerializerSettings settings, HttpStatusCode status) : this(settings) {
            StatusCode = status;
        }

        public void SetContent(T content) {
            Content = new StringContent(JsonConvert.SerializeObject(content, Settings));
            Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        }

        private JsonSerializerSettings Settings { get; set; }
    }
}