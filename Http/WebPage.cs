using System;

namespace Starship.Web.Http {
    public abstract class WebPage {

        protected WebPage(string url) {
            if (string.IsNullOrEmpty(url)) {
                throw new Exception("Null URL received for: " + GetType().Name);
            }

            Url = url;
        }

        public abstract void Parse(ParsedDocument document);

        public string Url { get; set; }
    }
}
