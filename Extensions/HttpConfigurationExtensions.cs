using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Starship.Core.Json;

namespace Starship.Web.Extensions {
    public static class HttpConfigurationExtensions {

        public static HttpConfiguration UseRecommendedConfiguration(this HttpConfiguration configuration) {
            return configuration
                //.UseMinimalJsonMediaContent()
                .UsePlainTextMediaType()
                .DetachXmlMediaType();
        }

        public static HttpConfiguration UseMinimalJsonMediaContent(this HttpConfiguration configuration) {
            configuration.Formatters.JsonFormatter.SerializerSettings = JsonSerializerSettingPresets.Minimal;
            return configuration;
        }

        public static HttpConfiguration UseSimpleJsonMediaContent(this HttpConfiguration configuration, params Assembly[] assembliesForTypeNameHandling) {
            var binder = new TypeNameSerializationBinder("{0}");
            binder.Assemblies.AddRange(assembliesForTypeNameHandling);

            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                SerializationBinder = binder
            };

            configuration.Formatters.JsonFormatter.SerializerSettings = settings;
            return configuration;
        }

        public static HttpConfiguration UseFullJsonMediaContent(this HttpConfiguration configuration, params Assembly[] assembliesForTypeNameHandling) {
            var binder = new TypeNameSerializationBinder("{0}");
            binder.Assemblies.AddRange(assembliesForTypeNameHandling);

            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                SerializationBinder = binder
            };

            configuration.Formatters.JsonFormatter.SerializerSettings = settings;
            return configuration;
        }

        public static HttpConfiguration UsePlainTextMediaType(this HttpConfiguration configuration) {
            configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            return configuration;
        }

        public static HttpConfiguration DetachXmlMediaType(this HttpConfiguration configuration) {
            var appXmlType = configuration.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            configuration.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            return configuration;
        }
    }
}