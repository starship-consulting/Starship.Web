using System.Collections.Generic;
using CsQuery;

namespace Starship.Web.Http {
    public class ParsedDocument {

        public ParsedDocument(string html) {
            Document = CQ.CreateDocument(html);
            Html = html;
            Links = new List<WebPage>();
            Items = new List<ContentItem>();
        }

        public string Html { get; set; }

        public CQ Document { get; set; }

        public List<WebPage> Links { get; set; }

        public List<ContentItem> Items { get; set; } 
    }
}
