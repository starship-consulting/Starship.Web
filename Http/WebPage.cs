using System;
using Starship.Web.Services;

namespace Starship.Web.Http {
    public abstract class WebPage {

        protected WebPage(string url) {
            if (string.IsNullOrEmpty(url)) {
                throw new Exception("Null URL received for: " + GetType().Name);
            }

            Url = url;
        }

        public virtual bool ShouldNavigateTo(ParsingService service) {
            return true;
        }

        public abstract void Parse(ParsedDocument document);

        public string Url { get; set; }
    }
}
