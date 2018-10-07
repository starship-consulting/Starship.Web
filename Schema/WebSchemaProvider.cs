using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Starship.Core.Json.Schema;

namespace Starship.Web.Schema {
    public class WebSchemaProvider<T> where T : DataOrientedJsonSchemaGenerator {

        public WebSchemaProvider(T generator) {
            Generator = generator;
        }

        public HttpResponseMessage GetHttpResponseMessage() {
            lock (SyncRoot) {
                if (Schema == null) {
                    Schema = Generator.GenerateSchemaObject();
                    SerializedSchema = JsonConvert.SerializeObject(Schema, Formatting.Indented);
                }
            }

            var content = new StringContent(JavascriptNamespace + " = " + SerializedSchema);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/javascript");

            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = content
            };
        }
        
        public string JavascriptNamespace = "window.schema";

        private string SerializedSchema { get; set; }

        private DataOrientedJsonSchemaGenerator Generator { get; set; }

        private Dictionary<string, object> Schema { get; set; }

        private readonly object SyncRoot = new object();
    }
}