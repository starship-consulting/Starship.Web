using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.OData;
using System.Web.Http.OData.Extensions;
using Newtonsoft.Json;
using Starship.Core.Extensions;
using Starship.Core.Json;

namespace Starship.Web.OData {
    public class ODataFormatAttribute : EnableQueryAttribute {

        public ODataFormatAttribute(bool ignoreNullProperties = false, params Type[] typeSources) {
            IgnoreNullProperties = ignoreNullProperties;
            PageSize = 500;
            TypeSources = typeSources.ToList();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
            try {
                base.OnActionExecuted(actionExecutedContext);
            }
            catch (TargetInvocationException ex) {
                throw ex.InnerException;
            }

            if (actionExecutedContext == null || actionExecutedContext.Response == null || !(actionExecutedContext.Response.Content is ObjectContent) || actionExecutedContext.Response.Content.As<ObjectContent>().Value == null) {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return;
            }

            var data = (actionExecutedContext.Response.Content as ObjectContent).Value;

            if (data == null) {
                data = new List<object>();
            }

            var configuration = actionExecutedContext.ActionContext.ControllerContext.Configuration;
            var negotiator = configuration.Services.GetContentNegotiator();
            var formatter = negotiator.Negotiate(data.GetType(), actionExecutedContext.Request, configuration.Formatters).Formatter;

            if (formatter is JsonMediaTypeFormatter) {

                formatter = new JsonMediaTypeFormatter {
                    SerializerSettings = JsonSerializerSettingPresets.Minimal
                };

                /*var binder = new TypeNameSerializationBinder("{0}");
                binder.Assemblies.AddRange(TypeSources.Select(each => each.Assembly));

                formatter = new JsonMediaTypeFormatter {
                    SerializerSettings = new JsonSerializerSettings {
                        TypeNameHandling = TypeNameHandling.Objects,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                        Formatting = Formatting.Indented,
                        ContractResolver = new AlphabeticalContractResolver(),
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        SerializationBinder = binder
                    }
                };*/
            }

            var properties = actionExecutedContext.Request.ODataProperties();
            ObjectContent content;

            if (properties != null && properties.TotalCount > 0) {
                var result = new {
                    total = properties.TotalCount,
                    results = data
                };

                content = new ObjectContent(result.GetType(), result, formatter);
            }
            else {
                content = new ObjectContent(data.GetType(), data, formatter);
            }

            actionExecutedContext.Response.Content = content;
        }

        public bool IgnoreNullProperties { get; set; }

        private List<Type> TypeSources { get; set; }
    }
}