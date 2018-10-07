using System;
using System.Web;
using Starship.Core.Context;

namespace Starship.Web.Context {
    public class HttpContextResolver : ContextResolver {

        protected override void Clear(string contextKey) {
            if (HttpContext.Current.Items.Contains(contextKey)) {
                HttpContext.Current.Items.Remove(contextKey);
            }
        }

        protected override string Get(string contextKey) {
            if(HttpContext.Current.Items.Contains(contextKey)) {
                return HttpContext.Current.Items[contextKey].ToString();
            }

            return string.Empty;
        }

        protected override void Set(string contextKey, string value) {
            HttpContext.Current.Items[contextKey] = value;
        }

        protected override IsContext InitializeContext(Type type) {
            var context = base.InitializeContext(type);
            HttpContext.Current.DisposeOnPipelineCompleted(context);
            return context;
        }
    }
}